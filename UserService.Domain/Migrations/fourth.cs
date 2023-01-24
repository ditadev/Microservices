#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace UserService.Model.Migrations;

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

        migrationBuilder.CreateTable(
            "IntegrationEventOutbox",
            table => new
            {
                ID = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Event = table.Column<string>("text", nullable: false),
                Data = table.Column<string>("text", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_IntegrationEventOutbox", x => x.ID); });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "IntegrationEventOutbox");

        migrationBuilder.DropColumn(
            "Version",
            "User");
    }
}