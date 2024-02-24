namespace PlanningPoker.Application.Abstractions.Commands;

public interface ICommandHandler<in TCommand>
{
    Task<CommandResult> HandleAsync(TCommand request);
}

public interface ICommandHandler<in TCommand, TCommandResult> where TCommandResult : class
{
    Task<CommandResult<TCommandResult>> HandleAsync(TCommand command);
}