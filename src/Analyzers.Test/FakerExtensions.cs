namespace Bogus;

internal static class FakerExtensions
{
    internal static string RandomAccessModifier(this Faker faker)
        => faker.PickRandom("public", "internal", "protected", "private");
}
