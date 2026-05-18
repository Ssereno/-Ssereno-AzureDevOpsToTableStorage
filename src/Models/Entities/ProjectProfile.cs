namespace AzureDevOpsToPowerBI.Models.Entities
{
    /// <summary>
    /// Database entity for a manually-maintained project profile record.
    /// Not populated from Azure DevOps — filled in by hand to support Power BI reports.
    /// </summary>
    public class ProjectProfile
    {
        /// <summary>Primary key (auto-incremented by EF Core).</summary>
        public int Id { get; set; }

        /// <summary>Project key — links this profile to the configured project.</summary>
        public string ProjectKey { get; set; } = string.Empty;

        public string? AreaSK { get; set; }
        public string? AreaPath { get; set; }
        public string? IterationName { get; set; }
        public string? IterationSK { get; set; }
        public int Velocity { get; set; }
        public int ProjectEffort { get; set; }
        public string? ProjectStartDate { get; set; }
        public string? ProjectEndDate { get; set; }
        public string? ProjectRealEndDate { get; set; }
    }
}
