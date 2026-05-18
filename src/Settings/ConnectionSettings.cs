using System.Configuration;

namespace AzureDevOpsToPowerBI.Settings.General
{
    /// <summary>
    /// Holds connection settings for Azure DevOps and the database storage provider.
    /// </summary>
    public class ConnectionSettings : ConfigurationElement
    {
        /// <summary>
        /// The Azure DevOps organisation URI (e.g. https://dev.azure.com/myorg).
        /// </summary>
        [ConfigurationProperty("TfsUri", IsRequired = true)]
        public string TfsUri => (string)this["TfsUri"];

        /// <summary>
        /// The Azure DevOps Personal Access Token used for authentication.
        /// </summary>
        [ConfigurationProperty("PersonalAccessToken", IsRequired = true)]
        public string PersonalAccessToken => (string)this["PersonalAccessToken"];

        /// <summary>
        /// The earliest work item creation date to synchronise from.
        /// </summary>
        [ConfigurationProperty("WorkItemSyncDate", IsRequired = true)]
        public string WorkItemSyncDate => (string)this["WorkItemSyncDate"];

        /// <summary>
        /// Storage configuration sub-element.
        /// </summary>
        [ConfigurationProperty("Storage")]
        public StorageSettings Storage => (StorageSettings)this["Storage"];
    }
}
