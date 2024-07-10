using System.Collections;
using UnityEngine;

public class MoveCommand<T> : ICommand where T : Transform
{
    private T entity;
    private Vector2 endPosition;
    private Vector2 previousPosition;

    private GameState previousState;
    private float lerpDuration;
    private Node endNode;
    private Node previousNode;

    private Checker capturedChecker;
    private bool wasKing;
    public MoveCommand(T entity, Node endNode, Node previousNode, GameState state, float lerpDuration)
    {
        this.entity = entity;
        this.endPosition = endNode.PositionInTheWorld;
        previousState = state;
        this.lerpDuration = lerpDuration;
        this.endNode = endNode;
        this.previousNode = previousNode;

    }

    public IEnumerator Execute()
    {
        previousPosition = entity.position;
        wasKing = entity.gameObject.GetComponent<Checker>().IsKing;
        yield return LerpMovement();
    }

    public void Undo()
    {
        if (!entity.gameObject.activeSelf)
            entity.gameObject.SetActive(true);

        if (!wasKing)
            entity.gameObject.GetComponent<Checker>().IsKing = false;

        if (capturedChecker != null)
        {
            capturedChecker.gameObject.SetActive(true);
            Node capturedNode = Graph.Instance.Nodes.Find(n => n.PositionInTheWorld == new Vector2(capturedChecker.transform.position.x, capturedChecker.transform.position.y));
            capturedNode.CheckerInThisNode = capturedChecker;
            GameManager.Instance.AddCounterUI(capturedChecker);
        }

        endNode.CheckerInThisNode = null;
        previousNode.CheckerInThisNode = entity.gameObject.GetComponent<Checker>();
        entity.position = previousPosition;
        TurnManager.Instance.RestartFromState(previousState);
    }

    public IEnumerator LerpMovement()
    {
        Vector2 startPos = entity.position;
        float t = 0;

        float percentage = 0;

        while (percentage <= 1)
        {
            yield return null;
            t += Time.deltaTime;
            percentage = t / lerpDuration;
            entity.position = Vector3.Lerp(startPos, endPosition, percentage);
        }
    }

    public void SetCapturedChecker(Checker checker)
    {
        capturedChecker = checker;
    }
}
