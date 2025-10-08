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
        public DbSet<Client> Client { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Offer> Offer { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<RoomService> RoomService { get; set; }
        public DbSet<RoomImage> RoomImage { get; set; }
        public DbSet<BranchImage> BranchImage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Room>().ToTable("Room");
            modelBuilder.Entity<Branch>().ToTable("Branch");
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Reservation>().ToTable("Reservation");
            modelBuilder.Entity<Offer>().ToTable("Offer");
            modelBuilder.Entity<Service>().ToTable("Service");
            modelBuilder.Entity<RoomService>().ToTable("RoomService");
            modelBuilder.Entity<RoomImage>().ToTable("RoomImage");
            modelBuilder.Entity<BranchImage>().ToTable("BranchImage");

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.Property(r => r.Status)
                      .HasMaxLength(50);

                entity.Property(r => r.StartDate).IsRequired();
                entity.Property(r => r.EndDate).IsRequired();
            });

            // Relación RoomService
            modelBuilder.Entity<RoomService>()
                .HasKey(rs => new { rs.RoomId, rs.ServiceId });

            modelBuilder.Entity<RoomService>()
                .HasOne(rs => rs.Room)
                .WithMany(r => r.RoomServices)
                .HasForeignKey(rs => rs.RoomId);

            modelBuilder.Entity<RoomService>()
                .HasOne(rs => rs.Service)
                .WithMany(s => s.RoomServices)
                .HasForeignKey(rs => rs.ServiceId);

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

            // Semillas de servicios
            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "WiFi", Description = "Conexión de alta velocidad gratuita en todas las habitaciones" },
                new Service { Id = 2, Name = "Televisión", Description = "Pantalla plana con cable internacional y streaming" },
                new Service { Id = 3, Name = "Parqueo", Description = "Estacionamiento privado con seguridad 24 horas" },
                new Service { Id = 4, Name = "Servicio al cuarto", Description = "Atención personalizada con menú variado disponible las 24 horas" },
                new Service { Id = 5, Name = "Piscina", Description = "Piscina al aire libre con bar incluido" },
                new Service { Id = 6, Name = "Aire acondicionado", Description = "Climatización moderna y silenciosa" },
                new Service { Id = 7, Name = "Spa", Description = "Acceso a spa con sauna, masajes y tratamientos de relajación" },
                new Service { Id = 8, Name = "Transporte", Description = "Shuttle gratuito desde y hacia el aeropuerto" },
                new Service { Id = 9, Name = "Gimnasio", Description = "Acceso a instalaciones deportivas y fitness" },
                new Service { Id = 10, Name = "Desayuno incluido", Description = "Buffet internacional o desayuno continental" },
                new Service { Id = 11, Name = "Pet Friendly", Description = "Habitaciones aptas para huéspedes con mascotas" },
                new Service { Id = 12, Name = "Caja fuerte", Description = "Seguridad adicional con caja fuerte privada" },
                new Service { Id = 13, Name = "Mini Bar", Description = "Bebidas y snacks disponibles en la habitación" },
                new Service { Id = 14, Name = "Balcón privado", Description = "Balcón con vistas panorámicas exclusivas" },
                new Service { Id = 15, Name = "Vista premium", Description = "Habitaciones con vista al lago, mar o montaña" }
            );
        }
    }
}
