using System.Configuration;

namespace AzureDevOpsToPowerBI.Settings.Logging
{
    /// <summary>
    /// Configuration element that holds individual logging parameters.
    /// </summary>
    public class LoggingConfig : ConfigurationElement
    {
        /// <summary>
        /// Minimum log level. Accepted values: Verbose, Debug, Information, Warning, Error, Fatal.
        /// Default: Information.
        /// </summary>
        [ConfigurationProperty("MinimumLevel", IsRequired = false, DefaultValue = "Information")]
        public string MinimumLevel => (string)this["MinimumLevel"];

        /// <summary>
        /// Path to the log file (e.g. logs\devops-sync.log).
        /// Rolling suffix is appended automatically per RollingInterval.
        /// </summary>
        [ConfigurationProperty("LogPath", IsRequired = true)]
        public string LogPath => (string)this["LogPath"];

        /// <summary>
        /// Rolling interval for log files. Accepted values: Day, Month, Hour, Infinite.
        /// Default: Day.
        /// </summary>
        [ConfigurationProperty("RollingInterval", IsRequired = false, DefaultValue = "Day")]
        public string RollingInterval => (string)this["RollingInterval"];

        /// <summary>
        /// Number of rolled log files to retain before the oldest is deleted.
        /// Default: 7.
        /// </summary>
        [ConfigurationProperty("RetainedFiles", IsRequired = false, DefaultValue = 7)]
        public int RetainedFiles => (int)this["RetainedFiles"];

        /// <summary>
        /// When true, log output is also written to the console.
        /// Set to false for scheduled/unattended execution.
        /// Default: true.
        /// </summary>
        [ConfigurationProperty("EnableConsole", IsRequired = false, DefaultValue = true)]
        public bool EnableConsole => (bool)this["EnableConsole"];
    }
}
