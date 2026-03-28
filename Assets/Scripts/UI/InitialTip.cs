using UnityEngine;

public class InitialTip : MonoBehaviour
{
    [SerializeField] private Canvas mainMenuCanvas;
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
        if (SaveManager.Instance.TryGetSnapshot(out SaveState state))
        {
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
        HideTip();
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
        mainMenuCanvas.enabled = false;
    }

    private void HideTip()
    {
        selfCanvas.enabled = false;
        mainMenuCanvas.enabled = true;
    }
}
