#region

using AutoBogus;
using FluentAssertions;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Games.VotingSystems.AddVotingSystem;
using PlanningPoker.Application.Security;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;
using PlanningPoker.UnitTests.Common.Extensions;

#endregion

namespace PlanningPoker.UnitTests.Application.Games.VotingSystems.AddVotingSystem;

public class AddVotingSystemCommandHandlerTests
{
    private readonly AddVotingSystemCommandHandler _handler;
    private readonly IVotingSystemsRepository _votingSystems;

    public AddVotingSystemCommandHandlerTests()
    {
        _votingSystems = Substitute.For<IVotingSystemsRepository>();
        var uow = Substitute.For<IUnitOfWork>();
        uow.VotingSystems.Returns(_votingSystems);
        var securityContext = Substitute.For<ISecurityContext>();
        securityContext.GetSecurityInformationAsync()
            .Returns(new AutoFaker<SecurityInformation>().Generate());
        _handler = new AddVotingSystemCommandHandler(securityContext, uow);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData(null)]
    public async Task HandleAsync_InvalidData_ReturnsValidationFailed(string invalidName)
    {
        var command = new AddVotingSystemCommand(invalidName, []);

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_ValidData_ReturnsGeneratedId()
    {
        var expectedVotingSystem = FakerInstance.NewValidVotingSystem();
        var command = new AddVotingSystemCommand(expectedVotingSystem.Name,
            expectedVotingSystem.GradeDetails.Values.ToList(), expectedVotingSystem.Description);
        _votingSystems.AddAsync(Arg.Any<VotingSystem>())
            .Returns(expectedVotingSystem);

        var result = await _handler.HandleAsync(command);

        result.Payload!.Id.Should().Be(expectedVotingSystem.Id.Value);
    }
}