using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureDevOpsToPowerBI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStoryPointsToBugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "StoryPoints",
                table: "Bugs",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoryPoints",
                table: "Bugs");
        }
    }
}
