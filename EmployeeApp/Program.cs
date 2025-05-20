using EmployeeApp.Exceptions;
using EmployeeApp.Logging;
using EmployeeApp.Services;
using EmployeeApp.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureSerilog() 
                .ConfigureServices(services =>
                {
                   
                    services.AddSingleton<IConsole, ConsoleWrapper>();
                    services.AddSingleton<IFileSystem, FileSystemWrapper>();
                    services.AddSingleton<IEmployeeService, EmployeeService>();
                    services.AddSingleton<GlobalExceptionHandler>(provider =>
                        new GlobalExceptionHandler(Log.Logger));

                    services.AddSingleton<Menu>();
                })
                .Build();

            var menu = host.Services.GetRequiredService<Menu>();
            menu.ShowMenu();
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