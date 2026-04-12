using UnityEngine;

public class InitialTip : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    private Canvas selfCanvas;
    private bool hasBeenUnderstood;

    private void Awake()
    {
        selfCanvas = GetComponent<Canvas>();
    }

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
        else
        {
            ShowTip();
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
        if (hasBeenUnderstood)
        {
            HideTip();
            return;
        }

        ShowTip();
    }

    private void ShowTip()
    {
        selfCanvas.enabled = true;
        mainMenu.SetActive(false);
    }

    public void HideTip()
    {
        if (!selfCanvas.enabled) return;
        selfCanvas.enabled = false;
        mainMenu.SetActive(true);
    }
}
