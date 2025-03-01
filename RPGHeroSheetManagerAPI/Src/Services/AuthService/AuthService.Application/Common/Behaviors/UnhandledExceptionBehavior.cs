namespace RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth.Common.Behaviors;

public class UnhandledExceptionBehavior<TRequest, TResponse>(
    UserManager<User> userManager,
    IHttpContextAccessor httpContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
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

            Log.Error(ex,
                "RPGHeroSheetManagerAPI Request: Unhandled Exception for Request {Name} {@UserId} {@UserName} {@Request}",
                requestName, userId ?? "Anonymous", userName, SanitizeHelpers.SanitizeLoggerRequest(request));

            throw;
        }
    }
}
