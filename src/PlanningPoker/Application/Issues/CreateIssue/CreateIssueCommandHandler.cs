using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;

namespace PlanningPoker.Application.Issues.CreateIssue
{
    public class CreateIssueCommandHandler : ICommandHandler<CreateIssueCommand, CreateIssueResult>
    {
        private readonly IUnitOfWork _uow;

        public CreateIssueCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CommandResult<CreateIssueResult>> HandleAsync(CreateIssueCommand command)
        {
            var issue = Issue.New(command.GameId, command.Name, command.Description, command.Link);

            var validationResult = issue.Validate();

            if (!validationResult.Success)
                return CommandResult<CreateIssueResult>.Fail(validationResult.Errors, CommandStatus.ValidationFailed);

            var createdIssue = await _uow.Issues.AddAsync(issue);

            await _uow.SaveChangesAsync();

            return CommandResult<CreateIssueResult>.Success(new CreateIssueResult(createdIssue.Id.Value));
        }
    }
}