namespace AzureDevOpsToPowerBI.Models.Entities
{
    /// <summary>
    /// Database entity for a Task work item.
    /// Inherits common fields from <see cref="WorkItemBase"/>.
    /// </summary>
    public class TfsTask : WorkItemBase
    {
        /// <summary>Original estimate in hours.</summary>
        public decimal? OriginalEstimate { get; set; }

        /// <summary>Completed work in hours.</summary>
        public decimal? CompletedWork { get; set; }
    }
}
