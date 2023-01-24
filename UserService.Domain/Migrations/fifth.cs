#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Model.Migrations;

/// <inheritdoc />
public partial class fifth : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            "Version",
            "User");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            "Version",
            "User",
            "integer",
            nullable: false,
            defaultValue: 0);
    }
}