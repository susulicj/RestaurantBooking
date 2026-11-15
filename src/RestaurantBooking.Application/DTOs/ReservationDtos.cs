using RestaurantBooking.Domain.Enums;

namespace RestaurantBooking.Application.DTOs;

public class CreateReservationRequest
{
    public DateTime StartTime { get; set; }  
    public int PartySize { get; set; }
}

public record ReservationResponse(
    int Id,
    int TableNumber,
    DateTime StartTimeLocal,
    DateTime EndTimeLocal,
    int PartySize,
    ReservationStatus Status);