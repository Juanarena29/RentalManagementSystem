namespace CT.Domain.Interfaces;

/// <summary>
/// Coordina transacciones cuando un UseCase necesita persistir
/// cambios en múltiples repositorios de forma atómica.
/// Implementa IAsyncDisposable para garantizar la limpieza de transacciones
/// incluso si el consumidor olvida hacer Commit/Rollback.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
