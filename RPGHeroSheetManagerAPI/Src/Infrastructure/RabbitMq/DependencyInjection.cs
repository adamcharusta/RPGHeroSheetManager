using Microsoft.Extensions.Configuration;

namespace RPGHeroSheetManagerAPI.Infrastructure.RabbitMq;

public static class DependencyInjection
{
    public static IServiceCollection AddRabbitMqServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var rabbitMqSettings = new RabbitMqSettings();

        configuration.GetSection("RabbitMq").Bind(rabbitMqSettings);

        Guard.Against.Null(rabbitMqSettings.UserName, nameof(rabbitMqSettings.UserName));
        Guard.Against.Null(rabbitMqSettings.Password, nameof(rabbitMqSettings.Password));
        Guard.Against.Null(rabbitMqSettings.Host, nameof(rabbitMqSettings.Host));
        Guard.Against.OutOfRange(rabbitMqSettings.Port, nameof(rabbitMqSettings.Port), 2,
            int.MaxValue);

        services.AddSingleton(rabbitMqSettings);
        services.AddScoped<IRabbitMqService, RabbitMqService>();

        return services;
    }
}
