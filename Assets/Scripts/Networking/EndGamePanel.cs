using TMPro;
using UnityEngine;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text resultText;

    // En esta clase se pueden mostrar más datos de la partida al terminar

    void Awake()
    {
        panel.SetActive(false);
    }

    public void Show(bool isWinner)
    {
        panel.SetActive(true);
        resultText.text = isWinner ? "VICTORY" : "DEFEAT";
        WritableText wt = resultText.GetComponent<WritableText>();
        wt.FillColor = isWinner ? Color.cyan : Color.red;
        wt.ResetText();
    }
}