using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Fuyu.Common.Hashing;

public static class AesHelper
{
    private const int IV_SIZE = 16;

    public static byte[] EncryptAes(byte[] data, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Padding = PaddingMode.Zeros;
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

        cs.Write(data, 0, data.Length);
        cs.Close();

        // Concat faster than making new, using that.
        return aes.IV.Concat(ms.ToArray()).ToArray();
    }

    // NOTE: removeTail should always be true unless the trailing zero byte is desirable to keep
    // -- seionmoya, 2025-01-11
    public static byte[] DecryptAes(byte[] data, byte[] key, bool removeTail = true)
    {
        using var aes = Aes.Create();
        aes.Padding = PaddingMode.Zeros;
        aes.Key = key;
        // get first 16 bytes as IV
        aes.IV = data[..IV_SIZE];

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var memory = new MemoryStream();
        using var cryptoStream = new CryptoStream(memory, decryptor, CryptoStreamMode.Write);

        // get bytes after the length 16
        var newData = data[IV_SIZE..];
        cryptoStream.Write(newData, 0, newData.Length);
        cryptoStream.Close();

        var result = memory.ToArray();
        if (removeTail)
        {
            // take the full length - 1
            result = result[..^1];
        }

        return result;
    }
}