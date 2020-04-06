
namespace Ng.Services
{
    /// <summary>
    /// Default interface for PasswordHashingService
    /// </summary>
    public interface IPasswordHashingService
    {
        /// <summary>
        /// Verifies the hashed password.
        /// </summary>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <param name="providedPassword">The provided password.</param>
        /// <returns>PasswordVerificationResult</returns>
        PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
        /// <summary>
        /// Hashes the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>a password hash</returns>
        string HashPassword(string password);
    }
}
