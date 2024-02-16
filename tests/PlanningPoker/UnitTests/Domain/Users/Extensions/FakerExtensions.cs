using Bogus;
using PlanningPoker.Domain.Issues;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.UnitTests.Domain.Users.Extensions
{
    public static class FakerExtensions
    {
        public static int InvalidId(this Faker faker) => faker.Random.Int(max: 0);

        public static int ValidId(this Faker faker) => faker.Random.Int(min: 1);

        public static string ValidEmail(this Faker faker) => faker.Person.Email;

        public static string InvalidEmail(this Faker faker) => faker.PickRandomParam(new string[] { null!, "", " ", faker.Random.String() });

        public static Invitation NewInvalidInvitation(this Faker faker) => Invitation.New(tenantId: faker.Random.Int(min: 1), to: faker.InvalidEmail(), faker.PickRandom<Role>());

        public static Invitation LoadValidInvitation(this Faker faker, int? tenantId = null, InvitationStatus? status = null, DateTime? expiresAtUtc = null, Role? role = null)
            => Invitation.Load(
                id: faker.ValidId(),
                tenantId: tenantId ?? faker.ValidId(),
                to: faker.Person.Email,
                role: role ?? faker.PickRandom<Role>(),
                token: Guid.NewGuid(),
                createdAtUtc: DateTime.UtcNow,
                sentAtUtc: DateTime.UtcNow,
                expiresAtUtc: expiresAtUtc ?? DateTime.UtcNow.AddMinutes(InvitationConstants.ExpirationTimeInMinutes),
                status: status ?? InvitationStatus.Sent);
        public static VotingSystem LoadValidVotingSystem(this Faker faker)
            => VotingSystem.Load(
                id: faker.ValidId(),
                tenantId: faker.ValidId(),
                description: faker.Random.String2(length: 10),
                userId: faker.ValidId(),
                possibleGrades: faker.Make(3, () => faker.Random.Int(min: 1).ToString()),
                sharing: SharingStatus.Requested);

        public static Game NewValidGame(this Faker faker, string? password = null, VotingSystem? votingSystem = null)
            => Game.New(
                tenantId: faker.ValidId(),
                name: faker.Random.String2(length: 5),
                userId: faker.ValidId(),
                password: password,
                votingSystem: votingSystem ?? faker.LoadValidVotingSystem());

        public static Game LoadValidGame(this Faker faker, string? password = null, VotingSystem? votingSystem = null)
            => Game.Load(
                id: faker.ValidId(),
                tenantId: faker.ValidId(),
                name: faker.Random.String2(length: 5),
                userId: faker.ValidId(),
                password: password,
                votingSystem: votingSystem ?? faker.LoadValidVotingSystem());

        public static VotingSystem InvalidVotingSystem(this Faker faker) =>
            VotingSystem.New(
                tenantId: faker.InvalidId(),
                description: string.Empty,
                userId: faker.InvalidId(),
                possibleGrades: new List<string>(),
                sharing: SharingStatus.Unshared);

        public static VotingSystem NewValidVotingSystem(this Faker faker, SharingStatus sharingStatus = SharingStatus.Undefined) =>
            VotingSystem.New(
                tenantId: faker.ValidId(),
                description: faker.Random.String2(length: 10),
                userId: faker.ValidId(),
                possibleGrades: faker.Make(3, () => faker.Random.Int().ToString()),
                sharing: sharingStatus);
    }
}
