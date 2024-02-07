namespace PlanningPoker.Domain.Abstractions
{
    public sealed record EntityId
    {
        public int Value { get; private set; }

        internal EntityId(int value) { Value = value; }

        public static EntityId AutoIncrement() => new(value: 0);

        public static implicit operator EntityId(int value) => new(value: value);
    }
}
