#region

using AutoBogus;
using FluentAssertions;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Abstractions.Security;
using PlanningPoker.Application.Games.CreateVotingSystem;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

#endregion

namespace PlanningPoker.UnitTests.Application.Games.CreateVotingSystem;

public class CreateVotingSystemCommandHandlerTests
{
    private readonly CreateVotingSystemCommandHandler _handler;
    private readonly IVotingSystemsRepository _votingSystems;

    public CreateVotingSystemCommandHandlerTests()
    {
        _votingSystems = Substitute.For<IVotingSystemsRepository>();
        var uow = Substitute.For<IUnitOfWork>();
        uow.VotingSystems.Returns(_votingSystems);
        var securityContext = Substitute.For<ISecurityContext>();
        securityContext.GetSecurityInformationAsync()
            .Returns(new AutoFaker<SecurityInformation>().Generate());
        _handler = new CreateVotingSystemCommandHandler(securityContext, uow);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData(null)]
    public async Task HandleAsync_InvalidData_ReturnsValidationFailed(string invalidName)
    {
        var command = new CreateVotingSystemCommand(invalidName, []);

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_ValidData_ReturnsGeneratedId()
    {
        var expectedVotingSystem = VotingSystem.New(FakerInstance.ValidId(), FakerInstance.Random.String2(10),
            FakerInstance.ValidId(), ["P", "M", "G"], FakerInstance.Random.Words());
        var command = new CreateVotingSystemCommand(expectedVotingSystem.Name,
            expectedVotingSystem.GradeDetails.Values.ToList(), expectedVotingSystem.Description);
        _votingSystems.AddAsync(Arg.Any<VotingSystem>())
            .Returns(expectedVotingSystem);

        var result = await _handler.HandleAsync(command);

        result.Data!.Id.Should().Be(expectedVotingSystem.Id.Value);
    }
}