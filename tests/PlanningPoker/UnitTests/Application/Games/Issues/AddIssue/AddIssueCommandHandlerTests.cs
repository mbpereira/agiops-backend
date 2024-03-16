#region

using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Games.Issues.AddIssue;
using PlanningPoker.Application.Tenants;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;
using PlanningPoker.UnitTests.Common.Extensions;

#endregion

namespace PlanningPoker.UnitTests.Application.Games.Issues.AddIssue;

public class AddIssueCommandHandlerTests
{
    private readonly AddIssueCommandHandler _handler;
    private readonly IUnitOfWork _uow;

    public AddIssueCommandHandlerTests()
    {
        var tenantContext = Substitute.For<ITenantContext>();
        _uow = Substitute.For<IUnitOfWork>();
        tenantContext.GetCurrentTenantAsync()
            .Returns(new TenantInformation(FakerInstance.ValidId()));
        _handler = new AddIssueCommandHandler(_uow, tenantContext);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("ab")]
    public async Task HandleAsync_InvalidDataProvided_ReturnsValidationFailed(string? invalidName)
    {
        var command = new AddIssueCommand(
            EntityId.Empty,
            invalidName!
        );

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_SuccessfulIssueCreation_ReturnsGeneratedId()
    {
        var expectedIssue = FakerInstance.NewValidIssue();
        var command = new AddIssueCommand(
            expectedIssue.GameId,
            expectedIssue.Name,
            description: expectedIssue.Description,
            link: expectedIssue.Link
        );
        _uow.Issues.AddAsync(Arg.Any<Issue>())
            .Returns(expectedIssue);

        var result = await _handler.HandleAsync(command);

        using var _ = new AssertionScope();
        result.Payload!.Id.Should().Be(expectedIssue.Id.Value);
        result.Status.Should().Be(CommandStatus.Success);
    }
}