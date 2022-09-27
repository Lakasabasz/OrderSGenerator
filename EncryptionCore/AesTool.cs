using System.Security.Cryptography;

namespace EncryptionCore;

public class AesTool
{
    private static readonly byte[] Salt = {
        229, 253, 121, 59, 56, 114, 44, 206
    };

    public static byte[] CreateKey(byte[] key, int keyBytes = 32)
    {
        const int iterations = 500;
        var keyGenerator = new Rfc2898DeriveBytes(key, Salt, iterations);
        return keyGenerator.GetBytes(keyBytes);
    }

    public static byte[] Encrypt(Aes aes, string productKey)
    {
        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        byte[] encrypted;
        using MemoryStream memoryStream = new();
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(productKey);
        }
        encrypted = memoryStream.ToArray();

        return encrypted;
    }

    public static string Decrypt(Aes aes, byte[] encrypted)
    {
        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using MemoryStream memoryStream = new(encrypted);
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using StreamReader streamReader = new StreamReader(cryptoStream);
        var decrypted = streamReader.ReadToEnd();
        return decrypted;
    }

    public static byte[] CreateIv(byte[] key)
    {
        if (key.Length < 16) throw new ArgumentException("Key is too short");
        return key.Take(16).ToArray();
    }
}