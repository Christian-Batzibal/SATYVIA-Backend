using Microsoft.EntityFrameworkCore;
using HotelReservationAPI.Models;

namespace HotelReservationAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Offer> Offers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ejemplo: configuración para evitar problemas de relaciones o nulls
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.Property(r => r.Status)
                      .HasMaxLength(50);

                // Si necesitás que algún campo sea opcional (nullable en DB)
                entity.Property(r => r.StartDate).IsRequired();
                entity.Property(r => r.EndDate).IsRequired();

            });
        }
    }
}
