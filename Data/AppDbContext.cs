using Microsoft.EntityFrameworkCore;
using HotelReservationAPI.Models;

namespace HotelReservationAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Room { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Offer> Offer { get; set; }
        public DbSet<RoomImage> RoomImage { get; set; }
        public DbSet<BranchImage> BranchImage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Room>().ToTable("Room");
            modelBuilder.Entity<Branch>().ToTable("Branch");
            modelBuilder.Entity<Reservation>().ToTable("Reservation");
            modelBuilder.Entity<Offer>().ToTable("Offer");
            modelBuilder.Entity<RoomImage>().ToTable("RoomImage");
            modelBuilder.Entity<BranchImage>().ToTable("BranchImage");

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.Property(r => r.Status)
                      .HasMaxLength(50);

                entity.Property(r => r.StartDate).IsRequired();
                entity.Property(r => r.EndDate).IsRequired();
            });
           

            // Relación RoomImage
            modelBuilder.Entity<RoomImage>()
                .HasOne(ri => ri.Room)
                .WithMany(r => r.RoomImages)
                .HasForeignKey(ri => ri.RoomId);

            // Relación BranchImage
            modelBuilder.Entity<BranchImage>()
                .HasOne(bi => bi.Branch)
                .WithMany(b => b.BranchImages)
                .HasForeignKey(bi => bi.BranchId);
           
        }
    }
}
