using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RestaurantBooking.Application.Booking;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Exceptions;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Domain.Enums;
using RestaurantBooking.Infrastructure.Persistence;

namespace RestaurantBooking.Infrastructure.Services;

public class AvailabilityService(AppDbContext db, IOptions<BookingSettings> options) : IAvailabilityService
{
    private readonly BookingSettings _settings = options.Value;

    public async Task<AvailabilityResponse> GetAsync(DateOnly date, int partySize, CancellationToken ct)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(_settings.TimeZoneId);
        var nowUtc = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(nowUtc, tz));

        if (date < today)
            throw new BadRequestException("Datum ne može biti u prošlosti.");
        if (date > today.AddDays(_settings.MaxDaysAhead))
            throw new BadRequestException($"Rezervacije su moguće najviše {_settings.MaxDaysAhead} dana unapred.");

        var empty = new AvailabilityResponse(date, partySize, []);

        var hours = await db.OpeningHours.AsNoTracking()
            .FirstOrDefaultAsync(o => o.DayOfWeek == date.DayOfWeek, ct);
        if (hours is null) return empty;

        var tableIds = await db.Tables.AsNoTracking()
            .Where(t => t.IsActive && t.Capacity >= partySize)
            .Select(t => t.Id)
            .ToListAsync(ct);
        if (tableIds.Count == 0) return empty;

        // Granice dana u UTC, da bismo iz baze uzeli samo rezervacije tog dana
        var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(date.ToDateTime(TimeOnly.MinValue), tz);
        var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(date.AddDays(1).ToDateTime(TimeOnly.MinValue), tz);

        var reservations = await db.Reservations.AsNoTracking()
            .Where(r => r.Status != ReservationStatus.Cancelled
                        && r.StartTimeUtc < dayEndUtc
                        && r.EndTimeUtc > dayStartUtc
                        && tableIds.Contains(r.DiningTableId))
            .Select(r => new { r.DiningTableId, r.StartTimeUtc, r.EndTimeUtc })
            .ToListAsync(ct);

        var duration = TimeSpan.FromMinutes(_settings.DurationMinutes);
        var slotStarts = SlotCalculator.GenerateSlotStarts(
            hours.OpensAt, hours.ClosesAt, _settings.DurationMinutes, _settings.SlotIntervalMinutes);

        var slots = new List<AvailableSlotResponse>();
        foreach (var start in slotStarts)
        {
            var slotStartUtc = TimeZoneInfo.ConvertTimeToUtc(date.ToDateTime(start), tz);
            var slotEndUtc = slotStartUtc + duration;

            if (slotStartUtc <= nowUtc) continue; // slot je već prošao

            var freeTables = tableIds.Count(id =>
                !reservations.Any(r => r.DiningTableId == id
                    && SlotCalculator.Overlaps(slotStartUtc, slotEndUtc, r.StartTimeUtc, r.EndTimeUtc)));

            if (freeTables > 0)
                slots.Add(new AvailableSlotResponse(start, start.Add(duration), freeTables));
        }

        return new AvailabilityResponse(date, partySize, slots);
    }
}