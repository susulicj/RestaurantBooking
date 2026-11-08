using System.ComponentModel.DataAnnotations;

namespace RestaurantBooking.Application.DTOs;

public class CreateTableRequest
{
    public int Number { get; set; }
    public int Capacity { get; set; }
}

public class UpdateTableRequest
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
}

public record TableResponse(int Id, int Number, int Capacity, bool IsActive);