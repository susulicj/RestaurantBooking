using FluentValidation.TestHelper;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Validators;

namespace RestaurantBooking.Tests.Validators;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    private static RegisterRequest Valid() => new()
    {
        Email = "ana@example.com",
        Password = "Password1",
        FullName = "Ana Anic"
    };

    [Fact]
    public void Valid_request_has_no_errors() =>
        _validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("ana@")]
    public void Invalid_email_has_error(string email)
    {
        var request = Valid();
        request.Email = email;
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("short1A")]       
    [InlineData("alllowercase1")] 
    [InlineData("NoDigitsHere")]  
    public void Weak_password_has_error(string password)
    {
        var request = Valid();
        request.Password = password;
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Empty_full_name_has_error()
    {
        var request = Valid();
        request.FullName = "";
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.FullName);
    }
}