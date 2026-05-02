using System.Collections;
using TypTyp.TextSystem;
using UnityEngine;

[RequireComponent(typeof(CardUI))]
public class CardUIChangeAnimation : MonoBehaviour
{
    [SerializeField] private float animationTime = 0.2f;
    [SerializeField] private CardDissolveEffect dissolveEffect;
    [SerializeField] private CanvasGroup nameAndCostGroup;
    private CardUI cardUI;

    void Awake()
    {
        cardUI = GetComponent<CardUI>();
    }

    public void PerformChange(CardDefinition def, ITextPipeline pipeline, int costModifier = 0)
    {
        dissolveEffect.Blink(animationTime, () => cardUI.UpdateCard(def, pipeline, costModifier));
        StopAllCoroutines();
        StartCoroutine(PerformGroupTransition());
    }

    IEnumerator PerformGroupTransition()
    {
        float speed = 2 / animationTime;
        while(nameAndCostGroup.alpha > 0)
        {
            nameAndCostGroup.alpha -= speed * Time.deltaTime;
            yield return null;
        }
        nameAndCostGroup.alpha = 0;
        while (nameAndCostGroup.alpha < 1)
        {
            nameAndCostGroup.alpha += speed * Time.deltaTime;
            yield return null;
        }
    }
}
