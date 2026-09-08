using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NutSort.Content
{
    public static class OriginalContentCodec
    {
        // FileLSSUtil.Decrypt, ELF RVA 0x9B4718: UTF-8, AES CBC (1), PKCS7 (2), StreamReader.
        public static string Decrypt(string ciphertext, string key, string iv)
        {
            byte[] bytes = Convert.FromBase64String(ciphertext.TrimStart('\uFEFF'));
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform transform = aes.CreateDecryptor())
                using (var input = new MemoryStream(bytes, false))
                using (var stream = new CryptoStream(input, transform, CryptoStreamMode.Read))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return reader.ReadToEnd();
            }
        }
    }
}
