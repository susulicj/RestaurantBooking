using FluentValidation;
using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Application.Validators;

public class AvailabilityQueryValidator : AbstractValidator<AvailabilityQuery>
{
    public AvailabilityQueryValidator()
    {
        RuleFor(x => x.Date).NotEqual(default(DateOnly))
            .WithMessage("Datum je obavezan (format 2026-11-20).");
        RuleFor(x => x.PartySize).InclusiveBetween(1, 20)
            .WithMessage("Broj gostiju mora biti između 1 i 20.");
    }
}