using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seamstress.Persistence.Migrations
{
    public partial class AddCustomerCreatedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.Sql(@"
                UPDATE ""Customers"" c
                SET ""CreatedAt"" = COALESCE(
                    (SELECT MIN(o.""CreatedAt"")
                     FROM ""Orders"" o
                     WHERE o.""CustomerId"" = c.""Id""),
                    NOW()
                );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Customers");
        }
    }
}
