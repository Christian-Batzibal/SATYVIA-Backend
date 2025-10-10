using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace HotelReservationAPI.Migrations
{
    public partial class RemoveClientColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Elimina cualquier restricción que involucre ClientId (sin importar el nombre)
            migrationBuilder.Sql(@"
                DECLARE @ConstraintName NVARCHAR(200);
                SELECT @ConstraintName = [name]
                FROM sys.foreign_keys
                WHERE parent_object_id = OBJECT_ID('Reservation')
                AND referenced_object_id = OBJECT_ID('Client');
                IF @ConstraintName IS NOT NULL
                BEGIN
                    EXEC('ALTER TABLE [Reservation] DROP CONSTRAINT [' + @ConstraintName + ']');
                END
            ");

            // 2️⃣ Elimina la columna si aún existe
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns 
                           WHERE Name = N'ClientId' AND Object_ID = Object_ID(N'Reservation'))
                BEGIN
                    ALTER TABLE [Reservation] DROP COLUMN [ClientId];
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // En caso de rollback, vuelve a agregar la columna
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Reservation",
                type: "int",
                nullable: true);
        }
    }
}
