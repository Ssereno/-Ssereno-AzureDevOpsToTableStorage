using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI.Models.Entities
{
    /// <summary>
    /// Database entity representing an Azure DevOps area node.
    /// Primary key is the composite natural key {ProjectKey, AreaSK}.
    /// </summary>
    public class Area
    {
        /// <summary>Project key — links this area to the configured project.</summary>
        public string ProjectKey { get; set; } = string.Empty;

        /// <summary>Area surrogate key from the Analytics OData feed. Part of the composite primary key.</summary>
        [JsonPropertyName("AreaSK")]
        public string AreaSK { get; set; } = string.Empty;

        [JsonPropertyName("AreaPath")]
        public string? AreaPath { get; set; }

        [JsonPropertyName("AreaId")]
        public string? AreaId { get; set; }
    }
}
