
using floral.Domain.Common;

namespace floral.Domain.Entities;

public class RefreshToken : BaseEntity
{
    private RefreshToken()
    {
        TokenHash = string.Empty;
    }

    public RefreshToken(
        long userId,
        string tokenHash,
        DateTime expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;

        RevokedAt = null;
    }

    public long UserId { get; private set; }

    public string TokenHash { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public bool IsRevoked()
    {
        return RevokedAt.HasValue;
    }

    public bool IsExpired()
    {
        return ExpiresAt <= DateTime.UtcNow;
    }

    public bool IsActive()
    {
        return !IsRevoked() && !IsExpired();
    }

    public void Revoke()
    {
        if (RevokedAt.HasValue)
        {
            return;
        }

        RevokedAt = DateTime.UtcNow;
    }
}
