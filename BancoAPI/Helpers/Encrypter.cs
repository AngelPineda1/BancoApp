using System.Security.Cryptography;
using System.Text;

namespace BancoAPI.Helpers
{
    public class Encrypter
    {
        public static string HashPassword(string password)
        {
            using (SHA512 shaM = SHA512.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedBytes = shaM.ComputeHash(passwordBytes);
                return BitConverter.ToString(hashedBytes).Replace("-", "");
            }
        }
        public bool IsPasswordChanged(string storedHash, string newPassword)
        {
            using (SHA512 shaM = SHA512.Create())
            {
                byte[] newPasswordBytes = Encoding.UTF8.GetBytes(newPassword);
                byte[] hashedNewPasswordBytes = shaM.ComputeHash(newPasswordBytes);
                string hashedNewPassword = BitConverter.ToString(hashedNewPasswordBytes).Replace("-", "");

                return !string.Equals(storedHash, hashedNewPassword, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
