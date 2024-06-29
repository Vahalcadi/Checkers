using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BattleState
{
    NoAction, Move, Pass, Busy
}
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    private BattleState battleState;
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
        battleState = BattleState.Move;

        yield return GameManager.Instance.DetectChecker();
       
        traversableNodes = GameManager.Instance.GetTraversableNodes(CurrentChecker);

        battleState = BattleState.Busy;

        yield return CheckValidEndNode(CurrentChecker);

        battleState = BattleState.NoAction;
    }

    public IEnumerator CheckValidEndNode(Checker checker)
    {
        if (checker.IsPlayer)
            yield return GameManager.Instance.DetectEndNode();
        /*else
        {
            GameManager.Instance.EndNode = checker.GetClosest(players, action);

            if ((checker as Enemy).CannotMove && action == BattleState.Move)
            {
                GameManager.Instance.ResetTraversableNodes(traversableNodes);
                yield break;
            }
        }*/

        if (traversableNodes.Contains(GameManager.Instance.EndNode))
        {

            GameManager.Instance.ResetTraversableNodes(traversableNodes);
            GameManager.Instance.HighlightEndNode();

            GameManager.Instance.CurrentNode.CheckerInThisNode = null;
            yield return checker.Move();
                     
            GameManager.Instance.ClearEndNodeHighlight();

            /**REMOVE FROM HERE**/
            StartCoroutine(PlayerMoveSelection());
            ////////////////////////
        }
        else
        {
            yield return CheckValidEndNode(checker);
        }
    }
}
