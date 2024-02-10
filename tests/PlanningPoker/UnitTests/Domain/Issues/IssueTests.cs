using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;

namespace PlanningPoker.UnitTests.Domain.Issues
{
    public class IssueTests
    {
        private readonly Faker _faker;

        public IssueTests()
        {
            _faker = new();
        }

        [Fact]
        public void ShouldReturnAutoIncrementAsIdWhenNewIssueIsCreated()
        {
            var issue = GetValidIssue();

            issue.Id.Should().Be(EntityId.AutoIncrement());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldReturnExpectedErrorsWhenProvidedDataIsNotValid(string name)
        {
            var issue = Issue.New(tenantId: 0, gameId: 0, name);
            var expectedErrros = new[]
            {
                new { Code = "Issue.GameId" },
                new { Code = "Issue.Name" },
            };

            var validationResult = issue.Validate();

            using var _ = new AssertionScope();
            validationResult.Success.Should().BeFalse();
            validationResult.Errors.Should().BeEquivalentTo(expectedErrros);
        }

        [Fact]
        public void ShoulCaculateAverage()
        {
            var issue = GetValidIssue();
            issue.RegisterGrade(userId: 1, grade: 1);
            issue.RegisterGrade(userId: 2, grade: 1);
            issue.RegisterGrade(userId: 3, grade: 1);

            var avg = issue.Average;

            avg.Should().Be(1);
        }

        [Fact]
        public void ShouldThrowExceptionWhenProvidedUserIdIsNotValid()
        {
            var issue = GetValidIssue();
            Action act = () => issue.RegisterGrade(userId: 0, 1);

            var ex = act.Should().Throw<DomainException>();

            ex.Which.Message.Should().Be("Provided user id is not valid.");
        }

        [Fact]
        public void ShouldRemoveExistingUserIdBeforeRegisterGradeToPreventDuplication()
        {
            var issue = GetValidIssue();
            issue.RegisterGrade(userId: 1, grade: 1);
            issue.RegisterGrade(userId: 1, grade: 1);

            var gradesCount = issue.Grades.Count;

            gradesCount.Should().Be(1);
        }

        private Issue GetValidIssue()
            => Issue.New(
                tenantId: _faker.Random.Int(min: 1),
                gameId: _faker.Random.Int(),
                name: _faker.Random.String2(length: 10),
                description: _faker.Random.String2(length: 10),
                link: _faker.Internet.Url());
    }
}
