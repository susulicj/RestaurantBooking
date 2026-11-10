namespace RestaurantBooking.Application.Booking;

public class BookingSettings
{
    public const string SectionName = "Booking";

    public string TimeZoneId { get; set; } = "Europe/Belgrade";
    public int DurationMinutes { get; set; } = 90;
    public int SlotIntervalMinutes { get; set; } = 30;
    public int MaxDaysAhead { get; set; } = 60;
}