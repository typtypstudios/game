using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SaveEncryption
{
    private const string Password = "Typtyp";
    private const string Prefix = "ENC1|";

    public static string EncryptForStorage(string plainText)
    {
        string normalized = plainText ?? string.Empty;
        return Prefix + Encrypt(normalized);
    }

    public static string DecryptFromStorage(string storedValue, string context)
    {
        if (string.IsNullOrEmpty(storedValue))
        {
            return string.Empty;
        }

        if (!storedValue.StartsWith(Prefix, StringComparison.Ordinal))
        {
            return storedValue;
        }

        try
        {
            return Decrypt(storedValue.Substring(Prefix.Length));
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[SaveEncryption] Failed to decrypt {context}: {exception.Message}");
            return string.Empty;
        }
    }

    private static string Encrypt(string plainText)
    {
        byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Password));

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();

        using MemoryStream memoryStream = new();
        memoryStream.Write(aes.IV, 0, aes.IV.Length);

        using (CryptoStream cryptoStream = new(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (StreamWriter writer = new(cryptoStream))
        {
            writer.Write(plainText);
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    private static string Decrypt(string cipherText)
    {
        byte[] fullCipher = Convert.FromBase64String(cipherText);
        if (fullCipher.Length < 16)
        {
            throw new Exception("Cipher text too short.");
        }

        byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Password));

        using Aes aes = Aes.Create();
        aes.Key = key;

        byte[] iv = new byte[16];
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using MemoryStream memoryStream = new(fullCipher, iv.Length, fullCipher.Length - iv.Length);
        using CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using StreamReader reader = new(cryptoStream);

        return reader.ReadToEnd();
    }
}
