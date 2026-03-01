using UnityEngine;

public class Deck : ScriptableObject
{
    [field: SerializeField] public CardDefinition[] Cards { get; private set; }
    
}