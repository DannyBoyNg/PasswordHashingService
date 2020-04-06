using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Ng.Services
{
    /// <summary>
    /// The Password hashing service
    /// </summary>
    /// <seealso cref="Ng.Services.IPasswordHashingService" />
    public class PasswordHashingService : IPasswordHashingService
    {
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public PasswordHashingSettings Settings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordHashingService"/> class.
        /// </summary>
        public PasswordHashingService() => Settings = new PasswordHashingSettings();

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordHashingService"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public PasswordHashingService(PasswordHashingSettings settings) => Settings = settings ?? new PasswordHashingSettings();

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordHashingService"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public PasswordHashingService(IOptions<PasswordHashingSettings> settings) => Settings = settings?.Value ?? new PasswordHashingSettings();

        /// <summary>
        /// Hashes the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>
        /// a hashed password.
        /// </returns>
        /// <exception cref="ArgumentNullException">password</exception>
        public string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            return Convert.ToBase64String(HashPasswordInternal(password, Settings.Rng, Settings.Prf, Settings.Pbkdf2IterCount, Settings.Pbkdf2SaltSize, Settings.Pbkdf2NumBytesRequested));
        }

        /// <summary>
        /// Verifies the hashed password.
        /// </summary>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <param name="providedPassword">The provided password.</param>
        /// <returns>
        /// Password Verification Result.
        /// </returns>
        /// <exception cref="ArgumentNullException">hashedPassword or providedPassword</exception>
        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null)
            {
                throw new ArgumentNullException(nameof(hashedPassword));
            }
            if (providedPassword == null)
            {
                throw new ArgumentNullException(nameof(providedPassword));
            }

            byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            if (decodedHashedPassword.Length == 0)
            {
                return PasswordVerificationResult.Failed;
            }
            if (VerifyHashedPasswordInternal(decodedHashedPassword, providedPassword, out int embeddedIterCount))
            {
                if (embeddedIterCount < Settings.Pbkdf2IterCount)
                {
                    return PasswordVerificationResult.SuccessRehashNeeded;
                }
                return PasswordVerificationResult.Success;
            }
            else
            {
                return PasswordVerificationResult.Failed;
            }
        }

        //Private internals
        private byte[] HashPasswordInternal(string password, RandomNumberGenerator rng, KeyDerivationPrf prf, int iterCount, int saltSize, int numBytesRequested)
        {
            byte[] salt = new byte[saltSize];
            rng.GetBytes(salt);
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01;
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return outputBytes;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Exceptions not of interest")]
        private bool VerifyHashedPasswordInternal(byte[] hashedPassword, string password, out int iterCount)
        {
            iterCount = default;

            try
            {
                KeyDerivationPrf prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
                iterCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
                int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

                if (saltLength < 128 / 8)
                {
                    return false;
                }
                byte[] salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

                int subkeyLength = hashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    return false;
                }
                byte[] expectedSubkey = new byte[subkeyLength];
                Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);
                return ByteArraysEqual(actualSubkey, expectedSubkey);
            }
            catch
            {
                return false;
            }
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
    }
}
