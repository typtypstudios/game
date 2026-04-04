using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image hoverImage;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(0.6f, 0.1f, 0.2f); // tu rojo

    [SerializeField] private float speed = 5f;

    private float targetFill = 0f;

    private void Update()
    {
        hoverImage.fillAmount = Mathf.MoveTowards(
            hoverImage.fillAmount,
            targetFill,
            speed * Time.deltaTime
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetFill = 1f;
        text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetFill = 0f;
        text.color = normalColor;
    }
}