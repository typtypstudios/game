using UnityEngine;

public class InitialTip : MonoBehaviour
{
    private bool hasBeenUnderstood;

    private void OnEnable()
    {
        SaveManager.Instance.OnBeforeSave += HandleBeforeSave;
        SaveManager.Instance.OnAfterLoad += HandleAfterLoad;
    }

    private void Start()
    {
        if (SaveManager.Instance.HasLoadedState)
        {
            SaveState state = SaveManager.Instance.GetState();
            ApplyState(state);
        }
    }

    private void OnDisable()
    {
        if (SaveManager.Instance == null) return;

        SaveManager.Instance.OnBeforeSave -= HandleBeforeSave;
        SaveManager.Instance.OnAfterLoad -= HandleAfterLoad;
    }

    public void Understood()
    {
        hasBeenUnderstood = true;
        SaveManager.Instance.Save();
    }

    private void HandleBeforeSave(SaveState state)
    {
        state.global.initialTipUnderstood = hasBeenUnderstood;
    }

    private void HandleAfterLoad(SaveState state)
    {
        ApplyState(state);
    }

    private void ApplyState(SaveState state)
    {
        hasBeenUnderstood = state.global.initialTipUnderstood;
        if(!hasBeenUnderstood)
        {
            NavigationController navController = FindFirstObjectByType<NavigationController>();
            navController.OverrideInitialScreen(Screens.InitialTip);
        }
    }
}
