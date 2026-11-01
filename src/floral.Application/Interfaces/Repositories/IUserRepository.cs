using floral.Domain.Entities;

namespace floral.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByPublicIdAsync(
        string publicId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? search,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(
        string email,
        string? excludePublicId = null,
        CancellationToken cancellationToken = default);

    Task<bool> PhoneNumberExistsAsync(
        string phoneNumber,
        string? excludePublicId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(
    string email,
    CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(
    long id,
    CancellationToken cancellationToken = default);

    void Update(User user);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}