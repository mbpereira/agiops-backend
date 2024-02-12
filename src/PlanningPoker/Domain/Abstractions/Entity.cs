namespace PlanningPoker.Domain.Abstractions
{
    public abstract class Entity<TEntity> 
        : Validatable<TEntity> where TEntity : Entity<TEntity>

    {
        public EntityId Id { get; init; }

        public Entity(EntityId id)
        {
            Id = id;
        }
    }
}
