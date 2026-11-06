using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Exceptions;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Domain.Entities;
using RestaurantBooking.Infrastructure.Persistence;

namespace RestaurantBooking.Infrastructure.Services;

public class OpeningHoursService(AppDbContext db) : IOpeningHoursService
{
    public async Task<List<OpeningHoursResponse>> GetAllAsync(CancellationToken ct) =>
        await db.OpeningHours.AsNoTracking()
            .OrderBy(o => o.DayOfWeek)
            .Select(o => new OpeningHoursResponse(o.Id, o.DayOfWeek, o.OpensAt, o.ClosesAt))
            .ToListAsync(ct);

    public async Task<OpeningHoursResponse> GetByIdAsync(int id, CancellationToken ct) =>
        ToResponse(await FindAsync(id, ct));

    public async Task<OpeningHoursResponse> CreateAsync(OpeningHoursRequest request, CancellationToken ct)
    {
        if (await db.OpeningHours.AnyAsync(o => o.DayOfWeek == request.DayOfWeek, ct))
            throw new ConflictException($"Radno vreme za {request.DayOfWeek} već postoji. Izmeni postojeći zapis.");

        var entity = new OpeningHours
        {
            DayOfWeek = request.DayOfWeek,
            OpensAt = request.OpensAt,
            ClosesAt = request.ClosesAt
        };
        db.OpeningHours.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    public async Task UpdateAsync(int id, OpeningHoursRequest request, CancellationToken ct)
    {
        var entity = await FindAsync(id, ct);

        if (await db.OpeningHours.AnyAsync(o => o.DayOfWeek == request.DayOfWeek && o.Id != id, ct))
            throw new ConflictException($"Radno vreme za {request.DayOfWeek} već postoji.");

        entity.DayOfWeek = request.DayOfWeek;
        entity.OpensAt = request.OpensAt;
        entity.ClosesAt = request.ClosesAt;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await FindAsync(id, ct);
        db.OpeningHours.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    private async Task<OpeningHours> FindAsync(int id, CancellationToken ct) =>
        await db.OpeningHours.FirstOrDefaultAsync(o => o.Id == id, ct)
        ?? throw new NotFoundException($"Radno vreme sa id {id} ne postoji.");

    private static OpeningHoursResponse ToResponse(OpeningHours o) =>
        new(o.Id, o.DayOfWeek, o.OpensAt, o.ClosesAt);
}