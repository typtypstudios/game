using UnityEngine;
using TMPro;
using System.Linq;

public class RitualController : MonoBehaviour
{
    [SerializeField] private TMP_Text filledText;
    [SerializeField] private bool dottedSpaces;
    private TMP_Text ritualText;
    private int currentIndex = 0;
    bool isErrorDisplayed = false;

    void Awake()
    {
        ritualText = GetComponent<TMP_Text>();
        if(dottedSpaces) ritualText.text = ritualText.text.Replace(" ", "·");
    }

    private void OnEnable()
    {
        InputHandler.Instance.AddListener(ProcessInput);
    }

    private void OnDisable()
    {
        InputHandler.Instance.RemoveListener(ProcessInput);
    }

    readonly string colorTag = "<color #FF0000>";
    private void ProcessInput(char c)
    {
        if (currentIndex == ritualText.text.Length) return;
        if (dottedSpaces && c == ' ') c = '·'; 
        if (c == ritualText.text[currentIndex])
        {
            filledText.text += c;
            currentIndex++;
            if (isErrorDisplayed)
            {
                ritualText.text = ritualText.text.Replace("</color>", "").Replace(colorTag, "");
                currentIndex -= colorTag.Length;
                isErrorDisplayed = false;
            }
        }
        else if(!isErrorDisplayed)
        {
            string original = ritualText.text;
            ritualText.text = original[..currentIndex] + colorTag + 
                original[currentIndex] + "</color>" + original[(currentIndex + 1)..];
            currentIndex += colorTag.Length;
            isErrorDisplayed = true;
        }
    }
}