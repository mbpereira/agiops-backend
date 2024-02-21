namespace PlanningPoker.Domain.Users
{
    public sealed record Email
    {
        public string Value { get; private set; }

        internal Email(string value)
        {
            Value = value;
        }

        public static Email Empty() => new(string.Empty);
    }
}