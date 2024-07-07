using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState
{
    NoAction, Move, Pass, Busy
}
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    private GameState gameState;
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
        gameState = GameState.Move;

        yield return GameManager.Instance.DetectChecker();
       
        traversableNodes = GameManager.Instance.GetTraversableNodes(CurrentChecker);

        gameState = GameState.Busy;

        yield return CheckValidEndNode(CurrentChecker);

        gameState = GameState.NoAction;
    }


    public IEnumerator EnemyMoveSelection()
    {
        gameState = GameState.Move;

        traversableNodes = GameManager.Instance.EnemySelectFirstMoveablePawn();

        gameState = GameState.Busy;

        yield return CheckValidEndNode(CurrentChecker);

        gameState = GameState.NoAction;
    }


    public IEnumerator CheckValidEndNode(Checker checker)
    {
        if (checker.IsPlayer)
        {
            yield return GameManager.Instance.DetectEndNode();
        }
        else
        {
            GameManager.Instance.EndNode = checker.GetFarthestNode(traversableNodes);

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

            if(CurrentChecker.IsPlayer)
                StartCoroutine(EnemyMoveSelection());
            else
                StartCoroutine(PlayerMoveSelection());

        }
        else
        {
            yield return CheckValidEndNode(checker);
        }
    }
}
