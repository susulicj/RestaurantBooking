using System.ComponentModel.DataAnnotations;

namespace RestaurantBooking.Application.DTOs;

public class OpeningHoursRequest
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpensAt { get; set; }
    public TimeOnly ClosesAt { get; set; }
}

public record OpeningHoursResponse(int Id, DayOfWeek DayOfWeek, TimeOnly OpensAt, TimeOnly ClosesAt);