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

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    void Update() //No sé si ponerlo en Start dará problemas, ni quiero saberlo
    {
        if (!parentCanvas.enabled) return;
        GetComponent<TypableController>().SetText(text);
        this.enabled = false;
    }
}
