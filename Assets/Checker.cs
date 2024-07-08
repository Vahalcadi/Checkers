using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    public bool IsPlayer { get { return isPlayer; } }

    public bool IsKing;

    public int movement;
    [SerializeField] private float lerpDuration;

    private void Start()
    {
        IsKing = false;
        GameManager.OnCheckerInitialisation?.Invoke(this);
    }

    public virtual IEnumerator Move()
    {
        //List<Node> path = Graph.Instance.FindPath(GameManager.Instance.CurrentNode, GameManager.Instance.EndNode);

        /*foreach (Node node in path)
        {
            Debug.Log("enter Move Player");
            yield return MoveEachNode(GameManager.Instance.EndNode);
        }*/

        GameManager.Instance.CurrentlyMovingChecker = this;
        GameManager.Instance.CurrentNode.CheckerInThisNode = null;
        yield return MoveEachNode(GameManager.Instance.EndNode);
        GameManager.Instance.CurrentNode = GameManager.Instance.EndNode;
        GameManager.Instance.EndNode.CheckerInThisNode = this;
        GameManager.Instance.CurrentlyMovingChecker = null;

        if (isPlayer && transform.position.y == 7)
            IsKing = true;
        else if (!isPlayer && transform.position.y == 0)
            IsKing = true;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (GameManager.Instance.CurrentlyMovingChecker == this)
        {
            if (collision.CompareTag("Enemy"))
            {
                Vector2 collisionTransform = new Vector2(collision.transform.position.x, collision.transform.position.y);

                Debug.Log(collisionTransform);

                Graph.Instance.Nodes.Find(n => n.PositionInTheWorld == collisionTransform).CheckerInThisNode = null;

                Destroy(collision.gameObject);
                Debug.Log("pawn disabled: " + collision.gameObject.name);
            }
            else if (collision.CompareTag("Player"))
            {
                Vector2 collisionTransform = new Vector2(collision.transform.position.x, collision.transform.position.y);

                Debug.Log(collisionTransform);

                Graph.Instance.Nodes.Find(n => n.PositionInTheWorld == collisionTransform).CheckerInThisNode = null;

                collision.gameObject.SetActive(false);
                Destroy(collision.gameObject);

                Debug.Log("pawn disabled: " + collision.gameObject.name);
            }
        }
    }

    public Node GetRandomNode(List<Node> path)
    {
        return path[Random.Range(0, path.Count)];
    }

    private void OnDestroy()
    {
        GameManager.OnCheckerDestroyed?.Invoke(this);
    }
}
