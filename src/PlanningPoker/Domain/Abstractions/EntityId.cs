namespace PlanningPoker.Domain.Abstractions;

public sealed record EntityId
{
    public static readonly EntityId Empty = new("");

    internal EntityId(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static EntityId Generate()
    {
        return new EntityId(Guid.NewGuid().ToString());
    }

    public static implicit operator EntityId(string value)
    {
        return new EntityId(value);
    }

    public static implicit operator string(EntityId id)
    {
        return id.Value;
    }
}