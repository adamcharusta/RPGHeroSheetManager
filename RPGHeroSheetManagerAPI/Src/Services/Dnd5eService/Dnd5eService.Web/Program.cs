using RPGHeroSheetManagerAPI.Dnd5eService.Application;
using RPGHeroSheetManagerAPI.Infrastructure.Logger;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilogLogging("Dnd5eService");

try
{
    builder.Services
        .AddApplicationServices()
        .AddWebServices();

    var app = builder.Build();

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
