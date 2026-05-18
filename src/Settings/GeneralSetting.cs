using System.Configuration;
using AzureDevOpsToPowerBI.Settings.General;

namespace AzureDevOpsToPowerBI.Settings
{
    /// <summary>
    /// Root configuration section for general connection and storage settings.
    /// </summary>
    public class GeneralSetting : ConfigurationSection
    {
        /// <summary>
        /// Azure DevOps connection parameters and storage provider configuration.
        /// </summary>
        [ConfigurationProperty("Connection")]
        public ConnectionSettings Connection => (ConnectionSettings)this["Connection"];

        /// <summary>
        /// Tag filter used when querying work items.
        /// </summary>
        [ConfigurationProperty("FilterTag")]
        public FilterTags FilterTag => (FilterTags)this["FilterTag"];
    }
}
