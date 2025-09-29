using Bogus;

namespace UsingAsync.Analyzers.Test;

[Binding]
internal sealed class ForEachStatementStepDefinitions(SharedStepsContext sharedStepsContext)
{
    private readonly SharedStepsContext _sharedStepsContext = sharedStepsContext;

    [Given("a non-async method that returns some Task inside a using-declaration scope in a foreach-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInsideAUsingDeclarationScopeInAForeachStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task {{methodName}}()
                {
                    foreach (var x in new int[] { 1, 2, 3 })
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
                    foreach (var x in new int[] { 1, 2, 3 })
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        await DoSomeAsynchronousWorkAsync({{variableName}});
                        return;
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns some Task inside a using-declaration scope in a foreach-statement")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeTaskInsideAUsingDeclarationScopeInAForeachStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var functionName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {{accessModifier}} static void {{methodName}}()
                {
                    {{functionName}}().Wait();

                    {|#0:static Task {{functionName}}()
                    {
                        foreach (var x in new int[] { 1, 2, 3 })
                        {
                            using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                            return DoSomeAsynchronousWorkAsync({{variableName}});
                        }

                        return Task.CompletedTask;
                    }|}
                }
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {{accessModifier}} static void {{methodName}}()
                {
                    {{functionName}}().Wait();

                    {|#0:static async Task {{functionName}}()
                    {
                        foreach (var x in new int[] { 1, 2, 3 })
                        {
                            using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                            await DoSomeAsynchronousWorkAsync({{variableName}});
                            return;
                        }
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns some generic-Task inside a using-declaration scope in a foreach-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskInsideAUsingDeclarationScopeInAForeachStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task<long> {{methodName}}()
                {
                    foreach (var x in new int[] { 1, 2, 3 })
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
                    foreach (var x in new int[] { 1, 2, 3 })
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return await DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }

                    return long.MaxValue;
                }|}
            """);
    }

    [Given("a non-async local function that returns some generic-Task inside a using-declaration scope in a foreach-statement")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeGenericTaskInsideAUsingDeclarationScopeInAForeachStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var functionName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {{accessModifier}} static long {{methodName}}()
                {
                    return {{functionName}}().Result;

                    {|#0:static Task<long> {{functionName}}()
                    {
                        foreach (var x in new int[] { 1, 2, 3 })
                        {
                            using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                            return DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                        }

                        return Task.FromResult(long.MaxValue);
                    }|}
                }
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {{accessModifier}} static long {{methodName}}()
                {
                    return {{functionName}}().Result;

                    {|#0:static async Task<long> {{functionName}}()
                    {
                        foreach (var x in new int[] { 1, 2, 3 })
                        {
                            using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                            return await DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                        }

                        return long.MaxValue;
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns some Task not inside a using-declaration scope in a foreach-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInAForeachStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    foreach (var x in new int[] { 1, 2, 3 })
                    {
                        return DoSomeAsynchronousWorkAsync(null);
                    }

                    return Task.CompletedTask;
                }|}
            """);
    }

    [Given("a non-async local function that returns some Task not inside a using-declaration scope in a foreach-statement")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInAForeachStatement()
    {
        var functionName = _sharedStepsContext.GenerateAsyncMethodName();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {{new Faker().RandomAccessModifier()}} static void {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    {{functionName}}().Wait();

                    {|#0:static Task {{functionName}}()
                    {
                        foreach (var x in new int[] { 1, 2, 3 })
                        {
                            return DoSomeAsynchronousWorkAsync(null);
                        }

                        return Task.CompletedTask;
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns some generic-Task not inside a using-declaration scope in a foreach-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskNotInsideAUsingDeclarationScopeInAForeachStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    foreach (var x in new int[] { 1, 2, 3 })
                    {
                        return DoSomeAsynchronousWorkAndGetPositionAsync(null);
                    }

                    return Task.FromResult(long.MaxValue);
                }|}
            """);
    }

    [Given("a non-async local function that returns some generic-Task not inside a using-declaration scope in a foreach-statement")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeGenericTaskNotInsideAUsingDeclarationScopeInAForeachStatement()
    {
        var functionName = _sharedStepsContext.GenerateAsyncMethodName();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {{new Faker().RandomAccessModifier()}} static long {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    return {{functionName}}().Result;

                    {|#0:static Task<long> {{functionName}}()
                    {
                        foreach (var x in new int[] { 1, 2, 3 })
                        {
                            return DoSomeAsynchronousWorkAndGetPositionAsync(null);
                        }

                        return Task.FromResult(long.MaxValue);
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns a completed Task inside a using-declaration scope in a foreach-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInsideAUsingDeclarationScopeInAForeachStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    foreach (var x in new int[] { 1, 2, 3 })
                    {
                        using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log");
                        return Task.CompletedTask;
                    }

                    return Task.CompletedTask;
                }|}
            """);
    }

    [Given("a non-async local function that returns a completed Task inside a using-declaration scope in a foreach-statement")]
    internal void GivenANonAsyncLocalFunctionThatReturnsACompletedTaskInsideAUsingDeclarationScopeInAForeachStatement()
    {
        var functionName = _sharedStepsContext.GenerateAsyncMethodName();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {{new Faker().RandomAccessModifier()}} static void {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    {{functionName}}().Wait();

                    {|#0:static Task {{functionName}}()
                    {
                        foreach (var x in new int[] { 1, 2, 3 })
                        {
                            using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log");
                            return Task.CompletedTask;
                        }

                        return Task.CompletedTask;
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns a completed generic-Task inside a using-declaration scope in a foreach-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedGenericTaskInsideAUsingDeclarationScopeInAForeachStatement()
    {
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    foreach (var x in new int[] { 1, 2, 3 })
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return Task.FromResult({{variableName}}.Position);
                    }

                    return Task.FromResult(long.MaxValue);
                }|}
            """);
    }

    [Given("a non-async local function that returns a completed generic-Task inside a using-declaration scope in a foreach-statement")]
    internal void GivenANonAsyncLocalFunctionThatReturnsACompletedGenericTaskInsideAUsingDeclarationScopeInAForeachStatement()
    {
        var functionName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {{new Faker().RandomAccessModifier()}} static long {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    return {{functionName}}().Result;

                    {|#0:static Task<long> {{functionName}}()
                    {
                        foreach (var x in new int[] { 1, 2, 3 })
                        {
                            using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                            return Task.FromResult({{variableName}}.Position);
                        }

                        return Task.FromResult(long.MaxValue);
                    }|}
                }
            """);
    }
}
