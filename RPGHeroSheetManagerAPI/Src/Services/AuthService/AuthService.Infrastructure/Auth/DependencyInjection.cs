using System.Text;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RPGHeroSheetManagerAPI.AuthService.Domain.Entities;
using RPGHeroSheetManagerAPI.AuthService.Infrastructure.Data;

namespace RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth;

internal static class DependencyInjection
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddIdentity<User, IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(opt =>
        {
            opt.SignIn.RequireConfirmedEmail = true;
        });

        var authenticationSettings = new AuthenticationSettings();

        config.GetSection("Authentication").Bind(authenticationSettings);

        Guard.Against.Null(authenticationSettings.JwtKey, nameof(authenticationSettings.JwtKey));
        Guard.Against.NullOrEmpty(authenticationSettings.JwtIssuer, nameof(authenticationSettings.JwtIssuer));
        Guard.Against.OutOfRange(authenticationSettings.JwtExpireDays, nameof(authenticationSettings.JwtExpireDays), 2,
            int.MaxValue);

        services.AddSingleton(authenticationSettings);
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = "Bearer";
            option.DefaultScheme = "Bearer";
            option.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = authenticationSettings.JwtIssuer,
                ValidAudience = authenticationSettings.JwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
            };
        });

        services.AddAuthorization();

        var superAdminSettings = new SuperAdminSettings();

        config.GetSection("SuperAdmin").Bind(superAdminSettings);

        Guard.Against.Null(superAdminSettings.UserName, nameof(superAdminSettings.UserName));
        Guard.Against.Null(superAdminSettings.Email, nameof(superAdminSettings.Email));
        Guard.Against.Null(superAdminSettings.Password, nameof(superAdminSettings.Password));


        services.AddSingleton(superAdminSettings);

        return services;
    }

    public static WebApplication UseAuthServices(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
