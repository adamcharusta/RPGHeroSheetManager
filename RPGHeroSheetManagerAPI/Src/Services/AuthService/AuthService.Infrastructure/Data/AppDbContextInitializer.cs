using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RPGHeroSheetManagerAPI.AuthService.Domain.Entities;
using RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth;
using RPGHeroSheetManagerAPI.Infrastructure.Auth;
using Serilog;

namespace RPGHeroSheetManagerAPI.AuthService.Infrastructure.Data;

public static class InitializerExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();

        await initializer.InitialiseAsync();

        await initializer.SeedAsync();
    }
}

public class AppDbContextInitializer(
    AppDbContext context,
    UserManager<User> userManager,
    RoleManager<IdentityRole<int>> roleManager,
    SuperAdminSettings superAdminSettings)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedRoles();
            await SeedSuperAdmin();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedRoles()
    {
        foreach (var roleName in Roles.GetAllRoles)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = roleName });
            }
        }
    }

    private async Task SeedSuperAdmin()
    {
        var normalizedEmail = userManager.NormalizeEmail(superAdminSettings.Email!);
        var normalizedUserName = userManager.NormalizeName(superAdminSettings.UserName!);

        var existingSuperAdmin = await userManager.GetUsersInRoleAsync(Roles.SuperAdmin);
        if (existingSuperAdmin.Any())
        {
            Log.Information("{SuperAdmin} already exists ({UserEmail}), skip creating.",
                Roles.SuperAdmin,
                existingSuperAdmin.First().Email);
            return;
        }

        var existingUser = await userManager.Users
            .FirstOrDefaultAsync(
                u => u.NormalizedEmail == normalizedEmail || u.NormalizedUserName == normalizedUserName);

        if (existingUser != null)
        {
            Log.Warning("User {UserEmail} already exists, but is not a {SuperAdmin}. We do not overwrite the role.",
                Roles.SuperAdmin,
                existingUser.Email);
            return;
        }

        var user = new User
        {
            Email = superAdminSettings.Email,
            UserName = superAdminSettings.UserName,
            EmailConfirmed = true,
            NormalizedEmail = normalizedEmail,
            NormalizedUserName = normalizedUserName
        };

        var result = await userManager.CreateAsync(user, superAdminSettings.Password!);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, Roles.SuperAdmin);
            Log.Information("{SuperAdmin} has been successfully created: {UserEmail}", Roles.SuperAdmin,
                user.Email);
        }
        else
        {
            Log.Error("Error creating {SuperAdmin}: {Errors}", Roles.SuperAdmin,
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
