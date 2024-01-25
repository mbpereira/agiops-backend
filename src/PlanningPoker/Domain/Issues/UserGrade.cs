using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Issues
{
    public sealed record UserGrade(EntityId UerId, decimal Grade);
}
