using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Issues
{
    public sealed record UserGrade
    {
        public EntityId UserId { get; private set; }
        public decimal Grade { get; private set; }

        internal UserGrade(EntityId userId, decimal grade)
        {
            UserId = userId;
            Grade = grade;
        }
    }
}
