using Microsoft.Extensions.DependencyInjection;
using RPGHeroSheetManagerAPI.Infrastructure.Application;

namespace RPGHeroSheetManagerAPI.Dnd5eService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddApplicationInfrastructure();
        return services;
    }
}
