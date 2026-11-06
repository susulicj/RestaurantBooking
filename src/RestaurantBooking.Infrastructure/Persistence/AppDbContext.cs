using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Domain.Entities;

namespace RestaurantBooking.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<DiningTable> Tables => Set<DiningTable>();
    public DbSet<OpeningHours> OpeningHours => Set<OpeningHours>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder builder) => builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}