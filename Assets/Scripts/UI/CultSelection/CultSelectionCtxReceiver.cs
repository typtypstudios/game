using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CultSelectionController))]
public class CultSelectionCtxReceiver : MonoBehaviour, INavigationCtxReceiver
{
    private CultSelectionController controller;
    private Stack<CultSelectionConfig> configStack = new();

    private void Awake()
    {
        controller = GetComponent<CultSelectionController>();
    }

    public void ReceiveContext(Screens prevScreen, bool isGoingBack)
    {
        CultSelectionConfig config = new();
        if(prevScreen == Screens.MainMenu)
        {
            config = new()
            {
                labelInfo = "Choose your cult!",
                OnCultChosen = () => FindFirstObjectByType<MainMenuManager>().Play(),
                showEquipmentButtons = true
            };
        }
        else if(prevScreen == Screens.DeckBuilder)
        {
            config = new()
            {
                labelInfo = "Choose your new cult!",
                OnCultChosen = () => FindFirstObjectByType<NavigationController>().GoBack(),
                showEquipmentButtons = false
            };
        }
        controller.SetConfiguration(config);
    }
}

public struct CultSelectionConfig
{
    public string labelInfo;
    public Action OnCultChosen;
    public bool showEquipmentButtons;
}
