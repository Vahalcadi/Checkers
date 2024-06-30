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

    public List<Node> FindPath(Node start, Node end)
    {
        if (!nodes.Contains(start) || !nodes.Contains(end))
        {
            Debug.LogError("not valid");
            Debug.Log("start: " + nodes.Contains(start));
            Debug.Log("end: " + nodes.Contains(end));
            return null;
        }

        Dictionary<Node, Node> map = new Dictionary<Node, Node>();
        Dictionary<Node, PathNode> costs = new Dictionary<Node, PathNode>();
        PriorityQueue priorityQueue = new PriorityQueue();

        foreach (Node node in nodes)
        {
            costs[node] = new PathNode(node);
        }

        costs[start].gValue = 0;
        priorityQueue.Enqueue(costs[start]);

        while (priorityQueue.Count > 0)
        {
            PathNode currentNode = priorityQueue.Dequeue();

            if (currentNode.Position == end.PositionInTheWorld)
                break;

            foreach (Edge edge in currentNode.Edges)
            {
                PathNode neighbour = costs[currentNode.Node.GetNeighbour(edge)];

                //int movementCost = currentNode.gValue + CalculateHeuristic(currentNode, neighbour);
                int movementCost = currentNode.gValue + edge.Weight;

                if (movementCost < neighbour.gValue)
                {
                    neighbour.gValue = movementCost;
                    neighbour.hValue = CalculateHeuristic(neighbour.Node, end);

                    if (neighbour.Node.CheckerInThisNode == null)
                    {
                        map[neighbour.Node] = currentNode.Node;

                        costs[currentNode.Node.GetNeighbour(edge)] = neighbour;
                        priorityQueue.Enqueue(neighbour);
                    }

                }

            }

        }

        List<Node> path = ReconstructPath(ref map, start, end);
        return path;
    }

    private List<Node> ReconstructPath(ref Dictionary<Node, Node> map, in Node startingPoint, in Node endPoint)
    {
        List<Node> path = new List<Node>();
        Node current = endPoint;

        Debug.Log("Starting Point: " + startingPoint);


        while (current != startingPoint)
        {
            if (current.CheckerInThisNode == null)
            {
                path.Add(current);
            }
            current = map[current];
        }

        // path.Add(startingPoint);

        path.Reverse();
        return path;
    }

    private int CalculateHeuristic(Node start, Node end)
    {
        var distance = Mathf.Abs(end.PositionInTheWorld.x - start.PositionInTheWorld.x) + Mathf.Abs(end.PositionInTheWorld.y - start.PositionInTheWorld.y);
        //D2 * (Min(Abs(X1 - X2), Abs(Y1 - Y2))

        //Mathf.Min(Mathf.Abs(end.PositionInTheWorld.x - start.PositionInTheWorld.x), Mathf.Abs(end.PositionInTheWorld.y - start.PositionInTheWorld.y));
        return (int)distance;
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
                    if (neighbour.Node.CheckerInThisNode == null)
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
                                else if(checkerIsKing)
                                    path.Add(neighbour.Node);
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

    public int FValue { get { return gValue + hValue; } }
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

public class PriorityQueue
{
    List<PathNode> nodes;
    private int count;

    public int Count { get { return count; } }

    public PriorityQueue()
    {
        nodes = new List<PathNode>();
        count = 0;
    }

    public void Enqueue(PathNode node)
    {
        nodes.Add(node);
        count++;
        ShiftUp(count - 1);
    }

    private void ShiftUp(int index)
    {
        int parentIndex;
        PathNode temp;
        if (index != 0)
        {
            parentIndex = GetParentIndex(index);
            if (nodes[parentIndex].FValue > nodes[index].FValue)
            {
                temp = nodes[parentIndex];
                nodes[parentIndex] = nodes[index];
                nodes[index] = temp;
                ShiftUp(parentIndex);
            }
            else if (nodes[parentIndex].FValue == nodes[index].FValue && nodes[parentIndex].hValue > nodes[index].hValue)
            {
                temp = nodes[parentIndex];
                nodes[parentIndex] = nodes[index];
                nodes[index] = temp;
                ShiftUp(parentIndex);
            }
        }
    }

    public PathNode Dequeue()
    {
        PathNode obj;
        if (count == 0)
        {
            throw new System.Exception("Queue is empty");
        }
        else
        {
            obj = nodes[0];
            nodes[0] = nodes[count - 1];
            count--;
            if (count > 0)
            {
                ShiftDown(0);
            }
        }
        nodes.RemoveAt(count);
        return obj;
    }

    private void ShiftDown(int nodeIndex)
    {
        int leftChildIndex, rightChildIndex, minIndex;
        PathNode temp;
        leftChildIndex = GetLeftChildIndex(nodeIndex);
        rightChildIndex = GetRightChildIndex(nodeIndex);
        if (rightChildIndex >= count)
        {
            if (leftChildIndex >= count)
                return;

            minIndex = leftChildIndex;

        }
        else
        {
            if (nodes[leftChildIndex].FValue < nodes[rightChildIndex].FValue)
                minIndex = leftChildIndex;
            else if (nodes[leftChildIndex].FValue == nodes[rightChildIndex].FValue && nodes[leftChildIndex].hValue < nodes[rightChildIndex].hValue)
                minIndex = leftChildIndex;
            else
                minIndex = rightChildIndex;
        }
        if (nodes[nodeIndex].FValue > nodes[minIndex].FValue)
        {
            temp = nodes[minIndex];
            nodes[minIndex] = nodes[nodeIndex];
            nodes[nodeIndex] = temp;
            ShiftDown(minIndex);
        }
        else if (nodes[nodeIndex].FValue > nodes[minIndex].FValue && nodes[nodeIndex].hValue > nodes[minIndex].hValue)
        {
            temp = nodes[minIndex];
            nodes[minIndex] = nodes[nodeIndex];
            nodes[nodeIndex] = temp;
            ShiftDown(minIndex);
        }
    }

    private int GetRightChildIndex(int index)
    {
        return (2 * index) + 2;
    }

    private int GetLeftChildIndex(int index)
    {
        return (2 * index) + 1;
    }

    private int GetParentIndex(int index)
    {
        return (index - 1) / 2;
    }


}