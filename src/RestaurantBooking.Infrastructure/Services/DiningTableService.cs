using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Exceptions;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Domain.Entities;
using RestaurantBooking.Infrastructure.Persistence;

namespace RestaurantBooking.Infrastructure.Services;

public class DiningTableService(AppDbContext db) : IDiningTableService
{
    public async Task<List<TableResponse>> GetAllAsync(CancellationToken ct) =>
        await db.Tables.AsNoTracking()
            .OrderBy(t => t.Number)
            .Select(t => new TableResponse(t.Id, t.Number, t.Capacity, t.IsActive))
            .ToListAsync(ct);

    public async Task<TableResponse> GetByIdAsync(int id, CancellationToken ct)
    {
        var t = await FindAsync(id, ct);
        return ToResponse(t);
    }

    public async Task<TableResponse> CreateAsync(CreateTableRequest request, CancellationToken ct)
    {
        if (await db.Tables.AnyAsync(t => t.Number == request.Number, ct))
            throw new ConflictException($"Sto sa brojem {request.Number} već postoji.");

        var table = new DiningTable { Number = request.Number, Capacity = request.Capacity };
        db.Tables.Add(table);
        await db.SaveChangesAsync(ct);
        return ToResponse(table);
    }

    public async Task UpdateAsync(int id, UpdateTableRequest request, CancellationToken ct)
    {
        var table = await FindAsync(id, ct);

        if (await db.Tables.AnyAsync(t => t.Number == request.Number && t.Id != id, ct))
            throw new ConflictException($"Sto sa brojem {request.Number} već postoji.");

        table.Number = request.Number;
        table.Capacity = request.Capacity;
        table.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var table = await FindAsync(id, ct);

        if (await db.Reservations.AnyAsync(r => r.DiningTableId == id, ct))
            throw new ConflictException("Sto ima rezervacije. Umesto brisanja, deaktiviraj ga (IsActive = false).");

        db.Tables.Remove(table);
        await db.SaveChangesAsync(ct);
    }

    private async Task<DiningTable> FindAsync(int id, CancellationToken ct) =>
        await db.Tables.FirstOrDefaultAsync(t => t.Id == id, ct)
        ?? throw new NotFoundException($"Sto sa id {id} ne postoji.");

    private static TableResponse ToResponse(DiningTable t) =>
        new(t.Id, t.Number, t.Capacity, t.IsActive);
}