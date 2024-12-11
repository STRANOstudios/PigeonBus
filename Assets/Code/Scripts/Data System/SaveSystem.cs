using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string EncryptionKey = "AndreaFrigerio01"; //16 bytes/characters

    private static string SavePath(string fileName) => Path.Combine(Application.persistentDataPath, fileName + ".json");

    public static void Save<T>(T data, string fileName)
    {
        string json = JsonUtility.ToJson(data);

        string encryptedJson = Encrypt(json, EncryptionKey);

        File.WriteAllText(SavePath(fileName), encryptedJson);
    }

    public static T Load<T>(string fileName) where T : new()
    {
        string path = SavePath(fileName);
        if (File.Exists(path))
        {
            string encryptedJson = File.ReadAllText(path);

            if (string.IsNullOrEmpty(encryptedJson)) return new T();

            string json = Decrypt(encryptedJson, EncryptionKey);

            return JsonUtility.FromJson<T>(json);
        }
        return new T();
    }

    public static bool Exists(string fileName)
    {
        return File.Exists(SavePath(fileName));
    }

    public static void Delete(string fileName)
    {
        string path = SavePath(fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static string Encrypt(string plainText, string key)
    {
        // Verifica la lunghezza della chiave
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        if (keyBytes.Length != 16)  // AES-128 richiede una chiave di 16 byte
        {
            throw new ArgumentException("La chiave deve essere lunga 16 byte per AES-128.");
        }

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.GenerateIV(); // Genera un IV casuale
            aes.Padding = PaddingMode.PKCS7; // Imposta il padding PKCS7

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(cs))
                    {
                        writer.Write(plainText);
                    }
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    private static string Decrypt(string cipherText, string key)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        byte[] iv = new byte[16];
        Array.Copy(cipherBytes, iv, iv.Length);

        byte[] encryptedData = new byte[cipherBytes.Length - iv.Length];
        Array.Copy(cipherBytes, iv.Length, encryptedData, 0, encryptedData.Length);

        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        if (keyBytes.Length != 16)
        {
            throw new ArgumentException("La chiave deve essere lunga 16 byte per AES-196.");
        }

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = iv;
            aes.Padding = PaddingMode.PKCS7; // Specifica il padding

            using (MemoryStream ms = new MemoryStream(encryptedData))
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (StreamReader reader = new StreamReader(cs))
            {
                return reader.ReadToEnd();
            }
        }
    }

}
