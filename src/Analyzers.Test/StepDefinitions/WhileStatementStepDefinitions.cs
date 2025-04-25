using Bogus;

namespace UsingAsync.Analyzers.Test;

[Binding]
internal sealed class WhileStatementStepDefinitions(SharedStepsContext sharedStepsContext)
{
    private readonly SharedStepsContext _sharedStepsContext = sharedStepsContext;

    [Given("a non-async method that returns some Task inside a using-declaration scope in a while-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInsideAUsingDeclarationScopeInAWhileStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task {{methodName}}()
                {
                    while (long.MaxValue > 0)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return DoSomeAsynchronousWorkAsync({{variableName}});
                    }
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task {{methodName}}()
                {
                    while (long.MaxValue > 0)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        await DoSomeAsynchronousWorkAsync({{variableName}});
                        return;
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some generic-Task inside a using-declaration scope in a while-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskInsideAUsingDeclarationScopeInAWhileStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task<long> {{methodName}}()
                {
                    while (long.MaxValue > 0)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task<long> {{methodName}}()
                {
                    while (long.MaxValue > 0)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return await DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some Task not inside a using-declaration scope in a while-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInAWhileStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    while (long.MaxValue > 0)
                    {
                        return DoSomeAsynchronousWorkAsync(null);
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some generic-Task not inside a using-declaration scope in a while-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskNotInsideAUsingDeclarationScopeInAWhileStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    while (long.MaxValue > 0)
                    {
                        return DoSomeAsynchronousWorkAndGetPositionAsync(null);
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns a completed Task inside a using-declaration scope in a while-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInsideAUsingDeclarationScopeInAWhileStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    while (long.MaxValue > 0)
                    {
                        using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log");
                        return Task.CompletedTask;
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns a completed generic-Task inside a using-declaration scope in a while-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedGenericTaskInsideAUsingDeclarationScopeInAWhileStatement()
    {
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    while (long.MaxValue > 0)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return Task.FromResult({{variableName}}.Position);
                    }
                }|}
            """);
    }
}
