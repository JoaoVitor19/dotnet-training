using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace dotnet_training.Helpers
{
    public class Utils
    {
        public static string HashPassword(string password)
        {
            // Use SHA-256 for hashing
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the password to bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // ComputeHash returns the hash bytes
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convert the hash bytes to a hexadecimal string
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Verify if the provided password matches the stored hash
            string hashedInputPassword = HashPassword(password);
            return hashedInputPassword == hashedPassword;
        }

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[32]; // Specify the size of the salt in bytes
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        public static string HashPasswordWithSalt(string senha, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(senha, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPasswordWithSalt(string password, string hashedPassword, byte[] salt)
        {
            string hashedPasswordInput = HashPasswordWithSalt(password, salt);
            return hashedPasswordInput == hashedPassword;
        }
    }
}
