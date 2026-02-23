using UnityEngine;

namespace TypTyp
{
    [CreateAssetMenu(menuName = "TypTyp/Settings")]
    public class Settings : ScriptableSingleton<Settings>
    {
        [field: SerializeField] public int P1_ID { get; private set; } = 1;
        [field: SerializeField] public int P2_ID { get; private set; } = 2;

        [field: SerializeField] public string P1_tag { get; private set; } = "Player1";
        [field: SerializeField] public string P2_tag { get; private set; } = "Player2";

        //Maxima cantidad de corrupci�n para perder
        [field: SerializeField] public float MaxCorruption { get; private set; } = 100;

        //Porcentaje de penalizaci�n de fallos. Ha de estar en el rango [0, 100]
        [field: SerializeField] public float MistakePenalizationPercentage { get; private set; } = 1.0f;

        //M�xima cantidad de man� 
        [field: SerializeField] public float MaxMana { get; private set; } = 100;

        //Ratio obtenci�n man� - progreso. Cuanto m�s alto, m�s man� se gana recitando el ritual
        [field: SerializeField] public float ManaGain { get; private set; } = 5;

        //Textos a completar para el ritual
        [field: SerializeField] public int MaxTextsProvided { get; private set; } = 10;

        [field: SerializeField] public bool DottedText { get; private set; } = true;
    }
}
