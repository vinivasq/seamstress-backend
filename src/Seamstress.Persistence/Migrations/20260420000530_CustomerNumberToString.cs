using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seamstress.Persistence.Migrations
{
    public partial class CustomerNumberToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Customers"" ALTER COLUMN ""Number"" TYPE text USING ""Number""::text;"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Customers"" ALTER COLUMN ""Number"" TYPE integer USING ""Number""::integer;"
            );
        }
    }
}
