using FluentValidation;
using floral.Application.Features.Auth.DTOs;

namespace floral.Application.Features.Auth.Validators;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("Email wajib diisi.")
            .EmailAddress()
            .WithMessage("Format email tidak valid.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("Password wajib diisi.");
    }
}