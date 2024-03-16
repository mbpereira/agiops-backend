#region

using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Games.VotingSystems.ChangeVotingSystem;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;
using PlanningPoker.UnitTests.Common.Extensions;

#endregion

namespace PlanningPoker.UnitTests.Application.Games.VotingSystems.ChangeVotingSystem;

public class ChangeVotingSystemCommandHandlerTests
{
    private readonly ChangeVotingSystemCommandHandler _handler;
    private readonly IVotingSystemsRepository _votingSystems;

    public ChangeVotingSystemCommandHandlerTests()
    {
        _votingSystems = Substitute.For<IVotingSystemsRepository>();
        var uow = Substitute.For<IUnitOfWork>();
        uow.VotingSystems.Returns(_votingSystems);
        _handler = new ChangeVotingSystemCommandHandler(uow);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task HandleAsync_InvalidId_ReturnsInvalidIdError(string invalidId)
    {
        var payload = new ChangeVotingSystemCommandPayload(FakerInstance.Random.String2(10),
            FakerInstance.Make(3, index => index.ToString()), FakerInstance.Random.Words());
        var command = new ChangeVotingSystemCommand(invalidId, payload);

        var result = await _handler.HandleAsync(command);

        result.Errors.Should().BeEquivalentTo([
            new
            {
                Code = "ChangeVotingSystemCommand.Id",
                Message = "Provided value cannot be null, empty or white space."
            }
        ]);
    }

    [Fact]
    public async Task HandleAsync_InvalidData_ReturnsValidationFailed()
    {
        var existingVotingSystem = FakerInstance.NewValidVotingSystem();
        var payload = new ChangeVotingSystemCommandPayload("", [], "");
        var command = new ChangeVotingSystemCommand(FakerInstance.ValidId(), payload);
        _votingSystems.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(existingVotingSystem);

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_RecordNotExists_ReturnsRecordNotFound()
    {
        var payload = new ChangeVotingSystemCommandPayload();
        var command = new ChangeVotingSystemCommand(FakerInstance.ValidId(), payload);

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.RecordNotFound);
    }

    [Fact]
    public async Task HandleAsync_NoDataProvided_DoNotUpdate()
    {
        var existingVotingSystem = FakerInstance.NewValidVotingSystem();
        var emptyData = new ChangeVotingSystemCommandPayload();
        var emptyCommand = new ChangeVotingSystemCommand(FakerInstance.ValidId(), emptyData);
        _votingSystems.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(existingVotingSystem);

        var result = await _handler.HandleAsync(emptyCommand);

        using var _ = new AssertionScope();
        existingVotingSystem.UpdatedAtUtc.Should().BeNull();
        await _votingSystems.DidNotReceive().ChangeAsync(Arg.Any<VotingSystem>());
        AssertEquivalent(existingVotingSystem, result);
    }

    [Fact]
    public async Task HandleAsync_ValidData_UpdateOnlyProvidedProperties()
    {
        var existingVotingSystem = FakerInstance.NewValidVotingSystem();
        var oldName = existingVotingSystem.Name;
        var oldGradeDetails = existingVotingSystem.GradeDetails;
        var oldDescription = existingVotingSystem.Description;
        var oldUserId = existingVotingSystem.UserId;
        var oldUpdateDate = existingVotingSystem.UpdatedAtUtc.GetValueOrDefault();
        var payload = new ChangeVotingSystemCommandPayload(FakerInstance.Random.String2(100),
            Description: FakerInstance.Random.Words(20));
        var command = new ChangeVotingSystemCommand(existingVotingSystem.Id, payload);
        _votingSystems.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(existingVotingSystem);

        var result = await _handler.HandleAsync(command);

        using var _ = new AssertionScope();
        await _votingSystems.Received().ChangeAsync(Arg.Is<VotingSystem>(v =>
            (v.Name == payload.Name && v.Name != oldName) &&
            v.GradeDetails == oldGradeDetails &&
            (v.Description == payload.Description && v.Description != oldDescription) &&
            v.UserId == oldUserId &&
            v.UpdatedAtUtc > oldUpdateDate
        ));
        AssertEquivalent(existingVotingSystem, result);
    }
    
    private static void AssertEquivalent(VotingSystem expected, CommandResult<ChangeVotingSystemResult> actual)
    {
        actual.Payload.Should().BeEquivalentTo(new
        {
            Id = expected.Id.Value,
            expected.Name,
            expected.Description,
            PossibleGrades = expected.GradeDetails.Values,
            UpdatedAt = expected.UpdatedAtUtc
        });
    }
}