using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Issues.CreateIssue;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;

namespace PlanningPoker.UnitTests.Application.Issues.CreateIssue
{
    public class CreateIssueCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly IUnitOfWork _uow;
        private readonly CreateIssueCommandHandler _handler;

        public CreateIssueCommandHandlerTests()
        {
            _faker = new();
            _uow = Substitute.For<IUnitOfWork>();
            _handler = new CreateIssueCommandHandler(_uow);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("ab")]
        public async Task ShouldReturnsValidationFailedWhenProvidedDataIsNotValid(string? invalidName)
        {
            var command = new CreateIssueCommand(
                    gameId: 0,
                    name: invalidName!
                );

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task ShouldReturnGeneratedIdWhenIssueWasCreated()
        {
            Issue expectedIssue = GetValidIssue();
            var command = new CreateIssueCommand(
                gameId: expectedIssue.GameId,
                name: expectedIssue.Name,
                description: expectedIssue.Description,
                link: expectedIssue.Link
            );
            _uow.Issues.AddAsync(Arg.Any<Issue>())
                .Returns(expectedIssue);

            var result = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            result.Data!.Id.Should().Be(expectedIssue.Id.Value);
            result.Status.Should().Be(CommandStatus.Success);
        }

        private Issue GetValidIssue()
            => Issue.New(
                id: _faker.Random.Int(min: 1),
                tenantId: _faker.Random.Int(min: 1),
                gameId: _faker.Random.Int(min: 1),
                name: _faker.Random.String2(length: 10),
                description: _faker.Random.Word(),
                link: _faker.Internet.Url());
    }
}
