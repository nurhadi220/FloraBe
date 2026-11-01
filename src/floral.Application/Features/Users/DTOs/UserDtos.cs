using floral.Domain.Entities;

namespace floral.Application.Features.Users.DTOs;

public class UserDto
{
    public string PublicId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string Status { get; set; } = string.Empty;

    public string RegistrationSource { get; set; } = string.Empty;

    public DateTime? LastLoginAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public static UserDto FromEntity(User user)
    {
        return new UserDto
        {
            PublicId = user.PublicId,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Status = user.Status.ToString(),
            RegistrationSource = user.RegistrationSource.ToString(),
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt
        };
    }
}

public class UserListDto
{
    public List<UserDto> Items { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages =>
        PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
}