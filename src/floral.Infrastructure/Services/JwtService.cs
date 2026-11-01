using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using floral.Application.Interfaces.Services;
using floral.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace floral.Infrastructure.Services;

/// <summary>
/// Implementasi JWT token service.
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        var secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException(
                "JWT SecretKey tidak dikonfigurasi.");

        var issuer = jwtSettings["Issuer"] ?? "floral-api";
        var audience = jwtSettings["Audience"] ?? "floral-client";

        var expirationMinutes = int.TryParse(
            jwtSettings["ExpirationMinutes"],
            out var minutes)
                ? minutes
                : 60;

        if (expirationMinutes <= 0)
        {
            throw new InvalidOperationException(
                "JWT ExpirationMinutes harus lebih dari 0.");
        }

        var keyBytes = Encoding.UTF8.GetBytes(secretKey);

        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT SecretKey minimal 32 byte.");
        }

        var key = new SymmetricSecurityKey(keyBytes);

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.PublicId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.Name),
            new("status", user.Status.ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(
                new Claim(JwtRegisteredClaimNames.Email, user.Email));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}