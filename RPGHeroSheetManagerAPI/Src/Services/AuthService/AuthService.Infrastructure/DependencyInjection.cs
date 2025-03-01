using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth;
using RPGHeroSheetManagerAPI.AuthService.Infrastructure.Data;
using RPGHeroSheetManagerAPI.Infrastructure.RabbitMq;

namespace RPGHeroSheetManagerAPI.AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddDataServices(configuration);
        services.AddAuthServices(configuration);
        services.AddRabbitMqServices(configuration);
        return services;
    }

    public static async Task<WebApplication> UseInfrastructureServicesAsync(this WebApplication app)
    {
        await app.UseDataServicesAsync();
        app.UseAuthServices();
        return app;
    }
}
