#region

using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

#endregion

namespace PlanningPoker.UnitTests.Domain.Games;

public class IssueTests
{
    private readonly Faker _faker = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void New_InvalidData_ReturnsErrorsWithPropertyDetails(string name)
    {
        var expectedErrors = new[]
        {
            new
            {
                Code = "Issue.Name",
                Message = "The provided string does not meet the minimum length requirement. Min length: 3."
            },
            new
            {
                Code = "TenantId",
                Message = "Provided value cannot be null, empty or white space."
            },
            new
            {
                Code = "Issue.GameId",
                Message = "Provided value cannot be null, empty or white space."
            }
        };

        var issue = Issue.New(EntityId.Empty, EntityId.Empty, name);

        using var _ = new AssertionScope();
        issue.IsValid.Should().BeFalse();
        issue.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void SetGame_AttemptToChangeIssueGame_ReturnsError()
    {
        var issue = GetValidIssue();

        issue.SetGame(_faker.ValidId());

        issue.Errors.Should().BeEquivalentTo([
            new
            {
                Code = "Issue.GameId",
                Message = "You cannot change the issue game, as it has already been set."
            }
        ]);
    }

    [Fact]
    public void RegisterGrade_InvalidUserId_ReturnsError()
    {
        var issue = GetValidIssue();

        issue.RegisterGrade(EntityId.Empty, "1");

        using var _ = new AssertionScope();
        issue.IsValid.Should().BeFalse();
        issue.Errors.Should().BeEquivalentTo([
            new
            {
                Code = "Issue.UserId",
                Message = "Provided value cannot be null, empty or white space."
            }
        ]);
    }

    [Fact]
    public void RegisterGrade_UserIdDuplication_RemoveExistingUserIdBeforeRegisteringNewUserGrade()
    {
        var issue = GetValidIssue();
        var userId = EntityId.Generate();
        issue.RegisterGrade(userId, "1");
        issue.RegisterGrade(userId, "1");

        var gradesCount = issue.UserGrades.Count;

        gradesCount.Should().Be(1);
    }

    private Issue GetValidIssue()
    {
        return Issue.New(
            FakerInstance.ValidId(),
            FakerInstance.ValidId(),
            FakerInstance.Random.String2(10),
            FakerInstance.Random.String2(10),
            FakerInstance.Internet.Url());
    }
}