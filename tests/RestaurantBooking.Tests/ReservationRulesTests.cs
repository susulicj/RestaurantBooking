using RestaurantBooking.Application.Booking;

namespace RestaurantBooking.Tests;

public class ReservationRulesTests
{
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