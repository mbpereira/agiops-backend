using Domain.Abstractions;
using Domain.Validation;
using FluentValidation;

namespace Domain.Companys
{
    public class CompanyTeam : Entity<CompanyTeam>
    {
        public int CompanyId { get; private set; }
        public string Name { get; set; }

        private CompanyTeam(int companyId, string name)
            : this(id: EntityId.AutoIncrement(), companyId, name)
        {
        }

        private CompanyTeam(EntityId id, int companyId, string name)
            : base(id)
        {
            Id = id;
            Name = name;
            CompanyId = companyId;
        }

        protected override void ConfigureValidationRules(Validator<CompanyTeam> validator)
        {
            validator.RuleFor(c => c.CompanyId)
                .GreaterThan(0);
            validator.RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(3);
        }

        public static CompanyTeam New(int companyId, string name) => new(companyId, name);

        public static CompanyTeam Load(EntityId id, int companyId, string name) => new(id, companyId, name);
    }
}
