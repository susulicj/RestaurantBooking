using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Interfaces;

namespace RestaurantBooking.Api.Controllers;

[ApiController]
[Route("api/availability")]
public class AvailabilityController(IAvailabilityService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AvailabilityResponse>> Get(
        [FromQuery] AvailabilityQuery query, CancellationToken ct) =>
        Ok(await service.GetAsync(query.Date, query.PartySize, ct));
}