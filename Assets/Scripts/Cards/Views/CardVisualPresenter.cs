using UnityEngine;

public class CardVisualPresenter : MonoBehaviour
{
    [SerializeField] private CardVisualTheme cardVisualTheme;
    [SerializeField] private MonoBehaviour cardViewBehaviour;

    private ICardView cardView;
    private CardDefinition currentCard;
    private int manaCost;
    private int currentMana;

    private void Awake()
    {
        cardView = cardViewBehaviour as ICardView;
        if (cardViewBehaviour && cardView == null)
        {
            Debug.LogError($"{nameof(CardVisualPresenter)} expected an {nameof(ICardView)} in cardViewBehaviour.", this);
        }
    }

    public void SetCard(CardDefinition card, int resolvedManaCost, int availableMana)
    {
        currentCard = card;
        manaCost = Mathf.Max(0, resolvedManaCost);
        currentMana = Mathf.Max(0, availableMana);
        Refresh();
    }

    public void SetMana(int availableMana)
    {
        currentMana = Mathf.Max(0, availableMana);
        Refresh();
    }

    public void SetResolvedCost(int resolvedManaCost)
    {
        manaCost = Mathf.Max(0, resolvedManaCost);
        Refresh();
    }

    public void Refresh()
    {
        if (cardView == null || !cardVisualTheme || currentCard == null)
        {
            return;
        }

        CardVisualState visualState = CardVisualStateBuilder.Build(currentCard, cardVisualTheme, manaCost, currentMana);
        cardView.Apply(visualState);
    }

    public void Clear()
    {
        currentCard = null;
        cardView?.Clear();
    }
}
