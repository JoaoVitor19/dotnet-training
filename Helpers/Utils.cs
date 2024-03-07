using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace dotnet_training.Helpers
{
    public class Utils
    {
        //The IsValidEmail method then calls the Regex.IsMatch(String, String) method to verify that the address conforms to a regular expression pattern.
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            // Regular expression pattern to check for at least one uppercase character
            string pattern = @"^(?=.*[A-Z]).+$";

            // Use Regex.IsMatch to check if the password matches the pattern
            return Regex.IsMatch(password, pattern);
        }

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
