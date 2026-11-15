using FluentValidation;
using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Application.Validators;

public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.StartTime).NotEqual(default(DateTime))
            .WithMessage("Vreme početka je obavezno (npr. 2026-11-20T19:00:00).");
        RuleFor(x => x.PartySize).InclusiveBetween(1, 20)
            .WithMessage("Broj gostiju mora biti između 1 i 20.");
    }
}