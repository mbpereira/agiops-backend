using FluentAssertions;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Plugins;

namespace PlanningPoker.UnitTests.ProjectManagementProviders;

public class PluginTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("ab")]
    public void NewWithApiToken_InvalidData_ReturnsErrors(string invalidName)
    {
        var pmProvider = Plugin.NewWithApiToken(FakerInstance.ValidId(), invalidName,
            FakerInstance.Random.String2(length: 20));

        pmProvider.Errors.Should().BeEquivalentTo([
            new
            {
                Code = "ProjectManagementProvider.Name",
                Message = $"The provided string does not meet the minimum length requirement. Min length: 3."
            }
        ]);
    }

    [Fact]
    public void NewWithApiToken_ValidData_ReturnsIsValidTrue()
    {
        var pmProvider = Plugin.NewWithApiToken(EntityId.Generate(),
            FakerInstance.Random.String2(length: 9), FakerInstance.Random.String2(length: 100));

        pmProvider.IsValid.Should().BeTrue();
    }
}