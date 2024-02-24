using PlanningPoker.Domain.Abstractions.Clock;

namespace PlanningPoker.Domain.Abstractions;

public abstract class Entity : Validatable
{
    protected IDateTimeProvider DateTimeProvider { get; }

    public EntityId Id { get; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    protected Entity(string id, IDateTimeProvider dateTimeProvider)
    {
        Id = id;
        DateTimeProvider = dateTimeProvider;
        CreatedAtUtc = DateTimeProvider.UtcNow();
    }

    protected Entity(string id)
        : this(id, DefaultDateTimeProvider.Instance)
    {
    }

    protected void Updated()
    {
        UpdatedAtUtc = DateTimeProvider.UtcNow();
    }
}