using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Application.Interfaces;

public interface IOpeningHoursService
{
    Task<List<OpeningHoursResponse>> GetAllAsync(CancellationToken ct);
    Task<OpeningHoursResponse> GetByIdAsync(int id, CancellationToken ct);
    Task<OpeningHoursResponse> CreateAsync(OpeningHoursRequest request, CancellationToken ct);
    Task UpdateAsync(int id, OpeningHoursRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}