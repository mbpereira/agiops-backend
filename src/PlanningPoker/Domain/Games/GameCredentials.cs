namespace PlanningPoker.Domain.Games
{
    public sealed record GameCredentials
    {
        public string Password { get; private set; }

        internal GameCredentials(string password)
        {
            Password = password;
        }
    }
}