namespace floral.Domain.Common;

public abstract class BaseEntity
{
    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }

    public DateTime? CreatedAt { get; private set; }
}
