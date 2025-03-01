namespace RPGHeroSheetManagerAPI.Dnd5eService.Application.WeatherForecasts.Queries.GetWeatherForecastsQuery;

public class GetWeatherForecastsBySizeQueryValidator : AbstractValidator<GetWeatherForecastsBySizeQuery>
{
    public GetWeatherForecastsBySizeQueryValidator()
    {
        RuleFor(x => x.Size).GreaterThan(0).NotEmpty();
    }
}
