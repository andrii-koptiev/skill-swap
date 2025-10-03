namespace SkillSwap.Application.Interfaces;

/// <summary>
/// Represents a database transaction within the Unit of Work
/// </summary>
public interface IUnitOfWorkTransaction : IDisposable
{
    /// <summary>
    /// Commit the transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback the transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
