using RPGHeroSheetManagerAPI.Infrastructure.Application.Behaviors;

namespace RPGHeroSheetManagerAPI.Infrastructure.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationInfrastructure(this IServiceCollection services,
        bool useMediatr = true)
    {
        var assembly = Assembly.GetCallingAssembly();

        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);

        if (useMediatr)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);

                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            });
        }

        return services;
    }
}
