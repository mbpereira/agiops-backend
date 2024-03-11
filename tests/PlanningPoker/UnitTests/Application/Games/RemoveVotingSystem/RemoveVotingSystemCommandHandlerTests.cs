using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Games.RemoveVotingSystem;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

namespace PlanningPoker.UnitTests.Application.Games.RemoveVotingSystem;

public class RemoveVotingSystemCommandHandlerTests
{
    private readonly IVotingSystemsRepository _votingSystems;
    private readonly RemoveVotingSystemCommandHandler _handler;

    public RemoveVotingSystemCommandHandlerTests()
    {
        _votingSystems = Substitute.For<IVotingSystemsRepository>();
        var uow = Substitute.For<IUnitOfWork>();
        uow.VotingSystems.Returns(_votingSystems);
        _handler = new RemoveVotingSystemCommandHandler(uow);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public async Task HandleAsync_InvalidId_ReturnsInvalidIdError(string invalidId)
    {
        var command = new RemoveVotingSystemCommand(invalidId);

        var result = await _handler.HandleAsync(command);

        result.Details.Should().BeEquivalentTo([
            new
            {
                Code = "RemoveVotingSystemCommand.Id",
                Message = "Provided value cannot be null, empty or white space."
            }
        ]);
    }

    [Fact]
    public async Task HandleAsync_RecordNotExists_ReturnsRecordNotFound()
    {
        var command = new RemoveVotingSystemCommand(FakerInstance.ValidId());

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.RecordNotFound);
    }

    [Fact]
    public async Task HandleAsync_RecordExists_RemoveVotingSystemAndReturnsSuccess()
    {
        var command = new RemoveVotingSystemCommand(FakerInstance.ValidId());
        _votingSystems.GetByIdAsync(Arg.Any<EntityId>()).Returns(FakerInstance.NewValidVotingSystem());
        var result = await _handler.HandleAsync(command);

        using var _ = new AssertionScope();
        await _votingSystems.Received().RemoveAsync(Arg.Any<VotingSystem>());
        result.Status.Should().Be(CommandStatus.Success);
    }
}