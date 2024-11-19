using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meshwark.Migrations
{
    /// <inheritdoc />
    public partial class addPersonalImage4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalImage",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "PersonalImagePath",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalImagePath",
                table: "Users");

            migrationBuilder.AddColumn<byte[]>(
                name: "PersonalImage",
                table: "Users",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
