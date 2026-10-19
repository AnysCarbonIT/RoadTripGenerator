using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RoadTripGenerator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoadTripsAndSteps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoadTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadTrips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Steps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoadTripId = table.Column<int>(type: "integer", nullable: false),
                    CountryCca3 = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Steps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Steps_Countries_CountryCca3",
                        column: x => x.CountryCca3,
                        principalTable: "Countries",
                        principalColumn: "Cca3",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Steps_RoadTrips_RoadTripId",
                        column: x => x.RoadTripId,
                        principalTable: "RoadTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoadTrips_UserId",
                table: "RoadTrips",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Steps_CountryCca3",
                table: "Steps",
                column: "CountryCca3");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_RoadTripId_Order",
                table: "Steps",
                columns: new[] { "RoadTripId", "Order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Steps");

            migrationBuilder.DropTable(
                name: "RoadTrips");
        }
    }
}
