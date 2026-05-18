namespace AzureDevOpsToPowerBI.Models.Entities
{
    /// <summary>
    /// Base class for all Azure DevOps work item entities persisted in the database.
    /// Contains the common fields shared across UserStory, Bug, and TfsTask.
    /// </summary>
    public abstract class WorkItemBase
    {
        /// <summary>The Azure DevOps work item identifier.</summary>
        public int WorkItemId { get; set; }

        /// <summary>
        /// The project key from configuration — identifies which project this item belongs to.
        /// Replaces the Azure Table Storage PartitionKey.
        /// </summary>
        public string ProjectKey { get; set; } = string.Empty;

        /// <summary>Work item title.</summary>
        public string? Title { get; set; }

        /// <summary>Current workflow state (e.g. Active, Closed, Resolved).</summary>
        public string? State { get; set; }

        /// <summary>Area surrogate key from the Analytics OData feed.</summary>
        public string? AreaSK { get; set; }

        /// <summary>Iteration surrogate key from the Analytics OData feed.</summary>
        public string? IterationSK { get; set; }

        /// <summary>Date the work item was created in Azure DevOps.</summary>
        public string? CreatedDate { get; set; }

        /// <summary>Effective activated date (derived from CompletedDate / ActivatedDate).</summary>
        public string? ActivatedDate { get; set; }

        /// <summary>Effective closed date (derived from CompletedDate / ClosedDate).</summary>
        public string? ClosedDate { get; set; }

        /// <summary>Effective resolved date (derived from CompletedDate / ClosedDate / ResolvedDate).</summary>
        public string? ResolvedDate { get; set; }

        /// <summary>Raw completed date returned by the Analytics API.</summary>
        public string? CompletedDate { get; set; }

        /// <summary>Work item ID of the parent (epic, user story, etc.).</summary>
        public int? ParentWorkItemId { get; set; }

        /// <summary>Semicolon-separated list of tag names.</summary>
        public string? TagNames { get; set; }
    }
}
