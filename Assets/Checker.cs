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

    MoveCommand<Transform> command;

    private void Start()
    {
        IsKing = false;
        GameManager.OnCheckerInitialisation?.Invoke(this);
    }

    public IEnumerator Move()
    {
        GameManager.Instance.CurrentlyMovingChecker = this;
        GameManager.Instance.CurrentNode.CheckerInThisNode = null;
        //yield return MoveEachNode(GameManager.Instance.EndNode);

        GameState state;
        if (isPlayer)
            state = GameState.PlayerMove;
        else
            state = GameState.EnemyMove;

        command = new MoveCommand<Transform>(transform, GameManager.Instance.EndNode, GameManager.Instance.CurrentNode, state, lerpDuration);
        yield return GameManager.Instance.commandProcessor.Execute(command);

        GameManager.Instance.CurrentNode = GameManager.Instance.EndNode;
        GameManager.Instance.EndNode.CheckerInThisNode = this;
        GameManager.Instance.CurrentlyMovingChecker = null;

        if (isPlayer && transform.position.y == 7)
            IsKing = true;
        else if (!isPlayer && transform.position.y == 0)
            IsKing = true;
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

                command.SetCapturedChecker(collision.gameObject.GetComponent<Checker>());
                collision.gameObject.SetActive(false);
                Debug.Log("pawn disabled: " + collision.gameObject.name);
            }
            else if (collision.CompareTag("Player"))
            {
                Vector2 collisionTransform = new Vector2(collision.transform.position.x, collision.transform.position.y);

                Debug.Log(collisionTransform);

                Graph.Instance.Nodes.Find(n => n.PositionInTheWorld == collisionTransform).CheckerInThisNode = null;

                command.SetCapturedChecker(collision.gameObject.GetComponent<Checker>());
                collision.gameObject.SetActive(false);
                Debug.Log("pawn disabled: " + collision.gameObject.name);
            }
        }
    }

    public Node GetRandomNode(List<Node> path)
    {
        return path[Random.Range(0, path.Count)];
    }

    private void OnDisable()
    {
        GameManager.OnCheckerDestroyed?.Invoke(this);
    }
}
