using TMPro;
using UnityEngine;

namespace TypTyp
{
    [CreateAssetMenu(menuName = "TypTyp/Settings")]
    public class Settings : ScriptableSingleton<Settings>
    {
        [Header("TAGS & IDs")]
        [field: SerializeField] public int P1_ID { get; private set; } = 1;
        [field: SerializeField] public int P2_ID { get; private set; } = 2;
        [field: SerializeField] public string P1_tag { get; private set; } = "Player1";
        [field: SerializeField] public string P2_tag { get; private set; } = "Player2";

        [Header("CORRUPTION RELATED")]
        //Maxima cantidad de corrupci�n para perder
        [field: SerializeField] public float MaxCorruption { get; private set; } = 100;
        [field: SerializeField] public float AutoHealPercentage { get; private set; } = 0.2f;
        [field: SerializeField] public float TimeToAutoHeal { get; private set; } = 3f;
        [field: SerializeField] public float AutoHealInterval { get; private set; } = 0.5f;
        //Porcentaje de penalizaci�n de fallos. Ha de estar en el rango [0, 100]
        [field: SerializeField] public float MistakePenalizationPercentage { get; private set; } = 1.0f;

        [Header("MANA RELATED")]
        //M�xima cantidad de man� 
        [field: SerializeField] public float MaxMana { get; private set; } = 100;
        [field: SerializeField] public int NumManaBars { get; private set; } = 5;
        //Ratio obtenci�n man� - progreso. Cuanto m�s alto, m�s man� se gana recitando el ritual
        [field: SerializeField] public float ManaGain { get; private set; } = 5;

        [Header("RITUAL RELATED")]
        //Textos a completar para el ritual
        [field: SerializeField] public int MaxTextsProvided { get; private set; } = 10;
        [field: SerializeField] public bool ShowSpaces { get; set; } = true;
        [field: SerializeField] public bool CapsLockWarning { get; set; } = true;

        [Header("DECK RELATED")]
        //Textos a completar para el ritual
        [field: SerializeField] public int DeckSize { get; private set; } = 6;

        [Header("SPELL RELATED")]
        //Textos a completar para el ritual
        [field: SerializeField] public int HandSize { get; private set; } = 3;

        [Header("Settings")]
        [field: SerializeField] public TMP_FontAsset DefaultFont { get; set; }
    }
}
