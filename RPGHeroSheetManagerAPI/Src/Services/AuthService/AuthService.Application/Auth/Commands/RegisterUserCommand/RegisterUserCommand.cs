using System.Text.Encodings.Web;
using RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth.Auth.Dtos;
using RPGHeroSheetManagerAPI.Infrastructure.Application.Exceptions;
using RPGHeroSheetManagerAPI.Infrastructure.Auth;
using RPGHeroSheetManagerAPI.Infrastructure.RabbitMq;
using RPGHeroSheetManagerAPI.Infrastructure.RabbitMq.Messages;

namespace RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth.Auth.Commands.RegisterUserCommand;

public record RegisterUserCommand : IRequest<AuthResponseMessageDto>
{
    public required string ConfirmEmail { get; init; }
    public required string ConfirmPassword { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Username { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<RegisterUserCommand, User>();
        }
    }
}

public class RegisterUserCommandHandler(IRabbitMqService rabbitMqService, UserManager<User> userManager, IMapper mapper)
    : IRequestHandler<RegisterUserCommand, AuthResponseMessageDto>
{
    public async Task<AuthResponseMessageDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = mapper.Map<User>(request);
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"Błąd rejestracji: {errors}");
        }

        await userManager.AddToRoleAsync(user, Roles.User);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = UrlEncoder.Default.Encode(token);

        var emailMessage = new ConfirmEmailMessage(user.Email!, encodedToken);

        await rabbitMqService.SendMessageAsync(emailMessage);

        return new AuthResponseMessageDto { Message = "Użytkownik zarejestrowany pomyślnie" };
    }
}
