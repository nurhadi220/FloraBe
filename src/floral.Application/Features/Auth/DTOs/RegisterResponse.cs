namespace floral.Application.Features.Auth.DTOs;

public class RegisterResponse
{
    public string PublicId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }
    public string MemberNumber { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}