using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureDevOpsToPowerBI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    WorkItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectKey = table.Column<string>(type: "TEXT", nullable: false),
                    StoryPoints = table.Column<double>(type: "REAL", nullable: true),
                    LeadTimeDays = table.Column<double>(type: "REAL", nullable: true),
                    CycleTimeDays = table.Column<double>(type: "REAL", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    AreaSK = table.Column<string>(type: "TEXT", nullable: true),
                    IterationSK = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<string>(type: "TEXT", nullable: true),
                    ActivatedDate = table.Column<string>(type: "TEXT", nullable: true),
                    ClosedDate = table.Column<string>(type: "TEXT", nullable: true),
                    ResolvedDate = table.Column<string>(type: "TEXT", nullable: true),
                    CompletedDate = table.Column<string>(type: "TEXT", nullable: true),
                    ParentWorkItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    TagNames = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => new { x.WorkItemId, x.ProjectKey });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Features");
        }
    }
}
