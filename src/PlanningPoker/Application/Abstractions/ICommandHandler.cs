namespace PlanningPoker.Application.Abstractions
{
    public interface ICommandHandler<TCommand>
    {
        public abstract Task<CommandResult> HandleAsync(TCommand request);
    }

    public interface ICommandHandler<TCommand, TCommandResult> where TCommandResult : class
    {
        public abstract Task<CommandResult<TCommandResult>> HandleAsync(TCommand command);
    }
}
