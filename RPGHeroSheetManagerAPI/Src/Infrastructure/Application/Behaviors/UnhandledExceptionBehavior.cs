using Microsoft.Extensions.Logging;

namespace RPGHeroSheetManagerAPI.Infrastructure.Application.Behaviors;

public class UnhandledExceptionBehavior<TRequest, TResponse>(
    ILogger<TRequest> logger,
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
            var userId = httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault();

            logger.LogError(ex,
                "RPGHeroSheetManagerAPI Request: Unhandled Exception for Request {Name} {@UserId} {@Request}",
                requestName, userId ?? "Anonymous", request);

            throw;
        }
    }
}
