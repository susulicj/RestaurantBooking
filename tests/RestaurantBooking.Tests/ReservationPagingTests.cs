using System.Net;
using System.Net.Http.Json;
using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Tests;

[Collection("Api")]
public class ReservationPagingTests(ApiFixture api)
{
    [Fact]
    public async Task Pages_do_not_overlap_and_total_count_is_correct()
    {
        var client = await api.CreateUserClientAsync();

        for (var i = 0; i < 5; i++)
        {
            var r = await client.PostAsJsonAsync("/api/reservations", new CreateReservationRequest
            {
                StartTime = DateTime.Today.AddDays(10 + i).AddHours(19),
                PartySize = 2
            });
            Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        }

        var page1 = await client.GetFromJsonAsync<PagedResult<ReservationResponse>>(
            "/api/reservations/my?page=1&pageSize=2", ApiFixture.Json);
        var page2 = await client.GetFromJsonAsync<PagedResult<ReservationResponse>>(
            "/api/reservations/my?page=2&pageSize=2" ,ApiFixture.Json);
        var page3 = await client.GetFromJsonAsync<PagedResult<ReservationResponse>>(
            "/api/reservations/my?page=3&pageSize=2", ApiFixture.Json);

        Assert.Equal(5, page1!.TotalCount);
        Assert.Equal(3, page1.TotalPages);
        Assert.Equal(2, page1.Items.Count);
        Assert.Equal(2, page2!.Items.Count);
        Assert.Single(page3!.Items);

        var allIds = page1.Items.Concat(page2.Items).Concat(page3.Items).Select(i => i.Id).ToList();
        Assert.Equal(5, allIds.Distinct().Count());
    }

    [Fact]
    public async Task Status_filter_returns_only_matching_reservations()
    {
        var client = await api.CreateUserClientAsync();

        var first = await client.PostAsJsonAsync("/api/reservations", new CreateReservationRequest
        {
            StartTime = DateTime.Today.AddDays(30).AddHours(19), PartySize = 2
        });
        var reservation = await first.Content.ReadFromJsonAsync<ReservationResponse>(ApiFixture.Json);
        await client.PostAsJsonAsync("/api/reservations", new CreateReservationRequest
        {
            StartTime = DateTime.Today.AddDays(31).AddHours(19), PartySize = 2
        });
        await client.PostAsync($"/api/reservations/{reservation!.Id}/cancel", null);

        var cancelled = await client.GetFromJsonAsync<PagedResult<ReservationResponse>>(
            "/api/reservations/my?status=Cancelled", ApiFixture.Json);

        Assert.Equal(1, cancelled!.TotalCount);
        Assert.All(cancelled.Items, i => Assert.Equal(Domain.Enums.ReservationStatus.Cancelled, i.Status));
    }

    [Fact]
    public async Task Invalid_page_size_is_rejected()
    {
        var client = await api.CreateUserClientAsync();
        var response = await client.GetAsync("/api/reservations/my?pageSize=500");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}