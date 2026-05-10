using System.Collections;
using TMPro;
using UnityEngine;

public class EndGameCanvas : MonoBehaviour, INavigationCtxReceiver
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text resultReasonText;
    [SerializeField] private GameObject exitButton;

    [Header("XP Related")]
    [SerializeField] private TMP_Text earnedXPText;
    [SerializeField] private ProgressionBar progressionBar;
    [SerializeField] private float animSpeed = 1;
    private bool isWinner;

    private void Awake()
    {
        exitButton.SetActive(false);

        resultReasonText.text = "";

        XPManager.Instance.OnXPUpdated += UpdateProgressionBar;
    }

    private void OnDisable() => XPManager.Instance.OnXPUpdated -= UpdateProgressionBar;

    public void ShowEndMatch(bool isWinner, MatchEndReason reason)
    {
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
        this.isWinner = isWinner;
        earnedXPText.gameObject.SetActive(isWinner);
        progressionBar.gameObject.SetActive(isWinner);
    }

    public void ReceiveContext(Screens prevScreen, bool isGoingBack, GameObject sender = null)
    {
        CanvasTransitionManager.OnDissolved += ProcessVictory;
    }

    private void ProcessVictory()
    {
        XPManager.Instance.ProcessVictory();
        CanvasTransitionManager.OnDissolved -= ProcessVictory;
    }

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