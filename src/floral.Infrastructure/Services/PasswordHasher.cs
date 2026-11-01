using floral.Application.Interfaces.Services;
namespace floral.Infrastructure.Services;

/// <summary>
/// Implementasi password hashing menggunakan BCrypt.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException(
                "Password tidak boleh kosong.",
                nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(
            password,
            workFactor: 12);
    }

    public bool VerifyPassword(
        string password,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(
                password,
                passwordHash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Hash tidak valid atau bukan hash BCrypt.
            return false;
        }
    }
}