using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantBooking.Domain.Entities;

namespace RestaurantBooking.Infrastructure.Persistence.Configurations;

public class DiningTableConfiguration : IEntityTypeConfiguration<DiningTable>
{
    public void Configure(EntityTypeBuilder<DiningTable> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.Number).IsUnique();
        builder.Property(t => t.Capacity).IsRequired();

        builder.HasData(
            Enumerable.Range(1, 8).Select(i => new DiningTable
            {
                Id = i,
                Number = i,
                Capacity = i <= 3 ? 2 : i <= 6 ? 4 : 6
            }));
    }
}