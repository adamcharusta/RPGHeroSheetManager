using Newtonsoft.Json;

namespace RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth.Common.Extensions;

public static class SanitizeHelpers
{
    public static object? SanitizeLoggerRequest<TRequest>(TRequest request) where TRequest : notnull
    {
        var requestJson = JsonConvert.SerializeObject(request);
        var requestDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestJson);

        if (requestDict != null)
        {
            foreach (var key in requestDict.Keys.ToList())
            {
                if (key.ToLower().Contains("password"))
                {
                    requestDict[key] = "***";
                }
            }
        }

        return requestDict;
    }
}
