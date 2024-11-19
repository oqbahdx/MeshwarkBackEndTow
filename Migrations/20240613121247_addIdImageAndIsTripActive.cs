using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meshwark.Migrations
{
    /// <inheritdoc />
    public partial class addIdImageAndIsTripActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdImage",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTripActive",
                table: "Users",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdImage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsTripActive",
                table: "Users");
        }
    }
}
