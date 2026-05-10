using System.Collections;
using TMPro;
using TypTyp.Cults;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ProgressionBar : MonoBehaviour
{
    [SerializeField] private Slider XPSlider;
    [SerializeField] private GameObject bar;
    [SerializeField] private GameObject maxLevelLabel;
    [SerializeField] private TMP_Text prevLvlText;
    [SerializeField] private TMP_Text nextLvlText;
    [SerializeField] private TMP_Text devotionPointsLeft;
    [SerializeField] private Image fillArea;
    [Header("Animation:")]
    [SerializeField] private float animSpeed = 0.1f;
    private string originalPointsLeftText;

    private void Awake()
    {
        originalPointsLeftText = devotionPointsLeft.text;
        XPManager.Instance.OnXPUpdated += ProcessXPUpdate;
    }

    private void OnDestroy()
    {
        XPManager.Instance.OnXPUpdated -= ProcessXPUpdate;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopUI(UISound.XPGain, fadeDuration: 1f);
        }
    }

    public void DisplayXP(float xp)
    {
        CultDefinition cult = RuntimeVariables.Instance.CurrentCult;
        int lvl = Mathf.FloorToInt(xp);
        int nextlvl = Mathf.Min(lvl + 1, cult.RankNames.Length - 1);
        nextLvlText.text = nextlvl.ToString();
        int currentlvl = nextlvl - 1;
        prevLvlText.text = (currentlvl).ToString();
        XPSlider.value = xp - currentlvl;
        bar.SetActive(lvl < nextlvl);
        maxLevelLabel.SetActive(lvl >= nextlvl);
        int pointsLeft = Mathf.RoundToInt((nextlvl - xp) * XPManager.Instance.XPPerRank);
        devotionPointsLeft.text = originalPointsLeftText.Replace("<value>", pointsLeft.ToString());
    }

    private void ProcessXPUpdate(float prevXP, float newXP)
    {
        StopAllCoroutines();
        StartCoroutine(GainAnimationCoroutine(prevXP, newXP));
    }

    IEnumerator GainAnimationCoroutine(float prevXP, float nextXP)
    {
        AudioManager.Instance.PlayUI(UISound.XPGain);
        while (prevXP != nextXP)
        {
            prevXP = Mathf.MoveTowards(prevXP, nextXP, Time.deltaTime * animSpeed);
            DisplayXP(prevXP);
            yield return null;
        }
        AudioManager.Instance.StopUI(UISound.XPGain, fadeDuration: 2f);
    }
}