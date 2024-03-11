#region

using FluentAssertions;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Plugins;
using PlanningPoker.UnitTests.Common.Extensions;

#endregion

namespace PlanningPoker.UnitTests.Domain.Plugins;

public class PluginTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("ab")]
    public void NewWithApiToken_InvalidData_ReturnsErrors(string invalidName)
    {
        var plugin = Plugin.NewWithApiToken(FakerInstance.ValidId(), invalidName,
            FakerInstance.PickRandom<PluginType>(), FakerInstance.Random.String2(20));

        plugin.Errors.Should().BeEquivalentTo([
            new
            {
                Code = "Plugin.Name",
                Message = "The provided string does not meet the minimum length requirement. Min length: 3."
            }
        ]);
    }

    [Fact]
    public void NewWithApiToken_ValidData_ReturnsIsValidTrue()
    {
        var plugin = Plugin.NewWithApiToken(EntityId.Generate(),
            FakerInstance.Random.String2(9), FakerInstance.PickRandom<PluginType>(),
            FakerInstance.Random.String2(100));

        plugin.IsValid.Should().BeTrue();
    }
}