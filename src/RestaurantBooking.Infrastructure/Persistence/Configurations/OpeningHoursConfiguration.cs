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
    }
}