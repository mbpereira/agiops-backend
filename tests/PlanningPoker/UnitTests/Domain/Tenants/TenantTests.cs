#region

using Bogus;
using FluentAssertions;
using PlanningPoker.Domain.Tenants;

#endregion

namespace PlanningPoker.UnitTests.Domain.Tenants;

public class TenantTests
{
    private readonly Faker _faker = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ab")]
    public void New_ShouldReturnExpectedErrorsWhenProvidedDataIsNotValid(string invalidName)
    {
        var expectedErrors = new[]
        {
            new
            {
                Code = "Tenant.Name",
                Message = "The provided string does not meet the minimum length requirement. Min length: 3."
            }
        };

        var tenant = Tenant.New(invalidName);

        tenant.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void New_ShouldReturnSuccessWhenProvidedDataIsValid()
    {
        var tenant = Tenant.New(_faker.Random.String2(3));

        tenant.IsValid.Should().BeTrue();
    }
}