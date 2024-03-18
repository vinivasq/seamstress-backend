using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Seamstress.Persistence.Migrations
{
    public partial class SalePlatformsOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SalePlatformId",
                table: "Orders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SalePlatforms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalePlatforms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SalePlatformId",
                table: "Orders",
                column: "SalePlatformId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_SalePlatforms_SalePlatformId",
                table: "Orders",
                column: "SalePlatformId",
                principalTable: "SalePlatforms",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_SalePlatforms_SalePlatformId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "SalePlatforms");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SalePlatformId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SalePlatformId",
                table: "Orders");
        }
    }
}
