namespace RestaurantBooking.Domain.Entities;

public class DiningTable
{
    public int Id { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;
}