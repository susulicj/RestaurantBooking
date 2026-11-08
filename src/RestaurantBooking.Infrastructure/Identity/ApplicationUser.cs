using Microsoft.AspNetCore.Identity;

namespace RestaurantBooking.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}