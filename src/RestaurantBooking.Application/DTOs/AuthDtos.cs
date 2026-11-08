using System.ComponentModel.DataAnnotations;

namespace RestaurantBooking.Application.DTOs;

public class RegisterRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string FullName { get; set; } = default!;
}

public class LoginRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public record AuthResponse(string UserId, string Email, IList<string> Roles, string Token);

