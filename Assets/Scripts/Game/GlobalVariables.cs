using UnityEngine;

public static class GlobalVariables
{
    //Tags para la diferenciación de jugadores
    public static string P1_tag { get; private set; } = "Player1";
    public static string P2_tag { get; private set; } = "Player2";

    //Máxima cantidad de corrupción para perder
    public static float MaxCorruption { get; private set; } = 100;

    //Porcentaje de penalización de fallos. Ha de estar en el rango [0, 100]
    public static float MistakePenalizationPercentage { get; private set; } = 1.0f; 

    //Máxima cantidad de maná 
    public static float MaxMana { get; private set; } = 100;

    //Ratio obtención maná - progreso. Cuanto más alto, más maná se gana recitando el ritual
    public static float ManaGain { get; private set; } = 5;

    //Textos a completar para el ritual
    public static int MaxTextsProvided { get; private set; } = 10; 
}
