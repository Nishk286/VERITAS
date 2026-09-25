using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VERITAS.Migrations
{
    /// <inheritdoc />
    public partial class AddHeadlineAndSourceUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Headline",
                table: "DetectionResults",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "DetectionResults",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Headline",
                table: "DetectionResults");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "DetectionResults");
        }
    }
}
