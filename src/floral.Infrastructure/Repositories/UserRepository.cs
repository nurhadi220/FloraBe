using floral.Application.Interfaces.Repositories;
using floral.Infrastructure.Persistence;
using floral.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace floral.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByPublicIdAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(
                user => user.PublicId == publicId
                    && user.DeletedAt == null,
                cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = BuildUserQuery(search);

        return await query
            .AsNoTracking()
            .OrderByDescending(user => user.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        return await BuildUserQuery(search)
            .CountAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        string? excludePublicId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Users
            .Where(user =>
                user.Email == email &&
                user.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(excludePublicId))
        {
            query = query.Where(
                user => user.PublicId != excludePublicId);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> PhoneNumberExistsAsync(
        string phoneNumber,
        string? excludePublicId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Users
            .Where(user =>
                user.PhoneNumber == phoneNumber &&
                user.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(excludePublicId))
        {
            query = query.Where(
                user => user.PublicId != excludePublicId);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(
            user,
            cancellationToken);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }

    public async Task<User?> GetByEmailAsync(
    string email,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(
                user => user.Email == email &&
                        user.DeletedAt == null,
                cancellationToken);
    }
    public async Task<User?> GetByIdAsync(
    long id,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(
                user => user.Id == id &&
                        user.DeletedAt == null,
                cancellationToken);
    }
    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<User> BuildUserQuery(string? search)
    {
        var query = _dbContext.Users
            .Where(user => user.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();

            query = query.Where(user =>
                user.Name.Contains(keyword) ||
                (user.Email != null &&
                 user.Email.Contains(keyword)) ||
                (user.PhoneNumber != null &&
                 user.PhoneNumber.Contains(keyword)));
        }

        return query;
    }
}