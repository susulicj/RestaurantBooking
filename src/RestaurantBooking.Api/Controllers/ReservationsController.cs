using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.Api.Extensions;
using RestaurantBooking.Application.Constants;
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
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<ReservationResponse>>> GetMine(CancellationToken ct) =>
        Ok(await service.GetMineAsync(User.GetUserId(), ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservationResponse>> GetById(int id, CancellationToken ct) =>
        Ok(await service.GetByIdAsync(id, User.GetUserId(), User.IsInRole(Roles.Admin), ct));

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<ReservationResponse>> Cancel(int id, CancellationToken ct) =>
        Ok(await service.CancelAsync(id, User.GetUserId(), User.IsInRole(Roles.Admin), ct));
}