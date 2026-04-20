using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seamstress.Persistence.Migrations
{
    public partial class DedupeColorsAndUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Trim names so canonical-form comparisons match physical storage.
            migrationBuilder.Sql(@"UPDATE ""Colors"" SET ""Name"" = trim(""Name"") WHERE ""Name"" <> trim(""Name"");");

            // 2. Repoint child references from duplicate Colors to the canonical (lowest Id per
            //    case-insensitive name) row. Done in two passes per child table to avoid violating
            //    the (ItemId, ColorId) composite PK on ItemsColors when both rows already exist.
            migrationBuilder.Sql(@"
                WITH canonical AS (
                    SELECT ""Id"" AS dup_id,
                           MIN(""Id"") OVER (PARTITION BY lower(""Name"")) AS canonical_id
                    FROM ""Colors""
                )
                UPDATE ""ItemsColors"" ic
                SET ""ColorId"" = c.canonical_id
                FROM canonical c
                WHERE ic.""ColorId"" = c.dup_id
                  AND c.dup_id <> c.canonical_id
                  AND NOT EXISTS (
                      SELECT 1 FROM ""ItemsColors"" ic2
                      WHERE ic2.""ItemId"" = ic.""ItemId"" AND ic2.""ColorId"" = c.canonical_id
                  );
            ");

            migrationBuilder.Sql(@"
                WITH canonical AS (
                    SELECT ""Id"" AS dup_id,
                           MIN(""Id"") OVER (PARTITION BY lower(""Name"")) AS canonical_id
                    FROM ""Colors""
                )
                DELETE FROM ""ItemsColors"" ic
                USING canonical c
                WHERE ic.""ColorId"" = c.dup_id
                  AND c.dup_id <> c.canonical_id;
            ");

            migrationBuilder.Sql(@"
                WITH canonical AS (
                    SELECT ""Id"" AS dup_id,
                           MIN(""Id"") OVER (PARTITION BY lower(""Name"")) AS canonical_id
                    FROM ""Colors""
                )
                UPDATE ""ItemOrder"" io
                SET ""ColorId"" = c.canonical_id
                FROM canonical c
                WHERE io.""ColorId"" = c.dup_id
                  AND c.dup_id <> c.canonical_id;
            ");

            // 3. Drop the now-orphaned duplicate Color rows.
            migrationBuilder.Sql(@"
                WITH canonical AS (
                    SELECT ""Id"" AS dup_id,
                           MIN(""Id"") OVER (PARTITION BY lower(""Name"")) AS canonical_id
                    FROM ""Colors""
                )
                DELETE FROM ""Colors""
                WHERE ""Id"" IN (SELECT dup_id FROM canonical WHERE dup_id <> canonical_id);
            ");

            // 4. Prevent recurrence at the DB layer.
            migrationBuilder.Sql(@"CREATE UNIQUE INDEX ""IX_Colors_Name_lower"" ON ""Colors"" (lower(""Name""));");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Colors_Name_lower"";");
            // Data merges are not reversible.
        }
    }
}
