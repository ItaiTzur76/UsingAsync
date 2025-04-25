namespace Microsoft.CodeAnalysis;

internal static class CompilationExtensions
{
    internal static INamedTypeSymbol? GetTaskClassType(this Compilation compilation)
        => compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
}
