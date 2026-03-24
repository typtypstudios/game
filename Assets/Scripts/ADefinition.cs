using UnityEngine;

public class ADefinition : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }
}
