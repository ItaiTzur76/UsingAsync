namespace Microsoft.CodeAnalysis;

internal static class SymbolExtensions
{
    internal static bool EqualsWithDefaultComparer(this ISymbol symbol, ISymbol? other)
        => symbol.Equals(other, SymbolEqualityComparer.Default);
}
