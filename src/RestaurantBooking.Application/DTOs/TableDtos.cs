using System.ComponentModel.DataAnnotations;

namespace RestaurantBooking.Application.DTOs;

public class CreateTableRequest
{
    [Range(1, 1000)] public int Number { get; set; }
    [Range(1, 50)] public int Capacity { get; set; }
}

public class UpdateTableRequest
{
    [Range(1, 1000)] public int Number { get; set; }
    [Range(1, 50)] public int Capacity { get; set; }
    public bool IsActive { get; set; }
}

public record TableResponse(int Id, int Number, int Capacity, bool IsActive);