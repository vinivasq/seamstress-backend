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
                name: "FK_ItemOrder_Sizings_AditionalSizingId",
                table: "ItemOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemsSizes",
                table: "ItemsSizes");

            migrationBuilder.DropIndex(
                name: "IX_ItemOrder_AditionalSizingId",
                table: "ItemOrder");

            migrationBuilder.DropColumn(
                name: "AditionalSizingId",
                table: "ItemOrder");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ItemsSizes",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<int>(
                name: "AditionalSizingId",
                table: "ItemOrder",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemsSizes",
                table: "ItemsSizes",
                columns: new[] { "ItemId", "SizeId" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemOrder_AditionalSizingId",
                table: "ItemOrder",
                column: "AditionalSizingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrder_Sizings_AditionalSizingId",
                table: "ItemOrder",
                column: "AditionalSizingId",
                principalTable: "Sizings",
                principalColumn: "Id");
        }
    }
}
