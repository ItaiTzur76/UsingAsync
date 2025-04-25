using Microsoft.CodeAnalysis.Testing;

namespace UsingAsync.Analyzers.Test;

[Binding]
internal sealed class SharedStepDefinitions(SharedStepsContext sharedStepsContext)
{
    private readonly SharedStepsContext _sharedStepsContext = sharedStepsContext;
    private DiagnosticResult _expected;

    [When("UsingAsync diagnostic is performed")]
    internal void WhenUsingAsyncDiagnosticIsPerformed()
    {
        _expected = CSharpCodeFixVerifier.Diagnostic(NonAsyncTaskMethodWithUsingAnalyzer.DiagnosticDescriptor)
            .WithLocation(0)
            .WithArguments(_sharedStepsContext.MethodName!);
    }

    [Then("the analyzer finds that diagnostic result and suggests the proper code-fix")]
    internal async Task ThenTheAnalyzerFindsThatDiagnosticResultAndSuggestsTheProperCodeFixAsync()
    {
        var source = _sharedStepsContext.Source!;
        await CSharpCodeFixVerifier.VerifyAnalyzerAsync(source, _expected);
        await CSharpCodeFixVerifier.VerifyCodeFixAsync(source, _expected, _sharedStepsContext.FixedSource!);
    }

    [Then("the analyzer finds no diagnostic results")]
    internal Task ThenTheAnalyzerFindsNoDiagnosticResultsAsync()
        => CSharpCodeFixVerifier.VerifyAnalyzerAsync(_sharedStepsContext.Source!);

    internal static string BuildSource(string sut) =>
        $$"""
        using System;
        using System.IO;
        using System.Threading.Tasks;

        namespace MyNamespace;

        public sealed class FileStreamWrapper(string path) : IDisposable
        {
            private readonly FileStream _fileStream = new(path, FileMode.Open, FileAccess.Read);

            public void Dispose() => _fileStream.Dispose();

            public long Position
            {
                get => _fileStream.Position;
                set => _fileStream.Position = value;
            }

            {{sut}}

            public static async Task<long> DoSomeAsynchronousWorkAndGetPositionAsync(FileStreamWrapper fileStreamWrapper)
            {
                await DoSomeAsynchronousWorkAsync(fileStreamWrapper);
                return fileStreamWrapper.Position;
            }

            public static async Task DoSomeAsynchronousWorkAsync(FileStreamWrapper fileStreamWrapper)
            {
                await Task.Delay(TimeSpan.FromSeconds(1)); // this mocks some long asynchronous work
                fileStreamWrapper.Position = 0;
            }
        }
        """;
}
