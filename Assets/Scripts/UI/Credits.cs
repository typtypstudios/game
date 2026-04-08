using System.Collections;
using UnityEngine;
using TypTyp.TextSystem.Typable;
using TypTyp.Input;
using UnityEngine.Events;

public class Credits : MonoBehaviour
{
    [SerializeField] private float creditsTime = 30f;
    [SerializeField] private float finalYPos;
    private RectTransform rt;
    private Vector2 initPos;
    private TypableController[] typables;
    private TypingInputListener[] listeners;
    private int currentButtonIdx = 0;

    public UnityEvent OnCreditsAchieved;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        initPos = rt.anchoredPosition;
        typables = GetComponentsInChildren<TypableController>(true);
        listeners = GetComponentsInChildren<TypingInputListener>(true);
        for (int i = 0; i < typables.Length; i++)
        {
            typables[i].OnComplete += OnButtonWritten;
            typables[i].enabled = false;
        }
        for (int i = 0; i < listeners.Length; i++)
        {
            listeners[i].enabled = false;
        }
    }

    public void PlayCredits()
    {
        StopCredits();
        StopAllCoroutines();
        StartCoroutine(PlayCreditsCoroutine());
    }

    public void StopCredits()
    {
        StopAllCoroutines();
        foreach (var t in typables)
        {
            t.enabled = false;
            t.ResetText();
        }
        foreach (var l in listeners)
        {
            l.enabled = false;
        }
        currentButtonIdx = 0;
    }

    public void ContactUs()
    {
        Application.OpenURL("https://linktr.ee/typtypstudios");
    }

    private void OnButtonWritten()
    {
        typables[currentButtonIdx].enabled = false;
        listeners[currentButtonIdx++].enabled = false;
        if (currentButtonIdx >= typables.Length)
        {
            LastButtonWritten();
            return;
        }
        typables[currentButtonIdx].enabled = true;
        listeners[currentButtonIdx].enabled = true;
    }

    IEnumerator PlayCreditsCoroutine()
    {
        if (typables.Length == 0) yield break;
        typables[0].enabled = true;
        listeners[0].enabled = true;
        rt.anchoredPosition = initPos;
        float creditsSpeed = (finalYPos - initPos.y) / creditsTime;
        while (rt.anchoredPosition.y < finalYPos)
        {
            rt.anchoredPosition += new Vector2(0, creditsSpeed * Time.deltaTime);
            yield return null;
        }
        StopCredits();
    }

    void LastButtonWritten()
    {
        OnCreditsAchieved?.Invoke();
    }
}
