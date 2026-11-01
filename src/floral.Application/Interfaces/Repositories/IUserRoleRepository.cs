using floral.Domain.Entities;

namespace floral.Application.Interfaces.Repositories;

public interface IUserRoleRepository
{
    Task AddAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default);
}