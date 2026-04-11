using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CastingCard : MonoBehaviour
{
    [SerializeField] private Sprite placeholderSprite;
    [SerializeField] private Image selectedCardImage;
    [SerializeField] private CardUIManager cardUIManager;
    private readonly Dictionary<CardUI, float> progressDictionary = new();
    private Image image;
    private CardDissolveEffect dissolveEffect;

    private void Awake()
    {
        if (!TryGetComponent(out dissolveEffect)) 
            Debug.LogError("Error: falta el componente CardDissolveEffect");
        image = GetComponent<Image>();
        image.sprite = placeholderSprite;
    }

    private void Start()
    {
        foreach(var card in cardUIManager.GetComponentsInChildren<CardUI>())
        {
            card.OnIdxChanged += OnCardUpdated;
        }
    }

    private void OnDestroy()
    {
        foreach (var card in cardUIManager.GetComponentsInChildren<CardUI>())
        {
            card.OnIdxChanged -= OnCardUpdated;
        }
    }

    private void OnCardUpdated(CardUI card, float progress, bool canBeCasted)
    {
        if (!canBeCasted) return;
        progressDictionary[card] = progress;
        float max = progressDictionary.Values.Max();
        dissolveEffect.SetDissolve(1 - max, true, 1);
        if (Mathf.Approximately(progressDictionary[card], 1))
        {
            dissolveEffect.FadeInAndOut(0.5f, 1, () => image.sprite = card.CardDefinition.Image);
        }
    }
}
