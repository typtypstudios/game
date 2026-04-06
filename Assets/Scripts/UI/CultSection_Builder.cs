using TMPro;
using TypTyp.Cults;
using UnityEngine;
using UnityEngine.UI;

public class CultSection_Builder : MonoBehaviour
{
    [SerializeField] private TMP_Text cultName;
    [SerializeField] private TMP_Text rankName;
    [SerializeField] private TMP_Text currentLevel;
    [SerializeField] private TMP_Text nextLevel;
    [SerializeField] private Slider lvlSlider;

    private void Awake() => SaveManager.Instance.OnAfterLoad += UpdateInfo;

    private void OnDestroy() => SaveManager.Instance.OnAfterLoad -= UpdateInfo;

    private void UpdateInfo(SaveState state)
    {
        CultDefinition cult = RuntimeVariables.Instance.CurrentCult;
        cultName.text = cult.Name;
        int lvl = Mathf.FloorToInt(RuntimeVariables.Instance.CurrentLevel);
        rankName.text = cult.RankNames[lvl];
        int nextlvl = Mathf.Min(lvl + 1, cult.RankNames.Length - 1);
        nextLevel.text = nextlvl.ToString();
        int currentlvl = nextlvl - 1;
        currentLevel.text = (currentlvl).ToString();
        lvlSlider.value = RuntimeVariables.Instance.CurrentLevel - currentlvl;
    }
}
