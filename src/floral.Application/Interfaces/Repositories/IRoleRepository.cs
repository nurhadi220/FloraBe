using floral.Domain.Entities;

namespace floral.Application.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default);
}