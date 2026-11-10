using RestaurantBooking.Application.Booking;

namespace RestaurantBooking.Tests;

public class SlotCalculatorTests
{
    [Fact]
    public void Generates_slots_until_last_booking_fits_before_closing()
    {
        var slots = SlotCalculator.GenerateSlotStarts(new TimeOnly(12, 0), new TimeOnly(23, 0), 90, 30);

        Assert.Equal(new TimeOnly(12, 0), slots.First());
        Assert.Equal(new TimeOnly(21, 30), slots.Last()); // 21:30 + 90 min = 23:00
        Assert.Equal(20, slots.Count);
    }

    [Fact]
    public void No_slots_when_opening_window_is_shorter_than_duration()
    {
        var slots = SlotCalculator.GenerateSlotStarts(new TimeOnly(12, 0), new TimeOnly(13, 0), 90, 30);
        Assert.Empty(slots);
    }

    [Fact]
    public void Overlapping_intervals_are_detected()
    {
        var a = new DateTime(2026, 11, 20, 18, 0, 0, DateTimeKind.Utc);
        var overlaps = SlotCalculator.Overlaps(a, a.AddMinutes(90), a.AddMinutes(30), a.AddMinutes(120));
        Assert.True(overlaps);
    }

    [Fact]
    public void Back_to_back_intervals_do_not_overlap()
    {
        var a = new DateTime(2026, 11, 20, 18, 0, 0, DateTimeKind.Utc);
        var overlaps = SlotCalculator.Overlaps(a, a.AddMinutes(90), a.AddMinutes(90), a.AddMinutes(180));
        Assert.False(overlaps);
    }
}