#region

using PlanningPoker.Domain.Abstractions.Clock;

#endregion

namespace PlanningPoker.Domain.Abstractions;

public abstract class Entity : Validatable
{
    protected Entity(string id, IDateTimeProvider dateTimeProvider)
        : this(id)
    {
        DateTimeProvider = dateTimeProvider;
    }

    protected Entity(string id)
    {
        Id = id;
        CreatedAtUtc = DateTimeProvider.UtcNow();
    }

    protected IDateTimeProvider DateTimeProvider { get; } = DefaultDateTimeProvider.Instance;

    public EntityId Id { get; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    protected void Updated()
    {
        UpdatedAtUtc = DateTimeProvider.UtcNow();
    }
}