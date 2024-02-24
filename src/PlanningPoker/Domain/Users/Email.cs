namespace PlanningPoker.Domain.Users;

public sealed record Email
{
    internal Email(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public static Email Empty()
    {
        return new Email(string.Empty);
    }
}