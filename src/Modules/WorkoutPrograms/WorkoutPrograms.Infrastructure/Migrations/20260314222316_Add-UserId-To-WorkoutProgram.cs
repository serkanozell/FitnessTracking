using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutPrograms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToWorkoutProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "workoutprograms",
                table: "WorkoutPrograms",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "workoutprograms",
                table: "WorkoutPrograms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPrograms_UserId",
                schema: "workoutprograms",
                table: "WorkoutPrograms",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkoutPrograms_UserId",
                schema: "workoutprograms",
                table: "WorkoutPrograms");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "workoutprograms",
                table: "WorkoutPrograms");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "workoutprograms",
                table: "WorkoutPrograms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
