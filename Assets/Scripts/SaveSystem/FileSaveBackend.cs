using System;
using System.IO;
using UnityEngine;

public class FileSaveBackend : ISaveBackend
{
    private readonly string rootPath;

    public FileSaveBackend()
    {
        rootPath = Path.Combine(Application.persistentDataPath, SaveKeys.SavesFolderName);

        try
        {
            Directory.CreateDirectory(rootPath);
        }
        catch (Exception exception)
        {
            Debug.LogError($"[FileSaveBackend] Failed to create save directory '{rootPath}': {exception.Message}");
        }
    }

    public void Save(string key, string data)
    {
        try
        {
            Directory.CreateDirectory(rootPath);
            string encrypted = SaveEncryption.EncryptForStorage(data);
            File.WriteAllText(GetPath(key), encrypted);
        }
        catch (Exception exception)
        {
            Debug.LogError($"[FileSaveBackend] Failed to save key '{key}': {exception.Message}");
        }
    }

    public string Load(string key)
    {
        try
        {
            string path = GetPath(key);
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            string stored = File.ReadAllText(path);
            return SaveEncryption.DecryptFromStorage(stored, $"file key '{key}'");
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[FileSaveBackend] Failed to load key '{key}': {exception.Message}");
            return string.Empty;
        }
    }

    public bool Exists(string key)
    {
        try
        {
            return File.Exists(GetPath(key));
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[FileSaveBackend] Failed to check key '{key}': {exception.Message}");
            return false;
        }
    }

    public void Delete(string key)
    {
        try
        {
            string path = GetPath(key);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[FileSaveBackend] Failed to delete key '{key}': {exception.Message}");
        }
    }

    private string GetPath(string key)
    {
        return Path.Combine(rootPath, key + ".json");
    }
}
