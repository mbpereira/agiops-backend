using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Issues
{
    public sealed class Game : TenantableAggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public EntityId UserId { get; private set; } = EntityId.Blank();
        public GameCredentials? Credentials { get; private set; }

        public Game(int id, int tenantId, string name, int userId, string? password = null)
            : base(id, tenantId)
        {
            Named(name);
            Owner(userId);
            DefinePassword(password);
        }

        public void Owner(int userId)
        {
            if (!userId.GreaterThan(0))
            {
                AddError(Error.GreaterThan(nameof(Game), nameof(userId), value: 0));
                return;
            }

            UserId = userId;
        }

        public void Named(string name)
        {
            if (!name.HasMinLength(minLength: 1))
            {
                AddError(Error.MinLength(nameof(Game), nameof(name), minLength: 1));
                return;
            }

            Name = name;
        }

        public void DefinePassword(string? password = null)
        {
            if (password.IsNullOrEmpty()) return;

            if (!password.HasMinLength(minLength: 6))
            {
                AddError(Error.MinLength(nameof(Game), nameof(password), minLength: 6));
                return;
            }

            Credentials = new GameCredentials(password!);
        }

        public static Game New(int tenantId, string name, int userId, string? password = null)
            => new(EntityId.AutoIncrement(), tenantId, name, userId, password);
        public static Game New(int id, int tenantId, string name, int userId, string? password = null)
            => new(id, tenantId, name, userId, password);
    }
}
