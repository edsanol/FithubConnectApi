namespace Application.Interfaces
{
    public interface ICryptographyApplication
    {
        byte[] GenerateUserSpecificKey(string userId, byte[] salt);
        byte[] GenerateIV(byte[] salt);
        string EncryptWithAes(string dataBase64, byte[] key, byte[] iv);
        string DecryptWithAes(string encryptedData, byte[] key, byte[] iv);
    }
}
