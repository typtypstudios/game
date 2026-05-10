using TMPro;
using TypTyp.TextSystem.Typable;
using UnityEngine;

[RequireComponent(typeof(TypableController))]
[RequireComponent(typeof(TMP_Text))]
public class PracticeRitual : MonoBehaviour
{
    [SerializeField] private string text;
    private TypableController controller;

    private void Awake()
    {
        controller = GetComponent<TypableController>();
    }

    void OnEnable()
    {
        controller.SetText(text);
    }

    public void ResetText()
    {
        controller.SetText(text);
    }
}
