using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InfoButtonBuilder : MonoBehaviour
{
    private GrimoireContentManager grimoireContent;
    private Button button;
    private CardDefinition card;

    private void Awake()
    {
        grimoireContent = FindFirstObjectByType<GrimoireContentManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnEnable()
    {
        card = GetComponentInParent<BuilderDisplayer>().Card;
    }

    private void OnButtonClick()
    {
        grimoireContent.GoToDefinition(card.Name);
    }
}