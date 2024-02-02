using Bogus;

namespace PlanningPoker.UnitTests.Helpers.Extensions
{
    public static class FakerExtensions
    {
        public static Faker<T> UsePrivateConstructor<T>(this Faker<T> faker) where T : class
            => faker.CustomInstantiator(f => (T) Activator.CreateInstance(typeof(T), nonPublic: true)!);
    }
}
