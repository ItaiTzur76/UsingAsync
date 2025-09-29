# UsingAsync for C#
![UsingAsync logo](https://raw.githubusercontent.com/ItaiTzur76/UsingAsync/main/Logo.png)
## Project Description
**UsingAsync** is a Rosyln-powered analyzer for warning C# developers about non-`async` `Task`-returning methods that might access disposed resources. It also contains a suggested code-fix for such cases.

## Motivation
The following case-study shows a scenario with a pitfall that the **UsingAsync** analyzer can help prevent. But first, a short introduction:

### Async Methods
Declaring a `Task`-returning method as `async` turns it into a "state machine" that can be viewed as a series of synchronous blocks separated with the `await` keyword. It might look something like this:
```csharp
public async Task DoSomethingAsync()
{
    // synchronous block 0 statements
    // ⋮
    await someTask1;
    // synchronous block 1 statements
    // ⋮
    await someTask2;
    // synchronous block 2 statements

    // ⋮

    // synchronous block N-1 statements
    // ⋮
    await someTaskN;
    // synchronous block N statements
    // ⋮
}
```
Of course, this is a very simplified, linear view; it can just as well contain conditionals and loops with `await` keywords of their own.

In case there is only one instance of the `await` keyword, at the end of the method, and nothing is performed after it, then it is redundant, because it does not serve as a "separator" between two synchronous blocks. In other words, the method:
```csharp
public async Task DoSomethingAsync()
{
    // synchronous block of statements containing no instance of the "await" keyword
    // ⋮
    await someTask;
}
```
can be replaced with the method:
```csharp
public Task DoSomethingAsync()
{
    // synchronous block of statements containing no instance of the "await" keyword
    // ⋮
    return someTask;
}
```
Similarly, the method:
```csharp
public async Task<TReturnType> DoSomethingAsync()
{
    // synchronous block of statements containing no instance of the "await" keyword
    // ⋮
    return await someTaskReturningTReturnType;
}
```
can be replaced with the method:
```csharp
public Task<TReturnType> DoSomethingAsync()
{
    // synchronous block of statements containing no instance of the "await" keyword
    // ⋮
    return someTaskReturningTReturnType;
}
```
These replacements also save the overhead of constructing the "state machine" so they might even be the better option. Any methods that `await DoSomethingAsync()` would simply end up awaiting the task that `DoSomethingAsync()` returns.

But there is a catch...

### Using Declarations
A `using` declaration is syntactic sugar for a `using` statement that spans to the end of its scope. For example:
```csharp
{
    // block 1 of statements
    // ⋮
    using var variableName = SomeDisposable();
    // block 2 of statements
    // ⋮
}
```
gets translated into:
```csharp
{
    // block 1 of statements
    // ⋮
    using (var variableName = SomeDisposable())
    {
        // block 2 of statements
        // ⋮
    }
}
```
which, in turn, gets translated into:
```csharp
{
    // block 1 of statements
    // ⋮
    var variableName = SomeDisposable();
    try
    {
        // block 2 of statements
        // ⋮
    }
    finally
    {
        variableName.Dispose();
    }
}
```
Bear with me, we're almost there...

### The Pitfall
Let's say you have a class that looks like this:
```csharp
public sealed class FileStreamWrapper(string path)
{
    private readonly FileStream _fileStream = new(path, FileMode.Open, FileAccess.Read);

    public long Position => _fileStream.Position;

    // Perhaps some other members
    // ⋮
}
```
This class just wraps a `FileStream` when a `string path` is passed to its constructor.

Now let's say you also have the following methods somewhere, which provide the `FileStream` as a `static` method:
```csharp
    public static Task<long> DoSomeAsynchronousWorkOnMyFileAndGetPositionAsync()
    {
        FileStreamWrapper fileStreamWrapper = new(@"C:\Windows\comsetup.log"); // or whatever other file that exists on your machine and is currently not in use
        return DoSomeAsynchronousWorkAndGetPositionAsync(fileStreamWrapper);
    }

    private static async Task<long> DoSomeAsynchronousWorkAndGetPositionAsync(FileStreamWrapper fileStreamWrapper)
    {
        await Task.Delay(TimeSpan.FromSeconds(1)); // this mocks some long asynchronous work
        return fileStreamWrapper.Position;
    }
```

Then at some point, someone comes along and reminds you that `FileStream` is disposable. So you decide to do the right thing and make your class (which contains a `FileStream` as a field) disposable, too:
```csharp
public sealed class FileStreamWrapper(string path) : IDisposable
{
    private readonly FileStream _fileStream = new(path, FileMode.Open, FileAccess.Read);

    public void Dispose()
    {
        _fileStream.Dispose();
    }

    public long Position => _fileStream.Position;

    // Perhaps some other members
    // ⋮
}
```
You also remember to add the `using` keyword to the allocation of the new `FileStreamWrapper` object in the `DoSomeAsynchronousWorkOnMyFileAndGetPositionAsync()` method:
```csharp
    public static Task<long> DoSomeAsynchronousWorkOnMyFileAndGetPositionAsync()
    {
        using FileStreamWrapper fileStreamWrapper = new(@"C:\Windows\comsetup.log"); // or whatever other file that exists on your machine and is currently not in use
        return DoSomeAsynchronousWorkAndGetPositionAsync(fileStreamWrapper);
    }
```
Having done all those sensible things, if you now `await DoSomeAsynchronousWorkOnMyFileAndGetPositionAsync()` you will end up with a runtime error that reads something like: `Cannot access a closed file.`

Go ahead and try it; I'll wait here until you're ready.

### Warum ich?
To see why this happens, we will remind ourselves that a `using`-declaration is syntactic sugar. So let's break it down into its `using`-statement form:
```csharp
    public static Task<long> DoSomeAsynchronousWorkOnMyFileAndGetPositionAsync()
    {
        using (FileStreamWrapper fileStreamWrapper = new(@"C:\Windows\comsetup.log")) // or whatever other file that exists on your machine and is currently not in use
        {
            return DoSomeAsynchronousWorkAndGetPositionAsync(fileStreamWrapper);
        }
    }
```
and then further down into its `try`-statement form:
```csharp
    public static Task<long> DoSomeAsynchronousWorkOnMyFileAndGetPositionAsync()
    {
        FileStreamWrapper fileStreamWrapper = new(@"C:\Windows\comsetup.log"); // or whatever other file that exists on your machine and is currently not in use
        try
        {
            return DoSomeAsynchronousWorkAndGetPositionAsync(fileStreamWrapper);
        }
        finally
        {
            fileStreamWrapper.Dispose();
        }
    }
```
And now you see it - `DoSomeAsynchronousWorkAndGetPositionAsync(fileStreamWrapper)` returns an incomplete task, but just before doing the `return` operation, `fileStreamWrapper.Dispose()` is invoked in the `finally` block. So it turns out that the internal call to `DoSomeAsynchronousWorkAndGetPositionAsync(fileStreamWrapper)` was *not* the last thing that happens in this method - a call to `.Dispose()` happens after it, which causes the runtime to dispose of the (file) resource that the running Task might still be using.

And this kind of runtime error will not be reported by the IDE!

The **UsingAsync** analyzer warns about such cases during design-time and therefore prevents these potential bugs during runtime. It also suggests making this method `async`, which means that the `DoSomeAsynchronousWorkAndGetPositionAsync(fileStreamWrapper)` will be properly awaited, so the resource it uses can be safely disposed.

## Install and Setup
To use the **UsingAsync** analyzer, include [the UsingAsync NuGet package](https://www.NuGet.org/packages/UsingAsync) in your C# project.

If you [manage your packages centrally](https://DevBlogs.microsoft.com/dotnet/introducing-central-package-management) you can do that manually by adding the following to the `ItemGroup` element containing all package versions in your `Directory.Packages.props` file:
```xml
<PackageVersion Include="UsingAsync" Version="1.1.0" />
```
You can then have **UsingAsync** analyze all projects in your solution by adding the following to an `ItemGroup` element in [your Directory.Build.props file](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory):
```xml
<PackageReference Include="UsingAsync" PrivateAssets="all" IncludeAssets="analyzers" />
```

## Contribute
If you find a problem or bug, or if you have a question or suggestion, please contact me using the [Contact owners &rarr;](https://www.NuGet.org/packages/UsingAsync#:~:text=Owners-,Contact%20owners%20%E2%86%92) link on [the NuGet page](https://www.NuGet.org/packages/UsingAsync).

Happy coding!