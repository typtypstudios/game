using TMPro;
using TypTyp.Cults;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ProgressionBar : MonoBehaviour
{
    private Slider XPSlider;
    [SerializeField] private TMP_Text prevLvlText;
    [SerializeField] private TMP_Text nextLvlText;

    private void Awake()
    {
        XPSlider = GetComponent<Slider>();
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
    }
}
