using FluentValidation.TestHelper;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Validators;

namespace RestaurantBooking.Tests.Validators;

public class CreateTableRequestValidatorTests
{
    private readonly CreateTableRequestValidator _validator = new();

    [Fact]
    public void Valid_request_has_no_errors()
    {
        var result = _validator.TestValidate(new CreateTableRequest { Number = 5, Capacity = 4 });
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(51)]
    public void Invalid_capacity_has_error(int capacity)
    {
        var result = _validator.TestValidate(new CreateTableRequest { Number = 5, Capacity = capacity });
        result.ShouldHaveValidationErrorFor(x => x.Capacity);
    }

    [Fact]
    public void Number_zero_has_error()
    {
        var result = _validator.TestValidate(new CreateTableRequest { Number = 0, Capacity = 4 });
        result.ShouldHaveValidationErrorFor(x => x.Number);
    }
}