using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostService.Domain.Migrations
{
    /// <inheritdoc />
    public partial class fifth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
