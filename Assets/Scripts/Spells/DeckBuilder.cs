using UnityEngine;

public class DeckBuilder : MonoBehaviour
{
    [SerializeField] private int numEquipped = 6;
    [SerializeField] private GameObject equippedCardPrefab;
    [SerializeField] private Transform equippedTransform;
}
