using Bogus;
using FluentAssertions;
using PlanningPoker.Domain.Issues;

namespace PlanningPoker.UnitTests.Domain.Issues
{
    public class VotingSystemTests
    {
        private readonly Faker _faker;

        public VotingSystemTests()
        {
            _faker = new();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("te")]
        public void ShouldReturnExpectedErrrors(string invalidDescription)
        {
            var expectedErrors = new[]
            {
                new { Code = "tenantId", Message = "Provided value must be greater than 0." },
                new { Code = "VotingSystem.description", Message = "The provided string does not meet the minimum length requirement. Min length: 3." },
                new { Code = "VotingSystem.grades", Message = "The list cannot be empty." },
                new { Code = "VotingSystem.userId", Message = "Provided value must be greater than 0." },
            };

            var votingSystem = VotingSystem.New(tenantId: 0, invalidDescription, userId: 0, new List<int>());

            votingSystem.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public void ShouldChangeDescription()
        {
            var newDescription = _faker.Random.String2(length: 10);
            var votingSystem = GetValidVotingSystem();

            votingSystem.Describe(newDescription);

            votingSystem.Description.Should().Be(newDescription);
        }

        private VotingSystem GetValidVotingSystem()
            => VotingSystem.New(
                tenantId: _faker.Random.Int(min: 1),
                description: _faker.Random.String2(length: 10),
                userId: _faker.Random.Int(),
                grades: _faker.Make(3, () => _faker.Random.Int()));
    }
}
