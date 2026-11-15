using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Domain.Entities;
using RestaurantBooking.Domain.Enums;
using RestaurantBooking.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
namespace RestaurantBooking.Tests;

public class ReservationConcurrencyTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .Build();

    private WebApplicationFactory<Program> _factory = default!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        var connectionString = _postgres.GetConnectionString();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(connectionString).Options;
        await using (var db = new AppDbContext(options))
            await db.Database.MigrateAsync();

        Environment.SetEnvironmentVariable("ConnectionStrings__Default", connectionString);
        Environment.SetEnvironmentVariable("Jwt__Key", "test-kljuc-test-kljuc-test-kljuc-123456");

        _factory = new WebApplicationFactory<Program>();
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var email = $"user{Guid.NewGuid():N}@test.com";

        var response = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest { Email = email, Password = "Password1", FullName = "Test User" });
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
        return client;
    }

    private static DateTime NextWeekAt19() => DateTime.Today.AddDays(7).AddHours(19);

    [Fact]
    public async Task Parallel_requests_for_same_slot_never_double_book()
    {
        var client = await CreateAuthenticatedClientAsync();
        var request = new CreateReservationRequest { StartTime = NextWeekAt19(), PartySize = 6 };

        var tasks = Enumerable.Range(0, 12)
            .Select(_ => client.PostAsJsonAsync("/api/reservations", request))
            .ToArray();
        var responses = await Task.WhenAll(tasks);

        var created = responses.Count(r => r.StatusCode == HttpStatusCode.Created);
        var conflicts = responses.Count(r => r.StatusCode == HttpStatusCode.Conflict);

        Assert.Equal(2, created);
        Assert.Equal(10, conflicts);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var perTable = await db.Reservations.GroupBy(r => r.DiningTableId).Select(g => g.Count()).ToListAsync();
        Assert.All(perTable, count => Assert.Equal(1, count));
    }

    [Fact]
    public async Task Database_rejects_overlapping_reservation_on_same_table()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var start = DateTime.UtcNow.AddDays(10);
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