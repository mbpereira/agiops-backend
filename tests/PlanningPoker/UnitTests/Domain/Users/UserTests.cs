#region

using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Users;

#endregion

namespace PlanningPoker.UnitTests.Domain.Users;

public class UserTests
{
    private static readonly Faker Faker = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void New_InvalidProperties_ReturnsErrorsWithPropertyDetails(string invalidName)
    {
        var expectedErrors = new[]
        {
            new
            {
                Code = "User.Name",
                Message = "The provided string does not meet the minimum length requirement. Min length: 3."
            }
        };

        var user = User.New(invalidName, Faker.Internet.Email());

        using var _ = new AssertionScope();
        user.Errors.Should().BeEquivalentTo(expectedErrors);
        user.IsValid.Should().BeFalse();
    }

    [Fact]
    public void NewGuest_ValidData_SetEmailAsNullAndCreateSessionId()
    {
        var user = User.NewGuest(Faker.Random.String2(10));

        using var _ = new AssertionScope();
        user.Email.Should().BeNull();
        user.Guest!.SessionId.Should().NotBeNull();
        user.IsGuest.Should().BeTrue();
    }

    [Fact]
    public void New_ValidEmail_SetGuestAsNullAndReturnsIsValidTrue()
    {
        var email = Faker.Internet.Email();

        var user = User.New(Faker.Random.String2(10), email);

        using var _ = new AssertionScope();
        user.Guest.Should().BeNull();
        user.IsGuest.Should().BeFalse();
        user.Email!.Value.Should().Be(email);
        user.IsValid.Should().BeTrue();
    }

    [Fact]
    public void New_InvalidEmail_dReturnsError()
    {
        var name = Faker.Random.String2(5);

        var user = User.New(name, null!);

        user.Errors.Should().BeEquivalentTo([
            new { Code = "User.Email", Message = "Provided email is not valid." }
        ]);
    }
}