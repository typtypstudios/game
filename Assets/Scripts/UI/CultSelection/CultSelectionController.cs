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
        //De momento no hay botones de ir a editar mazo, hay que ver cómo solucionar el hecho de que 
        //no se puedan escribir si en los tres pone lo mismo
        foreach(var button in buttons) button.UpdateInfo();
    }

    public void SetCult(int cultId)
    {
        SaveState state = SaveManager.Instance.GetState();
        state.slot.cultId = cultId;
        SaveManager.Instance.Save(false);
        SaveManager.Instance.Load();
        OnCultChosen?.Invoke();
        OnCultChosen = null;
    }
}
