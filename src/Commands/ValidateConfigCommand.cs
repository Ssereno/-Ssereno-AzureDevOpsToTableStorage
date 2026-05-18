using System.CommandLine;
using AzureDevOpsToPowerBI.Data;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsToPowerBI.Commands
{
    /// <summary>
    /// CLI command: <c>devops-sync validate-config</c>
    /// Validates the App.config settings and exits with code 0 (valid) or 1 (invalid).
    /// </summary>
    internal static class ValidateConfigCommand
    {
        internal static Command Build(AppSettings settings, ILogger logger)
        {
            var command = new Command("validate-config", "Validates App.config settings and reports any errors.");

            command.SetHandler(() =>
            {
                bool valid = true;

                if (string.IsNullOrWhiteSpace(settings.TfsUri))
                {
                    logger.LogError("Configuration error: TfsUri is missing in <General><Connection>.");
                    valid = false;
                }

                if (string.IsNullOrWhiteSpace(settings.PersonalAccessToken))
                {
                    logger.LogError("Configuration error: PersonalAccessToken is missing in <General><Connection>.");
                    valid = false;
                }

                if (string.IsNullOrWhiteSpace(settings.StorageProvider))
                {
                    logger.LogError("Configuration error: Storage Provider is missing in <General><Connection><Storage>.");
                    valid = false;
                }

                if (!new[] { "SQLite", "AzureSql" }.Contains(settings.StorageProvider, StringComparer.OrdinalIgnoreCase))
                {
                    logger.LogError("Configuration error: Storage Provider '{Provider}' is not supported. Use SQLite or AzureSql.",
                        settings.StorageProvider);
                    valid = false;
                }

                if (string.IsNullOrWhiteSpace(settings.StorageConnectionString))
                {
                    logger.LogError("Configuration error: Storage ConnectionString is missing.");
                    valid = false;
                }

                if (string.IsNullOrWhiteSpace(settings.WorkItemSyncDate) ||
                    !DateTime.TryParse(settings.WorkItemSyncDate, out _))
                {
                    logger.LogError("Configuration error: WorkItemSyncDate is missing or not a valid date.");
                    valid = false;
                }

                // Try creating the DbContext factory to validate provider
                if (valid)
                {
                    try
                    {
                        _ = new DbContextFactory(settings.StorageProvider, settings.StorageConnectionString);
                        logger.LogInformation("Configuration is valid.");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Configuration error: could not initialise storage.");
                        valid = false;
                    }
                }

                if (!valid) Environment.Exit(1);
            });

            return command;
        }
    }
}
