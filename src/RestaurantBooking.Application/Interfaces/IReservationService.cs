using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationResponse> CreateAsync(string userId, CreateReservationRequest request, CancellationToken ct);
    Task<List<ReservationResponse>> GetMineAsync(string userId, CancellationToken ct);
    Task<ReservationResponse> GetByIdAsync(int id, string userId, bool isAdmin, CancellationToken ct);
    Task<ReservationResponse> CancelAsync(int id, string userId, bool isAdmin, CancellationToken ct);
}