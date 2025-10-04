using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Domain.Common;

namespace SkillSwap.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation using Entity Framework Core
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public class Repository<T> : IRepository<T>
    where T : BaseEntity
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet.FindAsync([id], cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    )
    {
        if (predicate == null)
            return await _dbSet.CountAsync(cancellationToken).ConfigureAwait(false);

        return await _dbSet.CountAsync(predicate, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default
    )
    {
        if (pageNumber <= 0)
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

        if (pageSize <= 0)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        var query = _dbSet.AsQueryable();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }
        else
        {
            query = query.OrderBy(x => x.Id);
        }

        var skip = (pageNumber - 1) * pageSize;
        return await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var entityEntry = await _dbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return entityEntry.Entity;
    }

    public virtual async Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    )
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        await _dbSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
    }

    public virtual T Update(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        entity.UpdateTimestamp();
        var entityEntry = _dbSet.Update(entity);
        return entityEntry.Entity;
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        foreach (var entity in entities)
        {
            entity.UpdateTimestamp();
        }

        _dbSet.UpdateRange(entities);
    }

    public virtual void Delete(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Remove(entity);
    }

    public virtual async Task DeleteByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (entity != null)
        {
            Delete(entity);
        }
    }

    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        _dbSet.RemoveRange(entities);
    }

    public virtual async Task<int> DeleteAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        var entities = await FindAsync(predicate, cancellationToken).ConfigureAwait(false);
        var entitiesList = entities.ToList();

        if (entitiesList.Count > 0)
        {
            DeleteRange(entitiesList);
        }

        return entitiesList.Count;
    }
}
