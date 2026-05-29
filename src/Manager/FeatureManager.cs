using System.Net.Http.Headers;
using System.Text.Json;
using AzureDevOpsToPowerBI.Models.Dtos;
using AzureDevOpsToPowerBI.Models.Entities;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsToPowerBI.Manager
{
    /// <summary>
    /// Fetches Feature work items from the Azure DevOps Analytics OData feed.
    /// </summary>
    internal class FeatureManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FeatureManager> _logger;

        public FeatureManager(IHttpClientFactory httpClientFactory, ILogger<FeatureManager> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all Features for a project from the Analytics OData feed.
        /// </summary>
        /// <param name="tfsUri">The Azure DevOps organisation URI.</param>
        /// <param name="pat">Personal Access Token for authentication.</param>
        /// <param name="projectKey">Local project key (used as ProjectKey on the entity).</param>
        /// <param name="projectName">Azure DevOps project name (appears in the URL).</param>
        /// <param name="areaPath">Area path filter (e.g. FMM\FMM).</param>
        /// <param name="syncDate">Earliest creation date to sync.</param>
        /// <param name="tags">Tags to exclude from results.</param>
        internal async Task<List<Feature>> GetTfsFeaturesAsync(
            string tfsUri, string pat,
            string projectKey, string projectName, string areaPath,
            string syncDate, List<string> tags)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));

            string tagFilter = InternalHelper.GetTagFilter(tags);
            string notTagFilter = string.IsNullOrEmpty(tagFilter) ? "false" : tagFilter;

            string uri = string.Format(
                "{0}/{1}/_odata/v4.0-preview/WorkItems" +
                "?$select=WorkItemId,Title,WorkItemType,State,StoryPoints,LeadTimeDays,CycleTimeDays," +
                "CreatedDate,ResolvedDate,AreaSK,IterationSK,ActivatedDate,ClosedDate,CompletedDate,ParentWorkItemId,TagNames" +
                "&$filter=WorkItemType eq 'Feature' and State ne 'Removed' " +
                "and startswith(Area/AreaPath,'{2}') and not {3} and CreatedDate ge {4}" +
                "&$orderby=CreatedDate desc",
                tfsUri, projectName, areaPath, notTagFilter, syncDate);

            _logger.LogInformation("Fetching Features for project {Project}", projectKey);

            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ODataResponse<FeatureDto>>(body)
                ?? throw new InvalidOperationException("Failed to deserialise Feature response.");

            return result.Value.Select(dto => dto.ToEntity(projectKey)).ToList();
        }
    }
}
