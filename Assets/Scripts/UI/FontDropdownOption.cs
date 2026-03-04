using TMPro;
using UnityEngine;
using System.Collections;

public class FontDropdownOption : MonoBehaviour
{
    [SerializeField] private float transitionSpeed = 20f;
    private RectTransform rt;
    private Vector2 initPos = Vector3.zero;
    private FontDropdown dropdown;
    private WritableButton writableButton;
    private int fontIdx;
    private bool clicked = false;

    private void OnEnable()
    {
        if (!rt) return; //Significa que todavía nos e hizo el Initialize
        StartCoroutine(TransitionCoroutine(initPos));
        writableButton.Block = false;
    }

    public void Initialize(TMP_FontAsset font, int idx)
    {
        rt = GetComponent<RectTransform>();
        initPos = rt.anchoredPosition;
        rt.anchoredPosition = Vector2.zero;
        dropdown = GetComponentInParent<FontDropdown>();
        writableButton = GetComponent<WritableButton>();
        writableButton.OverrideText(font.name);
        GetComponentInChildren<TextMeshProUGUI>().font = font;
        fontIdx = idx;
    }

    public void OnButtonClick()
    {
        dropdown.SetFont(fontIdx);
        clicked = true;
        foreach (var option in dropdown.Options)
            option.StartCoroutine(option.TransitionCoroutine(Vector2.zero, true));
        transform.SetAsLastSibling();
    }

    IEnumerator TransitionCoroutine(Vector2 targetPos, bool disableAfter = false)
    {
        writableButton.Block = true;
        while (targetPos != rt.anchoredPosition)
        {
            rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, targetPos, transitionSpeed * Time.deltaTime);
            yield return null;
        }
        if (clicked)
        {
            clicked = false;
            dropdown.ToggleSelection();
        }
        if (disableAfter) gameObject.SetActive(false);
    }
}
