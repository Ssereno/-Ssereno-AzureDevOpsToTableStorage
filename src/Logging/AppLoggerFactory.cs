using Serilog;
using Serilog.Events;
using AzureDevOpsToPowerBI.Settings.Logging;

namespace AzureDevOpsToPowerBI.Logging
{
    /// <summary>
    /// Creates and configures the Serilog logger from <see cref="LoggingConfig"/> settings.
    /// Call <see cref="Configure"/> once at application startup before any logging occurs.
    /// </summary>
    public static class AppLoggerFactory
    {
        /// <summary>
        /// Configures Serilog's static <see cref="Log.Logger"/> from the provided settings.
        /// </summary>
        /// <param name="config">Logging configuration from App.config &lt;Logging&gt; section.</param>
        /// <exception cref="ArgumentException">Thrown when LogPath is empty.</exception>
        public static void Configure(LoggingConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.LogPath))
                throw new ArgumentException("LogPath must be specified in the <Logging> section.", nameof(config));

            if (!Enum.TryParse<LogEventLevel>(config.MinimumLevel, ignoreCase: true, out var minimumLevel))
                minimumLevel = LogEventLevel.Information;

            if (!Enum.TryParse<RollingInterval>(config.RollingInterval, ignoreCase: true, out var rollingInterval))
                rollingInterval = RollingInterval.Day;

            var logConfig = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .WriteTo.File(
                    path: config.LogPath,
                    rollingInterval: rollingInterval,
                    retainedFileCountLimit: config.RetainedFiles,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

            if (config.EnableConsole)
            {
                logConfig = logConfig.WriteTo.Console(
                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            }

            Log.Logger = logConfig.CreateLogger();
        }

        /// <summary>
        /// Flushes and disposes the Serilog logger. Call on application exit.
        /// </summary>
        public static void CloseAndFlush() => Log.CloseAndFlush();
    }
}
