namespace AzureDevOpsToPowerBI.Models.Entities
{
    /// <summary>
    /// Database entity for a User Story work item.
    /// Inherits common fields from <see cref="WorkItemBase"/>.
    /// </summary>
    public class UserStory : WorkItemBase
    {
        /// <summary>Story points estimate.</summary>
        public decimal? StoryPoints { get; set; }

        /// <summary>Lead time in days (from creation to closure).</summary>
        public decimal? LeadTimeDays { get; set; }

        /// <summary>Cycle time in days (from activation to closure).</summary>
        public decimal? CycleTimeDays { get; set; }
    }
}
