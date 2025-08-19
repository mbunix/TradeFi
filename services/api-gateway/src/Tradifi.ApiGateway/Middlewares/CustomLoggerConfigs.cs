using Serilog;
using Serilog.Events;

using Serilog.Sinks.SystemConsole.Themes;

namespace ApiGateway.Middlewares;
public static class CustomLoggerConfigs
{
    public static void AddCustomLogger(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", configuration["Serilog:Properties:ApplicationName"])
            .Enrich.WithProperty("Environment", configuration["Serilog:Properties:Environment"])
            .Enrich.WithProperty("ApplicationVersion", configuration["KernelServiceVersion"])
            .WriteTo.Seq(configuration["Serilog:Properties:SeqServerUrl"])
            .WriteTo.Console(theme: AnsiConsoleTheme.Grayscale)
            .WriteTo.File(@"/app/kernel/log.txt", rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: null, retainedFileCountLimit: null,
                shared: true
                , outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateBootstrapLogger();
        
        builder.Host.UseSerilog(logger);
    }
}