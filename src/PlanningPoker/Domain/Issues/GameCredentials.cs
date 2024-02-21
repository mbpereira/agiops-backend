namespace PlanningPoker.Domain.Issues
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