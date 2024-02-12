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
            [NotNull] EntityId id,
            [NotNull] EntityId userId,
            [NotNull] EntityId tenantId,
            [NotNull] Grant grant) : base(id)
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

        public static AccessGrant New(int userId, int tenantId, Resources resource, GrantScopes scope) =>
            new(EntityId.AutoIncrement(), userId, tenantId, new(resource, scope));

        public static AccessGrant New(int id, int userId, int tenantId, Resources resource, GrantScopes scope, int recordId) =>
            new(id, userId, tenantId, new(resource, scope, new(recordId)));
    }
}
