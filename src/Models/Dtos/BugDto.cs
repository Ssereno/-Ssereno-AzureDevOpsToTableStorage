using System.Text.Json.Serialization;
using AzureDevOpsToPowerBI.Models.Entities;

namespace AzureDevOpsToPowerBI.Models.Dtos
{
    /// <summary>
    /// Data Transfer Object for deserialising a Bug from the Azure DevOps Analytics OData feed.
    /// </summary>
    public class BugDto
    {
        [JsonPropertyName("WorkItemId")]
        public int WorkItemId { get; set; }

        [JsonPropertyName("Title")]
        public string? Title { get; set; }

        [JsonPropertyName("State")]
        public string? State { get; set; }

        [JsonPropertyName("AreaSK")]
        public string? AreaSK { get; set; }

        [JsonPropertyName("IterationSK")]
        public string? IterationSK { get; set; }

        [JsonPropertyName("CreatedDate")]
        public string? CreatedDate { get; set; }

        [JsonPropertyName("ActivatedDate")]
        public string? ActivatedDate { get; set; }

        [JsonPropertyName("ClosedDate")]
        public string? ClosedDate { get; set; }

        [JsonPropertyName("ResolvedDate")]
        public string? ResolvedDate { get; set; }

        [JsonPropertyName("CompletedDate")]
        public string? CompletedDate { get; set; }

        [JsonPropertyName("Severity")]
        public string? Severity { get; set; }

        [JsonPropertyName("ParentWorkItemId")]
        public string? ParentWorkItemId { get; set; }

        [JsonPropertyName("Custom_BugType")]
        public string? Custom_BugType { get; set; }

        [JsonPropertyName("Custom_TicketID")]
        public string? Custom_TicketID { get; set; }

        [JsonPropertyName("Custom_TicketPriority")]
        public string? Custom_TicketPriority { get; set; }

        /// <summary>
        /// Maps this DTO to a <see cref="Entities.Bug"/> database entity,
        /// applying effective-date logic from <see cref="InternalHelper"/>.
        /// </summary>
        public Entities.Bug ToEntity(string projectKey) => new()
        {
            WorkItemId           = WorkItemId,
            ProjectKey           = projectKey,
            Title                = Title,
            State                = State,
            AreaSK               = AreaSK,
            IterationSK          = IterationSK,
            CreatedDate          = CreatedDate,
            Severity             = Severity,
            Custom_BugType       = Custom_BugType,
            Custom_TicketID      = Custom_TicketID,
            Custom_TicketPriority = Custom_TicketPriority,
            ParentWorkItemId     = int.TryParse(ParentWorkItemId, out var pid) ? pid : null,
            ActivatedDate        = InternalHelper.GetEffectiveActivatedDate(CompletedDate!, ActivatedDate!),
            ResolvedDate         = InternalHelper.GetEffectiveResolutionDate(CompletedDate!, ClosedDate!, ResolvedDate!),
            ClosedDate           = InternalHelper.GetEffectiveCompletionDate(CompletedDate!, ClosedDate!)
        };
    }
}
