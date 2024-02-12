using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Users
{
    public record Grant(Resources Resources, GrantScope Scope, EntityId? RecordId = null);
}