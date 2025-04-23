using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.Hosting;

namespace EmployeeApp.Logging
{

    public static class LoggerConfig
    {
        public static IHostBuilder ConfigureSerilog(this IHostBuilder builder)
        {
            return builder.UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
            });
        }
    }
}
