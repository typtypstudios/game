using TypTyp;
using UnityEngine;

public class DeckBuilder : MonoBehaviour
{
    [SerializeField] private GameObject equippedCardPrefab;
    [SerializeField] private Transform equippedTransform;

    private void Awake()
    {
        for(int i = 0; i < Settings.Instance.DeckSize; i++) 
            Instantiate(equippedCardPrefab, equippedTransform);
    }
}
