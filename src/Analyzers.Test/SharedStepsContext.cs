using Bogus;

namespace UsingAsync.Analyzers.Test;

internal sealed class SharedStepsContext
{
    internal string? Source { get; set; }
    internal string? MethodName { get; set; }
    internal string? FixedSource { get; set; }

    internal string GenerateAsyncMethodName()
    {
        MethodName = $"{GenerateIdentifier(maxLength: 251)}Async";
        return MethodName;
    }

    internal static string GenerateIdentifier(int maxLength = 256)
    {
        const string LetterOrUnderscoreCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";
        const string AlphanumericOrUnderscoreCharacters = $"0123456789{LetterOrUnderscoreCharacters}";
        Faker faker = new();
        return $"{faker.Random.String2(length: 1, chars: LetterOrUnderscoreCharacters)}{faker.Random.String2(minLength: 0, maxLength: maxLength - 1, chars: AlphanumericOrUnderscoreCharacters)}";
    }
}
