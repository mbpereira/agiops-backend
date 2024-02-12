using Bogus;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.UnitTests.Domain.Users.Extensions
{
    public static class FakerExtensions
    {
        public static int InvalidId(this Faker faker) => faker.Random.Int(max: 0);

        public static int ValidId(this Faker faker) => faker.Random.Int(min: 1);

        public static string ValidEmail(this Faker faker) => faker.Person.Email;

        public static string InvalidEmail(this Faker faker) => faker.PickRandomParam(new string[] { null!, "", " ", faker.Random.String() });

        public static Invite NewInvalidInvite(this Faker faker) => Invite.New(tenantId: faker.Random.Int(min: 1), to: faker.InvalidEmail(), faker.PickRandom<Role>());

        public static Invite LoadValidInvite(this Faker faker, int? tenantId = null, InviteStatus? status = null, DateTime? expiresAtUtc = null)
            => Invite.Load(
                id: faker.ValidId(),
                tenantId: tenantId ?? faker.ValidId(),
                to: faker.Person.Email,
                role: faker.PickRandom<Role>(),
                token: Guid.NewGuid(),
                createdAtUtc: DateTime.UtcNow,
                sentAtUtc: DateTime.UtcNow,
                expiresAtUtc: expiresAtUtc ?? DateTime.UtcNow.AddMinutes(Invite.ExpirationTimeInMinutes),
                status: status ?? InviteStatus.Open);
    }
}
