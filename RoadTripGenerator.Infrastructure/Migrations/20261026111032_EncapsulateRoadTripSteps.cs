using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadTripGenerator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EncapsulateRoadTripSteps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Steps_RoadTripId_Order",
                table: "Steps");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_RoadTripId_CountryCca3",
                table: "Steps",
                columns: new[] { "RoadTripId", "CountryCca3" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Steps_RoadTripId_CountryCca3",
                table: "Steps");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_RoadTripId_Order",
                table: "Steps",
                columns: new[] { "RoadTripId", "Order" },
                unique: true);
        }
    }
}
