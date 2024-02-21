using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;
using PlanningPoker.UnitTests.Domain.Common.Extensions;

namespace PlanningPoker.UnitTests.Domain.Issues
{
    public class IssueTests
    {
        private readonly Faker _faker = new();

        [Fact]
        public void New_ShouldReturnAutoIncrementAsIdWhenNewIssueIsCreated()
        {
            var issue = GetValidIssue();

            issue.Id.Should().Be(EntityId.AutoIncrement());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void New_ShouldReturnExpectedErrorsWhenProvidedDataIsNotValid(string name)
        {
            var expectedErrros = new[]
            {
                new
                {
                    Code = "Issue.Name",
                    Message = "The provided string does not meet the minimum length requirement. Min length: 3."
                },
                new
                {
                    Code = "TenantId",
                    Message = "Provided value must be greater than 0."
                },
                new
                {
                    Code = "Issue.GameId",
                    Message = "Provided value must be greater than 0."
                }
            };

            var issue = Issue.New(tenantId: 0, gameId: 0, name);

            using var _ = new AssertionScope();
            issue.IsValid.Should().BeFalse();
            issue.Errors.Should().BeEquivalentTo(expectedErrros);
        }

        [Fact]
        public void SetGame_ShouldReturnErrorWhenTryingChangeIssueGame()
        {
            var issue = GetValidIssue();

            issue.SetGame(_faker.ValidId());

            issue.Errors.Should().BeEquivalentTo(new[]
            {
                new
                {
                    Code = "Issue.GameId",
                    Message = "You cannot change the issue game, as it has already been set."
                }
            });
        }

        [Fact]
        public void RegisterGrade_ShouldReturnsErrorWhenProvidedUserIdIsNotValid()
        {
            var issue = GetValidIssue();

            issue.RegisterGrade(userId: 0, grade: "1");

            using var _ = new AssertionScope();
            issue.IsValid.Should().BeFalse();
            issue.Errors.Should().BeEquivalentTo(new[]
            {
                new
                {
                    Code = "Issue.UserId",
                    Message = "Provided user id is not valid."
                }
            });
        }

        [Fact]
        public void RegisterGrade_ShouldRemoveExistingUserIdBeforeRegisterGradeToPreventDuplication()
        {
            var issue = GetValidIssue();
            issue.RegisterGrade(userId: 1, grade: "1");
            issue.RegisterGrade(userId: 1, grade: "1");

            var gradesCount = issue.UserGrades.Count;

            gradesCount.Should().Be(1);
        }

        private Issue GetValidIssue()
            => Issue.New(
                tenantId: _faker.Random.Int(min: 1),
                gameId: _faker.Random.Int(min: 1),
                name: _faker.Random.String2(length: 10),
                description: _faker.Random.String2(length: 10),
                link: _faker.Internet.Url());
    }
}