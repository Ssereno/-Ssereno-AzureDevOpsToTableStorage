using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI.Models.Entities
{
    /// <summary>
    /// Database entity representing an Azure DevOps iteration (sprint).
    /// Primary key is the composite natural key {ProjectKey, IterationSK}.
    /// </summary>
    public class Iteration
    {
        /// <summary>Project key — links this iteration to the configured project.</summary>
        public string ProjectKey { get; set; } = string.Empty;

        /// <summary>Iteration surrogate key from the Analytics OData feed. Part of the composite primary key.</summary>
        [JsonPropertyName("IterationSK")]
        public string IterationSK { get; set; } = string.Empty;

        [JsonPropertyName("IterationName")]
        public string? IterationName { get; set; }

        [JsonPropertyName("IterationPath")]
        public string? IterationPath { get; set; }

        [JsonPropertyName("StartDate")]
        public string? StartDate { get; set; }

        [JsonPropertyName("EndDate")]
        public string? EndDate { get; set; }

        /// <summary>Total sprint capacity in hours (populated from capacity data).</summary>
        public double? SprintHoursCapacity { get; set; }
    }
}
