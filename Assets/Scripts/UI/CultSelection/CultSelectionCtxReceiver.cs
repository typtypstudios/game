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

    public void ReceiveContext(Screens prevScreen, bool isGoingBack, GameObject sender = null)
    {
        CultSelectionConfig config = new();
        if ((prevScreen == Screens.DeckBuilder && !isGoingBack)
            || (prevScreen == Screens.MainMenu && sender.TryGetComponent(out NavigationObject _)))
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
                labelInfo = "Choose a cult to battle with!",
                OnCultChosen = () =>
                {
                    NavigationController c = FindFirstObjectByType<NavigationController>();
                    CanvasTransitionManager t = c.GetComponent<CanvasTransitionManager>();
                    c.GoTo(Screens.Loading, this.gameObject);
                    FindFirstObjectByType<MainMenuManager>().Play(t.TransitionTime);
                },
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
