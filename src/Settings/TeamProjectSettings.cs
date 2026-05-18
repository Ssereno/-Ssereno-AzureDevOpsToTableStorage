using System.Configuration;

namespace AzureDevOpsToPoweBI.Settings.TeamAndProjectSettings
{
    /// <summary>
    /// Configuration element representing a single Azure DevOps project.
    /// TfsUri and PersonalAccessToken are optional per-project overrides;
    /// when absent or empty the global values from &lt;General&gt; are used.
    /// </summary>
    public class TeamProjectSettings : ConfigurationElement
    {
        /// <summary>
        /// Unique key that identifies the project in the local database (used as ProjectKey).
        /// </summary>
        [ConfigurationProperty("ProjectKey", IsRequired = true, IsKey = true)]
        public string ProjectKey => (string)this["ProjectKey"];

        /// <summary>
        /// The Azure DevOps project name (as it appears in the URL).
        /// </summary>
        [ConfigurationProperty("ProjectName", IsRequired = true)]
        public string ProjectName => (string)this["ProjectName"];

        /// <summary>
        /// The area path used to filter work items (e.g. FMM\FMM).
        /// </summary>
        [ConfigurationProperty("AreaPath", IsRequired = true)]
        public string AreaPath => (string)this["AreaPath"];

        /// <summary>
        /// The Azure DevOps team name (used for iteration/capacity queries).
        /// </summary>
        [ConfigurationProperty("TeamName", IsRequired = true)]
        public string TeamName => (string)this["TeamName"];

        /// <summary>
        /// The iteration hierarchy level at which the project/product name lives.
        /// Used to filter iterations by project.
        /// </summary>
        [ConfigurationProperty("IterationLevel", IsRequired = true)]
        public int IterationLevel => (int)this["IterationLevel"];

        /// <summary>
        /// Optional. Overrides the global TfsUri for this project.
        /// Use when the project belongs to a different Azure DevOps organisation.
        /// </summary>
        [ConfigurationProperty("TfsUri", IsRequired = false, DefaultValue = "")]
        public string TfsUri => (string)this["TfsUri"];

        /// <summary>
        /// Optional. Overrides the global PersonalAccessToken for this project.
        /// Use when the project belongs to a different Azure DevOps organisation.
        /// </summary>
        [ConfigurationProperty("PersonalAccessToken", IsRequired = false, DefaultValue = "")]
        public string PersonalAccessToken => (string)this["PersonalAccessToken"];
    }
}
