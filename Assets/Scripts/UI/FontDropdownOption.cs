using TMPro;
using UnityEngine;
using System.Collections;
using TypTyp.TextSystem.Typable;

public class FontDropdownOption : MonoBehaviour
{
    [SerializeField] private float transitionTime = 0.5f;
    private float transitionSpeed;
    private RectTransform rt;
    private Vector2 initPos = Vector3.zero;
    private FontDropdown dropdown;
    private WritableButton writableButton;
    private int fontIdx;
    private bool activated = false;
    private bool completedByText = false;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        dropdown = GetComponentInParent<FontDropdown>();
        writableButton = GetComponent<WritableButton>();
        initPos = rt.anchoredPosition;
        rt.anchoredPosition = Vector2.zero;
        transitionSpeed = initPos.magnitude / transitionTime;
        GetComponent<TypableController>().OnComplete += () => completedByText = true;
    }

    public void Initialize(TMP_FontAsset font, int idx)
    {
        writableButton.OverrideText(font.name);
        GetComponentInChildren<TextMeshProUGUI>().font = font;
        fontIdx = idx;
    }

    public void ToggleActivation()
    {
        StopAllCoroutines();
        if (!activated)
        {
            gameObject.SetActive(true);
            StartCoroutine(TransitionCoroutine(initPos));
        }
        else StartCoroutine(TransitionCoroutine(Vector2.zero, true));
        activated = !activated;
    }

    public void OnButtonClick() 
    {
        dropdown.SetFont(fontIdx);
        transform.SetAsLastSibling();
        if(completedByText) dropdown.ToggleSelection();
        completedByText = false;
    }

    IEnumerator TransitionCoroutine(Vector2 targetPos, bool hiding = false)
    {
        writableButton.Block = true;
        while (targetPos != rt.anchoredPosition)
        {
            rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, targetPos, transitionSpeed * Time.deltaTime);
            yield return null;
        }
        if (hiding)
        {
            gameObject.SetActive(false);
            if (dropdown.ChosenOption == this) dropdown.DisplayFontInfo();
        }
        writableButton.Block = false;
    }
}
