using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Games.ChangeVotingSystem;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

namespace PlanningPoker.UnitTests.Application.Games.ChangeVotingSystem;

public class ChangeVotingSystemCommandHandlerTests
{
    private readonly IVotingSystemsRepository _votingSystems;
    private readonly ChangeVotingSystemCommandHandler _handler;

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
    public async Task HandleAsync_InvalidId_ReturnsInvalidIdError(string invalidId)
    {
        var data = new ChangeVotingSystemData(FakerInstance.Random.String2(10),
            FakerInstance.Make(3, index => index.ToString()), FakerInstance.Random.Words());
        var command = new ChangeVotingSystemCommand(invalidId, data);

        var result = await _handler.HandleAsync(command);

        result.Details.Should().BeEquivalentTo([
            new
            {
                Code = "UpdateVotingSystemCommand.Id",
                Message = "Provided value cannot be null, empty or white space."
            }
        ]);
    }

    [Fact]
    public async Task HandleAsync_InvalidData_ReturnsValidationFailed()
    {
        var existingVotingSystem = FakerInstance.NewValidVotingSystem();
        var data = new ChangeVotingSystemData("", [], "");
        var command = new ChangeVotingSystemCommand(FakerInstance.ValidId(), data);
        _votingSystems.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(existingVotingSystem);

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_RecordNotExists_ReturnsRecordNotFound()
    {
        var data = new ChangeVotingSystemData();
        var command = new ChangeVotingSystemCommand(FakerInstance.ValidId(), data);

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.RecordNotFound);
    }

    [Fact]
    public async Task HandleAsync_NoDataProvided_DoNotUpdate()
    {
        var existingVotingSystem = FakerInstance.NewValidVotingSystem();
        var emptyData = new ChangeVotingSystemData();
        var emptyCommand = new ChangeVotingSystemCommand(FakerInstance.ValidId(), emptyData);
        _votingSystems.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(existingVotingSystem);

        var result = await _handler.HandleAsync(emptyCommand);

        using var _ = new AssertionScope();
        existingVotingSystem.UpdatedAtUtc.Should().BeNull();
        await _votingSystems.DidNotReceive().ChangeAsync(Arg.Any<VotingSystem>());
        AssertEquivalent(result, existingVotingSystem);
    }

    private static void AssertEquivalent(CommandResult<ChangeVotingSystemResult> result,
        VotingSystem existingVotingSystem)
    {
        result.Data.Should().BeEquivalentTo(new
        {
            Id = existingVotingSystem.Id.Value,
            Name = existingVotingSystem.Name,
            Description = existingVotingSystem.Description,
            PossibleGrades = existingVotingSystem.GradeDetails.Values,
            UpdatedAt = existingVotingSystem.UpdatedAtUtc
        });
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
        var data = new ChangeVotingSystemData(Name: FakerInstance.Random.String2(100));
        var command = new ChangeVotingSystemCommand(existingVotingSystem.Id, data);
        _votingSystems.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(existingVotingSystem);

        var result = await _handler.HandleAsync(command);

        using var _ = new AssertionScope();
        await _votingSystems.Received().ChangeAsync(Arg.Is<VotingSystem>(v =>
            v.Name == data.Name &&
            v.Name != oldName &&
            v.GradeDetails == oldGradeDetails &&
            v.Description == oldDescription &&
            v.UserId == oldUserId &&
            v.UpdatedAtUtc > oldUpdateDate
        ));
        AssertEquivalent(result, existingVotingSystem);
    }
}