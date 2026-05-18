using System.CommandLine;
using System.Configuration;
using AzureDevOpsToPowerBI.Commands;
using AzureDevOpsToPowerBI.Logging;
using AzureDevOpsToPowerBI.Settings;
using AzureDevOpsToPowerBI.Settings.Logging;
using AzureDevOpsToPoweBI.Settings.TeamAndProjectSettings;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Application entry point.
    /// Initialises configuration, logger, and CLI command routing.
    /// No Console.ReadLine() calls — fully compatible with Task Scheduler and CI pipelines.
    /// </summary>
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            // --- 1. Load configuration ---
            GeneralSetting generalSection;
            TeamProjectSetting teamProjectSection;
            LoggingSetting loggingSection;

            try
            {
                generalSection     = (GeneralSetting)ConfigurationManager.GetSection("General")
                    ?? throw new ConfigurationErrorsException("Missing <General> section in App.config.");
                teamProjectSection = (TeamProjectSetting)ConfigurationManager.GetSection("TeamProject")
                    ?? throw new ConfigurationErrorsException("Missing <TeamProject> section in App.config.");
                loggingSection     = (LoggingSetting)ConfigurationManager.GetSection("Logging")
                    ?? throw new ConfigurationErrorsException("Missing <Logging> section in App.config.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[FATAL] Failed to load App.config: {ex.Message}");
                return 1;
            }

            // --- 2. Initialise logger ---
            try
            {
                AppLoggerFactory.Configure(loggingSection.Log);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[FATAL] Failed to configure logger: {ex.Message}");
                return 1;
            }

            // Wrap Serilog in Microsoft.Extensions.Logging for injection into managers
            using var loggerFactory = new SerilogLoggerFactory(Log.Logger);
            var startupLogger = loggerFactory.CreateLogger<Program>();

            try
            {
                // --- 3. Build AppSettings ---
                var settings = new AppSettings
                {
                    TfsUri                  = generalSection.Connection.TfsUri,
                    PersonalAccessToken     = generalSection.Connection.PersonalAccessToken,
                    WorkItemSyncDate        = generalSection.Connection.WorkItemSyncDate,
                    StorageProvider         = generalSection.Connection.Storage.Provider,
                    StorageConnectionString = generalSection.Connection.Storage.ConnectionString,
                    Tags                    = ParseTags(generalSection.FilterTag.Tags)
                };

                startupLogger.LogInformation("devops-sync starting. Provider: {Provider}", settings.StorageProvider);

                // --- 4. Build CLI root command ---
                var rootCommand = new RootCommand("devops-sync — Azure DevOps work item synchronisation tool.");

                rootCommand.AddCommand(SyncCommand.Build(settings, teamProjectSection, loggerFactory));
                rootCommand.AddCommand(ValidateConfigCommand.Build(settings, loggerFactory.CreateLogger("ValidateConfig")));

                // --- 5. Invoke ---
                return await rootCommand.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                startupLogger.LogCritical(ex, "Unhandled exception during startup.");
                return 1;
            }
            finally
            {
                AppLoggerFactory.CloseAndFlush();
            }
        }

        private static List<string> ParseTags(string tagString)
        {
            if (string.IsNullOrWhiteSpace(tagString))
                return [];

            return tagString
                .Split(';')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();
        }
    }
}
