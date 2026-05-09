using TMPro;
using TypTyp.TextSystem.Typable;
using UnityEngine;

/// <summary>
/// Esta solución es una puta chapuza, pero no me apetece tocar nada de los typables y estoy quemado.
/// </summary>
[RequireComponent(typeof(TypableController))]
[RequireComponent(typeof(TMP_Text))]
public class PracticeRitual : MonoBehaviour
{
    [SerializeField] private string text;
    private Canvas parentCanvas;
    private TypableController controller;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        controller = GetComponent<TypableController>();
    }

    public void ResetText()
    {
        controller.SetText(text);
    }

    void Update() //No sé si ponerlo en Start dará problemas, ni quiero saberlo
    {
        if (!parentCanvas.enabled) return;
        controller.SetText(text);
        this.enabled = false;
    }
}
