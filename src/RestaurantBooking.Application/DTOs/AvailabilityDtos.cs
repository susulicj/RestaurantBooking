namespace RestaurantBooking.Application.DTOs;

public class AvailabilityQuery
{
    public DateOnly Date { get; set; }
    public int PartySize { get; set; }
}

public record AvailableSlotResponse(TimeOnly StartTime, TimeOnly EndTime, int AvailableTables);

public record AvailabilityResponse(DateOnly Date, int PartySize, List<AvailableSlotResponse> Slots);