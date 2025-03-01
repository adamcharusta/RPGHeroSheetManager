using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RPGHeroSheetManagerAPI.Infrastructure.Application;
using RPGHeroSheetManagerAPI.Infrastructure.Application.Behaviors;

namespace RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddApplicationInfrastructure(false);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.UnhandledExceptionBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.PerformanceBehavior<,>));
        });

        return services;
    }
}
