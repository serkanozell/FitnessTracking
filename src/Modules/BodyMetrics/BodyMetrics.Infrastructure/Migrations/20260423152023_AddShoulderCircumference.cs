using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BodyMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShoulderCircumference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ShoulderCircumference",
                schema: "bodymetrics",
                table: "BodyMetrics",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShoulderCircumference",
                schema: "bodymetrics",
                table: "BodyMetrics");
        }
    }
}
