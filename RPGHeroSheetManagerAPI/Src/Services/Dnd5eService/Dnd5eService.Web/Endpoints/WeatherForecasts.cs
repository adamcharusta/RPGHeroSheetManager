using RPGHeroSheetManagerAPI.Dnd5eService.Application.WeatherForecasts.Dtos;
using RPGHeroSheetManagerAPI.Dnd5eService.Application.WeatherForecasts.Queries.GetWeatherForecastsQuery;
using RPGHeroSheetManagerAPI.Infrastructure.Web;

namespace RPGHeroSheetManagerAPI.Dnd5eService.Application.Endpoints;

public class WeatherForecasts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetWeatherForecasts)
            .MapGet(GetWeatherForecastsBySize, "{size}");
    }

    private Task<IEnumerable<WeatherForecast>> GetWeatherForecastsBySize(ISender sender, int size)
    {
        return sender.Send(new GetWeatherForecastsBySizeQuery(size));
    }

    private Task<IEnumerable<WeatherForecast>> GetWeatherForecasts(ISender sender)
    {
        return sender.Send(new GetWeatherForecastsQuery());
    }
}
