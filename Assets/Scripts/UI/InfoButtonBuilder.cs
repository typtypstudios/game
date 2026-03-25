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
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.parent.position);
        float newX = Mathf.Abs(transform.localPosition.x) * (screenPos.x > Screen.width * 0.5f ? -1 : 1);
        this.transform.localPosition = new(newX, transform.localPosition.y, transform.localPosition.z);
    }

    private void OnButtonClick()
    {
        builderCanvas.enabled = false;
        grimoireContent.GoToDefinition(card.Name);
        grimoireRetargeter.SetTarget(builderCanvas);
        grimoireCanvas.enabled = true;
    }
}