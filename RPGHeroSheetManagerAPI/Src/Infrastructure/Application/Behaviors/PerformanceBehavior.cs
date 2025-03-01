using System.Diagnostics;

namespace RPGHeroSheetManagerAPI.Infrastructure.Application.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
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
            var userId = httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault();
            var userName = httpContextAccessor.HttpContext?.Request.Headers["X-User-Name"].FirstOrDefault() ??
                           "Unknown User";

            Log.Warning(
                "RPGHeroSheetManagerAPI Long Running Request: {Name} ({ElapsedMilliseconds} ms) {@UserId} {@UserName} {@Request}",
                requestName, elapsedMilliseconds, userId ?? "Anonymous", userName, request);
        }

        return response;
    }
}
