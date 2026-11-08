using Microsoft.AspNetCore.Identity;
using RestaurantBooking.Application.Constants;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Exceptions;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Infrastructure.Identity;

namespace RestaurantBooking.Infrastructure.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator tokenGenerator) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
            throw new ConflictException("Korisnik sa tim email-om već postoji.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, Roles.Customer);

        IList<string> roles = [Roles.Customer];
        var token = tokenGenerator.Generate(user.Id, user.Email!, roles);
        return new AuthResponse(user.Id, user.Email!, roles, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException("Pogrešan email ili lozinka.");

        var roles = await userManager.GetRolesAsync(user);
        var token = tokenGenerator.Generate(user.Id, user.Email!, roles);
        return new AuthResponse(user.Id, user.Email!, roles, token);
    }
}