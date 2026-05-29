using Microsoft.EntityFrameworkCore;
using AzureDevOpsToPowerBI.Models.Entities;

namespace AzureDevOpsToPowerBI.Data
{
    /// <summary>
    /// EF Core database context for the devops-sync application.
    /// Work item entities (UserStory, Bug, TfsTask) are mapped as fully independent
    /// flat tables — no shared base table. WorkItemBase is a C#-only abstraction.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Work item tables — flat, denormalised, BI-friendly
        public DbSet<UserStory> UserStories => Set<UserStory>();
        public DbSet<Bug> Bugs => Set<Bug>();
        public DbSet<TfsTask> Tasks => Set<TfsTask>();
        public DbSet<Feature> Features => Set<Feature>();

        // Supporting tables
        public DbSet<Area> Areas => Set<Area>();
        public DbSet<Iteration> Iterations => Set<Iteration>();
        public DbSet<SprintCapacity> SprintCapacities => Set<SprintCapacity>();
        public DbSet<ProjectProfile> ProjectProfiles => Set<ProjectProfile>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Work item entities: fully independent flat tables ---
            // WorkItemBase is NOT registered as an EF entity; EF propagates its
            // C# properties into each concrete table automatically.

            modelBuilder.Entity<UserStory>(entity =>
            {
                entity.ToTable("UserStories");
                entity.HasKey(e => new { e.WorkItemId, e.ProjectKey });
            });

            modelBuilder.Entity<Bug>(entity =>
            {
                entity.ToTable("Bugs");
                entity.HasKey(e => new { e.WorkItemId, e.ProjectKey });
            });

            modelBuilder.Entity<TfsTask>(entity =>
            {
                entity.ToTable("Tasks");
                entity.HasKey(e => new { e.WorkItemId, e.ProjectKey });
            });

            modelBuilder.Entity<Feature>(entity =>
            {
                entity.ToTable("Features");
                entity.HasKey(e => new { e.WorkItemId, e.ProjectKey });
            });

            // --- Supporting entities ---
            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("Areas");
                entity.HasKey(e => new { e.ProjectKey, e.AreaSK });
            });

            modelBuilder.Entity<Iteration>(entity =>
            {
                entity.ToTable("Iterations");
                entity.HasKey(e => new { e.ProjectKey, e.IterationSK });
            });

            modelBuilder.Entity<SprintCapacity>(entity =>
            {
                entity.ToTable("SprintCapacities");
                entity.HasKey(e => new { e.ProjectKey, e.IterationSK });
            });

            modelBuilder.Entity<ProjectProfile>(entity =>
            {
                entity.ToTable("ProjectProfiles");
                entity.HasKey(e => e.Id);
            });
        }
    }
}
