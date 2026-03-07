using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class PlayerPrefsEncoder
{
    private static readonly string password = "Typtyp";
    private const string Prefix = "ENC1|";

    public static void SetString(string key, string value)
    {
        string encrypted = Encrypt(value);
        PlayerPrefs.SetString(key, Prefix + encrypted);
    }

    public static string GetString(string key, string defaultValue = "")
    {
        if (!PlayerPrefs.HasKey(key))
            return defaultValue;

        string stored = PlayerPrefs.GetString(key);
        if (!stored.StartsWith(Prefix))
            return stored;

        try
        {
            return Decrypt(stored.Substring(Prefix.Length));
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Decrypt failed for key {key}: {e.Message}");
            return defaultValue;
        }
    }

    static string Encrypt(string plainText)
    {
        byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();

        using MemoryStream ms = new();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (StreamWriter sw = new(cs))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    static string Decrypt(string cipherText)
    {
        byte[] fullCipher = Convert.FromBase64String(cipherText);

        if (fullCipher.Length < 16)
            throw new Exception("Cipher text too short.");

        byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));

        using Aes aes = Aes.Create();
        aes.Key = key;

        byte[] iv = new byte[16];
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using MemoryStream ms = new(fullCipher, iv.Length, fullCipher.Length - iv.Length);
        using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using StreamReader sr = new(cs);

        return sr.ReadToEnd();
    }
}
