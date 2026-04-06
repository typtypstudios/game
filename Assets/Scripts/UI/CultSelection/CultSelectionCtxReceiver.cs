using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CultSelectionController))]
public class CultSelectionCtxReceiver : MonoBehaviour, INavigationCtxReceiver
{
    private CultSelectionController controller;

    private void Awake()
    {
        controller = GetComponent<CultSelectionController>();
    }

    public void ReceiveContext(Screens prevScreen, bool isGoingBack)
    {
        CultSelectionConfig config = new();
        if (prevScreen == Screens.DeckBuilder && !isGoingBack)
        {
            config = new()
            {
                labelInfo = "Choose your new cult!",
                OnCultChosen = () => FindFirstObjectByType<NavigationController>().GoBack(),
                showEquipmentButtons = false
            };
        }
        else
        {
            config = new()
            {
                labelInfo = "Choose your cult!",
                OnCultChosen = () => FindFirstObjectByType<MainMenuManager>().Play(),
                showEquipmentButtons = true
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
