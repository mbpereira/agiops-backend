using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;

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
            SetUser(userId);
            Grant = grant;
        }

        public void SetUser(int userId)
        {
            if (UserId.Value.GreaterThan(0))
            {
                AddError(AccessGrantErrors.UserChange);
                return;
            }

            if (!userId.GreaterThan(0))
            {
                AddError(AccessGrantErrors.InvalidUserId);
                return;
            }

            UserId = new EntityId(userId);
        }

        public static AccessGrant New(int userId, int tenantId, Resources resource, GrantScopes scope) =>
            new(EntityId.AutoIncrement(), userId, tenantId, new(resource, scope));

        public static AccessGrant Load(int id, int userId, int tenantId, Resources resource, GrantScopes scope, int recordId) =>
            new(id, userId, tenantId, new(resource, scope, new(recordId)));
    }
}
