using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Ng.Services
{
    /// <summary>
    /// The container for the password hashing settings
    /// </summary>
    public record PasswordHashingSettings
    {
        /// <summary>
        /// Gets or sets the random number generator.
        /// </summary>
        public RandomNumberGenerator Rng { get; set; } = RandomNumberGenerator.Create();
        /// <summary>
        /// Gets or sets the pseudo-random function to be used in the key derivation process. Default: KeyDerivationPrf.HMACSHA256
        /// </summary>
        public KeyDerivationPrf Prf { get; set; } = KeyDerivationPrf.HMACSHA256;
        /// <summary>
        /// Gets or sets the number of iterations of the pseudo-random function to apply during the key derivation process. Default: 10000
        /// </summary>
        public int Pbkdf2IterCount { get; set; } = 10000;
        /// <summary>
        /// Gets or sets the size of the salt in bytes. Default: 16
        /// </summary>
        public int Pbkdf2SaltSize { get; set; } = 16;
        /// <summary>
        /// Gets or sets the desired length (in bytes) of the derived key. Default: 32
        /// </summary>
        public int Pbkdf2NumBytesRequested { get; set; } = 32;
    }
}
