namespace PlanningPoker.Application.Abstractions
{
    public interface ICommandHandler<TCommand>
    {
        Task<CommandResult> HandleAsync(TCommand request);
    }

    public interface ICommandHandler<TCommand, TCommandResult> where TCommandResult : class
    {
        Task<CommandResult<TCommandResult>> HandleAsync(TCommand command);
    }
}
