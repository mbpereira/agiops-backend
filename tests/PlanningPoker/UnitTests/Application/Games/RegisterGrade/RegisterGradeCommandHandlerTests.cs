#region

using AutoBogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Games.RegisterGrade;
using PlanningPoker.Application.Users;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

#endregion

namespace PlanningPoker.UnitTests.Application.Games.RegisterGrade;

public class RegisterGradeCommandHandlerTests
{
    private readonly IUserContext _authenticationContext;
    private readonly RegisterGradeCommandHandler _handler;
    private readonly IUnitOfWork _uow;

    public RegisterGradeCommandHandlerTests()
    {
        _authenticationContext = Substitute.For<IUserContext>();
        _uow = Substitute.For<IUnitOfWork>();
        _handler = new RegisterGradeCommandHandler(_uow, _authenticationContext);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnsNotFoundWhenIssueDoesNotExists()
    {
        var command = new RegisterGradeCommand(FakerInstance.ValidId(), GetValidGrade());

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.RecordNotFound);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidationFailedWhenProvidedDataIsNotValid()
    {
        var command = new RegisterGradeCommand(FakerInstance.InvalidId(), GetValidGrade());

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnsErrorWhenCurrentUserIdIsNotValid()
    {
        var expectedIssue = GetValidIssue();
        _uow.Issues.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(expectedIssue);
        _authenticationContext.GetCurrentUserAsync()
            .Returns(new UserInformation(EntityId.Empty));
        var command = new RegisterGradeCommand(FakerInstance.ValidId(), GetValidGrade());

        var result = await _handler.HandleAsync(command);

        using var _ = new AssertionScope();
        result.Status.Should().Be(CommandStatus.ValidationFailed);
        result.Details.Should().BeEquivalentTo(new[]
        {
            new { Code = "Issue.UserId", Message = "Provided value cannot be null, empty or white space." }
        });
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnsSuccessWhenGradeIsRegistered()
    {
        var expectedIssue = GetValidIssue();
        _uow.Issues.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(expectedIssue);
        _authenticationContext.GetCurrentUserAsync()
            .Returns(new AutoFaker<UserInformation>().RuleFor(u => u.Id, faker => faker.ValidId()));
        var command = new RegisterGradeCommand(FakerInstance.ValidId(), GetValidGrade());

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.Success);
    }

    private Issue GetValidIssue()
    {
        return Issue.New(
            FakerInstance.ValidId(),
            FakerInstance.ValidId(),
            FakerInstance.Random.String2(10),
            FakerInstance.Random.String2(10),
            FakerInstance.Internet.Url());
    }

    private string GetValidGrade()
    {
        return FakerInstance.Random.String(1, '0', '9');
    }
}