using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class CryptographyApplication : ICryptographyApplication
    {
        public string DecryptWithAes(string encryptedData, byte[] key, byte[] iv)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            csDecrypt.CopyTo(ms);
                            byte[] decryptedData = ms.ToArray();
                            return Convert.ToBase64String(decryptedData);
                        }
                    }
                }
            }
        }

        public string EncryptWithAes(string dataBase64, byte[] key, byte[] iv)
        {
            byte[] dataBytes = Convert.FromBase64String(dataBase64);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(dataBytes, 0, dataBytes.Length);
                        csEncrypt.FlushFinalBlock();
                    }

                    byte[] encryptedData = msEncrypt.ToArray();
                    return Convert.ToBase64String(encryptedData);
                }
            }
        }

        public byte[] GenerateIV(byte[] salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(salt);
                return hash.Take(16).ToArray();
            }
        }

        public byte[] GenerateUserSpecificKey(string userId, byte[] salt)
        {
            string combinedData = $"{userId}";
            byte[] combinedDataBytes = Encoding.UTF8.GetBytes(combinedData);

            using (HMACSHA256 hmac = new HMACSHA256(salt))
            {
                byte[] hash = hmac.ComputeHash(combinedDataBytes);
                return hash.Take(32).ToArray();
            }
        }
    }
}
