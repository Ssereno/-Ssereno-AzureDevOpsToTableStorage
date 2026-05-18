using System.Text.Json.Serialization;

namespace AzureDevOpsToPowerBI.Manager
{
    /// <summary>
    /// Generic wrapper for Azure DevOps Analytics OData responses.
    /// </summary>
    internal class ODataResponse<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; } = [];
    }
}
