using AutoBogus;
using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.UnitTests.Helpers
{
    public static class EntityIdHelpers
    {
        public static EntityId RandomId() => new AutoFaker<EntityId>().Generate();
    }
}
