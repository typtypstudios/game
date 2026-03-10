using TMPro;
using UnityEngine;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject exitButton;

    private void Awake()
    {
        panel.SetActive(false);
        exitButton.SetActive(false);
    }

    public void ShowEndMatch(bool isWinner)
    {
        panel.SetActive(true);
        exitButton.SetActive(false);

        resultText.text = isWinner ? "VICTORY" : "DEFEAT";

        WritableText wt = resultText.GetComponent<WritableText>();
        wt.FillColor = isWinner ? Color.cyan : Color.red;
        wt.ResetText();

        exitButton.SetActive(true);
    }
}