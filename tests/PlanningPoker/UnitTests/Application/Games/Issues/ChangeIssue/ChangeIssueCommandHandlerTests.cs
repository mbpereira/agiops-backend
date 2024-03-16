using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Games.Issues.ChangeIssue;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;
using PlanningPoker.UnitTests.Common.Extensions;

namespace PlanningPoker.UnitTests.Application.Games.Issues.ChangeIssue;

public class ChangeIssueCommandHandlerTests
{
    private readonly IIssuesRepository _issues;
    private readonly ChangeIssueCommandHandler _handler;

    public ChangeIssueCommandHandlerTests()
    {
        _issues = Substitute.For<IIssuesRepository>();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Issues.Returns(_issues);
        _handler = new ChangeIssueCommandHandler(uow);
    }

    [Fact]
    public async Task HandleAsync_InvalidId_ReturnsInvalidIdError()
    {
        var command = new ChangeIssueCommand(FakerInstance.InvalidId(), new ChangeIssueCommandPayload());

        var result = await _handler.HandleAsync(command);

        result.Errors.Should().BeEquivalentTo([
            new
            {
                Code = "ChangeIssueCommand.Id",
                Message = "Provided value cannot be null, empty or white space."
            }
        ]);
    }
    
    [Fact]
    public async Task HandleAsync_IssueNotExists_ReturnsRecordNotFound()
    {
        var command = new ChangeIssueCommand(FakerInstance.ValidId(), new ChangeIssueCommandPayload());
        
        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.RecordNotFound);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ab")]
    public async Task HandleAsync_InvalidData_ReturnsValidationFailed(string invalidName)
    {
        var expectedIssue = FakerInstance.NewValidIssue();
        var command = new ChangeIssueCommand(expectedIssue.Id, new ChangeIssueCommandPayload(Name: invalidName));
        _issues.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(expectedIssue);
        
        var result = await _handler.HandleAsync(command);
    
        result.Status.Should().Be(CommandStatus.ValidationFailed);
    }


    [Fact]
    public async Task HandleAsync_NoDataProvided_DoNotUpdate()
    {
        var existingIssue = FakerInstance.NewValidIssue();
        var emptyPayload = new ChangeIssueCommandPayload();
        var emptyCommand = new ChangeIssueCommand(FakerInstance.ValidId(), emptyPayload);
        _issues.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(existingIssue);

        var result = await _handler.HandleAsync(emptyCommand);

        using var _ = new AssertionScope();
        existingIssue.UpdatedAtUtc.Should().BeNull();
        await _issues.DidNotReceive().ChangeAsync(Arg.Any<Issue>());
        AssertEquivalent(existingIssue, result);
    }

    [Fact]
    public async Task HandleAsync_ValidData_UpdateOnlyProvidedProperties()
    {
        var oldIssue = FakerInstance.NewValidIssue();
        var oldName = oldIssue.Name;
        var oldDescription = oldIssue.Description;
        var oldUpdateDate = oldIssue.UpdatedAtUtc.GetValueOrDefault();
        var payload = new ChangeIssueCommandPayload(FakerInstance.Random.String2(100),
            Description: FakerInstance.Random.Words(20));
        var command = new ChangeIssueCommand(oldIssue.Id, payload);
        _issues.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(oldIssue);

        var result = await _handler.HandleAsync(command);

        using var _ = new AssertionScope();
        await _issues.Received().ChangeAsync(Arg.Is<Issue>(i =>
            (i.Name == payload.Name && i.Name != oldName) &&
            (i.Description == payload.Description && i.Description != oldDescription) &&
            i.UpdatedAtUtc > oldUpdateDate
        ));
        AssertEquivalent(oldIssue, result);
    }
    
    private static void AssertEquivalent(Issue expected, CommandResult<ChangeIssueResult> actual)
    {
        actual.Payload.Should().BeEquivalentTo(new
        {
            Id = expected.Id.Value,
            expected.Name,
            expected.Description,
            expected.Link,
            expected.UpdatedAtUtc,
            GameId = expected.GameId.Value
        });
    }
}