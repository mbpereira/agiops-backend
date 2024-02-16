using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;

namespace PlanningPoker.Domain.Issues
{
    public sealed class Game : TenantableAggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public EntityId UserId { get; private set; } = EntityId.Blank();
        public GameCredentials? Credentials { get; private set; }
        public GradeDetails GradeDetails { get; private set; } = GradeDetails.Empty();

        public Game(int id, int tenantId, string name, int userId, VotingSystem votingSystem, string? password = null)
            : base(id, tenantId)
        {
            SetName(name);
            SetOwner(userId);
            SetPassword(password);
            SetVotingSystem(votingSystem);
        }

        public void SetVotingSystem(VotingSystem votingSystem)
        {
            if (votingSystem is null || !votingSystem.IsValid)
            {
                AddError(GameErrors.InvalidVotingSystem);
                return;
            }

            GradeDetails = votingSystem.GradeDetails;
        }

        public void SetOwner(int userId)
        {
            if (UserId.Value.GreaterThan(0))
            {
                AddError(GameErrors.OwnerAlreadySet);
                return;
            }

            if (!userId.GreaterThan(0))
            {
                AddError(GameErrors.InvalidUserId);
                return;
            }

            UserId = userId;
        }

        public void SetName(string name)
        {
            if (!name.HasMinLength(minLength: 1))
            {
                AddError(GameErrors.InvalidName);
                return;
            }

            Name = name;
        }

        public void SetPassword(string? password = null)
        {
            if (password.IsNullOrEmpty())
            {
                Credentials = null;
                return;
            }

            if (!password.HasMinLength(minLength: 6))
            {
                AddError(GameErrors.InvalidPassword);
                return;
            }

            Credentials = new GameCredentials(password!);
        }

        public static Game New(int tenantId, string name, int userId, VotingSystem votingSystem, string? password = null)
            => new(EntityId.AutoIncrement(), tenantId, name, userId, votingSystem, password);

        public static Game Load(int id, int tenantId, string name, int userId, VotingSystem votingSystem, string? password = null)
            => new(id, tenantId, name, userId, votingSystem, password);
    }
}
