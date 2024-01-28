namespace PlanningPoker.Domain.Abstractions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
