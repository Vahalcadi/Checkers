using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState
{
    NoAction, PlayerMove, EnemyMove, Pass, Busy, EndGame
}
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public GameState gameState;
    [HideInInspector] public List<Node> traversableNodes = new List<Node>();
    public Checker CurrentChecker { get; set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        StartCoroutine(PlayerMoveSelection());
    }

    public IEnumerator PlayerMoveSelection()
    {
        if (gameState == GameState.EndGame)
            yield break;

        gameState = GameState.PlayerMove;

        yield return GameManager.Instance.DetectChecker();

        traversableNodes = GameManager.Instance.GetTraversableNodes(CurrentChecker);

        yield return CheckValidEndNode(CurrentChecker);

        gameState = GameState.NoAction;
    }


    public IEnumerator EnemyMoveSelection()
    {
        if (gameState == GameState.EndGame)
            yield break;

        gameState = GameState.EnemyMove;

        traversableNodes = GameManager.Instance.EnemySelectRandomMoveablePawn();

        yield return CheckValidEndNode(CurrentChecker);

        gameState = GameState.NoAction;
    }

    public IEnumerator EnemyMoveSelectionAfterClick()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        StartCoroutine(EnemyMoveSelection());
    }


    public IEnumerator CheckValidEndNode(Checker checker)
    {
        if (checker.IsPlayer)
        {
            yield return GameManager.Instance.DetectEndNode();
            if (GameManager.Instance.EndNode == null)
            {
                StartCoroutine(PlayerMoveSelection());
                yield break;
            }

        }
        else
        {
            GameManager.Instance.EndNode = checker.GetRandomNode(traversableNodes);

            /*if ((checker as Enemy).CannotMove && action == BattleState.Move)
            {
                GameManager.Instance.ResetTraversableNodes(traversableNodes);
                yield break;
            }*/
        }

        if (traversableNodes.Contains(GameManager.Instance.EndNode))
        {

            GameManager.Instance.ResetTraversableNodes(traversableNodes);
            GameManager.Instance.HighlightEndNode();

            GameManager.Instance.CurrentNode.CheckerInThisNode = null;
            yield return checker.Move();

            GameManager.Instance.ClearEndNodeHighlight();

            if (CurrentChecker.IsPlayer)
                StartCoroutine(EnemyMoveSelection());
            else
                StartCoroutine(PlayerMoveSelection());

        }
        else
        {
            yield return CheckValidEndNode(checker);
        }
    }

    public void RestartFromState(GameState state)
    {
        StopAllCoroutines();

        if (state == GameState.PlayerMove)
            StartCoroutine(PlayerMoveSelection());
        else if (state == GameState.EnemyMove)
            StartCoroutine(EnemyMoveSelectionAfterClick());
    }
}
