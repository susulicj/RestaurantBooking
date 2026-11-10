using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Application.Interfaces;

public interface IAvailabilityService
{
    Task<AvailabilityResponse> GetAsync(DateOnly date, int partySize, CancellationToken ct);
}