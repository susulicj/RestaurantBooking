using FluentValidation;
using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Application.Validators;

public class CreateTableRequestValidator : AbstractValidator<CreateTableRequest>
{
    public CreateTableRequestValidator()
    {
        RuleFor(x => x.Number).InclusiveBetween(1, 1000)
            .WithMessage("Broj stola mora biti između 1 i 1000.");
        RuleFor(x => x.Capacity).InclusiveBetween(1, 50)
            .WithMessage("Kapacitet mora biti između 1 i 50.");
    }
}

public class UpdateTableRequestValidator : AbstractValidator<UpdateTableRequest>
{
    public UpdateTableRequestValidator()
    {
        RuleFor(x => x.Number).InclusiveBetween(1, 1000)
            .WithMessage("Broj stola mora biti između 1 i 1000.");
        RuleFor(x => x.Capacity).InclusiveBetween(1, 50)
            .WithMessage("Kapacitet mora biti između 1 i 50.");
    }
}