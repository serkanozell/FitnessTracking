using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutSessions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutSessionsFilteredIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_UserId_Date",
                schema: "workoutsessions",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_WorkoutProgramSplitId",
                schema: "workoutsessions",
                table: "WorkoutSessions");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_UserId_Date",
                schema: "workoutsessions",
                table: "WorkoutSessions",
                columns: new[] { "UserId", "Date" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_WorkoutProgramId",
                schema: "workoutsessions",
                table: "WorkoutSessions",
                column: "WorkoutProgramId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_WorkoutProgramSplitId",
                schema: "workoutsessions",
                table: "WorkoutSessions",
                column: "WorkoutProgramSplitId",
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_UserId_Date",
                schema: "workoutsessions",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_WorkoutProgramId",
                schema: "workoutsessions",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_WorkoutProgramSplitId",
                schema: "workoutsessions",
                table: "WorkoutSessions");

            migrationBuilder.EnsureSchema(
                name: "outbox");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_UserId_Date",
                schema: "workoutsessions",
                table: "WorkoutSessions",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_WorkoutProgramSplitId",
                schema: "workoutsessions",
                table: "WorkoutSessions",
                column: "WorkoutProgramSplitId");
        }
    }
}
