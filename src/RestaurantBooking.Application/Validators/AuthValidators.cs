using FluentValidation;
using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email je obavezan.")
            .EmailAddress().WithMessage("Email nije u ispravnom formatu.")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Lozinka je obavezna.")
            .MinimumLength(8).WithMessage("Lozinka mora imati bar 8 znakova.")
            .Matches("[A-Z]").WithMessage("Lozinka mora imati bar jedno veliko slovo.")
            .Matches("[0-9]").WithMessage("Lozinka mora imati bar jednu cifru.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ime je obavezno.")
            .MaximumLength(100);
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}