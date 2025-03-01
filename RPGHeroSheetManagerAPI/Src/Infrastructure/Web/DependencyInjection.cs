namespace RPGHeroSheetManagerAPI.Infrastructure.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebInfrastructure(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddExceptionHandler<CustomExceptionHandler>();
        return services;
    }

    public static WebApplication UseWebInfrastructure(this WebApplication app)
    {
        app.UseHttpsRedirection();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        app.UseExceptionHandler(options => { });
        app.MapEndpoints();
        return app;
    }
}
