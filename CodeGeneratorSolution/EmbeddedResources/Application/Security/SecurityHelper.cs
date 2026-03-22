using System;
using System.Security.Cryptography;
using System.Text;

namespace CodeGeneratorSolution.EmbeddedResources.Application.Security
{
    public static class SecurityHelper
    {
        // Configuration: 32 bytes = 256 bits (Standard for SHA-256)
        private const int SaltSize = 32;

        /// <summary>
        /// A simple DTO to hold the Hash and Salt together.
        /// </summary>
        public class HashResult
        {
            public required string Hash { get; set; }
            public required string Salt { get; set; }
        }

        /// <summary>
        /// Generates a random Salt .
        /// </summary>
        private static string GenerateStringSalt()
        {
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }
        /// <summary>
        /// Generates a random Salt and Hashes the password with it.
        /// </summary>
        public static HashResult Hash(string password)
        {
            // 1. Generate a Cryptographically Secure Salt
            string salt = GenerateStringSalt();

            // 2. Hash the Password + Salt
            string hash = ComputeHash(password, salt);

            // 3. Return both to be saved in the DB
            return new HashResult
            {
                Hash = hash,
                Salt = salt
            };
        }

        /// <summary>
        /// Verifies a login attempt. Re-hashes the input password with the STORED salt
        /// and checks if it matches the STORED hash.
        /// </summary>
        public static bool Verify(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            // 1. Re-calculate the hash using the parameters from the DB
            string computedHash = ComputeHash(password, storedSalt);

            // 2. Compare (using a time-constant comparison is technically safer, 
            // but string comparison is acceptable for standard enterprise apps)
            return computedHash == storedHash;
        }

        /// <summary>
        /// Private helper to perform the actual SHA-256 Hashing.
        /// </summary>
        private static string ComputeHash(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                // Combine Password + Salt
                string combined = password + salt;
                byte[] bytes = Encoding.UTF8.GetBytes(combined);

                // Compute Hash
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Return as Base64 string (Standard storage format)
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}