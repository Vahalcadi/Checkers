using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static Action<Checker> OnCheckerInitialisation;
    public static Action<Checker> OnCheckerDestroyed;

    public static GameManager Instance;
    [SerializeField] private Color traversableColour;
    [SerializeField] private Color defaultColour;
    [SerializeField] private Color endNodeSelectedColour;
    [SerializeField] LayerMask NodeMask;
    [SerializeField] LayerMask CheckerMask;

    [SerializeField] private UIManager UIManager;

    private int whiteCheckerCounter;
    private int blackCheckerCounter;

    [HideInInspector] public List<Checker> enemyCheckers;

    public Node CurrentNode { get; set; }
    public Node EndNode { get; set; }
    public Checker CurrentlyMovingChecker { get; set; } //used to manage the capture interaction
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
        else
            yield return DetectChecker();

        yield return null;
    }

    public IEnumerator DetectEndNode()
    {
        IEnumerator<bool?> coroutine;
        yield return coroutine = WaitForMouseButtonDown();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit;

        if (coroutine.Current != null)
        {
            if (coroutine.Current.Value)
            {
                ResetTraversableNodes(TurnManager.Instance.traversableNodes);
                EndNode = null;
                yield break;
            }
        }

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

    IEnumerator<bool?> WaitForMouseButtonDown()
    {
        while (!Input.GetMouseButtonDown(0))
        {
            if (Input.GetMouseButtonDown(1))
            {
                yield return true;
                yield break;
            }
            else
                yield return null;
        }

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

    public void SetCheckersUI(Checker checker)
    {
        if (!checker.IsPlayer)
            enemyCheckers.Add(checker);

        UIManager.InitialiseCounters(checker, ref whiteCheckerCounter, ref blackCheckerCounter);
    }

    public void UpdateUI(Checker checker)
    {
        if (!checker.IsPlayer)
            enemyCheckers.Remove(checker);

        UIManager.UpdateCounters(checker, ref whiteCheckerCounter, ref blackCheckerCounter);
    }

    public void EndGame()
    {
        if (whiteCheckerCounter <= 0)
        {
            Debug.Log("you lost");
            UIManager.OpenEndgameScreen(false);
        }
        else if (blackCheckerCounter <= 0)
        {
            Debug.Log("you won");
            UIManager.OpenEndgameScreen(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public List<Node> EnemySelectRandomMoveablePawn()
    {
        List<Node> path = new List<Node>();
        List<Checker> checkers = new List<Checker>();
        Dictionary<Checker, List<Node>> dictionary = new Dictionary<Checker, List<Node>>();

        foreach (Checker checker in enemyCheckers)
        {
            TurnManager.Instance.CurrentChecker = checker;
            path = GetTraversableNodes(checker);
            if (path.Count > 0)
            {
                dictionary[checker] = path;
                ResetTraversableNodes(path);
                checkers.Add(checker);
            }
        }

        int random = Random.Range(0, dictionary.Count);
        TurnManager.Instance.CurrentChecker = checkers[random];
        CurrentNode = TurnManager.Instance.CurrentChecker.GetCurrentNode();
        return dictionary[checkers[random]];
    }

    private void OnEnable()
    {
        OnCheckerInitialisation += SetCheckersUI;
        OnCheckerDestroyed += UpdateUI;
    }

    private void OnDisable()
    {
        OnCheckerInitialisation -= SetCheckersUI;
        OnCheckerDestroyed -= UpdateUI;
    }
}
