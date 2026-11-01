using floral.Application.Interfaces.Repositories;
using floral.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace floral.Infrastructure.Persistence.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly AppDbContext _dbContext;

    public UserRoleRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.UserRoles.AddAsync(
            userRole,
            cancellationToken);
    }
}