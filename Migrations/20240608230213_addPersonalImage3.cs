using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meshwark.Migrations
{
    /// <inheritdoc />
    public partial class addPersonalImage3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PersonalImage",
                table: "Users",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalImage",
                table: "Users");
        }
    }
}
