namespace PlanningPoker.Domain.Abstractions
{
    public abstract class Entity : Validatable
    {
        public EntityId Id { get; init; }

        public Entity(int id)
        {
            Id = new EntityId(id);
        }
    }
}
