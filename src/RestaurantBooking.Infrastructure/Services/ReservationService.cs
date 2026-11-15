using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using RestaurantBooking.Application.Booking;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Exceptions;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Domain.Entities;
using RestaurantBooking.Domain.Enums;
using RestaurantBooking.Infrastructure.Persistence;

namespace RestaurantBooking.Infrastructure.Services;

public class ReservationService(AppDbContext db, IOptions<BookingSettings> options) : IReservationService
{
    private readonly BookingSettings _settings = options.Value;

    public async Task<ReservationResponse> CreateAsync(
        string userId, CreateReservationRequest request, CancellationToken ct)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(_settings.TimeZoneId);

        var localStart = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Unspecified);

        if (tz.IsInvalidTime(localStart))
            throw new BadRequestException("Izabrano vreme ne postoji (prelazak na letnje računanje vremena).");

        var startUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, tz);
        var endUtc = startUtc.AddMinutes(_settings.DurationMinutes);
        var nowUtc = DateTime.UtcNow;

        if (startUtc <= nowUtc)
            throw new BadRequestException("Rezervacija mora biti u budućnosti.");
        if (startUtc > nowUtc.AddDays(_settings.MaxDaysAhead))
            throw new BadRequestException($"Rezervacije su moguće najviše {_settings.MaxDaysAhead} dana unapred.");

        var startTime = TimeOnly.FromDateTime(localStart);
        if (!ReservationRules.IsAlignedToSlot(startTime, _settings.SlotIntervalMinutes))
            throw new BadRequestException($"Vreme mora biti na svakih {_settings.SlotIntervalMinutes} minuta (npr. 19:00 ili 19:30).");

        var hours = await db.OpeningHours.AsNoTracking()
            .FirstOrDefaultAsync(o => o.DayOfWeek == localStart.DayOfWeek, ct);
        if (hours is null || !ReservationRules.FitsOpeningHours(startTime, _settings.DurationMinutes, hours.OpensAt, hours.ClosesAt))
            throw new BadRequestException("Restoran u tom terminu ne radi.");

        const int maxAttempts = 3;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var tables = await db.Tables.AsNoTracking()
                .Where(t => t.IsActive && t.Capacity >= request.PartySize)
                .OrderBy(t => t.Capacity).ThenBy(t => t.Number)
                .ToListAsync(ct);

            var busyTableIds = await db.Reservations.AsNoTracking()
                .Where(r => r.Status != ReservationStatus.Cancelled
                            && r.StartTimeUtc < endUtc
                            && r.EndTimeUtc > startUtc)
                .Select(r => r.DiningTableId)
                .Distinct()
                .ToListAsync(ct);

            var table = tables.FirstOrDefault(t => !busyTableIds.Contains(t.Id));
            if (table is null)
                throw new ConflictException("Nema slobodnog stola za izabrani termin i broj gostiju.");

            var reservation = new Reservation
            {
                UserId = userId,
                DiningTableId = table.Id,
                StartTimeUtc = startUtc,
                EndTimeUtc = endUtc,
                PartySize = request.PartySize,
                Status = ReservationStatus.Confirmed,
                CreatedAtUtc = nowUtc
            };

            db.Reservations.Add(reservation);

            try
            {
                await db.SaveChangesAsync(ct);

                return new ReservationResponse(
                    reservation.Id, table.Number, localStart,
                    localStart.AddMinutes(_settings.DurationMinutes),
                    reservation.PartySize, reservation.Status);
            }
            catch (DbUpdateException ex) when (IsOverlapViolation(ex))
            {
                db.Entry(reservation).State = EntityState.Detached;
            }
        }

        throw new ConflictException("Termin je upravo zauzet. Pokušaj ponovo.");
    }

    private static bool IsOverlapViolation(DbUpdateException ex) =>
        ex.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation or PostgresErrorCodes.ExclusionViolation
        };
}