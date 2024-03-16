namespace PlanningPoker.Application.Abstractions.Commands;

public interface ICommandHandler<in TCommand> where TCommand : Command
{
    Task<CommandResult> HandleAsync(TCommand command);
}

public interface ICommandHandler<in TCommand, TCommandResult> where TCommandResult : class
{
    Task<CommandResult<TCommandResult>> HandleAsync(TCommand command);
}