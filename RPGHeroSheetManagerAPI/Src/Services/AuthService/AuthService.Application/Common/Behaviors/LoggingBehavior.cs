using MediatR.Pipeline;

namespace RPGHeroSheetManagerAPI.Infrastructure.Application.Behaviors;

public class LoggingBehavior<TRequest>(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var httpContext = httpContextAccessor.HttpContext;

        string? userId = null;
        var userName = "Unknown User";

        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            userId = userManager.GetUserId(httpContext.User);
            var user = await userManager.GetUserAsync(httpContext.User);
            userName = user?.UserName ?? "Unknown User";
        }

        if (!string.IsNullOrEmpty(userId))
        {
            Log.Information("RPGHeroSheetManagerAPI Request: {Name} {@UserId} {@UserName} {@Request}",
                requestName, userId, userName, SanitizeHelpers.SanitizeLoggerRequest(request));
        }
        else
        {
            Log.Information("RPGHeroSheetManagerAPI Request (No User): {Name} {@Request}",
                requestName, SanitizeHelpers.SanitizeLoggerRequest(request));
        }

        await Task.CompletedTask;
    }
}
