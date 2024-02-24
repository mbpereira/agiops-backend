#region

using PlanningPoker.Domain.Abstractions.Clock;

#endregion

namespace PlanningPoker.Domain.Abstractions;

public abstract class Entity : Validatable
{
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

    protected IDateTimeProvider DateTimeProvider { get; }

    public EntityId Id { get; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    protected void Updated()
    {
        UpdatedAtUtc = DateTimeProvider.UtcNow();
    }
}