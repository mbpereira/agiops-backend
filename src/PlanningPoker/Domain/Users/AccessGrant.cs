using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Validation;
using System.Diagnostics.CodeAnalysis;

namespace PlanningPoker.Domain.Users
{
    public sealed class AccessGrant : AggregateRoot<AccessGrant>, ITenantable
    {
        public EntityId UserId { get; private set; }
        public EntityId TenantId { get; private set; }
        public Grant Grant { get; private set; }

        private AccessGrant(
            [NotNull] EntityId userId,
            [NotNull] EntityId tenantId,
            [NotNull] Grant grant)
        {
            UserId = userId;
            TenantId = tenantId;
            Grant = grant;
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<AccessGrant> validator)
        {
            validator.CreateRuleFor(a => a.UserId.Value, nameof(UserId))
                .GreaterThan(0);

            validator.CreateRuleFor(a => a.TenantId.Value, nameof(TenantId))
                .GreaterThan(0);
        }

        public static AccessGrant New(int userId, int tenantId, Resources resource, GrantScope scope) =>
            new(userId, tenantId, new(resource, scope));

        public static AccessGrant New(int userId, int tenantId, Resources resource, GrantScope scope, int recordId) =>
            new(userId, tenantId, new(resource, scope, new(recordId)));
    }
}
