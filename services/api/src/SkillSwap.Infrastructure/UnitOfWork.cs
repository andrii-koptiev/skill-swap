using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SkillSwap.Application.Interfaces;
using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Domain.Common;
using SkillSwap.Infrastructure.Data;
using SkillSwap.Infrastructure.Repositories;

namespace SkillSwap.Infrastructure;

/// <summary>
/// Unit of Work implementation using Entity Framework Core
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SkillSwapDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private bool _disposed;

    // Specific repositories - lazy loaded
    private IUserRepository? _users;
    private ISkillRepository? _skills;
    private ISkillCategoryRepository? _skillCategories;
    private IUserSkillRepository? _userSkills;
    private IUserAvailabilityRepository? _userAvailability;
    private IUserPreferencesRepository? _userPreferences;
    private IRoleRepository? _roles;
    private IUserRoleRepository? _userRoles;
    private IRolePermissionRepository? _rolePermissions;

    public UnitOfWork(SkillSwapDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);

    public ISkillRepository Skills => _skills ??= new SkillRepository(_context);

    public ISkillCategoryRepository SkillCategories =>
        _skillCategories ??= new SkillCategoryRepository(_context);

    public IUserSkillRepository UserSkills => _userSkills ??= new UserSkillRepository(_context);

    public IUserAvailabilityRepository UserAvailability =>
        _userAvailability ??= new UserAvailabilityRepository(_context);

    public IUserPreferencesRepository UserPreferences =>
        _userPreferences ??= new UserPreferencesRepository(_context);

    public IRoleRepository Roles => _roles ??= new RoleRepository(_context);

    public IUserRoleRepository UserRoles => _userRoles ??= new UserRoleRepository(_context);

    public IRolePermissionRepository RolePermissions =>
        _rolePermissions ??= new RolePermissionRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            // Log the exception details here if needed
            throw new InvalidOperationException("Failed to save changes to the database.", ex);
        }
    }

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default
    )
    {
        var transaction = await _context
            .Database.BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);
        return new UnitOfWorkTransaction(transaction);
    }

    public IRepository<T> Repository<T>()
        where T : BaseEntity
    {
        var type = typeof(T);

        if (_repositories.ContainsKey(type))
        {
            return (IRepository<T>)_repositories[type];
        }

        var repository = new Repository<T>(_context);
        _repositories[type] = repository;
        return repository;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
            _repositories.Clear();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Unit of Work transaction implementation
/// </summary>
public class UnitOfWorkTransaction : IUnitOfWorkTransaction
{
    private readonly IDbContextTransaction _transaction;
    private bool _disposed;

    public UnitOfWorkTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw new InvalidOperationException("Failed to commit transaction.", ex);
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Log rollback failure but don't throw to avoid masking original exception
            throw new InvalidOperationException("Failed to rollback transaction.", ex);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction.Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
