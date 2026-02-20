using UnityEngine;
using TMPro;

public class RitualController : AInputListener
{
    [SerializeField] private TMP_Text filledText;
    [SerializeField] private bool dottedSpaces;
    private TMP_Text ritualText;
    private int idx = 0;
    bool isErrorDisplayed = false;

    void Awake()
    {
        ritualText = GetComponent<TMP_Text>();
        if(dottedSpaces) ritualText.text = ritualText.text.Replace(" ", "·");
    }

    readonly string colorTag = "<color #FF0000>";
    protected override void ProcessInput(char c)
    {
        if (idx == ritualText.text.Length) return;
        if (dottedSpaces && c == ' ') c = '·'; 
        if (c == ritualText.text[idx])
        {
            filledText.text += c;
            idx++;
            if (isErrorDisplayed)
            {
                ritualText.text = ritualText.text.Replace("</color>", "").Replace(colorTag, "");
                idx -= colorTag.Length;
                isErrorDisplayed = false;
            }
        }
        else if(!isErrorDisplayed)
        {
            string original = ritualText.text;
            ritualText.text = original[..idx] + colorTag + 
                original[idx] + "</color>" + original[(idx + 1)..];
            idx += colorTag.Length;
            isErrorDisplayed = true;
        }
    }
}