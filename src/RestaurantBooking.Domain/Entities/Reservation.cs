using RestaurantBooking.Domain.Enums;

namespace RestaurantBooking.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;  
    public int DiningTableId { get; set; }
    public DiningTable DiningTable { get; set; } = default!;
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public int PartySize { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public DateTime CreatedAtUtc { get; set; }
}