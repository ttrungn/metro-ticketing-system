using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangePropertyOfFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Feedback");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Feedback",
                newName: "Content");

            migrationBuilder.AddColumn<Guid>(
                name: "StationId",
                table: "Feedback",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StationId",
                table: "Feedback");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Feedback",
                newName: "Comment");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Feedback",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
