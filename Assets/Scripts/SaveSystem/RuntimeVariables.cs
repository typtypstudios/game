using TypTyp.Cults;
using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Este script está pensado para hacer accesibles variables runtime que podrían serlo desde 
/// GetData() de SaveManager, pero que no son tan triviales de obtener, para obtenerlas más rápidamente.
/// </summary>
public class RuntimeVariables : Singleton<RuntimeVariables>
{
    public CultDefinition CurrentCult { get; private set; }
    public int CurrentCultID { get; private set; }
    public float CurrentLevel { get; private set; }
    public List<CultRuntimeInfo> CultsInfo { get; private set; } = new();
    public int MaxLevel => CurrentCult.RankNames.Length - 1;
    public bool IsLoaded { get; private set; } = false;
    public event Action OnUpdated;

    public void UpdateVariables(SaveState saveState)
    {
        int cultId = saveState.slot.cultId;
        CurrentCult = CultRegister.Instance.GetById(cultId);
        CurrentCultID = cultId;
        CurrentLevel = Mathf.Min(saveState.slot.cultData[cultId].level, MaxLevel);
        CultsInfo.Clear();
        for(int i = 0; i < saveState.slot.cultData.Length; i++)
        {
            CultsInfo.Add(new()
            {
                cultId = i,
                cult = CultRegister.Instance.GetById(i),
                level = Mathf.FloorToInt(Mathf.Min(saveState.slot.cultData[i].level, MaxLevel)),
                equippedCards = saveState.slot.cultData[i].deck.equippedCardIds
            });
        }
        IsLoaded = true;
        OnUpdated?.Invoke();
    }
}

public struct CultRuntimeInfo
{
    public int cultId;
    public CultDefinition cult;
    public int level;
    public List<int> equippedCards;
}
