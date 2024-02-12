using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Users
{
    public record Grant(Resources Resource, GrantScope Scope, EntityId? RecordId = null);
}