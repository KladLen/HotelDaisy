using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelDaisy.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Apartaments_ApartamentId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "Apartaments");

            migrationBuilder.RenameColumn(
                name: "ApartamentId",
                table: "Reservations",
                newName: "ApartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_ApartamentId",
                table: "Reservations",
                newName: "IX_Reservations_ApartmentId");

            migrationBuilder.CreateTable(
                name: "Apartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberOfRooms = table.Column<int>(type: "int", nullable: true),
                    Balcony = table.Column<bool>(type: "bit", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartments", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Apartments_ApartmentId",
                table: "Reservations",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Apartments_ApartmentId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "Apartments");

            migrationBuilder.RenameColumn(
                name: "ApartmentId",
                table: "Reservations",
                newName: "ApartamentId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_ApartmentId",
                table: "Reservations",
                newName: "IX_Reservations_ApartamentId");

            migrationBuilder.CreateTable(
                name: "Apartaments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Balcony = table.Column<bool>(type: "bit", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    NumberOfRooms = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(6,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartaments", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Apartaments_ApartamentId",
                table: "Reservations",
                column: "ApartamentId",
                principalTable: "Apartaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
