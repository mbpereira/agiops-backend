using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Issues
{
    public sealed record UserGrade
    {
        public EntityId UserId { get; private set; }
        public string Grade { get; private set; }

        internal UserGrade(EntityId userId, string grade)
        {
            UserId = userId;
            Grade = grade;
        }
    }
}