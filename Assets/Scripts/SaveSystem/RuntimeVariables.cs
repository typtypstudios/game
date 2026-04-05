using TypTyp.Cults;
using UnityEngine;

/// <summary>
/// Este script está pensado para hacer accesibles variables runtime que podrían serlo desde 
/// GetData() de SaveManager, pero que no son tan triviales de obtener, para obtenerlas más rápidamente.
/// </summary>
public class RuntimeVariables : Singleton<RuntimeVariables>
{
    public CultDefinition CurrentCult { get; private set; }
    public float CurrentLevel { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        SaveManager.Instance.OnAfterLoad += UpdateVariables;
        UpdateVariables(SaveManager.Instance.GetState());
    }

    private void OnDestroy()
    {
        SaveManager.Instance.OnAfterLoad -= UpdateVariables;
    }

    private void UpdateVariables(SaveState saveState)
    {
        int cultId = saveState.slot.cultId;
        CurrentCult = CultRegister.Instance.GetById(cultId);
        CurrentLevel = saveState.slot.cultData[cultId].level;
    }
}
