using System.Configuration;

namespace AzureDevOpsToPowerBI.Settings.Logging
{
    /// <summary>
    /// Configuration section for Serilog logging.
    /// Registered in App.config as &lt;Logging&gt;.
    /// </summary>
    public class LoggingSetting : ConfigurationSection
    {
        /// <summary>
        /// The &lt;Log&gt; element containing logging parameters.
        /// </summary>
        [ConfigurationProperty("Log")]
        public LoggingConfig Log => (LoggingConfig)this["Log"];
    }
}
