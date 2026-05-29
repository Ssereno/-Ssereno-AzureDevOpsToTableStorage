using System.Text.Json.Serialization;
using AzureDevOpsToPowerBI.Models.Entities;

namespace AzureDevOpsToPowerBI.Models.Dtos
{
    /// <summary>
    /// Data Transfer Object for deserialising a Feature from the Azure DevOps Analytics OData feed.
    /// </summary>
    public class FeatureDto
    {
        [JsonPropertyName("WorkItemId")]
        public int WorkItemId { get; set; }

        [JsonPropertyName("Title")]
        public string? Title { get; set; }

        [JsonPropertyName("State")]
        public string? State { get; set; }

        [JsonPropertyName("WorkItemType")]
        public string? WorkItemType { get; set; }

        [JsonPropertyName("StoryPoints")]
        public double? StoryPoints { get; set; }

        [JsonPropertyName("LeadTimeDays")]
        public double? LeadTimeDays { get; set; }

        [JsonPropertyName("CycleTimeDays")]
        public double? CycleTimeDays { get; set; }

        [JsonPropertyName("CreatedDate")]
        public string? CreatedDate { get; set; }

        [JsonPropertyName("ResolvedDate")]
        public string? ResolvedDate { get; set; }

        [JsonPropertyName("AreaSK")]
        public string? AreaSK { get; set; }

        [JsonPropertyName("IterationSK")]
        public string? IterationSK { get; set; }

        [JsonPropertyName("ActivatedDate")]
        public string? ActivatedDate { get; set; }

        [JsonPropertyName("ClosedDate")]
        public string? ClosedDate { get; set; }

        [JsonPropertyName("ParentWorkItemId")]
        public int? ParentWorkItemId { get; set; }

        [JsonPropertyName("TagNames")]
        public string? TagNames { get; set; }

        [JsonPropertyName("CompletedDate")]
        public string? CompletedDate { get; set; }

        /// <summary>
        /// Maps this DTO to a <see cref="Entities.Feature"/> database entity,
        /// applying effective-date logic from <see cref="InternalHelper"/>.
        /// </summary>
        public Entities.Feature ToEntity(string projectKey) => new()
        {
            WorkItemId        = WorkItemId,
            ProjectKey        = projectKey,
            Title             = Title,
            State             = State,
            StoryPoints       = StoryPoints,
            LeadTimeDays      = LeadTimeDays,
            CycleTimeDays     = CycleTimeDays,
            CreatedDate       = CreatedDate,
            AreaSK            = AreaSK,
            IterationSK       = IterationSK,
            ParentWorkItemId  = ParentWorkItemId,
            TagNames          = TagNames,
            ActivatedDate     = InternalHelper.GetEffectiveActivatedDate(CompletedDate!, ActivatedDate!),
            ResolvedDate      = InternalHelper.GetEffectiveResolutionDate(CompletedDate!, ClosedDate!, ResolvedDate!),
            ClosedDate        = InternalHelper.GetEffectiveCompletionDate(CompletedDate!, ClosedDate!)
        };
    }
}
