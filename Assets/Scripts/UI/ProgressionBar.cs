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
    private string originalPointsLeftText;

    private void Awake()
    {
        originalPointsLeftText = devotionPointsLeft.text;
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
}
