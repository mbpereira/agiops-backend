using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Application.Users.RenewInvite
{
    public class RenewInviteCommandHandler : ICommandHandler<RenewInviteCommand>
    {
        private readonly IUnitOfWork _uow;

        public RenewInviteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CommandResult> HandleAsync(RenewInviteCommand command)
        {
            var validationResult = command.Validate();

            if (!validationResult.Success)
                return CommandResult.Fail(validationResult.Errors, CommandStatus.ValidationFailed);

            var invite = await _uow.Invites.GetByIdAsync(command.Id);

            if (invite is null)
                return CommandResult.Fail(CommandStatus.RecordNotFound);

            invite.Renew();

            await _uow.Invites.ChangeAsync(invite);
            await _uow.SaveChangesAsync();

            return CommandResult.Success();
        }
    }
}
