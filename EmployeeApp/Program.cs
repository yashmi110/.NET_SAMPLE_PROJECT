using System;
using System.Threading.Tasks;
using EmployeeApp;
using EmployeeApp.Data;
using EmployeeApp.Exceptions;
using EmployeeApp.Logging;
using EmployeeApp.Repositories;
using EmployeeApp.Services;
using EmployeeApp.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureSerilog()
                .ConfigureServices((context, services) =>
                {
                    // Configure DbContext
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    // Register repositories
                    services.AddScoped<IEmployeeRepository, EmployeeRepository>();
                    services.AddScoped<IDepartmentRepository, DepartmentRepository>();
                    services.AddScoped<IProjectRepository, ProjectRepository>();
                    services.AddScoped<IEmployeeProjectRepository, EmployeeProjectRepository>();

                    // Register services
                    services.AddScoped<IEmployeeService, EmployeeService>();
                    services.AddScoped<EmployeeStatistics>();

                    // Register infrastructure components
                    services.AddScoped<IConsole, ConsoleWrapper>();
                    services.AddSingleton<GlobalExceptionHandler>(provider =>
                        new GlobalExceptionHandler(Log.Logger));
                    services.AddSingleton<Menu>();
                })
                .Build();

            // Apply any pending migrations
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await context.Database.MigrateAsync();
            }

            // Run the application
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
