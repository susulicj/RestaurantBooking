using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Application.Interfaces;

public interface IDiningTableService
{
    Task<List<TableResponse>> GetAllAsync(CancellationToken ct);
    Task<TableResponse> GetByIdAsync(int id, CancellationToken ct);
    Task<TableResponse> CreateAsync(CreateTableRequest request, CancellationToken ct);
    Task UpdateAsync(int id, UpdateTableRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}