using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using AzureDevOpsToPowerBI.Models.Entities;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsToPowerBI.Manager
{
    /// <summary>
    /// Fetches Iteration (sprint) data from the Azure DevOps Analytics OData feed.
    /// </summary>
    internal class IterationManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IterationManager> _logger;

        public IterationManager(IHttpClientFactory httpClientFactory, ILogger<IterationManager> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all Iterations at the default level (1) for a project.
        /// </summary>
        internal Task<List<Iteration>> GetTfsIterationsAsync(
            string tfsUri, string pat,
            string projectKey, string projectName, string teamName)
            => GetTfsIterationsAsync(tfsUri, pat, projectKey, projectName, teamName, 1);

        /// <summary>
        /// Retrieves all Iterations at the specified level for a project.
        /// </summary>
        internal async Task<List<Iteration>> GetTfsIterationsAsync(
            string tfsUri, string pat,
            string projectKey, string projectName, string teamName,
            int iterationLevel)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));

            string uri = string.Format(
                "{0}/{1}/_odata/v4.0-preview/Iterations" +
                "?$select=IterationName,IterationPath,IterationSK,StartDate,EndDate" +
                "&$filter=IterationLevel{3} eq '{2}'",
                tfsUri, projectName, projectKey, iterationLevel);

            _logger.LogInformation("Fetching Iterations for project {Project}", projectKey);

            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IterationODataResponse>(body)
                ?? throw new InvalidOperationException("Failed to deserialise Iteration response.");

            return result.Value.Select(i =>
            {
                i.ProjectKey = projectKey;
                return i;
            }).ToList();
        }

        private class IterationODataResponse
        {
            [JsonPropertyName("value")]
            public List<Iteration> Value { get; set; } = [];
        }
    }
}
