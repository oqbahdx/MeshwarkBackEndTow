using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meshwark.Migrations
{
    /// <inheritdoc />
    public partial class updateTripModelNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "TripType",
                table: "Trips",
                newName: "Time");

            migrationBuilder.AddColumn<string>(
                name: "Day",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Month",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Trips",
                newName: "TripType");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Trips",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
