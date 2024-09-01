using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seamstress.Persistence.Migrations
{
    public partial class DropSetIdFromItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Sets_SetId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_SetId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SetId",
                table: "Items");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SetId",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_SetId",
                table: "Items",
                column: "SetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Sets_SetId",
                table: "Items",
                column: "SetId",
                principalTable: "Sets",
                principalColumn: "Id");
        }
    }
}
