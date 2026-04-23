using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutPrograms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToWorkoutProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "workoutprograms",
                table: "WorkoutPrograms",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "workoutprograms",
                table: "WorkoutPrograms");
        }
    }
}
