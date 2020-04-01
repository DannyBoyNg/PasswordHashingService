namespace DannyBoyNg.Services
{
    /// <summary>
    /// Password Verification Result
    /// </summary>
    public enum PasswordVerificationResult
    {
        /// <summary>
        /// Password verification failed
        /// </summary>
        Failed = 0,
        /// <summary>
        /// Password verification success
        /// </summary>
        Success = 1,
        /// <summary>
        /// Password verification success but password needs to be rehashed
        /// </summary>
        SuccessRehashNeeded = 2
    }
}
