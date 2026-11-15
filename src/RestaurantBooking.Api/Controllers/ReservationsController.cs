using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.Api.Extensions;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Interfaces;

namespace RestaurantBooking.Api.Controllers;

[ApiController]
[Route("api/reservations")]
[Authorize]
public class ReservationsController(IReservationService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ReservationResponse>> Create(
        CreateReservationRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(User.GetUserId(), request, ct);
        return StatusCode(StatusCodes.Status201Created, created);
    }
}