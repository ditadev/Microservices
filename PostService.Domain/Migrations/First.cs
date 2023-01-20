#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace PostService.Domain.Migrations;

/// <inheritdoc />
public partial class First : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "User",
            table => new
            {
                ID = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>("text", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_User", x => x.ID); });

        migrationBuilder.CreateTable(
            "Post",
            table => new
            {
                PostId = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Title = table.Column<string>("text", nullable: false),
                Content = table.Column<string>("text", nullable: false),
                UserId = table.Column<int>("integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Post", x => x.PostId);
                table.ForeignKey(
                    "FK_Post_User_UserId",
                    x => x.UserId,
                    "User",
                    "ID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_Post_UserId",
            "Post",
            "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Post");

        migrationBuilder.DropTable(
            "User");
    }
}