using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exercises.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageAndVideoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "exercises",
                table: "Exercises",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                schema: "exercises",
                table: "Exercises",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "exercises",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                schema: "exercises",
                table: "Exercises");
        }
    }
}
