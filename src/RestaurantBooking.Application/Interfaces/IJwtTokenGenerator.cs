namespace RestaurantBooking.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string Generate(string userId, string email, IEnumerable<string> roles);
}