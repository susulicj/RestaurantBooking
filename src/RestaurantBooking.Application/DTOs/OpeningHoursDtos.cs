using System.ComponentModel.DataAnnotations;

namespace RestaurantBooking.Application.DTOs;

public class OpeningHoursRequest : IValidatableObject
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpensAt { get; set; }
    public TimeOnly ClosesAt { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (OpensAt >= ClosesAt)
            yield return new ValidationResult(
                "Vreme otvaranja mora biti pre vremena zatvaranja.",
                [nameof(OpensAt), nameof(ClosesAt)]);
    }
}

public record OpeningHoursResponse(int Id, DayOfWeek DayOfWeek, TimeOnly OpensAt, TimeOnly ClosesAt);