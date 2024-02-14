using AutoBogus;
using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Issues.RegisterGrade;
using PlanningPoker.Application.Users;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;

namespace PlanningPoker.UnitTests.Application.Issues.RegisterGrade
{
    public class RegisterGradeCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly IUnitOfWork _uow;
        private readonly IUserContext _authenticationContext;
        private readonly RegisterGradeCommandHandler _handler;

        public RegisterGradeCommandHandlerTests()
        {
            _faker = new Faker();
            _authenticationContext = Substitute.For<IUserContext>();
            _uow = Substitute.For<IUnitOfWork>();
            _handler = new RegisterGradeCommandHandler(_uow, _authenticationContext);
        }

        [Fact]
        public async Task ShouldReturnsNotFoundWhenIssueDoesNotExists()
        {
            var command = new RegisterGradeCommand(issueId: _faker.Random.Int(min: 1), grade: _faker.Random.Decimal());

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.RecordNotFound);
        }

        [Fact]
        public async Task ShouldReturnValidationFailedWhenProvidedDataIsNotValid()
        {
            var command = new RegisterGradeCommand(issueId: 0, grade: _faker.Random.Decimal());

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task ShouldThrowsExceptionWhenCurrentUserIdIsNotValid()
        {
            var expectedIssue = GetValidIssue();
            _uow.Issues.GetByIdAsync(Arg.Any<EntityId>())
                .Returns(expectedIssue);
            _authenticationContext.GetCurrentUserAsync()
                .Returns(new UserInformation(Id: 0));
            var command = new RegisterGradeCommand(issueId: _faker.Random.Int(min: 1), grade: _faker.Random.Decimal());
            
            var result = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            result.Status.Should().Be(CommandStatus.ValidationFailed);
            result.Details.Should().BeEquivalentTo(new[] 
            { 
                new { Code = "Issue.RegisterGrade", Message = "Provided user id is not valid." } 
            });
        }

        [Fact]
        public async Task ShouldReturnsSuccessWhenGradeIsRegistered()
        {
            var expectedIssue = GetValidIssue();
            _uow.Issues.GetByIdAsync(Arg.Any<EntityId>())
                .Returns(expectedIssue);
            _authenticationContext.GetCurrentUserAsync()
                .Returns(new AutoFaker<UserInformation>().RuleFor(u => u.Id, faker => faker.Random.Int(min: 1)));
            var command = new RegisterGradeCommand(issueId: _faker.Random.Int(min: 1), grade: _faker.Random.Decimal());

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.Success);
        }

        private Issue GetValidIssue()
            => Issue.New(
                id: _faker.Random.Int(min: 1),
                tenantId: _faker.Random.Int(min: 1),
                gameId: _faker.Random.Int(min: 1),
                name: _faker.Random.String2(length: 10),
                description: _faker.Random.String2(length: 10),
                link: _faker.Internet.Url());
    }
}
