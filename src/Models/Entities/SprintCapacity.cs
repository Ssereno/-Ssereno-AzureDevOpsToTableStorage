using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI.Models.Entities
{
    /// <summary>
    /// Database entity representing the capacity of a team for a sprint.
    /// Primary key is the composite natural key {ProjectKey, IterationSK}.
    /// </summary>
    public class SprintCapacity
    {
        /// <summary>Project key — links this record to the configured project.</summary>
        public string ProjectKey { get; set; } = string.Empty;

        /// <summary>Iteration surrogate key from the Teams API. Part of the composite primary key.</summary>
        [JsonPropertyName("IterationSK")]
        public string IterationSK { get; set; } = string.Empty;

        [JsonPropertyName("ProjectName")]
        public string? ProjectName { get; set; }

        [JsonPropertyName("SprintName")]
        public string? SprintName { get; set; }

        [JsonPropertyName("SprintPath")]
        public string? SprintPath { get; set; }

        [JsonPropertyName("SprintHoursCapacity")]
        public double? SprintHoursCapacity { get; set; }
    }
}
