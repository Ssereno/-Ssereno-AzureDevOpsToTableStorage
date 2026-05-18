using AzureDevOpsToPowerBI.Models.Entities;

namespace AzureDevOpsToPowerBI.Data.Repositories
{
    /// <summary>
    /// Abstraction for persisting and cleaning work item data.
    /// </summary>
    public interface IWorkItemRepository
    {
        /// <summary>
        /// Inserts or updates a list of work items of type <typeparamref name="T"/>.
        /// Existing records (matched by WorkItemId + ProjectKey) are updated; new records are inserted.
        /// </summary>
        Task UpsertWorkItemsAsync<T>(List<T> items) where T : WorkItemBase;

        /// <summary>
        /// Removes work items of type <typeparamref name="T"/> whose CreatedDate is on or after <paramref name="fromDate"/>,
        /// within the given <paramref name="projectKey"/>.
        /// </summary>
        Task DeleteByDateAsync<T>(string projectKey, DateTime fromDate) where T : WorkItemBase;

        /// <summary>
        /// Removes work items of type <typeparamref name="T"/> that are not in state "Closed",
        /// within the given <paramref name="projectKey"/>.
        /// </summary>
        Task DeleteNonClosedAsync<T>(string projectKey) where T : WorkItemBase;

        /// <summary>
        /// Removes all work items of type <typeparamref name="T"/> for the given <paramref name="projectKey"/>.
        /// </summary>
        Task DeleteAllAsync<T>(string projectKey) where T : WorkItemBase;

        /// <summary>Inserts or updates a list of Area records.</summary>
        Task UpsertAreasAsync(List<Area> items);

        /// <summary>Inserts or updates a list of Iteration records.</summary>
        Task UpsertIterationsAsync(List<Iteration> items);

        /// <summary>Inserts or updates a list of SprintCapacity records.</summary>
        Task UpsertSprintCapacitiesAsync(List<SprintCapacity> items);

        /// <summary>Ensures a ProjectProfile table exists and seeds it with one placeholder row if empty.</summary>
        Task EnsureProjectProfileAsync(string projectKey);
    }
}
