using System.Diagnostics;

namespace RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(
    UserManager<User> userManager,
    IHttpContextAccessor httpContextAccessor
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();
        var response = await next();
        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
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

            Log.Warning(
                "RPGHeroSheetManagerAPI Long Running Request: {Name} ({ElapsedMilliseconds} ms) {@UserId} {@UserName} {@Request}",
                requestName, elapsedMilliseconds, userId ?? "Anonymous", userName,
                SanitizeHelpers.SanitizeLoggerRequest(request));
        }

        return response;
    }
}
