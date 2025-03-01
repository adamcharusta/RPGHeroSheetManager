using Microsoft.EntityFrameworkCore;
using RPGHeroSheetManagerAPI.AuthService.Infrastructure.Data;

namespace RPGHeroSheetManagerAPI.AuthService.Infrastructure.Auth.Auth.Commands.RegisterUserCommand;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public RegisterUserCommandValidator(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Hasło jest wymagane")
            .MinimumLength(8).WithMessage("Hasło musi mieć co najmniej 8 znaków")
            .MaximumLength(64).WithMessage("Hasło nie może mieć więcej niż 64 znaki")
            .Matches(@"[A-Z]").WithMessage("Hasło musi zawierać co najmniej jedną wielką literę")
            .Matches(@"[a-z]").WithMessage("Hasło musi zawierać co najmniej jedną małą literę")
            .Matches(@"\d").WithMessage("Hasło musi zawierać co najmniej jedną cyfrę")
            .Matches(@"[^\da-zA-Z]").WithMessage("Hasło musi zawierać co najmniej jeden znak specjalny");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Potwierdzenie hasła jest wymagane")
            .Equal(x => x.Password)
            .WithMessage("Hasła nie są takie same");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Adres email jest wymagany")
            .EmailAddress().WithMessage("Niepoprawny adres email")
            .MustAsync(BeUniqueEmail).WithMessage("Adres email jest już zajęty");

        RuleFor(x => x.ConfirmEmail)
            .NotEmpty().WithMessage("Potwierdzenie adresu email jest wymagane")
            .Equal(x => x.Email).WithMessage("Adresy email nie są takie same");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Nazwa użytkownika jest wymagana")
            .MinimumLength(3).WithMessage("Nazwa użytkownika musi mieć co najmniej 3 znaki")
            .MaximumLength(64).WithMessage("Nazwa użytkownika nie może mieć więcej niż 64 znaki")
            .MustAsync(BeUniqueUsername).WithMessage("Nazwa użytkownika jest już zajęta");
    }

    private async Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken)
    {
        var normalizedUsername = _userManager.NormalizeName(username);
        return !await _context.Users.AnyAsync(x => x.NormalizedUserName == normalizedUsername, cancellationToken);
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = _userManager.NormalizeEmail(email);
        return !await _context.Users.AnyAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    }
}
