using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutSessions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addworkoutprogramsplitidtosession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkoutProgramSplitId",
                schema: "workoutsessions",
                table: "WorkoutSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_WorkoutProgramSplitId",
                schema: "workoutsessions",
                table: "WorkoutSessions",
                column: "WorkoutProgramSplitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_WorkoutProgramSplitId",
                schema: "workoutsessions",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "WorkoutProgramSplitId",
                schema: "workoutsessions",
                table: "WorkoutSessions");
        }
    }
}
