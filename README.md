# DaemonBuilder

## Scope

The DaemonBuilder helps with

* running a daemon
* adding a CLI to execute instead if args are present
* providing dependency injection in both from the start (Constructor)

## Usage

```csharp
static class Program
{
    static void Main(string[] args)
    {
        var executor = new DaemonBuilder()
            .ConfigureServices(ConfigureServices)
            .AddCli<MyCli>()
            .AddDaemon<MyDaemon>()
            .Build();

        executor
            .Run(args);
    }

    private static IServiceCollection ConfigureServices()
    {
        // Microsoft.Extensions.DependencyInjectioni
    }
}
```