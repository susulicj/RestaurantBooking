using RestaurantBooking.Domain.Enums;

namespace RestaurantBooking.Application.DTOs;

public class CreateReservationRequest
{
    public DateTime StartTime { get; set; }  
    public int PartySize { get; set; }
}

public class ReservationFilter
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public ReservationStatus? Status { get; set; }
    public DateOnly? From { get; set; }   // lokalni datum, uključen
    public DateOnly? To { get; set; }     // lokalni datum, uključen
}

public class AdminReservationFilter : ReservationFilter
{
    public string? UserId { get; set; }
}

public record PagedResult<T>(List<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public record ReservationResponse(
    int Id,
    int TableNumber,
    DateTime StartTimeLocal,
    DateTime EndTimeLocal,
    int PartySize,
    ReservationStatus Status);