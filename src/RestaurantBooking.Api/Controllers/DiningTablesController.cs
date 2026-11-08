using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using RestaurantBooking.Application.Constants;

namespace RestaurantBooking.Api.Controllers;

[ApiController]
[Route("api/tables")]
public class DiningTablesController(IDiningTableService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TableResponse>>> GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TableResponse>> GetById(int id, CancellationToken ct) =>
        Ok(await service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<TableResponse>> Create(CreateTableRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, UpdateTableRequest request, CancellationToken ct)
    {
        await service.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}