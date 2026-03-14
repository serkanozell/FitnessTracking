using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutSessions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToWorkoutSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "workoutsessions",
                table: "WorkoutSessions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "workoutsessions",
                table: "WorkoutSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_UserId",
                schema: "workoutsessions",
                table: "WorkoutSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_UserId",
                schema: "workoutsessions",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "workoutsessions",
                table: "WorkoutSessions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "workoutsessions",
                table: "WorkoutSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
