using SkillSwap.Domain.Common;
using SkillSwap.Domain.Enums;

namespace SkillSwap.Domain.Entities;

/// <summary>
/// Represents a permission granted to a role
/// </summary>
public class RolePermission : BaseEntity
{
    /// <summary>
    /// Private constructor for Entity Framework
    /// </summary>
    private RolePermission() { }

    /// <summary>
    /// Creates a new RolePermission
    /// </summary>
    /// <param name="roleId">The ID of the role</param>
    /// <param name="permission">The permission being granted/denied</param>
    /// <param name="isGranted">Whether the permission is granted (true) or denied (false)</param>
    /// <param name="grantedBy">Who granted/denied this permission</param>
    public RolePermission(Guid roleId, Permission permission, bool isGranted, string grantedBy)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));
        if (string.IsNullOrWhiteSpace(grantedBy))
            throw new ArgumentException("GrantedBy cannot be null or empty", nameof(grantedBy));

        Id = Guid.NewGuid();
        RoleId = roleId;
        Permission = permission;
        IsGranted = isGranted;
        GrantedBy = grantedBy.Trim();
        GrantedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// The ID of the role this permission applies to
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// The specific permission
    /// </summary>
    public Permission Permission { get; private set; }

    /// <summary>
    /// Whether this permission is granted (true) or explicitly denied (false)
    /// </summary>
    public bool IsGranted { get; private set; }

    /// <summary>
    /// When this permission was granted/denied
    /// </summary>
    public DateTime GrantedAt { get; private set; }

    /// <summary>
    /// Who granted/denied this permission
    /// </summary>
    public string GrantedBy { get; private set; } = string.Empty;

    /// <summary>
    /// Optional conditions or context for this permission
    /// </summary>
    public string? Conditions { get; private set; }

    /// <summary>
    /// Navigation property to the role
    /// </summary>
    public Role Role { get; private set; } = null!;

    /// <summary>
    /// Updates the permission grant status
    /// </summary>
    /// <param name="isGranted">Whether to grant or deny the permission</param>
    /// <param name="updatedBy">Who is making this update</param>
    public void UpdatePermission(bool isGranted, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        IsGranted = isGranted;
        GrantedBy = updatedBy.Trim();
        GrantedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets conditions for this permission
    /// </summary>
    /// <param name="conditions">The conditions or context</param>
    public void SetConditions(string? conditions)
    {
        Conditions = conditions?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets a human-readable description of this permission assignment
    /// </summary>
    /// <returns>Description of the permission assignment</returns>
    public string GetDescription()
    {
        var action = IsGranted ? "granted" : "denied";
        var conditionsText = !string.IsNullOrEmpty(Conditions)
            ? $" with conditions: {Conditions}"
            : "";
        return $"Permission {Permission} {action} on {GrantedAt:yyyy-MM-dd} by {GrantedBy}{conditionsText}";
    }
}
