using floral.Application.Interfaces.Repositories;
using floral.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace floral.Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _dbContext;

    public RoleRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .FirstOrDefaultAsync(
                role => role.Slug == slug,
                cancellationToken);
    }
}