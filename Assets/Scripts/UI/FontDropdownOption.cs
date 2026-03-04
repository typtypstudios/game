using TMPro;
using UnityEngine;

public class FontDropdownOption : MonoBehaviour
{
    private FontDropdown dropdown;
    private WritableButton writableButton;
    private int fontIdx;

    public void Initialize(TMP_FontAsset font, int idx)
    {
        dropdown = GetComponentInParent<FontDropdown>();
        writableButton = GetComponent<WritableButton>();
        writableButton.OverrideText(font.name);
        GetComponentInChildren<TextMeshProUGUI>().font = font;
        fontIdx = idx;
    }

    public void OnButtonClick() => dropdown.SetFont(fontIdx);
}
