using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RPGHeroSheetManagerAPI.AuthService.Infrastructure.Data;

internal static class DependencyInjection
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, ConfigurationManager config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");


        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>();

        services.AddDbContext<AppDbContext>((sp, opt) =>
        {
            opt.UseSqlServer(connectionString);
        });

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<AppDbContextInitializer>();

        return services;
    }


    public static async Task<WebApplication> UseDataServicesAsync(this WebApplication app)
    {
        await app.InitialiseDatabaseAsync();

        return app;
    }
}
