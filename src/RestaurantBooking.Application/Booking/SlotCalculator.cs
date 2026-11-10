namespace RestaurantBooking.Application.Booking;

public static class SlotCalculator
{
    public static List<TimeOnly> GenerateSlotStarts(
        TimeOnly opensAt, TimeOnly closesAt, int durationMinutes, int intervalMinutes)
    {
        var slots = new List<TimeOnly>();
        var duration = TimeSpan.FromMinutes(durationMinutes);
        var interval = TimeSpan.FromMinutes(intervalMinutes);
        var end = closesAt.ToTimeSpan();

        for (var t = opensAt.ToTimeSpan(); t + duration <= end; t += interval)
            slots.Add(TimeOnly.FromTimeSpan(t));

        return slots;
    }

    public static bool Overlaps(DateTime startA, DateTime endA, DateTime startB, DateTime endB) =>
        startA < endB && startB < endA;
}