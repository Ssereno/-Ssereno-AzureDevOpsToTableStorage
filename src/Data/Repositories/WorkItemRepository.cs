using AzureDevOpsToPowerBI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsToPowerBI.Data.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IWorkItemRepository"/>.
    /// Accepts the <see cref="AppDbContext"/> via constructor injection.
    /// </summary>
    public class WorkItemRepository : IWorkItemRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<WorkItemRepository> _logger;

        public WorkItemRepository(AppDbContext db, ILogger<WorkItemRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task UpsertWorkItemsAsync<T>(List<T> items) where T : WorkItemBase
        {
            if (items.Count == 0) return;

            // Deduplicate in memory before touching EF tracking.
            // Azure DevOps OData can return the same item twice when tag filters
            // overlap; processing duplicates within the same SaveChanges batch
            // would cause a primary-key constraint violation.
            var distinct = items
                .GroupBy(i => new { i.WorkItemId, i.ProjectKey })
                .Select(g => g.First())
                .ToList();

            foreach (var item in distinct)
            {
                var existing = await _db.Set<T>()
                    .FirstOrDefaultAsync(e => e.WorkItemId == item.WorkItemId && e.ProjectKey == item.ProjectKey);

                if (existing is null)
                    await _db.Set<T>().AddAsync(item);
                else
                    _db.Entry(existing).CurrentValues.SetValues(item);
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Upserted {Count} {Type} records", distinct.Count, typeof(T).Name);
        }

        /// <inheritdoc/>
        public async Task DeleteByDateAsync<T>(string projectKey, DateTime fromDate) where T : WorkItemBase
        {
            string fromDateStr = fromDate.ToString("yyyy-MM-dd");

            var toDelete = await _db.Set<T>()
                .Where(e => e.ProjectKey == projectKey &&
                            e.CreatedDate != null &&
                            string.Compare(e.CreatedDate, fromDateStr) >= 0)
                .ToListAsync();

            _db.Set<T>().RemoveRange(toDelete);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Deleted {Count} {Type} records for {Project} with CreatedDate >= {Date}",
                toDelete.Count, typeof(T).Name, projectKey, fromDateStr);
        }

        /// <inheritdoc/>
        public async Task DeleteNonClosedAsync<T>(string projectKey) where T : WorkItemBase
        {
            var toDelete = await _db.Set<T>()
                .Where(e => e.ProjectKey == projectKey && e.State != "Closed")
                .ToListAsync();

            _db.Set<T>().RemoveRange(toDelete);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Deleted {Count} non-Closed {Type} records for {Project}",
                toDelete.Count, typeof(T).Name, projectKey);
        }

        /// <inheritdoc/>
        public async Task DeleteAllAsync<T>(string projectKey) where T : WorkItemBase
        {
            var toDelete = await _db.Set<T>()
                .Where(e => e.ProjectKey == projectKey)
                .ToListAsync();

            _db.Set<T>().RemoveRange(toDelete);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Deleted all {Count} {Type} records for {Project}",
                toDelete.Count, typeof(T).Name, projectKey);
        }

        /// <inheritdoc/>
        public async Task UpsertAreasAsync(List<Area> items)
        {
            if (items.Count == 0) return;

            // Deduplicate by natural key before EF tracking to prevent unique-index
            // violations when the OData feed returns the same AreaSK more than once.
            var distinct = items
                .Where(i => i.AreaSK != null)
                .GroupBy(i => new { i.ProjectKey, i.AreaSK })
                .Select(g => g.First())
                .ToList();

            foreach (var item in distinct)
            {
                var existing = await _db.Areas
                    .FirstOrDefaultAsync(e => e.ProjectKey == item.ProjectKey && e.AreaSK == item.AreaSK);

                if (existing is null)
                {
                    await _db.Areas.AddAsync(item);
                }
                else
                {
                    _db.Entry(existing).CurrentValues.SetValues(item);
                }
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Upserted {Count} Area records for {Project}",
                distinct.Count, distinct.FirstOrDefault()?.ProjectKey ?? "unknown");
        }

        /// <inheritdoc/>
        public async Task UpsertIterationsAsync(List<Iteration> items)
        {
            if (items.Count == 0) return;

            // Deduplicate by natural key before EF tracking.
            var distinct = items
                .Where(i => i.IterationSK != null)
                .GroupBy(i => new { i.ProjectKey, i.IterationSK })
                .Select(g => g.First())
                .ToList();

            foreach (var item in distinct)
            {
                var existing = await _db.Iterations
                    .FirstOrDefaultAsync(e => e.ProjectKey == item.ProjectKey && e.IterationSK == item.IterationSK);

                if (existing is null)
                {
                    await _db.Iterations.AddAsync(item);
                }
                else
                {
                    _db.Entry(existing).CurrentValues.SetValues(item);
                }
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Upserted {Count} Iteration records for {Project}",
                distinct.Count, distinct.FirstOrDefault()?.ProjectKey ?? "unknown");
        }

        /// <inheritdoc/>
        public async Task UpsertSprintCapacitiesAsync(List<SprintCapacity> items)
        {
            if (items.Count == 0) return;

            // Deduplicate by natural key before EF tracking.
            var distinct = items
                .Where(i => i.IterationSK != null)
                .GroupBy(i => new { i.ProjectKey, i.IterationSK })
                .Select(g => g.First())
                .ToList();

            foreach (var item in distinct)
            {
                var existing = await _db.SprintCapacities
                    .FirstOrDefaultAsync(e => e.ProjectKey == item.ProjectKey && e.IterationSK == item.IterationSK);

                if (existing is null)
                {
                    await _db.SprintCapacities.AddAsync(item);
                }
                else
                {
                    _db.Entry(existing).CurrentValues.SetValues(item);
                }
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Upserted {Count} SprintCapacity records for {Project}",
                distinct.Count, distinct.FirstOrDefault()?.ProjectKey ?? "unknown");
        }

        /// <inheritdoc/>
        public async Task EnsureProjectProfileAsync(string projectKey)
        {
            bool exists = await _db.ProjectProfiles.AnyAsync(p => p.ProjectKey == projectKey);
            if (!exists)
            {
                await _db.ProjectProfiles.AddAsync(new ProjectProfile
                {
                    ProjectKey       = projectKey,
                    AreaSK           = string.Empty,
                    AreaPath         = string.Empty,
                    IterationName    = string.Empty,
                    IterationSK      = string.Empty,
                    Velocity         = 0,
                    ProjectEffort    = 0,
                    ProjectStartDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    ProjectEndDate   = DateTime.Now.ToString("yyyy-MM-dd")
                });
                await _db.SaveChangesAsync();
                _logger.LogInformation("Created placeholder ProjectProfile for {Project}", projectKey);
            }
        }
    }
}

