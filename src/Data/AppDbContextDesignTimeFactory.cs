using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AzureDevOpsToPowerBI.Data
{
    /// <summary>
    /// Design-time factory used by EF Core tooling (dotnet ef migrations) to create
    /// an <see cref="AppDbContext"/> without requiring the full application host.
    /// Uses SQLite with a local development database by default.
    /// </summary>
    public class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=devops-sync-design.db")
                .Options;

            return new AppDbContext(options);
        }
    }
}
