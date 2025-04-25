using Bogus;

namespace UsingAsync.Analyzers.Test;

[Binding]
internal sealed class ForStatementStepDefinitions(SharedStepsContext sharedStepsContext)
{
    private readonly SharedStepsContext _sharedStepsContext = sharedStepsContext;

    [Given("a non-async method that returns some Task inside a using-declaration scope in a for-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInsideAUsingDeclarationScopeInAForStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task {{methodName}}()
                {
                    for (var x = 0; x < 8; ++x)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return DoSomeAsynchronousWorkAsync({{variableName}});
                    }

                    return Task.CompletedTask;
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task {{methodName}}()
                {
                    for (var x = 0; x < 8; ++x)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        await DoSomeAsynchronousWorkAsync({{variableName}});
                        return;
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some generic-Task inside a using-declaration scope in a for-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskInsideAUsingDeclarationScopeInAForStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task<long> {{methodName}}()
                {
                    for (var x = 0; x < 8; ++x)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }

                    return Task.FromResult(long.MaxValue);
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task<long> {{methodName}}()
                {
                    for (var x = 0; x < 8; ++x)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return await DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }

                    return long.MaxValue;
                }|}
            """);
    }

    [Given("a non-async method that returns some Task not inside a using-declaration scope in a for-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInAForStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    for (var x = 0; x < 8; ++x)
                    {
                        return DoSomeAsynchronousWorkAsync(null);
                    }

                    return Task.CompletedTask;
                }|}
            """);
    }

    [Given("a non-async method that returns some generic-Task not inside a using-declaration scope in a for-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskNotInsideAUsingDeclarationScopeInAForStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    for (var x = 0; x < 8; ++x)
                    {
                        return DoSomeAsynchronousWorkAndGetPositionAsync(null);
                    }

                    return Task.FromResult(long.MaxValue);
                }|}
            """);
    }

    [Given("a non-async method that returns a completed Task inside a using-declaration scope in a for-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInsideAUsingDeclarationScopeInAForStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    for (var x = 0; x < 8; ++x)
                    {
                        using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log");
                        return Task.CompletedTask;
                    }

                    return Task.CompletedTask;
                }|}
            """);
    }

    [Given("a non-async method that returns a completed generic-Task inside a using-declaration scope in a for-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedGenericTaskInsideAUsingDeclarationScopeInAForStatement()
    {
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    for (var x = 0; x < 8; ++x)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return Task.FromResult({{variableName}}.Position);
                    }

                    return Task.FromResult(long.MaxValue);
                }|}
            """);
    }
}
