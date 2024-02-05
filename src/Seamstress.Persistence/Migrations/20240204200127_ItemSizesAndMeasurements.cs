using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Seamstress.Persistence.Migrations
{
    public partial class ItemSizesAndMeasurements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrder_Sizes_SizeId",
                table: "ItemOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrder_Sizings_AditionalSizingId",
                table: "ItemOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemsSizes",
                table: "ItemsSizes");

            migrationBuilder.RenameColumn(
                name: "AditionalSizingId",
                table: "ItemOrder",
                newName: "ItemSizeId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemOrder_AditionalSizingId",
                table: "ItemOrder",
                newName: "IX_ItemOrder_ItemSizeId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ItemsSizes",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "SizeId",
                table: "ItemOrder",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemsSizes",
                table: "ItemsSizes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ItemSizeMeasurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemSizeId = table.Column<int>(type: "integer", nullable: false),
                    Measure = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSizeMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemSizeMeasurements_ItemsSizes_ItemSizeId",
                        column: x => x.ItemSizeId,
                        principalTable: "ItemsSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemsSizes_ItemId",
                table: "ItemsSizes",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSizeMeasurements_ItemSizeId",
                table: "ItemSizeMeasurements",
                column: "ItemSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrder_ItemsSizes_ItemSizeId",
                table: "ItemOrder",
                column: "ItemSizeId",
                principalTable: "ItemsSizes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrder_Sizes_SizeId",
                table: "ItemOrder",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrder_ItemsSizes_ItemSizeId",
                table: "ItemOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrder_Sizes_SizeId",
                table: "ItemOrder");

            migrationBuilder.DropTable(
                name: "ItemSizeMeasurements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemsSizes",
                table: "ItemsSizes");

            migrationBuilder.DropIndex(
                name: "IX_ItemsSizes_ItemId",
                table: "ItemsSizes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ItemsSizes");

            migrationBuilder.RenameColumn(
                name: "ItemSizeId",
                table: "ItemOrder",
                newName: "AditionalSizingId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemOrder_ItemSizeId",
                table: "ItemOrder",
                newName: "IX_ItemOrder_AditionalSizingId");

            migrationBuilder.AlterColumn<int>(
                name: "SizeId",
                table: "ItemOrder",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemsSizes",
                table: "ItemsSizes",
                columns: new[] { "ItemId", "SizeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrder_Sizes_SizeId",
                table: "ItemOrder",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrder_Sizings_AditionalSizingId",
                table: "ItemOrder",
                column: "AditionalSizingId",
                principalTable: "Sizings",
                principalColumn: "Id");
        }
    }
}
