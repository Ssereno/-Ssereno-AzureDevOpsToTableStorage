using System.Text.Json.Serialization;
using AzureDevOpsToPowerBI.Models.Entities;

namespace AzureDevOpsToPowerBI.Models.Dtos
{
    /// <summary>
    /// Data Transfer Object for deserialising a Task from the Azure DevOps Analytics OData feed.
    /// </summary>
    public class TfsTaskDto
    {
        [JsonPropertyName("WorkItemId")]
        public int WorkItemId { get; set; }

        [JsonPropertyName("Title")]
        public string? Title { get; set; }

        [JsonPropertyName("State")]
        public string? State { get; set; }

        [JsonPropertyName("WorkItemType")]
        public string? WorkItemType { get; set; }

        [JsonPropertyName("AreaSK")]
        public string? AreaSK { get; set; }

        [JsonPropertyName("IterationSK")]
        public string? IterationSK { get; set; }

        [JsonPropertyName("TagNames")]
        public string? TagNames { get; set; }

        [JsonPropertyName("ParentWorkItemId")]
        public int? ParentWorkItemId { get; set; }

        [JsonPropertyName("OriginalEstimate")]
        public decimal? OriginalEstimate { get; set; }

        [JsonPropertyName("CompletedWork")]
        public decimal? CompletedWork { get; set; }

        [JsonPropertyName("CreatedDate")]
        public string? CreatedDate { get; set; }

        [JsonPropertyName("ClosedDate")]
        public string? ClosedDate { get; set; }

        [JsonPropertyName("CompletedDate")]
        public string? CompletedDate { get; set; }

        /// <summary>
        /// Maps this DTO to a <see cref="TfsTask"/> database entity.
        /// </summary>
        public TfsTask ToEntity(string projectKey) => new()
        {
            WorkItemId       = WorkItemId,
            ProjectKey       = projectKey,
            Title            = Title,
            State            = State,
            AreaSK           = AreaSK,
            IterationSK      = IterationSK,
            TagNames         = TagNames,
            ParentWorkItemId = ParentWorkItemId,
            OriginalEstimate = OriginalEstimate,
            CompletedWork    = CompletedWork,
            CreatedDate      = CreatedDate,
            ClosedDate       = ClosedDate,
            CompletedDate    = CompletedDate
        };
    }
}
