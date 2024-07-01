using System.Collections;
using UnityEngine;

public class Checker : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    public bool IsPlayer { get { return isPlayer; } }

    public bool IsKing { get; set; }

    public int movement;
    [SerializeField] private float lerpDuration;

    private void Start()
    {
        IsKing = false;
    }

    public virtual IEnumerator Move()
    {
        //List<Node> path = Graph.Instance.FindPath(GameManager.Instance.CurrentNode, GameManager.Instance.EndNode);

        /*foreach (Node node in path)
        {
            Debug.Log("enter Move Player");
            yield return MoveEachNode(GameManager.Instance.EndNode);
        }*/

        yield return MoveEachNode(GameManager.Instance.EndNode);
        GameManager.Instance.CurrentNode = GameManager.Instance.EndNode;
        GameManager.Instance.EndNode.CheckerInThisNode = this;
        //HasMovedThisRound = true;
    }

    public IEnumerator MoveEachNode(Node node)
    {
        Vector2 startPos = transform.position;
        float t = 0;

        Vector2 arrival = new Vector3(node.PositionInTheWorld.x, node.PositionInTheWorld.y);

        float percentage = 0;

        while (percentage <= 1)
        {
            yield return null;
            t += Time.deltaTime;
            percentage = t / lerpDuration;
            transform.position = Vector3.Lerp(startPos, arrival, percentage);
        }
    }

    public Node GetCurrentNode()
    {
        Vector2 distance = new Vector2(transform.position.x, transform.position.y);

        return Graph.Instance.GetNode(distance);
    }
}
