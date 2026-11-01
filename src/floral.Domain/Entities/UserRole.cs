namespace floral.Domain.Entities;

public class UserRole
{
    public long UserId { get; private set; }

    public long RoleId { get; private set; }

    public long? AssignedBy { get; private set; }

    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;

    public Role Role { get; private set; } = null!;

    public User? AssignedByUser { get; private set; }

    private UserRole()
    {
        // Untuk EF Core
    }

    public UserRole(
        long userId,
        long roleId,
        long? assignedBy = null)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedBy = assignedBy;
        CreatedAt = DateTime.UtcNow;
    }
}