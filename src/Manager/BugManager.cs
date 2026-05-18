using System.Net.Http.Headers;
using System.Text.Json;
using AzureDevOpsToPowerBI.Models.Dtos;
using AzureDevOpsToPowerBI.Models.Entities;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsToPowerBI.Manager
{
    /// <summary>
    /// Fetches Bug work items from the Azure DevOps Analytics OData feed.
    /// </summary>
    internal class BugManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BugManager> _logger;

        public BugManager(IHttpClientFactory httpClientFactory, ILogger<BugManager> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all Bugs for a project from the Analytics OData feed.
        /// </summary>
        internal async Task<List<Bug>> GetBugsAsync(
            string tfsUri, string pat,
            string projectKey, string projectName, string areaPath,
            string syncDate)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));

            string uri = string.Format(
                "{0}/{1}/_odata/v4.0-preview/WorkItems" +
                "?$Select=WorkItemId,Title,State,AreaSK,IterationSK,CreatedDate,ActivatedDate," +
                "ClosedDate,ResolvedDate,CompletedDate,Severity,Custom_TicketID,Custom_TicketPriority,Custom_BugType" +
                "&$filter=WorkItemType eq 'Bug' and startswith(Area/AreaPath,'{2}') and CreatedDate ge {3}" +
                "&$orderby=CreatedDate desc",
                tfsUri, projectName, areaPath, syncDate);

            _logger.LogInformation("Fetching Bugs for project {Project}", projectKey);

            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ODataResponse<BugDto>>(body)
                ?? throw new InvalidOperationException("Failed to deserialise Bug response.");

            return result.Value.Select(dto => dto.ToEntity(projectKey)).ToList();
        }
    }
}
