using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [HideInInspector] public int WhiteCheckerCounter;
    [HideInInspector] public int BlackCheckerCounter;

    public TextMeshProUGUI WhiteCheckerCounterText;
    public TextMeshProUGUI BlackCheckerCounterText;

    public static UIManager Instance;
    private void Awake()
    {
        if(Instance != null)
            Destroy(Instance.gameObject);
        else 
            Instance = this;
    }

    public void InitialiseCounters(Checker checker)
    {
        if (checker.IsPlayer)
        {
            WhiteCheckerCounter++;
            WhiteCheckerCounterText.text = $"{WhiteCheckerCounter}";
        }
        else
        {
            BlackCheckerCounter++;
            BlackCheckerCounterText.text = $"{BlackCheckerCounter}";
        }
    }

    public void UpdateCounters(Checker checker)
    {
        if (checker.IsPlayer)
        {
            WhiteCheckerCounter--;
            WhiteCheckerCounterText.text = $"{WhiteCheckerCounter}";
        }
        else
        {
            BlackCheckerCounter--;
            BlackCheckerCounterText.text = $"{BlackCheckerCounter}";
        }

        GameManager.Instance.EndGame();
    }
}
