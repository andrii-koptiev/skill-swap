namespace SkillSwap.Domain.Enums;

/// <summary>
/// User account status enumeration
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// User account is inactive/suspended
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// User account is active and can use the platform
    /// </summary>
    Active = 1,

    /// <summary>
    /// User account is pending email verification
    /// </summary>
    PendingVerification = 2,

    /// <summary>
    /// User account is suspended due to violations
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// User account is permanently banned
    /// </summary>
    Banned = 4,
}

/// <summary>
/// User verification status
/// </summary>
public enum VerificationStatus
{
    /// <summary>
    /// User email is not verified
    /// </summary>
    Unverified = 0,

    /// <summary>
    /// User email is verified
    /// </summary>
    Verified = 1,

    /// <summary>
    /// User has additional skill verifications
    /// </summary>
    SkillVerified = 2,

    /// <summary>
    /// User is a trusted community member
    /// </summary>
    TrustedMember = 3,
}
