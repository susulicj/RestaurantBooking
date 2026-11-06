using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Interfaces;

namespace RestaurantBooking.Api.Controllers;

[ApiController]
[Route("api/opening-hours")]
public class OpeningHoursController(IOpeningHoursService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OpeningHoursResponse>>> GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OpeningHoursResponse>> GetById(int id, CancellationToken ct) =>
        Ok(await service.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<OpeningHoursResponse>> Create(OpeningHoursRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, OpeningHoursRequest request, CancellationToken ct)
    {
        await service.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}