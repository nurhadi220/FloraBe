using floral.Application.Features.Auth.DTOs;
using floral.Application.Features.Auth.Validators;
using floral.Application.Interfaces.Repositories;
using floral.Application.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using floral.Domain.Entities;
using floral.Domain.Enums;
using FluentValidation;

namespace floral.Application.Features.Auth.Services;


public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly RegisterValidator _registerValidator;
    private readonly LoginValidator _loginValidator;
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ITransactionManager _transactionManager;

    public AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    RegisterValidator registerValidator,
    LoginValidator loginValidator,
    IConfiguration configuration,
    IRefreshTokenRepository refreshTokenRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    ITransactionManager transactionManager)
{
    _userRepository = userRepository;
    _passwordHasher = passwordHasher;
    _registerValidator = registerValidator;
    _loginValidator = loginValidator;
    _configuration = configuration;
    _refreshTokenRepository = refreshTokenRepository;
    _roleRepository = roleRepository;
    _userRoleRepository = userRoleRepository;
    _transactionManager = transactionManager;
}

public async Task<RegisterResponse> RegisterAsync(
    RegisterRequest request,
    CancellationToken cancellationToken = default)
{
    // 1. Validasi request
    await _registerValidator.ValidateAndThrowAsync(
        request,
        cancellationToken);

    // 2. Normalisasi input
    var name = request.Name.Trim();

    var email = string.IsNullOrWhiteSpace(request.Email)
        ? null
        : request.Email.Trim().ToLowerInvariant();

    var phoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
        ? null
        : request.PhoneNumber.Trim();

    // 3. Cek email duplikat
    if (email is not null &&
        await _userRepository.EmailExistsAsync(
            email,
            cancellationToken: cancellationToken))
    {
        throw new InvalidOperationException(
            "Email sudah terdaftar.");
    }

    // 4. Cek nomor telepon duplikat
    if (phoneNumber is not null &&
        await _userRepository.PhoneNumberExistsAsync(
            phoneNumber,
            cancellationToken: cancellationToken))
    {
        throw new InvalidOperationException(
            "Nomor telepon sudah terdaftar.");
    }

    // 5. Hash password
    var passwordHash = _passwordHasher.HashPassword(
        request.Password);

    // 6. Buat User dengan username sementara yang unik
    var publicId = $"usr_{Guid.NewGuid():N}";
    var temporaryUsername = $"tmp_{Guid.NewGuid():N}"[..20];

    var user = new User(
        name: name,
        publicId: publicId,
        email: email,
        gender: request.Gender!.Value,
        phoneNumber: phoneNumber,
        passwordHash: passwordHash,
        registrationSource: RegistrationSource.Web,
        termsAcceptedAt: DateTime.UtcNow,
        termsVersion: request.TermsVersion.Trim());

    user.SetUsername(temporaryUsername);

    // 7. Simpan User dan role dalam satu transaksi
    await _transactionManager.ExecuteInTransactionAsync(
        async ct =>
        {
            // Simpan user agar mendapatkan Id dari database
            await _userRepository.AddAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);

            // Buat username berdasarkan Id user
            var username = $"user{user.Id:D4}";

            user.SetUsername(username);

            // Simpan username final
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(ct);

            // Cari role customer
            var customerRole = await _roleRepository.GetBySlugAsync(
                "customer",
                ct);

            if (customerRole is null)
            {
                throw new InvalidOperationException(
                    "Role customer belum tersedia.");
            }

            // Hubungkan user dengan role customer
            var userRole = new UserRole(
                user.Id,
                customerRole.Id,
                null);

            await _userRoleRepository.AddAsync(userRole, ct);

            await _userRepository.SaveChangesAsync(ct);
        },
        cancellationToken);

    // 8. Buat response
    return new RegisterResponse
    {
        PublicId = user.PublicId,
        MemberNumber = $"Member #{user.Id:D4}",
        Username = user.Username,
        Name = user.Name,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        Status = user.Status.ToString(),
        CreatedAt = user.CreatedAt
    };
}
    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validasi request
        await _loginValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        // 2. Normalisasi email
        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        // 3. Cari user berdasarkan email
        var user = await _userRepository.GetByEmailAsync(
            email,
            cancellationToken);

        // 4. Verifikasi user dan password
        if (user.Password is null ||
            !_passwordHasher.VerifyPassword(
                request.Password,
                user.Password))
        {
            throw new UnauthorizedAccessException(
                "Email atau password tidak valid.");
        }

        user.RecordLogin();

        _userRepository.Update(user);

        await _userRepository.SaveChangesAsync(
            cancellationToken);

        // 5. Ambil konfigurasi JWT
        var secretKey = _configuration["Jwt:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException(
                "Konfigurasi Jwt:SecretKey belum tersedia.");
        }

        var issuer = _configuration["Jwt:Issuer"]
            ?? "floral-api";

        var audience = _configuration["Jwt:Audience"]
            ?? "floral-client";

        var expiryMinutes = int.TryParse(
            _configuration["Jwt:ExpiryMinutes"],
            out var configuredExpiry)
                ? configuredExpiry
                : 60;

        var expiresAt = DateTime.UtcNow.AddMinutes(
            expiryMinutes);

        // 6. Buat claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.PublicId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.Name)
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(
                new Claim(ClaimTypes.Email, user.Email));
        }

        // 7. Buat signing key
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // 8. Generate JWT
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        // 9. Buat refresh token
        var refreshTokenValue = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

        // Hash refresh token untuk disimpan di database
        var refreshTokenHash = Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(refreshTokenValue)));

        // Refresh token berlaku selama 7 hari
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

        // Buat entity refresh token
        var refreshToken = new RefreshToken(
            user.Id,
            refreshTokenHash,
            refreshTokenExpiresAt);

        // Simpan refresh token ke database
        await _refreshTokenRepository.AddAsync(
            refreshToken,
            cancellationToken);

        await _refreshTokenRepository.SaveChangesAsync(
            cancellationToken);

        // 10. Kembalikan response
        return new LoginResponse
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,

            RefreshToken = refreshTokenValue,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,

            PublicId = user.PublicId,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Status = user.Status.ToString()
        };
    }

    public async Task<LoginResponse> RefreshAsync(
        RefreshRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validasi input
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new UnauthorizedAccessException(
                "Refresh token tidak valid.");
        }

        // 2. Hash refresh token yang dikirim frontend
        var refreshTokenHash = Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(request.RefreshToken)));

        // 3. Cari refresh token berdasarkan hash
        var storedRefreshToken =
            await _refreshTokenRepository.GetByTokenHashAsync(
                refreshTokenHash,
                cancellationToken);

        // 4. Pastikan token ditemukan dan masih aktif
        if (storedRefreshToken is null ||
            !storedRefreshToken.IsActive())
        {
            throw new UnauthorizedAccessException(
                "Refresh token tidak valid atau sudah kedaluwarsa.");
        }

        // 5. Cari user berdasarkan UserId dari refresh token
        var user = await _userRepository.GetByIdAsync(
            storedRefreshToken.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "User tidak ditemukan.");
        }

        // 6. Ambil konfigurasi JWT
        var secretKey = _configuration["Jwt:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException(
                "Konfigurasi Jwt:SecretKey belum tersedia.");
        }

        var issuer = _configuration["Jwt:Issuer"]
            ?? "floral-api";

        var audience = _configuration["Jwt:Audience"]
            ?? "floral-client";

        var expiryMinutes = int.TryParse(
            _configuration["Jwt:ExpiryMinutes"],
            out var configuredExpiry)
                ? configuredExpiry
                : 60;

        var expiresAt = DateTime.UtcNow.AddMinutes(
            expiryMinutes);

        // 7. Buat claims
        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.PublicId),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),

            new(
                ClaimTypes.Name,
                user.Name)
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Email,
                    user.Email));
        }

        // 8. Buat signing key
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // 9. Generate access token baru
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        // 10. Buat refresh token baru
        var refreshTokenValue = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

        var newRefreshTokenHash = Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(refreshTokenValue)));

        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

        var newRefreshToken = new RefreshToken(
            user.Id,
            newRefreshTokenHash,
            refreshTokenExpiresAt);

        // 11. Cabut refresh token lama
        storedRefreshToken.Revoke();

        // 12. Simpan refresh token baru
        await _refreshTokenRepository.AddAsync(
            newRefreshToken,
            cancellationToken);

        await _refreshTokenRepository.SaveChangesAsync(
            cancellationToken);

        // 13. Kembalikan response
        return new LoginResponse
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,

            RefreshToken = refreshTokenValue,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,

            PublicId = user.PublicId,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Status = user.Status.ToString()
        };
    }
    public async Task LogoutAsync(
        LogoutRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new UnauthorizedAccessException(
                "Refresh token tidak valid.");
        }

        // Hash harus sama dengan LoginAsync dan RefreshAsync
        var tokenHash = Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(request.RefreshToken)));

        var storedRefreshToken =
            await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

        if (storedRefreshToken is null ||
            !storedRefreshToken.IsActive())
        {
            throw new UnauthorizedAccessException(
                "Refresh token tidak valid atau sudah kedaluwarsa.");
        }

        // Cabut refresh token
        storedRefreshToken.Revoke();

        // Simpan perubahan
        await _refreshTokenRepository.SaveChangesAsync(
            cancellationToken);
    }
// untuk nge hash password
    private static string HashToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToBase64String(hashBytes);
    }

}
