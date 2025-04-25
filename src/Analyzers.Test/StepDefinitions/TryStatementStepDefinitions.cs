using Bogus;

namespace UsingAsync.Analyzers.Test;

[Binding]
internal sealed class TryStatementStepDefinitions(SharedStepsContext sharedStepsContext)
{
    private readonly SharedStepsContext _sharedStepsContext = sharedStepsContext;

    [Given("a non-async method that returns some Task inside a using-declaration scope in a terminal try-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInsideAUsingDeclarationScopeInATerminalTryStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task {{methodName}}()
                {
                    try
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return DoSomeAsynchronousWorkAsync({{variableName}});
                    }
                    catch (ArgumentException)
                    {
                        return Task.CompletedTask;
                    }
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task {{methodName}}()
                {
                    try
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        await DoSomeAsynchronousWorkAsync({{variableName}});
                    }
                    catch (ArgumentException)
                    {
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some Task inside a using-declaration scope in a non-terminal try-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInsideAUsingDeclarationScopeInANonTerminalTryStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task {{methodName}}()
                {
                    try
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return DoSomeAsynchronousWorkAsync({{variableName}});
                    }
                    catch (ArgumentException)
                    {
                    }

                    if (long.MinValue > 0)
                    {
                    }
                    return Task.CompletedTask;
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task {{methodName}}()
                {
                    try
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        await DoSomeAsynchronousWorkAsync({{variableName}});
                        return;
                    }
                    catch (ArgumentException)
                    {
                    }

                    if (long.MinValue > 0)
                    {
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some generic-Task inside a using-declaration scope in a try-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskInsideAUsingDeclarationScopeInATryStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task<long> {{methodName}}()
                {
                    try
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }
                    catch (ArgumentException)
                    {
                        return Task.FromResult(long.MaxValue);
                    }
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task<long> {{methodName}}()
                {
                    try
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return await DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }
                    catch (ArgumentException)
                    {
                        return long.MaxValue;
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some Task not inside a using-declaration scope in a terminal try-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInATerminalTryStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    try
                    {
                        return DoSomeAsynchronousWorkAsync(null);
                    }
                    catch (ArgumentException)
                    {
                        return Task.CompletedTask;
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some Task not inside a using-declaration scope in a non-terminal try-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInANonTerminalTryStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    try
                    {
                        return DoSomeAsynchronousWorkAsync(null);
                    }
                    catch (ArgumentException)
                    {
                    }

                    return Task.CompletedTask;
                }|}
            """);
    }

    [Given("a non-async method that returns some generic-Task not inside a using-declaration scope in a try-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskNotInsideAUsingDeclarationScopeInATryStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    try
                    {
                        return DoSomeAsynchronousWorkAndGetPositionAsync(null);
                    }
                    catch (ArgumentException)
                    {
                        return Task.FromResult(long.MaxValue);
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns a completed Task inside a using-declaration scope in a terminal try-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInsideAUsingDeclarationScopeInATerminalTryStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    try
                    {
                        using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log");
                        return Task.CompletedTask;
                    }
                    catch (ArgumentException)
                    {
                        return Task.CompletedTask;
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns a completed Task inside a using-declaration scope in a non-terminal try-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInsideAUsingDeclarationScopeInANonTerminalTryStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    try
                    {
                        using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log");
                        return Task.CompletedTask;
                    }
                    catch (ArgumentException)
                    {
                    }

                    return Task.CompletedTask;
                }|}
            """);
    }

    [Given("a non-async method that returns a completed generic-Task inside a using-declaration scope in a try-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedGenericTaskInsideAUsingDeclarationScopeInATryStatement()
    {
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    try
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return Task.FromResult({{variableName}}.Position);
                    }
                    catch (ArgumentException)
                    {
                        return Task.FromResult(long.MaxValue);
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some Task inside a using-declaration scope in a terminal catch-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInsideAUsingDeclarationScopeInATerminalCatchStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task {{methodName}}()
                {
                    try
                    {
                        return Task.CompletedTask;
                    }
                    catch (ArgumentException)
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
                    try
                    {
                    }
                    catch (ArgumentException)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        await DoSomeAsynchronousWorkAsync({{variableName}});
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some Task inside a using-declaration scope in a non-terminal catch-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInsideAUsingDeclarationScopeInANonTerminalCatchStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task {{methodName}}()
                {
                    long x = 0;
                    try
                    {
                        if (long.MaxValue > 0)
                        {
                            x = long.MaxValue;
                        }
                    }
                    catch (ArgumentException)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return DoSomeAsynchronousWorkAsync({{variableName}});
                    }

                    if (long.MinValue > 0)
                    {
                        x = long.MaxValue;
                    }
                    return Task.CompletedTask;
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task {{methodName}}()
                {
                    long x = 0;
                    try
                    {
                        if (long.MaxValue > 0)
                        {
                            x = long.MaxValue;
                        }
                    }
                    catch (ArgumentException)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        await DoSomeAsynchronousWorkAsync({{variableName}});
                        return;
                    }

                    if (long.MinValue > 0)
                    {
                        x = long.MaxValue;
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some generic-Task inside a using-declaration scope in a catch-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskInsideAUsingDeclarationScopeInACatchStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task<long> {{methodName}}()
                {
                    long x = 0;
                    try
                    {
                        if (long.MaxValue > 0)
                        {
                            x = long.MaxValue;
                        }
                    }
                    catch (ArgumentException)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }

                    return Task.FromResult(x);
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task<long> {{methodName}}()
                {
                    long x = 0;
                    try
                    {
                        if (long.MaxValue > 0)
                        {
                            x = long.MaxValue;
                        }
                    }
                    catch (ArgumentException)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return await DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }

                    return x;
                }|}
            """);
    }

    [Given("a non-async method that returns some Task not inside a using-declaration scope in a terminal catch-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInATerminalCatchStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    try
                    {
                        return Task.CompletedTask;
                    }
                    catch (ArgumentException)
                    {
                        return DoSomeAsynchronousWorkAsync(null);
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns some Task not inside a using-declaration scope in a non-terminal catch-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskNotInsideAUsingDeclarationScopeInANonTerminalCatchStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    long x = 0;
                    try
                    {
                        if (long.MaxValue > 0)
                        {
                            x = long.MaxValue;
                        }
                    }
                    catch (ArgumentException)
                    {
                        return DoSomeAsynchronousWorkAsync(null);
                    }

                    return Task.CompletedTask;
                }|}
            """);
    }

    [Given("a non-async method that returns some generic-Task not inside a using-declaration scope in a catch-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskNotInsideAUsingDeclarationScopeInACatchStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    long x = 0;
                    try
                    {
                        if (long.MaxValue > 0)
                        {
                            x = long.MaxValue;
                        }
                    }
                    catch (ArgumentException)
                    {
                        return DoSomeAsynchronousWorkAndGetPositionAsync(null);
                    }

                    return Task.FromResult(x);
                }|}
            """);
    }

    [Given("a non-async method that returns a completed Task inside a using-declaration scope in a terminal catch-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInsideAUsingDeclarationScopeInATerminalCatchStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    try
                    {
                        return Task.CompletedTask;
                    }
                    catch (ArgumentException)
                    {
                        using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log");
                        return Task.CompletedTask;
                    }
                }|}
            """);
    }

    [Given("a non-async method that returns a completed Task inside a using-declaration scope in a non-terminal catch-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInsideAUsingDeclarationScopeInANonTerminalCatchStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    long x = 0;
                    try
                    {
                        if (long.MaxValue > 0)
                        {
                            x = long.MaxValue;
                        }
                    }
                    catch (ArgumentException)
                    {
                        using FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log");
                        return Task.CompletedTask;
                    }

                    return Task.CompletedTask;
                }|}
            """);
    }

    [Given("a non-async method that returns a completed generic-Task inside a using-declaration scope in a catch-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedGenericTaskInsideAUsingDeclarationScopeInACatchStatement()
    {
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task<long> {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    try
                    {
                        return Task.FromResult(long.MaxValue);
                    }
                    catch (ArgumentException)
                    {
                        using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                        return Task.FromResult({{variableName}}.Position);
                    }
                }|}
            """);
    }
}
