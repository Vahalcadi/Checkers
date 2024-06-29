using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Color traversableColour;
    [SerializeField] private Color defaultColour;
    [SerializeField] private Color endNodeSelectedColour;
    [SerializeField] LayerMask NodeMask;
    [SerializeField] LayerMask CheckerMask;
    public Node CurrentNode { get; set; }
    public Node EndNode { get; set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
    }

    private void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            //EndNode = GetClickedNode(out RaycastHit hit);
            DetectEndNode();
        }*/
    }

    public IEnumerator DetectChecker()
    {
        yield return WaitForMouseButtonDown();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit;
        if (hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, CheckerMask))
        {
            if (hit.collider.gameObject.GetComponent<Checker>() != null)
            {
               TurnManager.Instance.CurrentChecker = hit.collider.gameObject.GetComponent<Checker>();
            }

            Debug.LogWarning(TurnManager.Instance.CurrentChecker);
        }

        yield return null;
    }

    public IEnumerator DetectEndNode()
    {
        yield return WaitForMouseButtonDown();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit;
        if (hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, NodeMask))
        {
            if (hit.collider.gameObject.GetComponent<Node>() != null)
            {
                EndNode = hit.collider.gameObject.GetComponent<Node>();
            }

            Debug.LogWarning(EndNode);
        }

        yield return null;
    }

    IEnumerator WaitForMouseButtonDown()
    {
        while (!Input.GetMouseButtonDown(0))
            yield return null;
    }

    public IEnumerator WaitForEscPressed()
    {
        while (!Input.GetKeyDown(KeyCode.Escape))
            yield return null;
    }

    public void ResetTraversableNodes(List<Node> path)
    {
        foreach (Node node in path)
        {
            Color color = defaultColour;
            node.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void HighlightEndNode()
    {
        EndNode.gameObject.GetComponent<SpriteRenderer>().color = endNodeSelectedColour;
    }

    public void ClearEndNodeHighlight()
    {
        Color color = defaultColour;

        EndNode.gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    public List<Node> GetTraversableNodes(Checker checker)
    {
        Debug.Log(checker);
        CurrentNode = checker.GetCurrentNode();

        int distance = checker.movement;


        List<Node> path = Graph.Instance.GetTraversableNodes(CurrentNode, distance);

        path.Remove(CurrentNode); //should be start node afterwards

        foreach (Node node in path)
        {
            node.gameObject.GetComponent<SpriteRenderer>().color = traversableColour;
        }

        return path;
    }

    public void HiglightNodes(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            node.gameObject.GetComponent<SpriteRenderer>().color = traversableColour;
        }
    }
}
