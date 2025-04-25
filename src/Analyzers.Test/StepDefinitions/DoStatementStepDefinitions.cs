using Bogus;

namespace UsingAsync.Analyzers.Test;

[Binding]
internal sealed class DoStatementStepDefinitions(SharedStepsContext sharedStepsContext)
{
    private readonly SharedStepsContext _sharedStepsContext = sharedStepsContext;

    [Given("a non-async method that returns some Task inside a using-declaration scope in a do-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInsideAUsingDeclarationScopeInADoStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task {{methodName}}()
                {
                    do
                    {
                        if (long.MaxValue > 0)
                        {
                            using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                            return DoSomeAsynchronousWorkAsync({{variableName}});
                        }
                    }
                    while (long.MaxValue > 0);
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task {{methodName}}()
                {
                    do
                    {
                        if (long.MaxValue > 0)
                        {
                            using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                            await DoSomeAsynchronousWorkAsync({{variableName}});
                            return;
                        }
                    }
                    while (long.MaxValue > 0);
                }|}
            """);
    }

    [Given("a non-async method that returns some generic-Task inside a using-declaration scope in a do-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskInsideAUsingDeclarationScopeInADoStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task<long> {{methodName}}()
                {
                    do
                    {
                        if (long.MaxValue > 0)
                        {
                            using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                            return DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                        }
                    }
                    while (long.MaxValue > 0);
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task<long> {{methodName}}()
                {
                    do
                    {
                        if (long.MaxValue > 0)
                        {
                            using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                            return await DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                        }
                    }
                    while (long.MaxValue > 0);
                }|}
            """);
    }

    [Given("a non-async method that returns some Task not inside a using-declaration scope in a do-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInADoStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    do
                    {
                        if (long.MaxValue > 0)
                        {
                            return DoSomeAsynchronousWorkAsync(null);
                        }
                    }
                    while (long.MaxValue > 0);
                }|}
            """);
    }

    [Given("a non-async method that returns some generic-Task not inside a using-declaration scope in a do-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskNotInsideAUsingDeclarationScopeInADoStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    do
                    {
                        if (long.MaxValue > 0)
                        {
                            return DoSomeAsynchronousWorkAndGetPositionAsync(null);
                        }
                    }
                    while (long.MaxValue > 0);
                }|}
            """);
    }

    [Given("a non-async method that returns a completed Task inside a using-declaration scope in a do-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInsideAUsingDeclarationScopeInADoStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    do
                    {
                        if (long.MaxValue > 0)
                        {
                            using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log");
                            return Task.CompletedTask;
                        }
                    }
                    while (long.MaxValue > 0);
                }|}
            """);
    }

    [Given("a non-async method that returns a completed generic-Task inside a using-declaration scope in a do-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedGenericTaskInsideAUsingDeclarationScopeInADoStatement()
    {
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    do
                    {
                        if (long.MaxValue > 0)
                        {
                            using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                            return Task.FromResult({{variableName}}.Position);
                        }
                    }
                    while (long.MaxValue > 0);
                }|}
            """);
    }
}
