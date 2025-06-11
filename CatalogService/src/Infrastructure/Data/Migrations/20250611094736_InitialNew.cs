using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StationRoute_Station_DestinationStationId",
                table: "StationRoute");

            migrationBuilder.DropForeignKey(
                name: "FK_StationRoute_Station_EntryStationId",
                table: "StationRoute");

            migrationBuilder.DropIndex(
                name: "IX_StationRoute_DestinationStationId",
                table: "StationRoute");

            migrationBuilder.DropIndex(
                name: "IX_StationRoute_EntryStationId",
                table: "StationRoute");

            migrationBuilder.DropColumn(
                name: "DestinationStationId",
                table: "StationRoute");

            migrationBuilder.DropColumn(
                name: "EntryStationId",
                table: "StationRoute");

            migrationBuilder.RenameColumn(
                name: "Length",
                table: "StationRoute",
                newName: "DistanceToNext");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DistanceToNext",
                table: "StationRoute",
                newName: "Length");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationStationId",
                table: "StationRoute",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "EntryStationId",
                table: "StationRoute",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_StationRoute_DestinationStationId",
                table: "StationRoute",
                column: "DestinationStationId");

            migrationBuilder.CreateIndex(
                name: "IX_StationRoute_EntryStationId",
                table: "StationRoute",
                column: "EntryStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_StationRoute_Station_DestinationStationId",
                table: "StationRoute",
                column: "DestinationStationId",
                principalTable: "Station",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StationRoute_Station_EntryStationId",
                table: "StationRoute",
                column: "EntryStationId",
                principalTable: "Station",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
