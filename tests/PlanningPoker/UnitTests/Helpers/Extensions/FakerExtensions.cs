#region

using Bogus;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Abstractions.Clock;
using PlanningPoker.Domain.Games;
using PlanningPoker.Domain.Invitations;
using PlanningPoker.Domain.Users;

#endregion

namespace PlanningPoker.UnitTests.Helpers.Extensions;

public static class FakerExtensions
{
    public static string InvalidId(this Faker _)
    {
        return EntityId.Empty;
    }

    public static string ValidId(this Faker _)
    {
        return EntityId.Generate();
    }

    public static string ValidEmail(this Faker faker)
    {
        return faker.Person.Email;
    }

    public static string InvalidEmail(this Faker faker)
    {
        return faker.PickRandomParam([null!, "", " ", faker.Random.String()]);
    }

    public static Invitation NewInvalidInvitation(this Faker faker)
    {
        return Invitation.New(
            EntityId.Empty,
            faker.InvalidEmail(),
            faker.PickRandom<Role>(),
            DefaultDateTimeProvider.Instance);
    }

    public static Invitation NewValidInvitation(this Faker faker, string? tenantId = null,
        InvitationStatus? status = null, DateTime? expiresAtUtc = null, Role? role = null,
        IDateTimeProvider? dateTimeProvider = null)
    {
        return Invitation.New(
            tenantId ?? faker.ValidId(),
            faker.Person.Email,
            role ?? faker.PickRandom<Role>(),
            dateTimeProvider ?? DefaultDateTimeProvider.Instance);
    }

    public static Game NewValidGame(this Faker faker, string? password = null, VotingSystem? votingSystem = null,
        string? teamId = null)
    {
        return Game.New(
            faker.ValidId(),
            faker.Random.String2(5),
            faker.ValidId(),
            password: password,
            votingSystem: votingSystem ?? faker.NewValidVotingSystem(),
            teamId: teamId);
    }

    public static VotingSystem InvalidVotingSystem(this Faker faker)
    {
        return VotingSystem.New(
            faker.InvalidId(),
            string.Empty,
            faker.InvalidId(),
            new List<string>());
    }

    public static VotingSystem NewValidVotingSystem(this Faker faker)
    {
        return VotingSystem.New(
            faker.ValidId(),
            faker.Random.String2(10),
            faker.ValidId(),
            faker.Make(3, () => faker.Random.Int().ToString()),
            faker.Random.Words());
    }
}