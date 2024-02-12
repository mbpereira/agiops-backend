using FluentValidation;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Users.RenewInvite
{
    public class RenewInviteCommand : Command<RenewInviteCommand>
    {
        public int Id { get; private set; }

        public RenewInviteCommand(int id)
        {
            Id = id;
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<RenewInviteCommand> validator)
        {
            validator.CreateRuleFor(c => c.Id)
                .GreaterThan(0);
        }
    }
}
