using System.Net;
using System.Net.Http.Json;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Domain.Enums;

namespace RestaurantBooking.Tests;

[Collection("Api")]
public class ReservationCancellationTests(ApiFixture api)
{
    private static CreateReservationRequest At(int daysAhead, int hour = 19, int partySize = 2) =>
        new() { StartTime = DateTime.Today.AddDays(daysAhead).AddHours(hour), PartySize = partySize };

    [Fact]
    public async Task Owner_can_cancel_reservation_and_slot_is_freed()
    {
        var client = await api.CreateUserClientAsync();
        var created = await client.PostAsJsonAsync("/api/reservations", At(5, partySize: 6));
        var reservation = await created.Content.ReadFromJsonAsync<ReservationResponse>(ApiFixture.Json);

        var cancel = await client.PostAsync($"/api/reservations/{reservation!.Id}/cancel", null);

        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
        var body = await cancel.Content.ReadFromJsonAsync<ReservationResponse>(ApiFixture.Json);
        Assert.Equal(ReservationStatus.Cancelled, body!.Status);

        var again = await client.PostAsJsonAsync("/api/reservations", At(5, partySize: 6));
        Assert.Equal(HttpStatusCode.Created, again.StatusCode);
    }

    [Fact]
    public async Task Cancelling_twice_returns_conflict()
    {
        var client = await api.CreateUserClientAsync();
        var created = await client.PostAsJsonAsync("/api/reservations", At(6));
        var reservation = await created.Content.ReadFromJsonAsync<ReservationResponse>(ApiFixture.Json);

        await client.PostAsync($"/api/reservations/{reservation!.Id}/cancel", null);
        var second = await client.PostAsync($"/api/reservations/{reservation.Id}/cancel", null);

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Other_user_gets_404_for_foreign_reservation()
    {
        var owner = await api.CreateUserClientAsync();
        var stranger = await api.CreateUserClientAsync();

        var created = await owner.PostAsJsonAsync("/api/reservations", At(7));
        var reservation = await created.Content.ReadFromJsonAsync<ReservationResponse>(ApiFixture.Json);

        var get = await stranger.GetAsync($"/api/reservations/{reservation!.Id}");
        var cancel = await stranger.PostAsync($"/api/reservations/{reservation.Id}/cancel", null);

        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, cancel.StatusCode);
    }

    [Fact]
    public async Task Reservation_without_token_is_unauthorized()
    {
        var anonymous = api.Factory.CreateClient();
        var response = await anonymous.GetAsync("/api/reservations/my");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Customer_cannot_list_all_reservations_but_admin_can()
    {
        var customer = await api.CreateUserClientAsync();
        var admin = await api.CreateAdminClientAsync();

        Assert.Equal(HttpStatusCode.Forbidden, (await customer.GetAsync("/api/reservations")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/api/reservations")).StatusCode);
    }
}