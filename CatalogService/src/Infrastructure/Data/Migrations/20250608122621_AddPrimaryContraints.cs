using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrimaryContraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StationRoute",
                table: "StationRoute");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StationRoute",
                table: "StationRoute",
                columns: new[] { "StationId", "RouteId", "EntryStationId", "DestinationStationId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StationRoute",
                table: "StationRoute");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StationRoute",
                table: "StationRoute",
                columns: new[] { "StationId", "RouteId" });
        }
    }
}
