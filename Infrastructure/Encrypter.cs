using System;
using System.Security.Cryptography;
using System.Text;

namespace TestLab.Infrastructure
{
    public class Encrypter
    {
        public static string Encrypt(string password, string salt)
        {
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] saltBytes = Encoding.Unicode.GetBytes(salt);

            byte[] cipherBytes = ProtectedData.Protect(passwordBytes, saltBytes, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(cipherBytes);
        }

        public static string Decrypt(string cipher, string salt)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipher);
            byte[] saltBytes = Encoding.Unicode.GetBytes(salt);

            byte[] passwordBytes = ProtectedData.Unprotect(cipherBytes, saltBytes, DataProtectionScope.CurrentUser);

            return Encoding.Unicode.GetString(passwordBytes);
        }
    }
}
