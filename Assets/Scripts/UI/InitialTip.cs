using UnityEngine;

public class InitialTip : MonoBehaviour
{
    [SerializeField] private GameObject tipPanel;
    [SerializeField] private GameObject tutorialPanel;
    private bool hasBeenUnderstood;
    private TurnPageEffect turnPageEffect;

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
        turnPageEffect = GetComponent<TurnPageEffect>();
        turnPageEffect.OnBlankPage += () =>
        {
            tipPanel.SetActive(false);
            tutorialPanel.SetActive(true);
        };
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
        turnPageEffect.TurnPage();
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
            tutorialPanel.SetActive(false);
            NavigationController navController = FindFirstObjectByType<NavigationController>();
            navController.OverrideInitialScreen(Screens.InitialTip);
        }
    }
}
