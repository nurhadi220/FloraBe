using floral.Domain.Enums;

namespace floral.Application.Features.Auth.DTOs;

public class RegisterRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public GenderUser? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string Password { get; set; } = string.Empty;

    public bool TermsAccepted { get; set; }

    public string TermsVersion { get; set; } = string.Empty;
}