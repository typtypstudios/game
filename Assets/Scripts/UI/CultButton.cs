using System.Collections.Generic;
using TMPro;
using TypTyp;
using UnityEngine;
using UnityEngine.UI;

public class CultButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text levelTMP;
    [SerializeField] private GameObject displayerPrefab;
    [SerializeField] private Transform deckTransform;
    private List<InfoDisplayer> displayers = new();
    private WritableButton writableButton;
    private string defaultLevelText;

    private void Awake()
    {
        writableButton = GetComponent<WritableButton>();
        defaultLevelText = levelTMP.text;
        InstantiateDisplayers();
    }

    private void InstantiateDisplayers()
    {
        for(int i = 0; i < Settings.Instance.DeckSize; i++)
        {
            GameObject displayerGO = Instantiate(displayerPrefab, deckTransform);
            displayers.Add(displayerGO.GetComponent<InfoDisplayer>());
        }
    }

    public void SetCultInfo(CultRuntimeInfo cultInfo)
    {
        writableButton.OverrideText(cultInfo.cult.Name);
        //FALTA IMAGEN DE CULTO
        int cultLevel = cultInfo.level;
        levelTMP.text = defaultLevelText.Replace("<level>", cultLevel.ToString()).
            Replace("<rankName>", cultInfo.cult.RankNames[cultLevel]);
        for (int i = 0; i < displayers.Count; i++)
        {
            int cardID = cultInfo.equippedCards.Count > 0 ? cultInfo.equippedCards[i] : i;
            displayers[i].SetInfo(CardRegister.Instance.GetById(cardID));
        }
    }
}
