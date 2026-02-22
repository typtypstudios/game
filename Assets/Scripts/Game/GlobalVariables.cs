using UnityEngine;

public static class GlobalVariables
{
    //Tags para la diferenciaci�n de jugadores
    public static string P1_tag { get; private set; } = "Player1";
    public static string P2_tag { get; private set; } = "Player2";

    //M�xima cantidad de corrupci�n para perder
    public static float MaxCorruption { get; private set; } = 100;

    //Porcentaje de penalizaci�n de fallos. Ha de estar en el rango [0, 100]
    public static float MistakePenalizationPercentage { get; private set; } = 1.0f; 

    //M�xima cantidad de man� 
    public static float MaxMana { get; private set; } = 100;

    //Ratio obtenci�n man� - progreso. Cuanto m�s alto, m�s man� se gana recitando el ritual
    public static float ManaGain { get; private set; } = 5;

    //Textos a completar para el ritual
    public static int MaxTextsProvided { get; private set; } = 10; 
}
