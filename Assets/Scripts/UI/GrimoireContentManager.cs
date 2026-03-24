using UnityEngine;
using System.Linq;

public class GrimoireContentManager : MonoBehaviour
{
    private GrimoireInfoDisplayer[] displayers;
    private CardDefinition[] defaultCards;
    private StatusEffectDefinition[] effects;
    
    private void Awake()
    {
        displayers = GetComponentsInChildren<GrimoireInfoDisplayer>();
        defaultCards = CardRegister.Instance.RegisteredItems.OrderBy(c => c.CardName).ToArray();
        effects = StatusEffectRegister.Instance.RegisteredItems.OrderBy(e => e.EffectName).ToArray();
    }

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i < defaultCards.Length) 
            {
                displayers[i].SetCard(defaultCards[i]); 
                displayers[i].gameObject.SetActive(true);
            }
            else displayers[i].gameObject.SetActive(false);
        }
    }
}
