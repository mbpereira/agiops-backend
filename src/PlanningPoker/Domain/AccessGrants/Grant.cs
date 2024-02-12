using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.AccessGrants
{
    public record Grant(Resources Resources, GrantScope Scope, EntityId? RecordId = null);
}