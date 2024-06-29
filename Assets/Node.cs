using System.Collections.Generic;
using UnityEngine;


public class Node : MonoBehaviour
{
    public Vector2 PositionInTheWorld => transform.position;
    [SerializeField] private List<Edge> edges;
    [SerializeField] private Checker checkerInThisNode;

    public Checker CheckerInThisNode { get { return checkerInThisNode; } set { checkerInThisNode = value; } }
    public List<Edge> Edges { get { return edges; } }

    private void Awake()
    {
        foreach (var edge in edges)
        {
            if (edge.NodeB.Equals(this))
                continue;
            else
            {
                edge.SetNodeA(this);
                edge.NodeB.GetAlreadyExistingEdges(edge);
            }
        }
    }

    public Node GetNeighbour(Edge edge)
    {
        if (edge.NodeA.PositionInTheWorld == PositionInTheWorld)
            return edge.NodeB;
        else
            return edge.NodeA;
    }

    public void GetAlreadyExistingEdges(Edge edge)
    {
        bool canAdd = true;
        foreach (var e in edges)
        {
            if (e.Equals(edge))
                canAdd = false;
        }

        if (canAdd)
            edges.Add(edge);
    }

}
