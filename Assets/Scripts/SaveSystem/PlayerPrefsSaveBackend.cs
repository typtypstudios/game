using System;
using UnityEngine;

public class PlayerPrefsSaveBackend : ISaveBackend
{
    public void Save(string key, string data)
    {
        try
        {
            string encrypted = SaveEncryption.EncryptForStorage(data);
            PlayerPrefs.SetString(key, encrypted);
            PlayerPrefs.Save();
        }
        catch (Exception exception)
        {
            Debug.LogError($"[PlayerPrefsSaveBackend] Failed to save key '{key}': {exception.Message}");
        }
    }

    public string Load(string key)
    {
        try
        {
            string stored = PlayerPrefs.GetString(key, string.Empty);
            return SaveEncryption.DecryptFromStorage(stored, $"player prefs key '{key}'");
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[PlayerPrefsSaveBackend] Failed to load key '{key}': {exception.Message}");
            return string.Empty;
        }
    }

    public bool Exists(string key)
    {
        try
        {
            return PlayerPrefs.HasKey(key);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[PlayerPrefsSaveBackend] Failed to check key '{key}': {exception.Message}");
            return false;
        }
    }

    public void Delete(string key)
    {
        try
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[PlayerPrefsSaveBackend] Failed to delete key '{key}': {exception.Message}");
        }
    }
}
