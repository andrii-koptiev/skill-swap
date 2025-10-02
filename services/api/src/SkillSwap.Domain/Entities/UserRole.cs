using SkillSwap.Domain.Common;

namespace SkillSwap.Domain.Entities;

/// <summary>
/// Represents the assignment of a role to a user
/// </summary>
public class UserRole : BaseEntity
{
    /// <summary>
    /// Private constructor for Entity Framework
    /// </summary>
    private UserRole() { }

    /// <summary>
    /// Creates a new UserRole assignment
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="roleId">The ID of the role</param>
    /// <param name="assignedBy">Who assigned this role</param>
    public UserRole(Guid userId, Guid roleId, string assignedBy)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));
        if (string.IsNullOrWhiteSpace(assignedBy))
            throw new ArgumentException("AssignedBy cannot be null or empty", nameof(assignedBy));

        Id = Guid.NewGuid();
        UserId = userId;
        RoleId = roleId;
        AssignedBy = assignedBy.Trim();
        AssignedAt = DateTime.UtcNow;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// The ID of the user assigned to the role
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The ID of the role assigned to the user
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// When the role was assigned
    /// </summary>
    public DateTime AssignedAt { get; private set; }

    /// <summary>
    /// Who assigned this role (could be a user ID, system identifier, etc.)
    /// </summary>
    public string AssignedBy { get; private set; } = string.Empty;

    /// <summary>
    /// When the role assignment expires (null means no expiration)
    /// </summary>
    public DateTime? ExpiresAt { get; private set; }

    /// <summary>
    /// Whether this role assignment is currently active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Navigation property to the role
    /// </summary>
    public Role Role { get; private set; } = null!;

    /// <summary>
    /// Sets an expiration date for this role assignment
    /// </summary>
    /// <param name="expiresAt">When the role assignment should expire</param>
    public void SetExpiration(DateTime expiresAt)
    {
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration date must be in the future", nameof(expiresAt));

        ExpiresAt = expiresAt;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes the expiration date from this role assignment
    /// </summary>
    public void RemoveExpiration()
    {
        ExpiresAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates this role assignment
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates this role assignment
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if this role assignment is currently valid
    /// </summary>
    /// <returns>True if the assignment is active and not expired</returns>
    public bool IsValid()
    {
        return IsActive && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);
    }
}
