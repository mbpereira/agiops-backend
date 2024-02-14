using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Users
{
    public sealed record Grant
    {
        public Resources Resource { get; private set; }
        public GrantScopes Scope { get; private set; }
        public EntityId? RecordId { get; private set; } = null;

        internal Grant(Resources resource, GrantScopes scope, EntityId? recordId = null)
        {
            Resource = resource;
            Scope = scope;
            RecordId = recordId;
        }
    }
}