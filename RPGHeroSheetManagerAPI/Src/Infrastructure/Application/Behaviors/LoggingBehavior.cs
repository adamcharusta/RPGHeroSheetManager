using MediatR.Pipeline;

namespace RPGHeroSheetManagerAPI.Infrastructure.Application.Behaviors;

public class LoggingBehavior<TRequest>(IHttpContextAccessor httpContextAccessor) : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault();
        var userName = httpContextAccessor.HttpContext?.Request.Headers["X-User-Name"].FirstOrDefault() ??
                       "Unknown User";

        if (!string.IsNullOrEmpty(userId))
        {
            Log.Information("RPGHeroSheetManagerAPI Request: {Name} {@UserId} {@UserName} {@Request}",
                requestName, userId, userName, request);
        }
        else
        {
            Log.Information("RPGHeroSheetManagerAPI Request (No User): {Name} {@Request}",
                requestName, request);
        }

        await Task.CompletedTask;
    }
}
