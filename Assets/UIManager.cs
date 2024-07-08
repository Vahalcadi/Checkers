using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI WhiteCheckerCounterText;
    [SerializeField] private TextMeshProUGUI BlackCheckerCounterText;

    [SerializeField] private GameObject EndGameScreen;
    [SerializeField] private TextMeshProUGUI ResultText;

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

    public void OpenEndgameScreen(bool won)
    {
        if (won)
            ResultText.text = "Player Wins";
        else
            ResultText.text = "AI Wins";

        EndGameScreen.SetActive(true);
    }
}
