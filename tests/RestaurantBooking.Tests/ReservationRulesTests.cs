using RestaurantBooking.Application.Booking;

namespace RestaurantBooking.Tests;

public class ReservationRulesTests
{
    [Theory]
    [InlineData(48, true)]
    [InlineData(24, true)]    
    [InlineData(23, false)]
    [InlineData(1, false)]
    public void Cancellation_window(int hoursBeforeStart, bool expected)
    {
        var now = new DateTime(2026, 11, 20, 12, 0, 0, DateTimeKind.Utc);
        var start = now.AddHours(hoursBeforeStart);
        Assert.Equal(expected, ReservationRules.CanCancel(start, now, 24));
    }
    [Theory]
    [InlineData(19, 0, true)]
    [InlineData(19, 30, true)]
    [InlineData(19, 15, false)]
    [InlineData(19, 45, false)]
    public void Slot_alignment(int hour, int minute, bool expected) =>
        Assert.Equal(expected, ReservationRules.IsAlignedToSlot(new TimeOnly(hour, minute), 30));

    [Theory]
    [InlineData(12, 0, true)]    
    [InlineData(21, 30, true)]  
    [InlineData(22, 0, false)]   
    [InlineData(11, 30, false)]  
    public void Fits_opening_hours(int hour, int minute, bool expected) =>
        Assert.Equal(expected,
            ReservationRules.FitsOpeningHours(new TimeOnly(hour, minute), 90, new TimeOnly(12, 0), new TimeOnly(23, 0)));
}