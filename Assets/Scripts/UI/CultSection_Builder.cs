using TMPro;
using TypTyp.Cults;
using UnityEngine;

public class CultSection_Builder : MonoBehaviour
{
    [SerializeField] private TMP_Text cultName;
    [SerializeField] private TMP_Text rankName;
    [SerializeField] private ProgressionBar progressionBar;

    private void Awake()
    {
        SaveManager.Instance.OnAfterLoad += UpdateInfo;
    }

    private void Start()
    {
        if (RuntimeVariables.Instance.IsLoaded) UpdateInfo(SaveManager.Instance.GetState());
    }

    private void OnDestroy() => SaveManager.Instance.OnAfterLoad -= UpdateInfo;

    private void UpdateInfo(SaveState state)
    {
        CultDefinition cult = RuntimeVariables.Instance.CurrentCult;
        cultName.text = cult.Name;
        int lvl = Mathf.FloorToInt(RuntimeVariables.Instance.CurrentLevel);
        rankName.text = cult.RankNames[lvl];
        progressionBar.DisplayXP(RuntimeVariables.Instance.CurrentLevel);
    }
}
