using System.Net.Http.Headers;
using System.Text.Json;
using AzureDevOpsToPowerBI.Models.Dtos;
using AzureDevOpsToPowerBI.Models.Entities;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsToPowerBI.Manager
{
    /// <summary>
    /// Fetches Task work items from the Azure DevOps Analytics OData feed.
    /// </summary>
    internal class TaskManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TaskManager> _logger;

        public TaskManager(IHttpClientFactory httpClientFactory, ILogger<TaskManager> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all Tasks for a project from the Analytics OData feed.
        /// </summary>
        internal async Task<List<TfsTask>> GetTfsTasksAsync(
            string tfsUri, string pat,
            string projectKey, string projectName, string areaPath,
            string syncDate)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));

            string uri = string.Format(
                "{0}/{1}/_odata/v4.0-preview/WorkItems" +
                "?$select=WorkItemId,Title,WorkItemType,State,AreaSK,IterationSK,TagNames," +
                "ParentWorkItemId,OriginalEstimate,CompletedWork,CompletedDate,CreatedDate,ClosedDate" +
                "&$filter=WorkItemType eq 'Task' and State ne 'Removed' " +
                "and startswith(Area/AreaPath,'{2}') and CreatedDate ge {3}" +
                "&$orderby=CreatedDate desc",
                tfsUri, projectName, areaPath, syncDate);

            _logger.LogInformation("Fetching Tasks for project {Project}", projectKey);

            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ODataResponse<TfsTaskDto>>(body)
                ?? throw new InvalidOperationException("Failed to deserialise Task response.");

            return result.Value.Select(dto => dto.ToEntity(projectKey)).ToList();
        }
    }
}
