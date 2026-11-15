namespace RestaurantBooking.Application.Booking;

public static class ReservationRules
{
    public static bool IsAlignedToSlot(TimeOnly time, int slotIntervalMinutes) =>
        time.Second == 0 && time.Millisecond == 0
        && (time.Hour * 60 + time.Minute) % slotIntervalMinutes == 0;

    public static bool FitsOpeningHours(
        TimeOnly start, int durationMinutes, TimeOnly opensAt, TimeOnly closesAt)
    {
        var startSpan = start.ToTimeSpan();
        var endSpan = startSpan + TimeSpan.FromMinutes(durationMinutes);
        return startSpan >= opensAt.ToTimeSpan() && endSpan <= closesAt.ToTimeSpan();
    }
}