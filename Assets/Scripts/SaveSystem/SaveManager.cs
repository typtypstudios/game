using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[NoAutoCreate]
[CreateAssetMenu(fileName = nameof(SaveManager), menuName = "TypTyp/SaveManager")]
public class SaveManager : ScriptableSingleton<SaveManager>
{
    private ISaveBackend backend;
    private SaveState currentState;
    private string activeSlotId;
    private bool initialLoadPending;
    private bool isInitialized;

    public event Action<SaveState> OnBeforeSave;
    public event Action<SaveState> OnAfterLoad;

    public bool HasLoadedState { get; private set; }
    public string ActiveSlotId => activeSlotId;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeRuntime()
    {
        Instance.ResetRuntimeState();
        Instance.EnsureInitialized();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleInitialSceneLoaded;
        isInitialized = false;
    }

    public bool HasActiveSlot()
    {
        EnsureInitialized();
        return !string.IsNullOrWhiteSpace(activeSlotId);
    }

    public void CreateSlot(string slotId)
    {
        EnsureInitialized();
        SetActiveSlot(slotId);

        SaveData data = NormalizeSlotData(new SaveData());
        SaveDataInternal(data);
        currentState.slot = DeepCopy(data);
        HasLoadedState = true;
        OnAfterLoad?.Invoke(GetSnapshot());
    }

    public void SetActiveSlot(string slotId)
    {
        EnsureInitialized();
        if (string.IsNullOrWhiteSpace(slotId))
        {
            Debug.LogWarning("[SaveManager] Tried to set an empty slot id.");
            return;
        }

        activeSlotId = slotId;
        PlayerPrefs.SetString(SaveKeys.ActiveSlotPrefsKey, activeSlotId);
        PlayerPrefs.Save();
    }

    public void Save()
    {
        EnsureInitialized();
        EnsureActiveSlot();

        SaveState state = NormalizeState(GetSnapshot() ?? new SaveState());
        OnBeforeSave?.Invoke(state);
        NormalizeState(state);

        SaveDataInternal(state.slot);
        SaveGlobalSettingsInternal(state.global);
        currentState = DeepCopy(state);
        HasLoadedState = true;
    }

    public void Load()
    {
        EnsureInitialized();
        EnsureActiveSlot();

        SaveData loadedData = new SaveData();
        if (backend.Exists(SaveKeys.GetSlotKey(activeSlotId)))
        {
            string json = backend.Load(SaveKeys.GetSlotKey(activeSlotId));
            loadedData = DeserializeOrDefault(json, new SaveData(), "slot save");
        }

        currentState.slot = NormalizeSlotData(loadedData);
        LoadGlobalSettingsInternal();
        currentState = NormalizeState(currentState);
        HasLoadedState = true;
        OnAfterLoad?.Invoke(GetSnapshot());
    }

    public bool TryGetSnapshot(out SaveState state)
    {
        EnsureInitialized();
        state = HasLoadedState ? GetSnapshot() : null;
        return state != null;
    }

    private void EnsureInitialized()
    {
        if (isInitialized)
        {
            return;
        }

        backend = CreateBackend();
        currentState = NormalizeState(new SaveState());
        activeSlotId = PlayerPrefs.GetString(SaveKeys.ActiveSlotPrefsKey, SaveKeys.DefaultSlotId);
        LoadGlobalSettingsInternal();

        initialLoadPending = backend.Exists(SaveKeys.GetSlotKey(activeSlotId));
        SceneManager.sceneLoaded -= HandleInitialSceneLoaded;
        SceneManager.sceneLoaded += HandleInitialSceneLoaded;
        isInitialized = true;
    }

    private void ResetRuntimeState()
    {
        SceneManager.sceneLoaded -= HandleInitialSceneLoaded;
        backend = null;
        currentState = null;
        activeSlotId = string.Empty;
        initialLoadPending = false;
        HasLoadedState = false;
        isInitialized = false;
    }

    private void HandleInitialSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        SceneManager.sceneLoaded -= HandleInitialSceneLoaded;

        if (initialLoadPending)
        {
            initialLoadPending = false;
            Load();
            return;
        }

        currentState = NormalizeState(currentState);
        HasLoadedState = true;
        OnAfterLoad?.Invoke(GetSnapshot());
    }

    private void EnsureActiveSlot()
    {
        if (!HasActiveSlot())
        {
            SetActiveSlot(SaveKeys.DefaultSlotId);
        }
    }

    private void SaveDataInternal(SaveData data)
    {
        backend.Save(SaveKeys.GetSlotKey(activeSlotId), JsonUtility.ToJson(NormalizeSlotData(data), true));
    }

    private void LoadGlobalSettingsInternal()
    {
        if (!backend.Exists(SaveKeys.GlobalSettingsKey))
        {
            currentState.global = new GlobalSettingsData();
            return;
        }

        string json = backend.Load(SaveKeys.GlobalSettingsKey);
        currentState.global = DeserializeOrDefault(json, new GlobalSettingsData(), "global settings");
    }

    private void SaveGlobalSettingsInternal(GlobalSettingsData data)
    {
        string json = JsonUtility.ToJson(data ?? new GlobalSettingsData(), true);
        backend.Save(SaveKeys.GlobalSettingsKey, json);
    }

    private static ISaveBackend CreateBackend()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return new PlayerPrefsSaveBackend();
#else
        return new FileSaveBackend();
#endif
    }

    private SaveState GetSnapshot()
    {
        return DeepCopy(NormalizeState(currentState));
    }

    private static SaveState NormalizeState(SaveState state)
    {
        state ??= new SaveState();
        state.slot = NormalizeSlotData(state.slot);
        state.global ??= new GlobalSettingsData();
        return state;
    }

    private static SaveData NormalizeSlotData(SaveData data)
    {
        data ??= new SaveData();
        data.profile ??= new ProfileSaveData();
        data.deck ??= new DeckSaveData();
        data.deck.equippedCardIds ??= new System.Collections.Generic.List<int>();
        return data;
    }

    private static T DeserializeOrDefault<T>(string json, T fallback, string context) where T : class
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return fallback;
        }

        try
        {
            return JsonUtility.FromJson<T>(json) ?? fallback;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[SaveManager] Failed to deserialize {context}: {exception.Message}");
            return fallback;
        }
    }

    private static T DeepCopy<T>(T source) where T : class
    {
        if (source == null)
        {
            return null;
        }

        return DeserializeOrDefault(JsonUtility.ToJson(source), source, typeof(T).Name);
    }
}
