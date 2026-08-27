using System;
using System.Security.Cryptography;

namespace Bike_STore_Project
{
    /// <summary>
    /// Password hashing using PBKDF2 (Rfc2898DeriveBytes).
    /// Stored format:
    ///   PBKDF2$<iterations>$<saltBase64>$<hashBase64>
    /// </summary>
    public static class PasswordHasher
    {
        private const int SaltSize = 16;      // 128-bit
        private const int KeySize = 32;       // 256-bit
        private const int Iterations = 100_000;

        public static string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] key;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                key = pbkdf2.GetBytes(KeySize);

            return $"PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (string.IsNullOrWhiteSpace(stored)) return false;

            var parts = stored.Split('$');
            if (parts.Length != 4) return false;
            if (!parts[0].Equals("PBKDF2", StringComparison.OrdinalIgnoreCase)) return false;

            if (!int.TryParse(parts[1], out int iters) || iters <= 0) return false;

            byte[] salt, expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch
            {
                return false;
            }

            byte[] actual;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iters, HashAlgorithmName.SHA256))
                actual = pbkdf2.GetBytes(expected.Length);

            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
    }
}
