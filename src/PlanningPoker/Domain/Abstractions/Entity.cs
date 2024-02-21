namespace PlanningPoker.Domain.Abstractions
{
    public abstract class Entity(int id) : Validatable
    {
        public EntityId Id { get; init; } = new(id);
    }
}
