using UnityEngine;

[System.Serializable]
public class Edge
{
    private Node nodeA;
    [SerializeField] private Node nodeB;

    public Edge()
    {

    }

    public Node NodeA { get { return nodeA; } }
    public Node NodeB { get { return nodeB; } }
    public int Weight { get { return 1; } }


    public Node ReturnNodeFromPosition(Vector2 pos)
    {
        Node nodeToReturn = null;
        if (nodeA.PositionInTheWorld == pos)
            nodeToReturn = NodeA;
        else if(nodeB.PositionInTheWorld == pos)
            nodeToReturn = NodeB;
        
        return nodeToReturn;
    }

    public void SetNodeA(Node node)
    {
        nodeA = node;
    }
}
