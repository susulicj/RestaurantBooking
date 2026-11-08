using System.ComponentModel.DataAnnotations;

namespace RestaurantBooking.Application.DTOs;

public class RegisterRequest
{
    [Required, EmailAddress] public string Email { get; set; } = default!;
    [Required] public string Password { get; set; } = default!;
    [Required, MaxLength(100)] public string FullName { get; set; } = default!;
}

public class LoginRequest
{
    [Required, EmailAddress] public string Email { get; set; } = default!;
    [Required] public string Password { get; set; } = default!;
}

public record AuthResponse(string UserId, string Email, IList<string> Roles, string Token);


