namespace AzureDevOpsToPowerBI.Models.Entities
{
    /// <summary>
    /// Database entity for a Bug work item.
    /// Inherits common fields from <see cref="WorkItemBase"/>.
    /// </summary>
    public class Bug : WorkItemBase
    {
        /// <summary>Bug severity (e.g. 1 - Critical, 2 - High).</summary>
        public string? Severity { get; set; }

        /// <summary>Custom field: bug classification type.</summary>
        public string? Custom_BugType { get; set; }

        /// <summary>Custom field: external ticket/issue identifier.</summary>
        public string? Custom_TicketID { get; set; }

        /// <summary>Custom field: external ticket priority.</summary>
        public string? Custom_TicketPriority { get; set; }
    }
}
