using Bogus;

namespace UsingAsync.Analyzers.Test;

[Binding]
internal sealed class BlockStepDefinitions(SharedStepsContext sharedStepsContext)
{
    private readonly SharedStepsContext _sharedStepsContext = sharedStepsContext;

    [Given("a non-async method that returns some Task inside a using-declaration scope in a terminal block")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInsideAUsingDeclarationScopeInATerminalBlock()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task {{methodName}}()
                {
                    const string path = @"C:\Windows\comsetup.log";

                    {
                        using FileStreamWrapper {{variableName}} = new(path);
                        return DoSomeAsynchronousWorkAsync({{variableName}});
                    }
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task {{methodName}}()
                {
                    const string path = @"C:\Windows\comsetup.log";

                    {
                        using FileStreamWrapper {{variableName}} = new(path);
                        await DoSomeAsynchronousWorkAsync({{variableName}});
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns some Task inside a using-declaration scope in a terminal block")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeTaskInsideAUsingDeclarationScopeInATerminalBlock()
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
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            using FileStreamWrapper {{variableName}} = new(path);
                            return DoSomeAsynchronousWorkAsync({{variableName}});
                        }
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
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            using FileStreamWrapper {{variableName}} = new(path);
                            await DoSomeAsynchronousWorkAsync({{variableName}});
                        }
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns some Task inside a using-declaration scope in a non-terminal block")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInsideAUsingDeclarationScopeInANonTerminalBlock()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task {{methodName}}()
                {
                    if (long.MaxValue > 0)
                    {
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            using FileStreamWrapper {{variableName}} = new(path);
                            return DoSomeAsynchronousWorkAsync({{variableName}});
                        }
                    }

                    if (long.MaxValue > 0)
                    {
                    }
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task {{methodName}}()
                {
                    if (long.MaxValue > 0)
                    {
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            using FileStreamWrapper {{variableName}} = new(path);
                            await DoSomeAsynchronousWorkAsync({{variableName}});
                            return;
                        }
                    }

                    if (long.MaxValue > 0)
                    {
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns some Task inside a using-declaration scope in a non-terminal block")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeTaskInsideAUsingDeclarationScopeInANonTerminalBlock()
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
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            using FileStreamWrapper {{variableName}} = new(path);
                            return DoSomeAsynchronousWorkAsync({{variableName}});
                        }

                        if (long.MaxValue > 0)
                        {
                        }
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
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            using FileStreamWrapper {{variableName}} = new(path);
                            await DoSomeAsynchronousWorkAsync({{variableName}});
                            return;
                        }

                        if (long.MaxValue > 0)
                        {
                        }
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns some generic-Task inside a using-declaration scope in a block")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskInsideAUsingDeclarationScopeInABlock()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task<long> {{methodName}}()
                {
                    const string path = @"C:\Windows\comsetup.log";

                    {
                        using FileStreamWrapper {{variableName}} = new(path);
                        return DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task<long> {{methodName}}()
                {
                    const string path = @"C:\Windows\comsetup.log";

                    {
                        using FileStreamWrapper {{variableName}} = new(path);
                        return await DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns some generic-Task inside a using-declaration scope in a block")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeGenericTaskInsideAUsingDeclarationScopeInABlock()
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
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            using FileStreamWrapper {{variableName}} = new(path);
                            return DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                        }
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
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            using FileStreamWrapper {{variableName}} = new(path);
                            return await DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                        }
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns some Task not inside a using-declaration scope in a terminal block")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInATerminalBlock()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    const string path = @"C:\Windows\comsetup.log";

                    {
                        return DoSomeAsynchronousWorkAsync(null);
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns some Task not inside a using-declaration scope in a terminal block")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInATerminalBlock()
    {
        var functionName = _sharedStepsContext.GenerateAsyncMethodName();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {{new Faker().RandomAccessModifier()}} static void {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    {{functionName}}().Wait();

                    {|#0:static Task {{functionName}}()
                    {
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            return DoSomeAsynchronousWorkAsync(null);
                        }
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns some Task not inside a using-declaration scope in a non-terminal block")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInANonTerminalBlock()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    if (long.MaxValue > 0)
                    {
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            return DoSomeAsynchronousWorkAsync(null);
                        }
                    }

                    return Task.CompletedTask;
                }|}
            """);
    }

    [Given("a non-async local function that returns some Task not inside a using-declaration scope in a non-terminal block")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInANonTerminalBlock()
    {
        var functionName = _sharedStepsContext.GenerateAsyncMethodName();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {{new Faker().RandomAccessModifier()}} static void {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    {{functionName}}().Wait();

                    {|#0:static Task {{functionName}}()
                    {
                        if (long.MaxValue > 0)
                        {
                            const string path = @"C:\Windows\comsetup.log";

                            {
                                return DoSomeAsynchronousWorkAsync(null);
                            }
                        }

                        return Task.CompletedTask;
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns some generic-Task not inside a using-declaration scope in a block")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskNotInsideAUsingDeclarationScopeInABlock()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    const string path = @"C:\Windows\comsetup.log";

                    {
                        return DoSomeAsynchronousWorkAndGetPositionAsync(null);
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns some generic-Task not inside a using-declaration scope in a block")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeGenericTaskNotInsideAUsingDeclarationScopeInABlock()
    {
        var functionName = _sharedStepsContext.GenerateAsyncMethodName();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {{new Faker().RandomAccessModifier()}} static long {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    return {{functionName}}().Result;

                    {|#0:static Task<long> {{functionName}}()
                    {
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            return DoSomeAsynchronousWorkAndGetPositionAsync(null);
                        }
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns a completed Task inside a using-declaration scope in a terminal block")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInsideAUsingDeclarationScopeInATerminalBlock()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    const string path = @"C:\Windows\comsetup.log";

                    {
                        using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(path);
                        return Task.CompletedTask;
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns a completed Task inside a using-declaration scope in a terminal block")]
    internal void GivenANonAsyncLocalFunctionThatReturnsACompletedTaskInsideAUsingDeclarationScopeInATerminalBlock()
    {
        var functionName = _sharedStepsContext.GenerateAsyncMethodName();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {{new Faker().RandomAccessModifier()}} static void {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    {{functionName}}().Wait();

                    {|#0:static Task {{functionName}}()
                    {
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(path);
                            return Task.CompletedTask;
                        }
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns a completed Task inside a using-declaration scope in a non-terminal block")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInsideAUsingDeclarationScopeInANonTerminalBlock()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    if (long.MaxValue > 0)
                    {
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(path);
                            return Task.CompletedTask;
                        }
                    }

                    return Task.CompletedTask;
                }|}
            """);
    }

    [Given("a non-async local function that returns a completed Task inside a using-declaration scope in a non-terminal block")]
    internal void GivenANonAsyncLocalFunctionThatReturnsACompletedTaskInsideAUsingDeclarationScopeInANonTerminalBlock()
    {
        var functionName = _sharedStepsContext.GenerateAsyncMethodName();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {{new Faker().RandomAccessModifier()}} static void {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    {{functionName}}().Wait();

                    {|#0:static Task {{functionName}}()
                    {
                        if (long.MaxValue > 0)
                        {
                            const string path = @"C:\Windows\comsetup.log";

                            {
                                using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(path);
                                return Task.CompletedTask;
                            }
                        }

                        return Task.CompletedTask;
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns a completed generic-Task inside a using-declaration scope in a block")]
    internal void GivenANonAsyncMethodThatReturnsACompletedGenericTaskInsideAUsingDeclarationScopeInABlock()
    {
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    const string path = @"C:\Windows\comsetup.log";

                    {
                        using FileStreamWrapper {{variableName}} = new(path);
                        return Task.FromResult({{variableName}}.Position);
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns a completed generic-Task inside a using-declaration scope in a block")]
    internal void GivenANonAsyncLocalFunctionThatReturnsACompletedGenericTaskInsideAUsingDeclarationScopeInABlock()
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
                        const string path = @"C:\Windows\comsetup.log";

                        {
                            using FileStreamWrapper {{variableName}} = new(path);
                            return Task.FromResult({{variableName}}.Position);
                        }
                    }|}
                }
            """);
    }
}
