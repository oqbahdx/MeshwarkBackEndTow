using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meshwark.Migrations
{
    /// <inheritdoc />
    public partial class updateTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "Month",
                table: "Trips",
                newName: "RiderIds");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Trips",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "RiderIds",
                table: "Trips",
                newName: "Month");

            migrationBuilder.AddColumn<string>(
                name: "Day",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
