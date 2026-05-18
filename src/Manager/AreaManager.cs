using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using AzureDevOpsToPowerBI.Models.Entities;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsToPowerBI.Manager
{
    /// <summary>
    /// Fetches Area nodes from the Azure DevOps Analytics OData feed.
    /// </summary>
    internal class AreaManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AreaManager> _logger;

        public AreaManager(IHttpClientFactory httpClientFactory, ILogger<AreaManager> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all Areas matching the given area path for a project.
        /// </summary>
        internal async Task<List<Area>> GetAreasAsync(
            string tfsUri, string pat,
            string projectKey, string projectName, string areaPath)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));

            string uri = string.Format(
                "{0}/{1}/_odata/v4.0-preview/Areas" +
                "?$filter=AreaPath eq '{2}'" +
                "&$select=AreaId,AreaSK,AreaPath",
                tfsUri, projectName, areaPath);

            _logger.LogInformation("Fetching Areas for project {Project}", projectKey);

            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AreaODataResponse>(body)
                ?? throw new InvalidOperationException("Failed to deserialise Area response.");

            return result.Value.Select(a =>
            {
                a.ProjectKey = projectKey;
                return a;
            }).ToList();
        }

        private class AreaODataResponse
        {
            [JsonPropertyName("value")]
            public List<Area> Value { get; set; } = [];
        }
    }
}
