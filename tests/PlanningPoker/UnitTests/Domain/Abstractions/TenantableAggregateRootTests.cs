using Bogus;
using FluentAssertions;
using PlanningPoker.UnitTests.Domain.Common.Extensions;

namespace PlanningPoker.UnitTests.Domain.Abstractions
{
    public class TenantableAggregateRootTests
    {
        private readonly Faker _faker;

        public TenantableAggregateRootTests()
        {
            _faker = new();
        }

        [Fact]
        public void ShouldReturnErrorWhenTryingChangeTenantIdFromEntity()
        {
            var dummy = new Dummy(_faker.ValidId(), _faker.ValidId());

            dummy.SetTenant(_faker.ValidId());

            dummy.Errors.Should().BeEquivalentTo(new[]
            {
                new { Code = "TenantId", Message = "Tenant id cannot be changed." }
            });
        }
    }
}