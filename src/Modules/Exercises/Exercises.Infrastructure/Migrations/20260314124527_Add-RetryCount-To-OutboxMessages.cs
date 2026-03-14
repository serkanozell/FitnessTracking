using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exercises.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRetryCountToOutboxMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                schema: "outbox",
                table: "OutboxMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "exercises",
                table: "Exercises",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RetryCount",
                schema: "outbox",
                table: "OutboxMessages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "exercises",
                table: "Exercises",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
