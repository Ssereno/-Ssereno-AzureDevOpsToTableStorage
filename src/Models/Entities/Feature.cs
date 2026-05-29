namespace AzureDevOpsToPowerBI.Models.Entities
{
    /// <summary>
    /// Database entity for a Feature work item.
    /// Inherits common fields from <see cref="WorkItemBase"/>.
    /// </summary>
    public class Feature : WorkItemBase
    {
        /// <summary>Story points estimate.</summary>
        public double? StoryPoints { get; set; }

        /// <summary>Lead time in days (from creation to closure).</summary>
        public double? LeadTimeDays { get; set; }

        /// <summary>Cycle time in days (from activation to closure).</summary>
        public double? CycleTimeDays { get; set; }
    }
}
