using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Users;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Users.SendInvitation
{
    public class SendInvitationCommand : Command<SendInvitationCommand>
    {
        public string To { get; private set; }
        public Role Role { get; private set; }

        public SendInvitationCommand(string to, Role role)
        {
            To = to;
            Role = role;
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<SendInvitationCommand> validator)
        {
        }
    }
}