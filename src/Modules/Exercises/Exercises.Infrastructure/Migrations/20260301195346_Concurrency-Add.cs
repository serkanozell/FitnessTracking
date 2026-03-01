using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exercises.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConcurrencyAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "exercises",
                table: "Exercises",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "exercises",
                table: "Exercises");
        }
    }
}
