using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantBooking.Domain.Entities;

namespace RestaurantBooking.Infrastructure.Persistence.Configurations;

public class OpeningHoursConfiguration : IEntityTypeConfiguration<OpeningHours>
{
    public void Configure(EntityTypeBuilder<OpeningHours> builder)
    {
        builder.HasKey(o => o.Id);
        builder.HasIndex(o => o.DayOfWeek).IsUnique(); // jedan zapis po danu
    
        builder.HasData(
            Enumerable.Range(0, 7).Select(d => new OpeningHours
            {
                Id = d + 1,
                DayOfWeek = (DayOfWeek)d,
                OpensAt = new TimeOnly(12, 0),
                ClosesAt = new TimeOnly(23, 0)
            }));
    
    }
}