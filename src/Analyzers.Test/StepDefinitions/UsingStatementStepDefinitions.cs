using Bogus;

namespace UsingAsync.Analyzers.Test;

[Binding]
internal sealed class UsingStatementStepDefinitions(SharedStepsContext sharedStepsContext)
{
    private readonly SharedStepsContext _sharedStepsContext = sharedStepsContext;

    [Given("a non-async method that returns some Task in a terminal using-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInATerminalUsingStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task {{methodName}}()
                {
                    using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                    {
                        return DoSomeAsynchronousWorkAsync({{variableName}});
                    }
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task {{methodName}}()
                {
                    using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                    {
                        await DoSomeAsynchronousWorkAsync({{variableName}});
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns some Task in a terminal using-statement")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeTaskInATerminalUsingStatement()
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
                        using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                        {
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
                        using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                        {
                            await DoSomeAsynchronousWorkAsync({{variableName}});
                        }
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns some Task in a non-terminal using-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeTaskInANonTerminalUsingStatement()
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
                        using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                        {
                            return DoSomeAsynchronousWorkAsync({{variableName}});
                        }
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
                    if (long.MaxValue > 0)
                    {
                        using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                        {
                            await DoSomeAsynchronousWorkAsync({{variableName}});
                            return;
                        }
                    }

                    if (long.MinValue > 0)
                    {
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns some Task in a non-terminal using-statement")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeTaskInANonTerminalUsingStatement()
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
                        if (long.MaxValue > 0)
                        {
                            using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                            {
                                return DoSomeAsynchronousWorkAsync({{variableName}});
                            }
                        }

                        if (long.MinValue > 0)
                        {
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
                        if (long.MaxValue > 0)
                        {
                            using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                            {
                                await DoSomeAsynchronousWorkAsync({{variableName}});
                                return;
                            }
                        }

                        if (long.MinValue > 0)
                        {
                        }
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns some generic-Task in a using-statement")]
    internal void GivenANonAsyncMethodThatReturnsSomeGenericTaskInAUsingStatement()
    {
        var accessModifier = new Faker().RandomAccessModifier();
        var methodName = _sharedStepsContext.GenerateAsyncMethodName();
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static Task<long> {{methodName}}()
                {
                    using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                    {
                        return DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }
                }|}
            """);

        _sharedStepsContext.FixedSource = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{accessModifier}} static async Task<long> {{methodName}}()
                {
                    using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                    {
                        return await DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns some generic-Task in a using-statement")]
    internal void GivenANonAsyncLocalFunctionThatReturnsSomeGenericTaskInAUsingStatement()
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
                        using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                        {
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
                        using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                        {
                            return await DoSomeAsynchronousWorkAndGetPositionAsync({{variableName}});
                        }
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns a completed Task in a terminal using-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInATerminalUsingStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    using (FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log"))
                    {
                        return Task.CompletedTask;
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns a completed Task in a terminal using-statement")]
    internal void GivenANonAsyncLocalFunctionThatReturnsACompletedTaskInATerminalUsingStatement()
    {
        var functionName = _sharedStepsContext.GenerateAsyncMethodName();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {{new Faker().RandomAccessModifier()}} static void {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    {{functionName}}().Wait();

                    {|#0:static Task {{functionName}}()
                    {
                        using (FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log"))
                        {
                            return Task.CompletedTask;
                        }
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns a completed Task in a non-terminal using-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedTaskInANonTerminalUsingStatement()
    {
        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    if (long.MaxValue > 0)
                    {
                        using (FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log"))
                        {
                            return Task.CompletedTask;
                        }
                    }

                    return Task.CompletedTask;
                }|}
            """);
    }

    [Given("a non-async local function that returns a completed Task in a non-terminal using-statement")]
    internal void GivenANonAsyncLocalFunctionThatReturnsACompletedTaskInANonTerminalUsingStatement()
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
                            using (FileStreamWrapper {{SharedStepsContext.GenerateIdentifier()}} = new(@"C:\Windows\comsetup.log"))
                            {
                                return Task.CompletedTask;
                            }
                        }

                        return Task.CompletedTask;
                    }|}
                }
            """);
    }

    [Given("a non-async method that returns a completed generic-Task in a using-statement")]
    internal void GivenANonAsyncMethodThatReturnsACompletedGenericTaskInAUsingStatement()
    {
        var variableName = SharedStepsContext.GenerateIdentifier();

        _sharedStepsContext.Source = SharedStepDefinitions.BuildSource(
            $$"""
            {|#0:{{new Faker().RandomAccessModifier()}} static Task {{_sharedStepsContext.GenerateAsyncMethodName()}}()
                {
                    using FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log");
                    {
                        return Task.FromResult({{variableName}}.Position);
                    }
                }|}
            """);
    }

    [Given("a non-async local function that returns a completed generic-Task in a using-statement")]
    internal void GivenANonAsyncLocalFunctionThatReturnsACompletedGenericTaskInAUsingStatement()
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
                        using (FileStreamWrapper {{variableName}} = new(@"C:\Windows\comsetup.log"))
                        {
                            return Task.FromResult({{variableName}}.Position);
                        }
                    }|}
                }
            """);
    }
}
