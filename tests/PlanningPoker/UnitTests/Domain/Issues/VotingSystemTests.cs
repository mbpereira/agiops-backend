using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
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
            var votingSystem = VotingSystem.New(tenantId: 0, invalidDescription, userId: 0);
            var expectedErrors = new[]
            {
                new { Code = "VotingSystem.Description" },
                new { Code = "VotingSystem.UserId" },
                new { Code = "VotingSystem.Grades" }
            };

            var validationResult = votingSystem.Validate();

            using var _ = new AssertionScope();
            validationResult.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public void ShouldRegisterGrade()
        {
            VotingSystem votingSystem = GetValidVotingSystem();

            votingSystem.AddGrade(_faker.Random.Int());

            votingSystem.Grades.Count.Should().Be(1);
        }

        private VotingSystem GetValidVotingSystem()
            => VotingSystem.New(tenantId: _faker.Random.Int(min: 1), description: _faker.Random.String2(length: 10), userId: _faker.Random.Int());

        [Fact]
        public void ShouldClearGrades()
        {
            var votingSystem = GetValidVotingSystem();
            votingSystem.AddGrade(_faker.Random.Int());

            votingSystem.ClearGrades();

            votingSystem.Grades.Should().BeEmpty();
        }

        [Fact]
        public void ShouldChangeDescription()
        {
            var newDescription = _faker.Random.String2(length: 10);
            var votingSystem = GetValidVotingSystem();

            votingSystem.ChangeDescription(newDescription);

            votingSystem.Description.Should().Be(newDescription);
        }
    }
}
