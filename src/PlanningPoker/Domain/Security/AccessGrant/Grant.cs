using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Security.AccessGrant
{
    public record Grant(Resources Resources, GrantScope Scope, EntityId? RecordId = null);
}