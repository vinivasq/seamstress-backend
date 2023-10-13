using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seamstress.Persistence.Migrations
{
    public partial class OrderExecutorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExecutorId",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutorId",
                table: "Orders");
        }
    }
}
