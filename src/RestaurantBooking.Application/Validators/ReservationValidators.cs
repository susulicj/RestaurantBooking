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

public class ReservationFilterValidator : AbstractValidator<ReservationFilter>
{
    public ReservationFilterValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From!.Value)
            .When(x => x.From.HasValue && x.To.HasValue)
            .WithMessage("'to' ne može biti pre 'from'.");
    }
}

public class AdminReservationFilterValidator : AbstractValidator<AdminReservationFilter>
{
    public AdminReservationFilterValidator()
    {
        Include(new ReservationFilterValidator());
    }
}