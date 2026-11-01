using floral.Application.Features.Auth.DTOs;
using floral.Domain.Enums;
using FluentValidation;

namespace floral.Application.Features.Auth.Validators;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        // Name
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Nama wajib diisi.")
            .MaximumLength(50)
            .WithMessage("Nama maksimal 50 karakter.");

        // Email
        RuleFor(request => request.Email)
            .EmailAddress()
            .WithMessage("Format email tidak valid.")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.Email)
            .MaximumLength(255)
            .WithMessage("Email maksimal 255 karakter.");

        // Phone Number
        RuleFor(request => request.PhoneNumber)
            .MaximumLength(20)
            .WithMessage("Nomor telepon maksimal 20 karakter.");

        // Minimal salah satu: Email atau Phone Number
        RuleFor(request => request)
            .Must(request =>
                !string.IsNullOrWhiteSpace(request.Email) ||
                !string.IsNullOrWhiteSpace(request.PhoneNumber))
            .WithMessage("Email atau nomor telepon wajib diisi.");

        // Gender
        RuleFor(request => request.Gender)
            .NotNull()
            .WithMessage("Jenis kelamin wajib dipilih.");

        RuleFor(request => request.Gender)
            .Must(gender =>
                gender == GenderUser.Pria ||
                gender == GenderUser.Wanita)
            .WithMessage("Jenis kelamin tidak valid.")
            .When(request => request.Gender.HasValue);

        // Password
        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("Password wajib diisi.")
            .MinimumLength(8)
            .WithMessage("Password minimal 8 karakter.");

        // Terms
        RuleFor(request => request.TermsAccepted)
            .Equal(true)
            .WithMessage("Kamu harus menyetujui syarat dan ketentuan.");

        RuleFor(request => request.TermsVersion)
            .NotEmpty()
            .WithMessage("Versi syarat dan ketentuan wajib diisi.")
            .MaximumLength(10)
            .WithMessage("Versi syarat dan ketentuan maksimal 10 karakter.");
    }
}