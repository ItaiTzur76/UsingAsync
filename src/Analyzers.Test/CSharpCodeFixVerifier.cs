using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using UsingAsync.CSharp.CodeFixes;

using NonAsyncTaskMethodWithUsingCodeFixTester = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    UsingAsync.Analyzers.NonAsyncTaskMethodWithUsingAnalyzer,
    UsingAsync.CSharp.CodeFixes.NonAsyncTaskMethodWithUsingCodeFix,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace UsingAsync.Analyzers.Test;

internal static class CSharpCodeFixVerifier
{
    internal static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
        => CSharpCodeFixVerifier<NonAsyncTaskMethodWithUsingAnalyzer, NonAsyncTaskMethodWithUsingCodeFix, DefaultVerifier>
            .Diagnostic(descriptor);

    internal static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        => VerifyAnalyzerAsync(new NonAsyncTaskMethodWithUsingCodeFixTester { TestCode = source }, expected);

    internal static Task VerifyCodeFixAsync(string source, DiagnosticResult expected, string fixedSource)
        => VerifyAnalyzerAsync(
            new NonAsyncTaskMethodWithUsingCodeFixTester { TestCode = source, FixedCode = fixedSource }, [expected]);

    private static Task VerifyAnalyzerAsync(NonAsyncTaskMethodWithUsingCodeFixTester test, DiagnosticResult[] expected)
    {
        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync(CancellationToken.None);
    }
}
