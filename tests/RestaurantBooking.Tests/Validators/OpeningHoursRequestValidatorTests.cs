using FluentValidation.TestHelper;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Validators;

namespace RestaurantBooking.Tests.Validators;

public class OpeningHoursRequestValidatorTests
{
    private readonly OpeningHoursRequestValidator _validator = new();

    [Fact]
    public void Opens_before_closes_is_valid()
    {
        var request = new OpeningHoursRequest
        {
            DayOfWeek = DayOfWeek.Monday,
            OpensAt = new TimeOnly(12, 0),
            ClosesAt = new TimeOnly(23, 0)
        };
        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Opens_after_closes_has_error()
    {
        var request = new OpeningHoursRequest
        {
            DayOfWeek = DayOfWeek.Monday,
            OpensAt = new TimeOnly(23, 0),
            ClosesAt = new TimeOnly(12, 0)
        };
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.ClosesAt);
    }
}