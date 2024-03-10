#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Abstractions.Security;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

#endregion

namespace PlanningPoker.Application.Games.CreateVotingSystem;

public class CreateVotingSystemCommandHandler(ISecurityContext securityContext, IUnitOfWork uow)
    : ICommandHandler<CreateVotingSystemCommand, CreateVotingSystemResult>
{
    public async Task<CommandResult<CreateVotingSystemResult>> HandleAsync(CreateVotingSystemCommand command)
    {
        var securityInformation = await securityContext.GetSecurityInformationAsync();

        var votingSystem = VotingSystem.New(securityInformation.Tenant.Id, command.Name, securityInformation.User.Id,
            command.PossibleGrades, command.Description);

        if (!votingSystem.IsValid)
            return CommandResult<CreateVotingSystemResult>.Fail(votingSystem.Errors, CommandStatus.ValidationFailed);

        var created = await uow.VotingSystems.AddAsync(votingSystem);

        return CommandResult<CreateVotingSystemResult>.Success(new CreateVotingSystemResult(created.Id));
    }
}