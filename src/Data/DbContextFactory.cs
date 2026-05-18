using Microsoft.EntityFrameworkCore;
using AzureDevOpsToPowerBI.Settings;
using AzureDevOpsToPowerBI.Settings.General;
using System.Configuration;

namespace AzureDevOpsToPowerBI.Data
{
    /// <summary>
    /// Factory that creates an <see cref="AppDbContext"/> configured for the
    /// storage provider declared in App.config (&lt;General&gt;&lt;Connection&gt;&lt;Storage&gt;).
    /// Supported providers: "SQLite", "AzureSql".
    /// </summary>
    public class DbContextFactory
    {
        private readonly string _provider;
        private readonly string _connectionString;

        public DbContextFactory(string provider, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentException("Storage provider must be specified.", nameof(provider));
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Storage connection string must be specified.", nameof(connectionString));

            _provider = provider;
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates and returns a configured <see cref="AppDbContext"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the configured provider value is not recognised.
        /// </exception>
        public AppDbContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            switch (_provider.Trim())
            {
                case "SQLite":
                    optionsBuilder.UseSqlite(_connectionString);
                    break;

                case "AzureSql":
                    optionsBuilder.UseSqlServer(_connectionString);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported storage provider '{_provider}'. " +
                        "Accepted values: SQLite, AzureSql.");
            }

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
