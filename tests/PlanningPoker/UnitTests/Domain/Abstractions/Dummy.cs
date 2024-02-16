using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.UnitTests.Domain.Abstractions
{
    public class Dummy : TenantableAggregateRoot
    {
        public Dummy(int id, int tenantId) : base(id, tenantId)
        {
        }
    }
}
