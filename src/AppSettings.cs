namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// Runtime settings loaded from App.config at startup.
    /// Passed explicitly to managers — not accessed as a global singleton from business logic.
    /// </summary>
    internal sealed class AppSettings
    {
        public string TfsUri { get; init; } = string.Empty;
        public string PersonalAccessToken { get; init; } = string.Empty;
        public string WorkItemSyncDate { get; init; } = string.Empty;
        public string StorageProvider { get; init; } = string.Empty;
        public string StorageConnectionString { get; init; } = string.Empty;
        public List<string> Tags { get; init; } = [];
    }
}
