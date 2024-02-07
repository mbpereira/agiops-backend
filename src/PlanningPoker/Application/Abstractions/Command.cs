using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Application.Abstractions
{
    public abstract class Command<TCommand> : Validatable<TCommand>
        where TCommand : Command<TCommand>
    {
    }
}
