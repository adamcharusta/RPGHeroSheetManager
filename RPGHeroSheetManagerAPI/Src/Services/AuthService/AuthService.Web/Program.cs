using RPGHeroSheetManagerAPI.Infrastructure.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebInfrastructure();

var app = builder.Build();

app.UseWebInfrastructure();

app.Run();
