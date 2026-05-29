## Why

Currently, the application synchronizes User Stories, Bugs, Tasks, and supporting data, but it lacks the capability to synchronize Azure DevOps Features. Adding Features will enable more comprehensive tracking, reporting, and visualization of higher-level work items within the destination storage.

## What Changes

- **New Feature Entity and DTO**: Create `Feature` entity and `FeatureDto` to represent and map Azure DevOps Features.
- **Feature Fetching Manager**: Create `FeatureManager` to fetch Features from Azure DevOps Analytics via OData, filtering by `WorkItemType eq 'Feature'`.
- **Database Schema Update**: Register the `Feature` entity in `AppDbContext` and generate a migration to create the `Features` table.
- **Sync Command Integration**: Update `SyncCommand` to include `Feature` synchronization and cleanup (deletion) following the established pattern for User Stories.

## Capabilities

### New Capabilities
- `sync-features`: Syncs Azure DevOps Features to the local storage, including initial loading, incremental syncing, and deleting obsolete features according to selected modes.

### Modified Capabilities

## Impact

- **Models**: New C# files under `src/Models/Entities/Feature.cs` and `src/Models/Dtos/FeatureDto.cs`.
- **Managers**: New C# file under `src/Manager/FeatureManager.cs`.
- **Database**: Update `src/Data/AppDbContext.cs` and create a EF Core migration adding the `Features` table.
- **Commands**: Update `src/Commands/SyncCommand.cs` to incorporate `Feature` syncing logic.
