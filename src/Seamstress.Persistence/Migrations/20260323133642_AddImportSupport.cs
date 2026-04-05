using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Seamstress.Persistence.Migrations
{
    public partial class AddImportSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeasurementsDescription",
                table: "Items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalePlatformId",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.CreateTable(
                name: "ImportMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SalePlatformId = table.Column<int>(type: "integer", nullable: false),
                    MappingsJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportMappings_SalePlatforms_SalePlatformId",
                        column: x => x.SalePlatformId,
                        principalTable: "SalePlatforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_ExternalId_SalePlatformId",
                table: "Items",
                columns: new[] { "ExternalId", "SalePlatformId" },
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Items_SalePlatformId",
                table: "Items",
                column: "SalePlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportMappings_SalePlatformId",
                table: "ImportMappings",
                column: "SalePlatformId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_SalePlatforms_SalePlatformId",
                table: "Items",
                column: "SalePlatformId",
                principalTable: "SalePlatforms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_SalePlatforms_SalePlatformId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "ImportMappings");

            migrationBuilder.DropIndex(
                name: "IX_Items_ExternalId_SalePlatformId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_SalePlatformId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MeasurementsDescription",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SalePlatformId",
                table: "Items");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
