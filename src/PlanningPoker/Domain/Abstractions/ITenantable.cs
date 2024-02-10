namespace PlanningPoker.Domain.Abstractions
{
    public interface ITenantable
    {
        public EntityId TenantId { get; }
    }
}
