using System.Security.Cryptography;
using System.Text;

namespace PracticeCoreMVC.Services
{
    public class PasswordSecurity
    {
        private readonly string pepper;
        
        public PasswordSecurity(IConfiguration config) {
            this.pepper = config.GetSection("SecurityKey")["PepperValue"];
        }
        public string HashPasswordWithSaltAndPepper(string password, string salt)
        {
            string saltedPassword = password + salt + pepper;
            return ComputeHash(saltedPassword);
        }
        public bool VerifyPasswordWithSaltAndPepper(string password, string hashedPassword, string salt)
        {
            string saltedPassword = password + salt + pepper;
            string hashedAttempt = ComputeHash(saltedPassword);

            return string.Equals(hashedPassword, hashedAttempt);
        }
        [Obsolete]
        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}