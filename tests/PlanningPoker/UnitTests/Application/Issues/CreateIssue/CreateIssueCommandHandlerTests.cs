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
                    GameId: 0,
                    Name: invalidName!
                );

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task ShouldReturnGeneratedIdWhenIssueWasCreated()
        {
            var expectedIssue = Issue.New(_faker.Random.Int(min: 1), _faker.Random.Int(min: 1), _faker.Random.String2(length: 10), _faker.Random.Word(), _faker.Internet.Url());
            var command = new CreateIssueCommand(
                GameId: expectedIssue.GameId,
                Name: expectedIssue.Name,
                Description: expectedIssue.Description,
                Link: expectedIssue.Link
            );
            _uow.Issues.AddAsync(Arg.Any<Issue>())
                .Returns(expectedIssue);

            var result = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            result.Data!.Id.Should().Be(expectedIssue.Id.Value);
            result.Status.Should().Be(CommandStatus.Success);
        }
    }
}
