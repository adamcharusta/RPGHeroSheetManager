using RPGHeroSheetManagerAPI.AuthService.Infrastructure;
using RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth;
using RPGHeroSheetManagerAPI.Infrastructure.Logger;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilogLogging("AuthService");

try
{
    builder.Services
        .AddInfrastructureServices(builder.Configuration)
        .AddApplicationServices()
        .AddWebServices();

    var app = builder.Build();

    await app.UseInfrastructureServicesAsync();
    app.UseWebServices();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    builder.Host.DisposeSerilogLogging();
}


public partial class Program;
