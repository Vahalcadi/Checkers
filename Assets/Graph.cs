using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] private List<Node> nodes;

    public List<Node> Nodes { get { return nodes; } }

    public static Graph Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        else
            Instance = this;
    }

    public Node GetNode(Vector2 position)
    {
        return nodes.Find(n => n.PositionInTheWorld == position);
    }

    public List<Node> GetTraversableNodes(Node start, int moveCount)
    {
        List<Node> path = new List<Node>();
        Dictionary<Node, PathNode> costs = new Dictionary<Node, PathNode>();
        Queue<PathNode> queue = new Queue<PathNode>();

        foreach (Node node in nodes)
        {
            costs[node] = new PathNode(node);
        }

        costs[start].gValue = 0;
        queue.Enqueue(costs[start]);
        bool checkerIsKing = TurnManager.Instance.CurrentChecker.IsKing;
        bool checkerIsPlayer = TurnManager.Instance.CurrentChecker.IsPlayer;
        while (queue.Count > 0)
        {
            PathNode currentNode = queue.Dequeue();

            foreach (Edge edge in currentNode.Edges)
            {
                PathNode neighbour = costs[currentNode.Node.GetNeighbour(edge)];

                //int movementCost = currentNode.gValue + CalculateHeuristic(currentNode, neighbour);
                int movementCost = currentNode.gValue + edge.Weight;

                if (movementCost < neighbour.gValue)
                {
                    //if (neighbour.Node.CheckerInThisNode == null)
                    neighbour.gValue = movementCost;

                    if (neighbour.gValue <= moveCount)
                    {
                        //neighbour.isTraversable = true;
                        if (!path.Contains(neighbour.Node))
                        {
                            if (neighbour.Node.CheckerInThisNode == null)
                            {
                                if (!checkerIsKing && checkerIsPlayer)
                                {
                                    if (neighbour.Node.PositionInTheWorld.y - currentNode.Position.y > 0) //means he is moving forward
                                    {
                                        path.Add(neighbour.Node);
                                    }
                                }
                                else if (!checkerIsKing && !checkerIsPlayer)
                                {
                                    if (neighbour.Node.PositionInTheWorld.y - currentNode.Position.y < 0) //means he is moving backward
                                    {
                                        path.Add(neighbour.Node);
                                    }
                                }
                                else
                                    path.Add(neighbour.Node);
                            }
                            else
                            {

                                int differenceX;
                                int differenceY;

                                differenceX = (int)neighbour.Position.x - (int)start.PositionInTheWorld.x;
                                differenceY = (int)neighbour.Position.y - (int)start.PositionInTheWorld.y;

                                Vector2 position = new Vector2(differenceX, differenceY);
                                Node nodeToAdd = null;

                                if (neighbour.Edges.Find(e => e.ReturnNodeFromPosition(neighbour.Position + position) != null) == null)
                                    continue;

                                nodeToAdd = neighbour.Edges.Find(e => e.ReturnNodeFromPosition(neighbour.Position + position) != null).ReturnNodeFromPosition(neighbour.Position + position);

                                if (nodeToAdd.CheckerInThisNode != null)
                                    continue;

                                Debug.Log(nodeToAdd.PositionInTheWorld);


                                if (differenceY < 0 && !checkerIsKing) //means checker is going backwards
                                {
                                    if (neighbour.Node.CheckerInThisNode.IsPlayer && !TurnManager.Instance.CurrentChecker.IsPlayer) // means enemy checker is playing
                                        path.Add(nodeToAdd);
                                }
                                else if (differenceY > 0 && !checkerIsKing) //means checker is going forward
                                {
                                    if (!neighbour.Node.CheckerInThisNode.IsPlayer && TurnManager.Instance.CurrentChecker.IsPlayer) // means player checker is playing
                                        path.Add(nodeToAdd);
                                }
                                else if (checkerIsKing) // means checker can go forward or backwards
                                {
                                    if (!neighbour.Node.CheckerInThisNode.IsPlayer && TurnManager.Instance.CurrentChecker.IsPlayer) // means player checker is playing
                                        path.Add(nodeToAdd);
                                    else if (neighbour.Node.CheckerInThisNode.IsPlayer && !TurnManager.Instance.CurrentChecker.IsPlayer) // means enemy checker is playing
                                        path.Add(nodeToAdd);
                                }


                            }
                        }

                        costs[currentNode.Node.GetNeighbour(edge)] = neighbour;

                        queue.Enqueue(neighbour);
                    }
                }
            }

        }

        return path;
    }
}

public class PathNode
{
    Node node;

    public int gValue;
    public int hValue;

    //public bool isTraversable = false;

    public Vector2 Position { get { return node.PositionInTheWorld; } }
    public List<Edge> Edges { get { return node.Edges; } }
    public Node Node { get { return node; } }
    public PathNode(Node node)
    {
        this.node = node;
        gValue = int.MaxValue;
        hValue = int.MaxValue;
    }

    /*public Node GetNeighbour(Edge edge)
    {
        if (edge.NodeA.PositionInTheWorld == Position)
            return edge.NodeB;
        else
            return edge.NodeA;
    }*/

    public static explicit operator PathNode(Node v)
    {
        return new PathNode(v);
    }
}