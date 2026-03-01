using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutSessions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConcurrencyAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "workoutsessions",
                table: "WorkoutSessions",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "workoutsessions",
                table: "WorkoutSessionExercises",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "workoutsessions",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "workoutsessions",
                table: "WorkoutSessionExercises");
        }
    }
}
