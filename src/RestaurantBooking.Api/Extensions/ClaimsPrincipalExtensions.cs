using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace RestaurantBooking.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? throw new UnauthorizedAccessException("Korisnik nije prijavljen.");
}