using RPGHeroSheetManagerAPI.Infrastructure.Web;

namespace RPGHeroSheetManagerAPI.Dnd5eService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddWebInfrastructure();
        return services;
    }

    public static WebApplication UseWebServices(this WebApplication app)
    {
        app.UseWebInfrastructure();
        return app;
    }
}
