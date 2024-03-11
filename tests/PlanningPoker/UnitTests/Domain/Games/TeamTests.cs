#region

using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Games;
using PlanningPoker.Domain.Validation;
using PlanningPoker.UnitTests.Common.Extensions;

#endregion

namespace PlanningPoker.UnitTests.Domain.Games;

public class TeamTests
{
    [Fact]
    public void New_ValidaData_ReturnsIsValidTrue()
    {
        var team = Team.New(FakerInstance.ValidId(), FakerInstance.Random.String2(10));

        team.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("12")]
    public void New_InvalidData_ReturnsInvalidPropertyErrors(string invalidName)
    {
        var expectedErrors = new[]
        {
            new Error("TenantId", "Provided value cannot be null, empty or white space."),
            new Error("Team.Name", "The provided string does not meet the minimum length requirement. Min length: 3.")
        };

        var team = Team.New(FakerInstance.InvalidId(), invalidName);

        using var _ = new AssertionScope();
        team.IsValid.Should().BeFalse();
        team.Errors.Should().BeEquivalentTo(expectedErrors);
    }
}