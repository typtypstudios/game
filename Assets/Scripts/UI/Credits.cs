using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] private float creditsTime = 30f;
    [SerializeField] private float finalYPos;
    private RectTransform rt;
    private Vector2 initPos;
    private WritableButton[] buttons;
    private int currentButtonIdx = 0;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        initPos = rt.anchoredPosition;
        buttons = GetComponentsInChildren<WritableButton>();
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].Block = true;
            buttons[i].GetComponent<Button>().interactable = false;
            buttons[i].GetComponent<Button>().onClick.AddListener(OnButtonWritten);
        }
    }

    public void PlayCredits()
    {
        StopAllCoroutines();
        StartCoroutine(PlayCreditsCoroutine());
    }

    public void StopCredits()
    {
        StopAllCoroutines();
        foreach (var b in buttons)
        {
            b.ResetButton(true);
            b.Block = true;
        }
        currentButtonIdx = 0;
    }

    public void ContactUs()
    {
        Application.OpenURL("https://linktr.ee/typtypstudios");
    }

    private void OnButtonWritten()
    {
        buttons[currentButtonIdx++].Block = true;
        if (currentButtonIdx >= buttons.Length) return;
        buttons[currentButtonIdx].Block = false;
    }

    IEnumerator PlayCreditsCoroutine()
    {
        buttons[0].Block = false;
        rt.anchoredPosition = initPos;
        float creditsSpeed = (finalYPos - initPos.y) / creditsTime;
        while (rt.anchoredPosition.y < finalYPos)
        {
            rt.anchoredPosition += new Vector2(0, creditsSpeed * Time.deltaTime);
            yield return null;
        }
        StopCredits();
    }
}
