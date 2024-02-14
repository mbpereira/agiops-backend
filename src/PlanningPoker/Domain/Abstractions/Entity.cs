namespace PlanningPoker.Domain.Abstractions
{
    public abstract class Entity : Validatable
    {
        public EntityId Id { get; init; }

        public Entity(EntityId id)
        {
            Id = id;
        }
    }
}
