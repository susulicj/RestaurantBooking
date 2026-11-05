using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantBooking.Domain.Entities;

namespace RestaurantBooking.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.UserId).IsRequired().HasMaxLength(450);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(r => r.DiningTable)
               .WithMany()
               .HasForeignKey(r => r.DiningTableId);

        // Ključ za nedelju 3: sprečava dupli termin na istom stolu,
        // ali otkazana rezervacija ne blokira termin
        builder.HasIndex(r => new { r.DiningTableId, r.StartTimeUtc })
               .IsUnique()
               .HasFilter("\"Status\" <> 'Cancelled'");
    }
}