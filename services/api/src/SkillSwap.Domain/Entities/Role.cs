using SkillSwap.Domain.Common;
using SkillSwap.Domain.Enums;

namespace SkillSwap.Domain.Entities;

/// <summary>
/// Represents a role within the SkillSwap system
/// </summary>
public class Role : BaseEntity
{
    private readonly List<UserRole> _userRoles = new();
    private readonly List<RolePermission> _rolePermissions = new();

    /// <summary>
    /// Private constructor for Entity Framework
    /// </summary>
    private Role() { }

    /// <summary>
    /// Creates a new Role instance
    /// </summary>
    /// <param name="roleType">The type of role</param>
    /// <param name="name">The role name</param>
    /// <param name="description">Optional description of the role</param>
    public Role(RoleType roleType, string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be null or empty", nameof(name));

        Id = Guid.NewGuid();
        RoleType = roleType;
        Name = name.Trim();
        Description = description?.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// The type of role
    /// </summary>
    public RoleType RoleType { get; private set; }

    /// <summary>
    /// The name of the role
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Optional description of the role and its purpose
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Whether this role is currently active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Collection of users assigned to this role
    /// </summary>
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    /// <summary>
    /// Collection of permissions granted to this role
    /// </summary>
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    /// <summary>
    /// Updates the role information
    /// </summary>
    /// <param name="name">New role name</param>
    /// <param name="description">New role description</param>
    public void UpdateRole(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be null or empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the role
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the role
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if this role has a specific permission
    /// </summary>
    /// <param name="permission">The permission to check</param>
    /// <returns>True if the role has the permission</returns>
    public bool HasPermission(Permission permission)
    {
        return IsActive && _rolePermissions.Any(rp => rp.Permission == permission && rp.IsGranted);
    }

    /// <summary>
    /// Gets all granted permissions for this role
    /// </summary>
    /// <returns>Collection of granted permissions</returns>
    public IEnumerable<Permission> GetGrantedPermissions()
    {
        return _rolePermissions.Where(rp => rp.IsGranted).Select(rp => rp.Permission);
    }
}
