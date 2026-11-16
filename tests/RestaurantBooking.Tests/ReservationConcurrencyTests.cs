using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Domain.Entities;
using RestaurantBooking.Domain.Enums;
using RestaurantBooking.Infrastructure.Persistence;

namespace RestaurantBooking.Tests;

[Collection("Api")]
public class ReservationConcurrencyTests(ApiFixture api)
{
    [Fact]
    public async Task Parallel_requests_for_same_slot_never_double_book()
    {
        var client = await api.CreateUserClientAsync();

        var request = new CreateReservationRequest
        {
            StartTime = DateTime.Today.AddDays(20).AddHours(19),
            PartySize = 6
        };

        var responses = await Task.WhenAll(
            Enumerable.Range(0, 12).Select(_ => client.PostAsJsonAsync("/api/reservations", request)));

        Assert.Equal(2, responses.Count(r => r.StatusCode == HttpStatusCode.Created));
        Assert.Equal(10, responses.Count(r => r.StatusCode == HttpStatusCode.Conflict));
    }

    [Fact]
    public async Task Database_rejects_overlapping_reservation_on_same_table()
    {
        using var scope = api.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var start = DateTime.UtcNow.AddDays(40);
        db.Reservations.Add(new Reservation
        {
            UserId = "u1", DiningTableId = 1, StartTimeUtc = start, EndTimeUtc = start.AddMinutes(90),
            PartySize = 2, Status = ReservationStatus.Confirmed, CreatedAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        db.Reservations.Add(new Reservation
        {
            UserId = "u2", DiningTableId = 1, StartTimeUtc = start.AddMinutes(30), EndTimeUtc = start.AddMinutes(120),
            PartySize = 2, Status = ReservationStatus.Confirmed, CreatedAtUtc = DateTime.UtcNow
        });

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }
}