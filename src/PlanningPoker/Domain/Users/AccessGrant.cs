using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public sealed class AccessGrant : TenantableAggregateRoot
    {
        public EntityId UserId { get; private set; } = EntityId.Blank();
        public Grant Grant { get; private set; }

        private AccessGrant(
            int id,
            int userId,
            int tenantId,
            Grant grant) : base(id, tenantId)
        {
            DefineUser(userId);
            Grant = grant;
        }

        public void DefineUser(int userId)
        {
            if (!userId.GreaterThan(0))
            {
                AddError(Error.GreaterThan(nameof(Invitation), nameof(userId), value: 0));
                return;
            }

            UserId = new EntityId(userId);
        }

        public static AccessGrant New(int userId, int tenantId, Resources resource, GrantScopes scope) =>
            new(EntityId.AutoIncrement(), userId, tenantId, new(resource, scope));

        public static AccessGrant New(int id, int userId, int tenantId, Resources resource, GrantScopes scope, int recordId) =>
            new(id, userId, tenantId, new(resource, scope, new(recordId)));
    }
}
