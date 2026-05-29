
using System.CommandLine;
using AzureDevOpsToPowerBI.Data;
using AzureDevOpsToPowerBI.Data.Repositories;
using AzureDevOpsToPowerBI.Manager;
using AzureDevOpsToPowerBI.Models.Entities;
using AzureDevOpsToPowerBI.Settings;
using AzureDevOpsToPowerBI.Settings.General;
using AzureDevOpsToPoweBI.Settings.TeamAndProjectSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MELLogger = Microsoft.Extensions.Logging.ILogger;

namespace AzureDevOpsToPowerBI.Commands
{
    /// <summary>
    /// CLI command: <c>devops-sync sync --mode &lt;full|date|state&gt; [--project &lt;key&gt;]</c>
    /// </summary>
    internal static class SyncCommand
    {
        internal static Command Build(
            AppSettings settings,
            TeamProjectSetting teamProjectSetting,
            ILoggerFactory loggerFactory)
        {
            var modeOption = new Option<string>(
                name: "--mode",
                description: "Synchronisation mode: full, date, or state.")
            { IsRequired = true };

            modeOption.AddCompletions("full", "date", "state");

            var projectOption = new Option<string?>(
                name: "--project",
                description: "Optional. Only sync the project with this key. Syncs all projects when omitted.");

            var command = new Command("sync", "Synchronise Azure DevOps work items to the local database.");
            command.AddOption(modeOption);
            command.AddOption(projectOption);

            command.SetHandler(async (string mode, string? projectFilter) =>
            {
                var logger = loggerFactory.CreateLogger("SyncCommand");

                if (!new[] { "full", "date", "state" }.Contains(mode, StringComparer.OrdinalIgnoreCase))
                {
                    logger.LogError("Invalid --mode '{Mode}'. Accepted values: full, date, state.", mode);
                    Environment.Exit(1);
                    return;
                }

                logger.LogInformation("Starting sync. Mode: {Mode}, Project filter: {Filter}",
                    mode, projectFilter ?? "all");

                var factory = new DbContextFactory(settings.StorageProvider, settings.StorageConnectionString);
                await using var db = factory.Create();
                await db.Database.MigrateAsync();

                var httpFactory  = new DefaultHttpClientFactory();
                var repo         = new WorkItemRepository(db, loggerFactory.CreateLogger<WorkItemRepository>());

                var projects = teamProjectSetting.Projects
                    .Where(p => projectFilter is null ||
                                p.ProjectKey.Equals(projectFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (projects.Count == 0)
                {
                    logger.LogWarning("No matching projects found for filter '{Filter}'.", projectFilter);
                    return;
                }

                // Step 1 — pre-sync cleanup (all projects together for full/date)
                await DeleteBeforeSyncAsync(mode, projects, repo, settings, logger);

                // Step 2 — sync each project
                foreach (var project in projects)
                {
                    string tfsUri = string.IsNullOrEmpty(project.TfsUri) ? settings.TfsUri : project.TfsUri;
                    string pat    = string.IsNullOrEmpty(project.PersonalAccessToken) ? settings.PersonalAccessToken : project.PersonalAccessToken;

                    try
                    {
                        await SyncProjectAsync(mode, project, tfsUri, pat, settings, repo, httpFactory, loggerFactory);
                        logger.LogInformation("{Project} sync complete.", project.ProjectKey);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Sync failed for project {Project}.", project.ProjectKey);
                        Environment.Exit(1);
                    }
                }

                logger.LogInformation("All projects synced successfully.");

            }, modeOption, projectOption);

            return command;
        }

        private static async Task DeleteBeforeSyncAsync(
            string mode,
            IEnumerable<TeamProjectSettings> projects,
            IWorkItemRepository repo,
            AppSettings settings,
            MELLogger logger)
        {
            logger.LogInformation("Cleaning data before sync (mode: {Mode}).", mode);

            foreach (var project in projects)
            {
                if (mode.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    if (!DateTime.TryParse(settings.WorkItemSyncDate, out var syncDate))
                    {
                        logger.LogError("WorkItemSyncDate is not a valid date: {Date}", settings.WorkItemSyncDate);
                        Environment.Exit(1);
                    }

                    await repo.DeleteByDateAsync<UserStory>(project.ProjectKey, syncDate);
                    await repo.DeleteByDateAsync<Bug>(project.ProjectKey, syncDate);
                    await repo.DeleteByDateAsync<TfsTask>(project.ProjectKey, syncDate);
                    await repo.DeleteByDateAsync<Feature>(project.ProjectKey, syncDate);
                }
                else if (mode.Equals("state", StringComparison.OrdinalIgnoreCase))
                {
                    await repo.DeleteNonClosedAsync<UserStory>(project.ProjectKey);
                    await repo.DeleteNonClosedAsync<Bug>(project.ProjectKey);
                    await repo.DeleteNonClosedAsync<TfsTask>(project.ProjectKey);
                    await repo.DeleteNonClosedAsync<Feature>(project.ProjectKey);
                }
                else // full
                {
                    await repo.DeleteAllAsync<UserStory>(project.ProjectKey);
                    await repo.DeleteAllAsync<Bug>(project.ProjectKey);
                    await repo.DeleteAllAsync<TfsTask>(project.ProjectKey);
                    await repo.DeleteAllAsync<Feature>(project.ProjectKey);
                }
            }
        }

        private static async Task SyncProjectAsync(
            string mode,
            TeamProjectSettings project,
            string tfsUri, string pat,
            AppSettings settings,
            IWorkItemRepository repo,
            IHttpClientFactory httpFactory,
            ILoggerFactory loggerFactory)
        {
            var usManager  = new UserStoriesManager(httpFactory, loggerFactory.CreateLogger<UserStoriesManager>());
            var bugManager = new BugManager(httpFactory, loggerFactory.CreateLogger<BugManager>());
            var taskMgr    = new TaskManager(httpFactory, loggerFactory.CreateLogger<TaskManager>());
            var areaMgr    = new AreaManager(httpFactory, loggerFactory.CreateLogger<AreaManager>());
            var iterMgr    = new IterationManager(httpFactory, loggerFactory.CreateLogger<IterationManager>());
            var capMgr     = new SprintCapacityManager(httpFactory, loggerFactory.CreateLogger<SprintCapacityManager>());
            var featureMgr = new FeatureManager(httpFactory, loggerFactory.CreateLogger<FeatureManager>());

            // Always sync supporting data
            var areas = await areaMgr.GetAreasAsync(tfsUri, pat,
                project.ProjectKey, project.ProjectName, project.AreaPath);
            await repo.UpsertAreasAsync(areas);

            var iterations = await iterMgr.GetTfsIterationsAsync(tfsUri, pat,
                project.ProjectKey, project.ProjectName, project.TeamName, project.IterationLevel);
            await repo.UpsertIterationsAsync(iterations);

            var capacity = await capMgr.GetCapacityAsync(tfsUri, pat,
                project.ProjectKey, project.ProjectName, project.TeamName);
            await repo.UpsertSprintCapacitiesAsync(capacity);

            if (!mode.Equals("state", StringComparison.OrdinalIgnoreCase))
            {
                // full or date: insert fresh data
                var userStories = await usManager.GetTfsUserStoriesAsync(tfsUri, pat,
                    project.ProjectKey, project.ProjectName, project.AreaPath,
                    settings.WorkItemSyncDate, settings.Tags);
                await repo.UpsertWorkItemsAsync(userStories);

                var tasks = await taskMgr.GetTfsTasksAsync(tfsUri, pat,
                    project.ProjectKey, project.ProjectName, project.AreaPath, settings.WorkItemSyncDate);
                await repo.UpsertWorkItemsAsync(tasks);

                var bugs = await bugManager.GetBugsAsync(tfsUri, pat,
                    project.ProjectKey, project.ProjectName, project.AreaPath, settings.WorkItemSyncDate);
                await repo.UpsertWorkItemsAsync(bugs);

                var features = await featureMgr.GetTfsFeaturesAsync(tfsUri, pat,
                    project.ProjectKey, project.ProjectName, project.AreaPath,
                    settings.WorkItemSyncDate, settings.Tags);
                await repo.UpsertWorkItemsAsync(features);
            }
            else
            {
                // state: upsert non-closed
                var userStories = await usManager.GetTfsUserStoriesAsync(tfsUri, pat,
                    project.ProjectKey, project.ProjectName, project.AreaPath,
                    settings.WorkItemSyncDate, settings.Tags);
                await repo.UpsertWorkItemsAsync(userStories);

                var tasks = await taskMgr.GetTfsTasksAsync(tfsUri, pat,
                    project.ProjectKey, project.ProjectName, project.AreaPath, settings.WorkItemSyncDate);
                await repo.UpsertWorkItemsAsync(tasks);

                var bugs = await bugManager.GetBugsAsync(tfsUri, pat,
                    project.ProjectKey, project.ProjectName, project.AreaPath, settings.WorkItemSyncDate);
                await repo.UpsertWorkItemsAsync(bugs);

                var features = await featureMgr.GetTfsFeaturesAsync(tfsUri, pat,
                    project.ProjectKey, project.ProjectName, project.AreaPath,
                    settings.WorkItemSyncDate, settings.Tags);
                await repo.UpsertWorkItemsAsync(features);
            }

            // TODO Remove
            //await repo.EnsureProjectProfileAsync(project.ProjectKey);
        }
    }

    /// <summary>
    /// Simple <see cref="IHttpClientFactory"/> implementation that creates a new client per call.
    /// In a DI container this would be replaced by the built-in factory.
    /// </summary>
    internal class DefaultHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new();
    }
}
