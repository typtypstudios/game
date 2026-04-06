using TypTyp.Cults;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Este script está pensado para hacer accesibles variables runtime que podrían serlo desde 
/// GetData() de SaveManager, pero que no son tan triviales de obtener, para obtenerlas más rápidamente.
/// </summary>
public class RuntimeVariables : Singleton<RuntimeVariables>
{
    public CultDefinition CurrentCult { get; private set; }
    public float CurrentLevel { get; private set; }
    public List<CultRuntimeInfo> CultsInfo { get; private set; } = new();

    public void UpdateVariables(SaveState saveState)
    {
        int cultId = saveState.slot.cultId;
        CurrentCult = CultRegister.Instance.GetById(cultId);
        CurrentLevel = saveState.slot.cultData[cultId].level;
        CultsInfo.Clear();
        for(int i = 0; i < saveState.slot.cultData.Length; i++)
        {
            CultsInfo.Add(new()
            {
                cultId = i,
                cult = CultRegister.Instance.GetById(i),
                level = Mathf.FloorToInt(saveState.slot.cultData[i].level),
                equippedCards = saveState.slot.cultData[i].deck.equippedCardIds
            });
        }
    }
}

public struct CultRuntimeInfo
{
    public int cultId;
    public CultDefinition cult;
    public int level;
    public List<int> equippedCards;
}
