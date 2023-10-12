using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seamstress.Persistence.Migrations
{
    public partial class UserFirstName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AspNetUsers",
                newName: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersUser_UserId",
                table: "OrdersUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersUser_AspNetUsers_UserId",
                table: "OrdersUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdersUser_AspNetUsers_UserId",
                table: "OrdersUser");

            migrationBuilder.DropIndex(
                name: "IX_OrdersUser_UserId",
                table: "OrdersUser");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "AspNetUsers",
                newName: "Name");
        }
    }
}
