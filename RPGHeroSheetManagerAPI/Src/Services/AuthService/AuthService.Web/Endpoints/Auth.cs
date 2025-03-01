using MediatR;
using RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth.Auth.Commands.RegisterUserCommand;
using RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth.Auth.Dtos;
using RPGHeroSheetManagerAPI.Infrastructure.Web;

namespace RPGHeroSheetManagerAPI.AuthService.Infrastructure.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(RegisterUser, "register");
    }

    private Task<AuthResponseMessageDto> RegisterUser(ISender sender, RegisterUserCommand command)
    {
        return sender.Send(command);
    }
}
