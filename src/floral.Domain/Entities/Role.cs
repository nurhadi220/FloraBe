namespace floral.Domain.Entities;

public class Role
{
    public long Id { get; private set; }

    public string Slug { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsSystem { get; private set; } = true;

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    // Navigation property
    public ICollection<UserRole> UserRoles { get; private set; }
        = new List<UserRole>();

    private Role()
    {
        // Untuk EF Core
    }

    public Role(
        string slug,
        string name,
        string? description = null,
        bool isSystem = true)
    {
        Slug = slug;
        Name = name;
        Description = description;
        IsSystem = isSystem;
        CreatedAt = DateTime.UtcNow;
    }
}