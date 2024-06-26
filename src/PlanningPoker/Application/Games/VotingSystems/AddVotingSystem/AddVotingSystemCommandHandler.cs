#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Security;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

#endregion

namespace PlanningPoker.Application.Games.VotingSystems.AddVotingSystem;

public class AddVotingSystemCommandHandler(ISecurityContext securityContext, IUnitOfWork uow)
    : ICommandHandler<AddVotingSystemCommand, AddVotingSystemResult>
{
    public async Task<CommandResult<AddVotingSystemResult>> HandleAsync(AddVotingSystemCommand command)
    {
        var securityInformation = await securityContext.GetSecurityInformationAsync();

        var votingSystem = VotingSystem.New(securityInformation.Tenant.Id, command.Name, securityInformation.User.Id,
            command.PossibleGrades, command.Description);

        if (!votingSystem.IsValid)
            return (votingSystem.Errors, CommandStatus.ValidationFailed);

        var created = await uow.VotingSystems.AddAsync(votingSystem);

        return new AddVotingSystemResult(created.Id);
    }
}