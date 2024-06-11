namespace BancoAPI.Helpers
{
    public class Verifier
    {
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            string hashedInput = Encrypter.HashPassword(password);

            return hashedInput.ToLower().Equals(hashedPassword.ToLower(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
