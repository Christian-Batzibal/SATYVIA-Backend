using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelReservationAPI.Migrations
{
    /// <inheritdoc />
    public partial class Add15Services : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomService",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomService", x => new { x.RoomId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_RoomService_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomService_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Service",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Conexión de alta velocidad gratuita en todas las habitaciones", "WiFi" },
                    { 2, "Pantalla plana con cable internacional y streaming", "Televisión" },
                    { 3, "Estacionamiento privado con seguridad 24 horas", "Parqueo" },
                    { 4, "Atención personalizada con menú variado disponible las 24 horas", "Servicio al cuarto" },
                    { 5, "Piscina al aire libre con bar incluido", "Piscina" },
                    { 6, "Climatización moderna y silenciosa", "Aire acondicionado" },
                    { 7, "Acceso a spa con sauna, masajes y tratamientos de relajación", "Spa" },
                    { 8, "Shuttle gratuito desde y hacia el aeropuerto", "Transporte" },
                    { 9, "Acceso a instalaciones deportivas y fitness", "Gimnasio" },
                    { 10, "Buffet internacional o desayuno continental", "Desayuno incluido" },
                    { 11, "Habitaciones aptas para huéspedes con mascotas", "Pet Friendly" },
                    { 12, "Seguridad adicional con caja fuerte privada", "Caja fuerte" },
                    { 13, "Bebidas y snacks disponibles en la habitación", "Mini Bar" },
                    { 14, "Balcón con vistas panorámicas exclusivas", "Balcón privado" },
                    { 15, "Habitaciones con vista al lago, mar o montaña", "Vista premium" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomService_ServiceId",
                table: "RoomService",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomService");

            migrationBuilder.DropTable(
                name: "Service");
        }
    }
}
