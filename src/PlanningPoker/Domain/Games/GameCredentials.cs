namespace PlanningPoker.Domain.Games;

public sealed record GameCredentials
{
    internal GameCredentials(string password)
    {
        Password = password;
    }

    public string Password { get; private set; }
}