#region

using Bogus;
using FluentAssertions;

#endregion

namespace PlanningPoker.UnitTests.Domain.Abstractions;

public class TenantableAggregateRootTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void SetTenant_AlreadySetTenant_ReturnsError()
    {
        var dummy = new Dummy(_faker.ValidId(), _faker.ValidId());

        dummy.SetTenant(_faker.ValidId());

        dummy.Errors.Should().BeEquivalentTo([
            new { Code = "TenantId", Message = "Tenant id cannot be changed." }
        ]);
    }
}