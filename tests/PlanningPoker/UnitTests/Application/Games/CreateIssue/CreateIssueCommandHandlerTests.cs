using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Games.CreateIssue;
using PlanningPoker.Application.Tenants;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

namespace PlanningPoker.UnitTests.Application.Games.CreateIssue
{
    public class CreateIssueCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly IUnitOfWork _uow;
        private readonly CreateIssueCommandHandler _handler;

        public CreateIssueCommandHandlerTests()
        {
            _faker = new();
            var tenantContext = Substitute.For<ITenantContext>();
            _uow = Substitute.For<IUnitOfWork>();
            tenantContext.GetCurrentTenantAsync()
                .Returns(new TenantInformation(Id: _faker.Random.Int(min: 1)));
            _handler = new CreateIssueCommandHandler(_uow, tenantContext);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("ab")]
        public async Task HandleAsync_ShouldReturnsValidationFailedWhenProvidedDataIsNotValid(string? invalidName)
        {
            var command = new CreateIssueCommand(
                gameId: 0,
                name: invalidName!
            );

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnGeneratedIdWhenIssueWasCreated()
        {
            var expectedIssue = GetValidIssue();
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
            => Issue.Load(
                id: _faker.Random.Int(min: 1),
                tenantId: _faker.Random.Int(min: 1),
                gameId: _faker.Random.Int(min: 1),
                name: _faker.Random.String2(length: 10),
                description: _faker.Random.Word(),
                link: _faker.Internet.Url());
    }
}