using EmployeeApp.Exceptions;
using EmployeeApp.Logging;
using EmployeeApp.Services;
using EmployeeApp.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureSerilog()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IConsole, ConsoleWrapper>();
                    services.AddScoped<IFileSystem, FileSystemWrapper>();
                    services.AddSingleton<IEmployeeService, EmployeeService>();
                    services.AddSingleton<EmployeeStatistics>();
                    services.AddSingleton<GlobalExceptionHandler>(provider =>
                        new GlobalExceptionHandler(Log.Logger));
                    services.AddSingleton<Menu>();

                    // Add a service to track instances
                    services.AddSingleton<InstanceTracker>();
                })
                .Build();

            using (var scope1 = host.Services.CreateScope())
            {
                var tracker1 = scope1.ServiceProvider.GetRequiredService<InstanceTracker>();
                var console1 = scope1.ServiceProvider.GetRequiredService<IConsole>();
                var fileSystem1 = scope1.ServiceProvider.GetRequiredService<IFileSystem>();
                var employeeService1 = scope1.ServiceProvider.GetRequiredService<IEmployeeService>();

                tracker1.Track("Scope 1", console1, fileSystem1, employeeService1);

                // Get services again in the same scope
                var console1a = scope1.ServiceProvider.GetRequiredService<IConsole>();
                var fileSystem1a = scope1.ServiceProvider.GetRequiredService<IFileSystem>();
                var employeeService1a = scope1.ServiceProvider.GetRequiredService<IEmployeeService>();
                tracker1.Track("Scope 1 - Second Resolve", console1a, fileSystem1a, employeeService1a);
            }

            using (var scope2 = host.Services.CreateScope())
            {
                var tracker2 = scope2.ServiceProvider.GetRequiredService<InstanceTracker>();
                var console2 = scope2.ServiceProvider.GetRequiredService<IConsole>();
                var fileSystem2 = scope2.ServiceProvider.GetRequiredService<IFileSystem>();
                var employeeService2 = scope2.ServiceProvider.GetRequiredService<IEmployeeService>();

                tracker2.Track("Scope 2", console2, fileSystem2, employeeService2);
            }

            // Continue with your menu
            using var menuScope = host.Services.CreateScope();
            var menu = menuScope.ServiceProvider.GetRequiredService<Menu>();
            await menu.ShowMenu();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

public class InstanceTracker
{
    private readonly ILogger _logger;

    public InstanceTracker(ILogger logger)
    {
        _logger = logger;
    }

    public void Track(string scopeName, params object[] services)
    {
        _logger.Information($"--- {scopeName} ---");
        foreach (var service in services)
        {
            _logger.Information($"{service.GetType().Name}: {service.GetHashCode()}");
        }
    }
}