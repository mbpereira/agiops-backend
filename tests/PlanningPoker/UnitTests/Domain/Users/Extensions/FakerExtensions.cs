using Bogus;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.UnitTests.Domain.Users.Extensions
{
    public static class FakerExtensions
    {
        public static int InvalidId(this Faker faker) => faker.Random.Int(max: 0);

        public static int ValidId(this Faker faker) => faker.Random.Int(min: 1);

        public static string ValidEmail(this Faker faker) => faker.Person.Email;

        public static Invite ValidInvite(this Faker faker, int? tenantId = null, InviteStatus? status = null, DateTime? expiresAtUtc = null)
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
