#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace PostService.Domain.Migrations;

/// <inheritdoc />
public partial class fourth : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            "Version",
            "User",
            "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            "Version",
            "User");
    }
}