using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Issues
{
    public sealed class VotingSystem : TenantableAggregateRoot
    {
        public EntityId UserId { get; private set; } = EntityId.Blank();
        public string Description { get; private set; } = string.Empty;
        public IList<int> Grades { get; private set; } = new List<int>();

        private VotingSystem(int id, int tenantId, string description, int userId, IList<int> grades) : base(id, tenantId)
        {
            Describe(description);
            DefineOwner(userId);
            DefinePossbileGrades(grades);
        }

        public void DefineOwner(int userId)
        {
            if (UserId.Value.GreaterThan(0))
            {
                AddError(new Error(nameof(VotingSystem), nameof(userId), VotingSystemConstants.Messages.OwnerAlreadyDefined));
                return;
            }

            if (!userId.GreaterThan(0))
            {
                AddError(Error.GreaterThan(nameof(VotingSystem), nameof(userId), value: 0));
                return;
            }

            UserId = new EntityId(userId);
        }

        public void DefinePossbileGrades(IList<int> grades)
        {
            if (grades.IsEmpty())
            {
                AddError(Error.EmptyCollection(nameof(VotingSystem), nameof(grades)));
                return;
            }

            Grades = grades.AsReadOnly();
        }

        public void Describe(string description)
        {
            if (!description.HasMinLength(minLength: 3))
            {
                AddError(Error.MinLength(nameof(VotingSystem), nameof(description), minLength: 3));
                return;
            }

            Description = description;
        }

        public static VotingSystem New(int tenantId, string description, int userId, IList<int> grades)
            => new(EntityId.AutoIncrement(), tenantId, description, userId, grades);
    }
}
