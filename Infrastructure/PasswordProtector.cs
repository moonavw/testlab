using System;
using System.Security.Cryptography;
using System.Text;

namespace TestLab.Infrastructure
{
    public class PasswordProtector
    {
        public static string Encrypt(string password, string salt = null)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt ?? Constants.EncryptionKey);

            byte[] cipherBytes = ProtectedData.Protect(passwordBytes, saltBytes, DataProtectionScope.LocalMachine);

            return Convert.ToBase64String(cipherBytes);
        }

        public static string Decrypt(string cipher, string salt = null)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipher);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt ?? Constants.EncryptionKey);

            byte[] passwordBytes = ProtectedData.Unprotect(cipherBytes, saltBytes, DataProtectionScope.LocalMachine);

            return Encoding.UTF8.GetString(passwordBytes);
        }
    }
}
