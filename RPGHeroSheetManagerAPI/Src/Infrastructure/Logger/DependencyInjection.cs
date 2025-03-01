using Serilog.Events;

namespace RPGHeroSheetManagerAPI.Infrastructure.Logger;

public static class DependencyInjection
{
    public static IHostBuilder AddSerilogLogging(this IHostBuilder hostBuilder, string serviceName)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.WithProperty("ServiceName", serviceName)
            .WriteTo.Console(
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Log.Logger = logger;

        hostBuilder.UseSerilog(logger);

        return hostBuilder;
    }

    public static void DisposeSerilogLogging(this IHostBuilder host)
    {
        Log.CloseAndFlush();
    }
}
