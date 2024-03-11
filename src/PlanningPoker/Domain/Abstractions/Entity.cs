#region

using PlanningPoker.Domain.Abstractions.Clock;

#endregion

namespace PlanningPoker.Domain.Abstractions;

public abstract class Entity : Validatable
{
    private IDateTimeProvider? _dateTimeProvider;

    protected Entity(string id, IDateTimeProvider dateTimeProvider)
    {
        Id = id;
        _dateTimeProvider = dateTimeProvider;
        CreatedAtUtc = DateTimeProvider.UtcNow();
    }

    protected Entity(string id)
        : this(id, DefaultDateTimeProvider.Instance)
    {
    }

    protected IDateTimeProvider DateTimeProvider => _dateTimeProvider ??= DefaultDateTimeProvider.Instance;

    public EntityId Id { get; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public void Updated()
    {
        UpdatedAtUtc = DateTimeProvider.UtcNow();
    }
}