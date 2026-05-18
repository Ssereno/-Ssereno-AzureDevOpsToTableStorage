using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using AzureDevOpsToPowerBI.Models.Entities;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsToPowerBI.Manager
{
    /// <summary>
    /// Fetches sprint capacity data from the Azure DevOps REST API (Teams API).
    /// Uses the REST API directly, replacing the removed TFS SDK dependency.
    /// </summary>
    internal class SprintCapacityManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SprintCapacityManager> _logger;

        public SprintCapacityManager(IHttpClientFactory httpClientFactory, ILogger<SprintCapacityManager> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves sprint capacity for all team iterations using the Azure DevOps REST API.
        /// </summary>
        internal async Task<List<SprintCapacity>> GetCapacityAsync(
            string tfsUri, string pat,
            string projectKey, string projectName, string teamName)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));

            // Get team iterations
            string iterationsUri = $"{tfsUri}/{projectName}/{teamName}/_apis/work/teamsettings/iterations?api-version=7.0";
            _logger.LogInformation("Fetching sprint iterations for project {Project}, team {Team}", projectKey, teamName);

            var iterResponse = await client.GetAsync(iterationsUri);
            iterResponse.EnsureSuccessStatusCode();

            var iterBody = await iterResponse.Content.ReadAsStringAsync();
            var iterResult = JsonSerializer.Deserialize<TeamIterationsResponse>(iterBody)
                ?? throw new InvalidOperationException("Failed to deserialise team iterations.");

            var capacities = new List<SprintCapacity>();

            foreach (var iteration in iterResult.Value)
            {
                try
                {
                    double totalCapacity = await GetIterationCapacityAsync(
                        client, tfsUri, projectName, teamName, iteration.Id, iteration);

                    capacities.Add(new SprintCapacity
                    {
                        ProjectKey          = projectKey,
                        ProjectName         = projectKey,
                        SprintName          = iteration.Name,
                        SprintPath          = iteration.Path,
                        IterationSK         = iteration.Id.ToString(),
                        SprintHoursCapacity = totalCapacity
                    });

                    _logger.LogInformation("Total capacity for {Sprint}: {Hours} hours", iteration.Name, totalCapacity);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not retrieve capacity for iteration {Sprint}", iteration.Name);
                }
            }

            return capacities;
        }

        private static async Task<double> GetIterationCapacityAsync(
            HttpClient client, string tfsUri, string projectName, string teamName,
            Guid iterationId, TeamIteration iteration)
        {
            string capacityUri =
                $"{tfsUri}/{projectName}/{teamName}/_apis/work/teamsettings/iterations/{iterationId}/capacities?api-version=7.0";

            var capResponse = await client.GetAsync(capacityUri);
            if (!capResponse.IsSuccessStatusCode) return 0;

            var capBody = await capResponse.Content.ReadAsStringAsync();
            var capResult = JsonSerializer.Deserialize<TeamCapacityResponse>(capBody);
            if (capResult?.Value == null) return 0;

            if (iteration.Attributes?.StartDate == null || iteration.Attributes?.FinishDate == null)
                return 0;

            var start = iteration.Attributes.StartDate.Value;
            var end   = iteration.Attributes.FinishDate.Value;
            int workdays = CountWorkdays(start, end);

            double total = 0;
            foreach (var member in capResult.Value)
            {
                double dailyCap = member.Activities?.Sum(a => a.CapacityPerDay) ?? 0;
                int daysOff = member.DaysOff?.Sum(d => CountWorkdays(d.Start.Date, d.End.Date)) ?? 0;
                total += dailyCap * Math.Max(0, workdays - daysOff);
            }

            return total;
        }

        private static int CountWorkdays(DateTime start, DateTime end)
            => Enumerable.Range(0, (end - start).Days + 1)
                .Select(d => start.AddDays(d))
                .Count(dt => dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday);

        // --- Response models ---

        private class TeamIterationsResponse
        {
            [JsonPropertyName("value")]
            public List<TeamIteration> Value { get; set; } = [];
        }

        private class TeamIteration
        {
            [JsonPropertyName("id")]
            public Guid Id { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("path")]
            public string? Path { get; set; }

            [JsonPropertyName("attributes")]
            public TeamIterationAttributes? Attributes { get; set; }
        }

        private class TeamIterationAttributes
        {
            [JsonPropertyName("startDate")]
            public DateTime? StartDate { get; set; }

            [JsonPropertyName("finishDate")]
            public DateTime? FinishDate { get; set; }
        }

        private class TeamCapacityResponse
        {
            [JsonPropertyName("value")]
            public List<TeamMemberCapacity> Value { get; set; } = [];
        }

        private class TeamMemberCapacity
        {
            [JsonPropertyName("activities")]
            public List<Activity>? Activities { get; set; }

            [JsonPropertyName("daysOff")]
            public List<DateRange>? DaysOff { get; set; }
        }

        private class Activity
        {
            [JsonPropertyName("capacityPerDay")]
            public double CapacityPerDay { get; set; }
        }

        private class DateRange
        {
            [JsonPropertyName("start")]
            public DateTime Start { get; set; }

            [JsonPropertyName("end")]
            public DateTime End { get; set; }
        }
    }
}
