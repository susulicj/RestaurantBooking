using FluentValidation;
using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Application.Validators;

public class OpeningHoursRequestValidator : AbstractValidator<OpeningHoursRequest>
{
    public OpeningHoursRequestValidator()
    {
        RuleFor(x => x.DayOfWeek).IsInEnum()
            .WithMessage("Nepoznat dan u nedelji.");

        RuleFor(x => x.ClosesAt)
            .GreaterThan(x => x.OpensAt)
            .WithMessage("Vreme zatvaranja mora biti posle vremena otvaranja.");
    }
}