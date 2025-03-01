using RPGHeroSheetManagerAPI.Dnd5eService.Application.WeatherForecasts.Dtos;

namespace RPGHeroSheetManagerAPI.Dnd5eService.Application.WeatherForecasts.Queries.GetWeatherForecastsQuery;

public record GetWeatherForecastsBySizeQuery(int Size) : IRequest<IEnumerable<WeatherForecast>>;

public class
    GetWeatherForecastsBySizeHandler : IRequestHandler<GetWeatherForecastsBySizeQuery, IEnumerable<WeatherForecast>>
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public async Task<IEnumerable<WeatherForecast>> Handle(GetWeatherForecastsBySizeQuery request,
        CancellationToken cancellationToken)
    {
        var rng = new Random();

        var result = Enumerable.Range(1, request.Size).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        });

        return await Task.FromResult(result);
    }
}
