using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace DannyBoyNg.Services
{
    /// <summary>
    /// The container for the password hashing settings
    /// </summary>
    public class PasswordHashingSettings
    {
        /// <summary>
        /// Gets or sets the random number generator.
        /// </summary>
        public RandomNumberGenerator Rng { get; set; } = RandomNumberGenerator.Create();
        /// <summary>
        /// Gets or sets the pseudo-random function to be used in the key derivation process.
        /// </summary>
        public KeyDerivationPrf Prf { get; set; } = KeyDerivationPrf.HMACSHA256;
        /// <summary>
        /// Gets or sets the number of iterations of the pseudo-random function to apply during the key derivation process.
        /// </summary>
        public int IterCount { get; set; } = 10000;
        /// <summary>
        /// Gets or sets the size of the salt in bytes.
        /// </summary>
        public int SaltSize { get; set; } = 128 / 8;
        /// <summary>
        /// Gets or sets the desired length (in bytes) of the derived key.
        /// </summary>
        public int NumBytesRequested { get; set; } = 256 / 8;
    }
}
