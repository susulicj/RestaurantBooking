using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationResponse> CreateAsync(string userId, CreateReservationRequest request, CancellationToken ct);
}