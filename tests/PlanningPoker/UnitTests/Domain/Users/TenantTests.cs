using Bogus;
using FluentAssertions;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.UnitTests.Domain.Users
{
    public class TenantTests
    {
        private readonly Faker _faker;

        public TenantTests()
        {
            _faker = new();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ab")]
        public void ShouldReturnExpectedErrorsWhenProvidedDataIsNotValid(string invalidName)
        {
            var expectedErrors = new[]
            {
                new { Code = "Tenant.name", Message = "The provided string does not meet the minimum length requirement. Min length: 3." }
            };

            var tenant = Tenant.New(name: invalidName);

            tenant.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public void ShouldReturnSuccessWhenProvidedDataIsValid()
        {
            var tenant = Tenant.New(_faker.Random.String2(length: 3));

            tenant.IsValid.Should().BeTrue();
        }
    }
}
