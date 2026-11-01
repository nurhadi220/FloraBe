using floral.Domain.Common;
using floral.Domain.Enums;

namespace floral.Domain.Entities;

public class User : BaseEntity
{
    private User()
    {
        Name = string.Empty;
        PublicId = string.Empty;
        TermsVersion = string.Empty;
    }

    public User(
        string name,
        string publicId,
        string? email,
        GenderUser gender,
        string? phoneNumber,
        string? passwordHash,
        RegistrationSource registrationSource,
        DateTime termsAcceptedAt,
        string termsVersion)
    {
        Name = name;
        PublicId = publicId;
        Email = email;
        Gender = gender;
        PhoneNumber = phoneNumber;
        Password = passwordHash;

        Status = UserStatus.Pending;
        RegistrationSource = registrationSource;

        TermsAcceptedAt = termsAcceptedAt;
        TermsVersion = termsVersion;

        UpdatedAt = null;
        DeletedAt = null;
    }

    public string PublicId { get; private set; }

    public string Name { get; private set; }

    public string? Email { get; private set; }
    public GenderUser Gender { get; private set; }

    public string Username { get; private set; } = string.Empty;

    public string? PhoneNumber { get; private set; }

    public string? Password { get; private set; }

    public string? AvatarPath { get; private set; }

    public UserStatus Status { get; private set; }

    public RegistrationSource RegistrationSource { get; private set; }

    public DateTime? EmailVerifiedAt { get; private set; }

    public DateTime? PhoneVerifiedAt { get; private set; }

    public DateTime TermsAcceptedAt { get; private set; }

    public string TermsVersion { get; private set; }

    public DateTime? MarketingOptInAt { get; private set; }

    public DateTime? LastLoginAt { get; private set; }

    public string? RememberToken { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public void UpdateProfile(
        string name,
        string? email,
        string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Nama tidak boleh kosong.",
                nameof(name));
        }

        Name = name.Trim();
        Email = email;
        PhoneNumber = phoneNumber;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAvatar(string? avatarPath)
    {
        AvatarPath = avatarPath;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        Status = UserStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username tidak boleh kosong.");

        Username = username.Trim();
    }

    public void Deactivate()
    {
        Status = UserStatus.Deactivated;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkEmailAsVerified()
    {
        EmailVerifiedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkPhoneAsVerified()
    {
        PhoneVerifiedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException(
                "Password hash tidak boleh kosong.",
                nameof(passwordHash));
        }

        Password = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsDeleted()
    {
        return DeletedAt.HasValue;
    }

    public bool IsActive()
    {
        return Status == UserStatus.Active
            && !IsDeleted();
    }
}