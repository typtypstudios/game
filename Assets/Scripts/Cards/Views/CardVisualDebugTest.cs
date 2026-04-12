using UnityEngine;

public class CardVisualDebugTest : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private CardVisualPresenter presenter;

    [Header("Test Data")]
    [SerializeField] private CardDefinition card;
    [SerializeField, Min(0)] private int resolvedCost = 1;
    [SerializeField, Min(0)] private int currentMana = 1;

    [ContextMenu("Apply Test Card")]
    public void ApplyTestCard()
    {
        if (!presenter)
        {
            Debug.LogWarning($"{nameof(CardVisualDebugTest)}: Missing {nameof(CardVisualPresenter)} reference.", this);
            return;
        }

        if (!card)
        {
            Debug.LogWarning($"{nameof(CardVisualDebugTest)}: Missing {nameof(CardDefinition)} test card.", this);
            return;
        }

        presenter.SetCard(card, resolvedCost, currentMana);
    }

    [ContextMenu("Apply Only Mana")]
    public void ApplyOnlyMana()
    {
        if (!presenter)
        {
            Debug.LogWarning($"{nameof(CardVisualDebugTest)}: Missing {nameof(CardVisualPresenter)} reference.", this);
            return;
        }

        presenter.SetMana(currentMana);
    }

    [ContextMenu("Apply Only Cost")]
    public void ApplyOnlyCost()
    {
        if (!presenter)
        {
            Debug.LogWarning($"{nameof(CardVisualDebugTest)}: Missing {nameof(CardVisualPresenter)} reference.", this);
            return;
        }

        presenter.SetResolvedCost(resolvedCost);
    }

    [ContextMenu("Clear Test Card")]
    public void ClearTestCard()
    {
        if (!presenter)
        {
            Debug.LogWarning($"{nameof(CardVisualDebugTest)}: Missing {nameof(CardVisualPresenter)} reference.", this);
            return;
        }

        presenter.Clear();
    }
}
