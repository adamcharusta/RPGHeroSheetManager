namespace RPGHeroSheetManagerAPI.Infrastructure.Auth;

public class Roles
{
    public const string User = "User";
    public const string Admin = "Admin";
    public const string SuperAdmin = "SuperAdmin";

    public static List<string> GetAllRoles { get; } = typeof(Roles)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(fi => fi is { IsLiteral: true, IsInitOnly: false })
        .Select(fi => fi.GetRawConstantValue()?.ToString()!)
        .ToList();
}
