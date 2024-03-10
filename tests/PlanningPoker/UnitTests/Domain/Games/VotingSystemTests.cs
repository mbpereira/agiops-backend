#region

using Bogus;
using FluentAssertions;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

#endregion

namespace PlanningPoker.UnitTests.Domain.Games;

public class VotingSystemTests
{
    private readonly Faker _faker = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("te")]
    public void New_InvalidData_ReturnsValidationErrors(string invalidDescription)
    {
        var expectedErrors = new[]
        {
            new { Code = "TenantId", Message = "Provided value cannot be null, empty or white space." },
            new
            {
                Code = "VotingSystem.Name",
                Message = "The provided string does not meet the minimum length requirement. Min length: 3."
            },
            new { Code = "VotingSystem.Grades", Message = "The list cannot be empty." },
            new { Code = "VotingSystem.UserId", Message = "Provided value cannot be null, empty or white space." }
        };

        var votingSystem = VotingSystem.New(EntityId.Empty, invalidDescription, EntityId.Empty,
            []);

        votingSystem.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void SetName_ValidName_ChangeName()
    {
        var newDescription = _faker.Random.String2(10);
        var votingSystem = GetValidVotingSystem();

        votingSystem.SetName(newDescription);

        votingSystem.Name.Should().Be(newDescription);
    }

    [Fact]
    public void GradeDetails_NonNumericGrades_ReturnsIsQuantifiableFalse()
    {
        var votingSystem = GetValidVotingSystem();
        votingSystem.SetPossibleGrades(["P", "M", "G"]);

        var gradeDetails = votingSystem.GradeDetails;

        gradeDetails.IsQuantifiable.Should().BeFalse();
    }

    [Fact]
    public void GradeDetails_NumericGrades_ReturnsIsQuantifiableTrue()
    {
        var votingSystem = GetValidVotingSystem();
        votingSystem.SetPossibleGrades(["1", "2", "3"]);

        var gradeDetails = votingSystem.GradeDetails;

        gradeDetails.IsQuantifiable.Should().BeTrue();
    }

    private VotingSystem GetValidVotingSystem()
    {
        return _faker.NewValidVotingSystem();
    }
}