using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI WhiteCheckerCounterText;
    public TextMeshProUGUI BlackCheckerCounterText;

    public void InitialiseCounters(Checker checker, ref int WhiteCheckerCounter, ref int BlackCheckerCounter)
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

    public void UpdateCounters(Checker checker, ref int WhiteCheckerCounter, ref int BlackCheckerCounter)
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
