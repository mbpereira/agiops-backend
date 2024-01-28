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
            var votingSystem = VotingSystem.New(invalidDescription, userId: 0, shared: false);
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
            var votingSystem = VotingSystem.New(description: _faker.Random.String2(length: 10), userId: _faker.Random.Int(), shared: false);

            votingSystem.AddGrade(_faker.Random.Int());

            votingSystem.Grades.Count.Should().Be(1);
        }

        [Fact]
        public void ShouldClearGrades()
        {
            var votingSystem = VotingSystem.New(description: _faker.Random.String2(length: 10), userId: _faker.Random.Int(), shared: false);
            votingSystem.AddGrade(_faker.Random.Int());

            votingSystem.ClearGrades();

            votingSystem.Grades.Should().BeEmpty();
        }

        [Fact]
        public void ShouldChangeDescription()
        {
            var newDescription = _faker.Random.String2(length: 10);
            var votingSystem = VotingSystem.New(description: _faker.Random.String2(length: 10), userId: _faker.Random.Int(), shared: false);

            votingSystem.ChangeDescription(newDescription);

            votingSystem.Description.Should().Be(newDescription);
        }

        [Fact]
        public void ShouldSetRevised()
        {
            var votingSystem = VotingSystem.New(description: _faker.Random.String2(length: 10), userId: _faker.Random.Int(), shared: false);

            votingSystem.Revise(revised: true);

            votingSystem.Revised.Should().BeTrue();
        }

        [Fact]
        public void ShouldSetShared()
        {
            var votingSystem = VotingSystem.New(description: _faker.Random.String2(length: 10), userId: _faker.Random.Int(), shared: false);

            votingSystem.Share(shared: true);

            votingSystem.Shared.Should().BeTrue();
        }
    }
}
