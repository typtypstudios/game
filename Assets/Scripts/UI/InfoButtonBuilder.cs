using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InfoButtonBuilder : MonoBehaviour
{
    private Canvas builderCanvas;
    private Canvas grimoireCanvas;
    private BackButtonRetargeter grimoireRetargeter;
    private GrimoireContentManager grimoireContent;
    private Button button;
    private CardDefinition card;

    private void Awake()
    {
        builderCanvas = GetComponentInParent<Canvas>();
        grimoireContent = FindFirstObjectByType<GrimoireContentManager>();
        grimoireCanvas = grimoireContent.GetComponentInParent<Canvas>();
        grimoireRetargeter = grimoireCanvas.GetComponentInChildren<BackButtonRetargeter>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnEnable()
    {
        card = GetComponentInParent<BuilderDisplayer>().Card;
    }

    private void OnButtonClick()
    {
        builderCanvas.enabled = false;
        grimoireContent.GoToDefinition(card.Name);
        grimoireRetargeter.SetTarget(builderCanvas);
        grimoireCanvas.enabled = true;
    }
}