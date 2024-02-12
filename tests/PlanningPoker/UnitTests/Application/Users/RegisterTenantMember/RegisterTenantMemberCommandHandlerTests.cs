using Bogus;

namespace PlanningPoker.UnitTests.Application.Users.RegisterTenantMember
{
    public class RegisterTenantMemberCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly RegisterTenantMemberCommandHandler _handler;

        public RegisterTenantMemberCommandHandlerTests()
        {
            _faker = new();
            _handler = new RegisterTenantMemberCommandHandler();
        }

        [Fact]
        public async Task ShouldReturnValidationErrorWhenProvidedInviteIdIsExpired()
        {

        }
    }

    public class RegisterTenantMemberCommandHandler
    {
    }
}
