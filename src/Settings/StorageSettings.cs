using System.Configuration;

namespace AzureDevOpsToPowerBI.Settings.General
{
    /// <summary>
    /// Storage provider configuration.
    /// Supported providers: SQLite, AzureSql.
    /// </summary>
    public class StorageSettings : ConfigurationElement
    {
        /// <summary>
        /// The storage provider to use. Accepted values: "SQLite" or "AzureSql".
        /// </summary>
        [ConfigurationProperty("Provider", IsRequired = true)]
        public string Provider => (string)this["Provider"];

        /// <summary>
        /// The connection string for the selected provider.
        /// For SQLite: "Data Source=devops.db"
        /// For AzureSql: standard SQL Server connection string.
        /// </summary>
        [ConfigurationProperty("ConnectionString", IsRequired = true)]
        public string ConnectionString => (string)this["ConnectionString"];
    }
}
