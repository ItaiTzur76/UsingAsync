using Bogus;

namespace UsingAsync.Analyzers.Test;

[Binding]
internal sealed class SwitchStatementStepDefinitions(SharedStepsContext sharedStepsContext)
{
    private readonly SharedStepsContext _sharedStepsContext = sharedStepsContext;

    [Given("a non-async method that returns some Task inside a using-declaration scope in a switch-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInsideAUsingDeclarationScopeInASwitchStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task {{methodName}}()
                {
                    long x = 0;
                    ++x;
                    switch (x)
                    {
                        case 10:
                        case 20:
                            {
                                using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                                return DoSomeAsynchronousWorkAsync({{variableName}});
                            }
                        default:
                            return Task.CompletedTask;
                    }
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task {{methodName}}()
                {
                    long x = 0;
                    ++x;
                    switch (x)
                    {
                        case 10:
                        case 20:
                            {
                                using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                                await DoSomeAsynchronousWorkAsync({{variableName}});
                                return;
                            }
                        default:
                            return;
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some generic-Task inside a using-declaration scope in a switch-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskInsideAUsingDeclarationScopeInASwitchStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task<long> {{methodName}}()
                {
                    long x = 0;
                    ++x;
                    switch (x)
                    {
                        case 10:
                        case 20:
                            {
                                using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                                return DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                            }
                        default:
                            return Task.FromResult(long.MaxValue);
                    }
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task<long> {{methodName}}()
                {
                    long x = 0;
                    ++x;
                    switch (x)
                    {
                        case 10:
                        case 20:
                            {
                                using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                                return await DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                            }
                        default:
                            return long.MaxValue;
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some Task not inside a using-declaration scope in all cases of a switch-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInAllCasesOfASwitchStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    long x = 0;
                    ++x;
                    switch (x)
                    {
                        case 10:
                        case 20:
                            return DoSomeAsynchronousWorkAsync(null);
                        default:
                            return Task.CompletedTask;
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some generic-Task not inside a using-declaration scope in all cases of a switch-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskNotInsideAUsingDeclarationScopeInAllCasesOfASwitchStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    long x = 0;
                    ++x;
                    switch (x)
                    {
                        case 10:
                        case 20:
                            return DoSomeAsynchronousWorkAndGetPositionAsync(null);
                        default:
                            return Task.FromResult(long.MaxValue);
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns a completed Task inside a using-declaration scope in all cases of a switch-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInsideAUsingDeclarationScopeInAllCasesOfASwitchStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    long x = 0;
                    ++x;
                    switch (x)
                    {
                        case 10:
                        case 20:
                            return Task.CompletedTask;
                        default:
                            return Task.CompletedTask;
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns a completed generic-Task inside a using-declaration scope in all cases of a switch-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedGenericTaskInsideAUsingDeclarationScopeInAllCasesOfASwitchStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    long x = 0;
                    ++x;
                    switch (x)
                    {
                        case 10:
                        case 20:
                            return Task.FromResult(long.MaxValue);
                        default:
                            return Task.FromResult(long.MinValue);
                    }
                }|}
            """);
    }
}
