using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Users
{
    public record Grant(Resources Resource, GrantScopes Scope, EntityId? RecordId = null);
}