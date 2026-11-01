namespace floral.Application.Interfaces.Services;

public interface ITransactionManager
{
    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);
}