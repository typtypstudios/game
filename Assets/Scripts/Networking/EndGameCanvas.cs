using System.Collections;
using TMPro;
using UnityEngine;

public class EndGameCanvas : MonoBehaviour
{
    [SerializeField] private float showDelay = 1.0f;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text resultReasonText;
    [SerializeField] private GameObject exitButton;

    [Header("XP Related")]
    [SerializeField] private TMP_Text earnedXPText;
    [SerializeField] private ProgressionBar progressionBar;
    [SerializeField] private float animSpeed = 1;

    private void Awake()
    {
        exitButton.SetActive(false);

        resultReasonText.text = "";

        XPManager.Instance.OnXPUpdated += UpdateProgressionBar;
    }

    private void OnDisable() => XPManager.Instance.OnXPUpdated -= UpdateProgressionBar;

    public void ShowEndMatch(bool isWinner, MatchEndReason reason)
    {
        Invoke(nameof(GoToResults), showDelay);
        exitButton.SetActive(false);
        resultText.text = isWinner ? "VICTORY" : "DEFEAT";
        WritableText wt = resultText.GetComponent<WritableText>();
        if (wt != null)
        {
            wt.FillColor = isWinner ? Color.cyan : Color.red;
            wt.RebindText();
        }

        if (resultReasonText != null)
            resultReasonText.text = GetReasonText(isWinner, reason);

        exitButton.SetActive(true);
        if (isWinner) XPManager.Instance.ProcessVictory();
        earnedXPText.gameObject.SetActive(isWinner);
        progressionBar.gameObject.SetActive(isWinner);
    }

    private void GoToResults() => 
        FindFirstObjectByType<NavigationController>().GoTo(Screens.Results, this.gameObject);

    public void Return()
    {
        FindFirstObjectByType<NavigationController>().GoTo(Screens.Loading, this.gameObject);
        SceneLoader.Instance.LoadScene(0, true);
    }

    private string GetReasonText(bool isWinner, MatchEndReason reason)
    {
        switch (reason)
        {
            case MatchEndReason.RitualCompleted:
                return isWinner
                    ? "You completed the ritual!"
                    : "Your opponent completed the ritual";

            case MatchEndReason.CorruptionOverflow:
                return isWinner
                    ? "Your opponent succumbed to corruption"
                    : "You succumbed to corruption";

            case MatchEndReason.Disconnection:
                return isWinner
                    ? "Your opponent disconnected"
                    : "You lost connection";

            default:
                return string.Empty;
        }
    }

    private void UpdateProgressionBar(float prevXP, float nextXP)
    {
        StopAllCoroutines();
        StartCoroutine(GainAnimationCoroutine(prevXP, nextXP));
    }

    IEnumerator GainAnimationCoroutine(float prevXP, float nextXP)
    {
        int xpEarned = Mathf.RoundToInt((nextXP - prevXP) * XPManager.Instance.XPPerRank);
        Color pointsColor = RuntimeVariables.Instance.CurrentCult.Color;
        earnedXPText.text = earnedXPText.text.Replace("<points>",
            Utils.ApplyColorToText(xpEarned.ToString() + " Devotion Points", pointsColor));
        while (prevXP != nextXP)
        {
            prevXP = Mathf.MoveTowards(prevXP, nextXP, Time.deltaTime * animSpeed);
            progressionBar.DisplayXP(prevXP);
            yield return null;
        }
    }
}