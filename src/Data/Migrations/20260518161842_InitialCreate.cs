using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureDevOpsToPowerBI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    ProjectKey = table.Column<string>(type: "TEXT", nullable: false),
                    AreaSK = table.Column<string>(type: "TEXT", nullable: false),
                    AreaPath = table.Column<string>(type: "TEXT", nullable: true),
                    AreaId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => new { x.ProjectKey, x.AreaSK });
                });

            migrationBuilder.CreateTable(
                name: "Bugs",
                columns: table => new
                {
                    WorkItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectKey = table.Column<string>(type: "TEXT", nullable: false),
                    Severity = table.Column<string>(type: "TEXT", nullable: true),
                    Custom_BugType = table.Column<string>(type: "TEXT", nullable: true),
                    Custom_TicketID = table.Column<string>(type: "TEXT", nullable: true),
                    Custom_TicketPriority = table.Column<string>(type: "TEXT", nullable: true),
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
                    table.PrimaryKey("PK_Bugs", x => new { x.WorkItemId, x.ProjectKey });
                });

            migrationBuilder.CreateTable(
                name: "Iterations",
                columns: table => new
                {
                    ProjectKey = table.Column<string>(type: "TEXT", nullable: false),
                    IterationSK = table.Column<string>(type: "TEXT", nullable: false),
                    IterationName = table.Column<string>(type: "TEXT", nullable: true),
                    IterationPath = table.Column<string>(type: "TEXT", nullable: true),
                    StartDate = table.Column<string>(type: "TEXT", nullable: true),
                    EndDate = table.Column<string>(type: "TEXT", nullable: true),
                    SprintHoursCapacity = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iterations", x => new { x.ProjectKey, x.IterationSK });
                });

            migrationBuilder.CreateTable(
                name: "ProjectProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectKey = table.Column<string>(type: "TEXT", nullable: false),
                    AreaSK = table.Column<string>(type: "TEXT", nullable: true),
                    AreaPath = table.Column<string>(type: "TEXT", nullable: true),
                    IterationName = table.Column<string>(type: "TEXT", nullable: true),
                    IterationSK = table.Column<string>(type: "TEXT", nullable: true),
                    Velocity = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectEffort = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectStartDate = table.Column<string>(type: "TEXT", nullable: true),
                    ProjectEndDate = table.Column<string>(type: "TEXT", nullable: true),
                    ProjectRealEndDate = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SprintCapacities",
                columns: table => new
                {
                    ProjectKey = table.Column<string>(type: "TEXT", nullable: false),
                    IterationSK = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectName = table.Column<string>(type: "TEXT", nullable: true),
                    SprintName = table.Column<string>(type: "TEXT", nullable: true),
                    SprintPath = table.Column<string>(type: "TEXT", nullable: true),
                    SprintHoursCapacity = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SprintCapacities", x => new { x.ProjectKey, x.IterationSK });
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    WorkItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectKey = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalEstimate = table.Column<decimal>(type: "TEXT", nullable: true),
                    CompletedWork = table.Column<decimal>(type: "TEXT", nullable: true),
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
                    table.PrimaryKey("PK_Tasks", x => new { x.WorkItemId, x.ProjectKey });
                });

            migrationBuilder.CreateTable(
                name: "UserStories",
                columns: table => new
                {
                    WorkItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectKey = table.Column<string>(type: "TEXT", nullable: false),
                    StoryPoints = table.Column<decimal>(type: "TEXT", nullable: true),
                    LeadTimeDays = table.Column<decimal>(type: "TEXT", nullable: true),
                    CycleTimeDays = table.Column<decimal>(type: "TEXT", nullable: true),
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
                    table.PrimaryKey("PK_UserStories", x => new { x.WorkItemId, x.ProjectKey });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Bugs");

            migrationBuilder.DropTable(
                name: "Iterations");

            migrationBuilder.DropTable(
                name: "ProjectProfiles");

            migrationBuilder.DropTable(
                name: "SprintCapacities");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "UserStories");
        }
    }
}
