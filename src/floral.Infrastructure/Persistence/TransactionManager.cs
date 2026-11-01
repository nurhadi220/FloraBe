using floral.Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace floral.Infrastructure.Persistence;

public class TransactionManager : ITransactionManager
{
    private readonly AppDbContext _dbContext;

    public TransactionManager(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            await operation(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}