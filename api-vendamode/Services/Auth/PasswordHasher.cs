using System.Security.Cryptography;
using System.Text;

namespace api_vendamode.Services.Auth;

public interface IPasswordHasher
{
    byte[] GenerateSalt();
    byte[] HashPassword(string password, byte[] salt);
    bool VerifyPassword(string password, byte[] hashedPassword, byte[] salt);
}

public class PasswordHasher : IPasswordHasher
{
    private readonly byte[] _key;
    private readonly byte[] _iv;
    private readonly IConfiguration _configuration;

    public PasswordHasher(IConfiguration configuration)
    {
        _configuration = configuration;
        string encryptionKey = configuration.GetValue<string>("EncryptionKey") ?? "";
        _key = Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 32));
        _iv = Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 16));
    }

    public byte[] GenerateSalt()
    {
        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }

    public byte[] HashPassword(string password, byte[] salt)
    {
        using (var rfc2898 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
        {
            return rfc2898.GetBytes(32);
        }
    }

    public bool VerifyPassword(string password, byte[] hashedPassword, byte[] salt)
    {
        var hashedInputPassword = HashPassword(password, salt);
        return hashedInputPassword.SequenceEqual(hashedPassword);
    }

    public byte[] EncryptPassword(string password)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = _key;
            aesAlg.IV = _iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(password);
                    }
                }
                return msEncrypt.ToArray();
            }
        }
    }

    public string DecryptPassword(byte[] cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = _key;
            aesAlg.IV = _iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}
