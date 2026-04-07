using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CultSelectionController : MonoBehaviour
{
    [SerializeField] private GameObject cultButtonPrefab;
    [SerializeField] private Transform cultButtonsParent;
    [SerializeField] private TMP_Text labelTMP;
    private readonly List<CultButton> buttons = new();
    private Action OnCultChosen;

    void Start()
    {
        foreach(var cultInfo in RuntimeVariables.Instance.CultsInfo)
        {
            CultButton cultButton = Instantiate(cultButtonPrefab, cultButtonsParent).GetComponent<CultButton>();
            buttons.Add(cultButton);
            cultButton.SetCultInfo(cultInfo);
        }
    }

    public void SetConfiguration(CultSelectionConfig config)
    {
        labelTMP.text = config.labelInfo;
        OnCultChosen = config.OnCultChosen;
        foreach (var button in buttons)
        {
            button.UpdateInfo();
            button.ToggleEditButton(config.showEquipmentButtons);
        }
    }

    public void SetCult(int cultId, bool triggerAction = true)
    {
        SaveState state = SaveManager.Instance.GetState();
        state.slot.cultId = cultId;
        SaveManager.Instance.Save(false);
        SaveManager.Instance.Load();
        if (triggerAction)
        {
            OnCultChosen?.Invoke();
            OnCultChosen = null;
        }
    }
}
