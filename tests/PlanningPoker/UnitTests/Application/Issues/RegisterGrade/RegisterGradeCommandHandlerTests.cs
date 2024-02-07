using AutoBogus;
using Bogus;
using FluentAssertions;
using NSubstitute;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;

namespace PlanningPoker.UnitTests.Application.Issues.RegisterGrade
{
    public class RegisterGradeCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly IUnitOfWork _uow;
        private readonly IAuthenticationContext _authenticationContext;
        private readonly RegisterGradeCommandHandler _handler;

        public RegisterGradeCommandHandlerTests()
        {
            _faker = new Faker();
            _authenticationContext = Substitute.For<IAuthenticationContext>();
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
            var expectedIssue = Issue.New(
                _faker.Random.Int(min: 1), _faker.Random.Int(min: 1), _faker.Random.String2(length: 10), _faker.Internet.Url(), _faker.Internet.Url());
            _uow.Issues.GetByIdAsync(Arg.Any<EntityId>())
                .Returns(expectedIssue);
            var command = new RegisterGradeCommand(issueId: _faker.Random.Int(min: 1), grade: _faker.Random.Decimal());
            var act = async () => await _handler.HandleAsync(command);

            var ex = await act.Should().ThrowAsync<DomainException>();

            ex.Which.Message.Should().Be("Provided user id is not valid.");
        }

        [Fact]
        public async Task ShouldReturnsSuccessWhenGradeIsRegistered()
        {
            var expectedIssue = Issue.New(
                _faker.Random.Int(min: 1), _faker.Random.Int(min: 1), _faker.Random.String2(length: 10), _faker.Internet.Url(), _faker.Internet.Url());
            _uow.Issues.GetByIdAsync(Arg.Any<EntityId>())
                .Returns(expectedIssue);
            _authenticationContext.GetCurrentUserAsync()
                .Returns(new AutoFaker<UserInformation>().RuleFor(u => u.Id, faker => faker.Random.Int(min: 1)));
            var command = new RegisterGradeCommand(issueId: _faker.Random.Int(min: 1), grade: _faker.Random.Decimal());

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.Success);
        }
    }
}
