namespace Domain.Abstractions
{
    public record EntityId(int Value)
    {
        public static EntityId AutoIncrement() => new(Value: 0);
    }
}
