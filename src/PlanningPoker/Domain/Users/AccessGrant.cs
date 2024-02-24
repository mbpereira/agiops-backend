#region

using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Domain.Users;

public sealed class AccessGrant : TenantableAggregateRoot
{
    private AccessGrant(
        string id,
        string userId,
        string tenantId,
        Grant grant) : base(id, tenantId)
    {
        SetUser(userId);
        Grant = grant;
    }

    public EntityId UserId { get; private set; } = EntityId.Empty;
    public Grant Grant { get; private set; }

    public void SetUser(string userId)
    {
        if (UserId.Value.IsPresent())
        {
            AddError(AccessGrantErrors.UserChange);
            return;
        }

        if (!userId.IsPresent())
        {
            AddError(AccessGrantErrors.InvalidUserId);
            return;
        }

        UserId = userId;
    }

    public static AccessGrant New(string userId, string tenantId, Resources resource, GrantScopes scope)
    {
        return new AccessGrant(EntityId.Generate(), userId, tenantId, new Grant(resource, scope));
    }
}