## Context

The application currently synchronizes User Stories, Bugs, and Tasks from Azure DevOps Analytics OData feed to a local database. In `SyncCommand`, supporting data (Areas, Iterations, Capacities) and work items are fetched, upserted, and cleaned up using specialized managers and a generic repository `IWorkItemRepository`.
This design document defines how to introduce Feature work items, adhering to the existing codebase patterns.

## Goals / Non-Goals

**Goals:**
- Implement a `Feature` entity that inherits from `WorkItemBase`.
- Implement a `FeatureDto` class to deserialize features from OData and map them to `Feature` entities.
- Implement a `FeatureManager` to fetch Features from Azure DevOps using OData with the filter `WorkItemType eq 'Feature'`.
- Register the `Feature` entity in `AppDbContext` and generate a database migration to create the `Features` table.
- Integrate Feature synchronization, incremental deletes, and full cleanup inside `SyncCommand`.

**Non-Goals:**
- Syncing other work item types like Epics, Enablers, or custom types.
- Refactoring the generic `IWorkItemRepository` implementation since it already supports any `T where T : WorkItemBase`.

## Decisions

### 1. Model Features with Inheritance from WorkItemBase
- **Rationale**: Feature work items share almost all metadata fields (WorkItemId, Title, State, AreaSK, IterationSK, CreatedDate, ClosedDate, ResolvedDate, ActivatedDate, ParentWorkItemId, TagNames) with User Stories, Bugs, and Tasks. Inheriting from `WorkItemBase` leverages the existing generic repository operations.
- **Alternative considered**: Make Feature fully independent without inheriting from `WorkItemBase`. Rejected because we would have to duplicate all basic properties and write custom non-generic repository methods for upserting and deleting features.

### 2. Dedicated FeatureManager and FeatureDto
- **Rationale**: We will create `FeatureManager` and `FeatureDto` by mirroring `UserStoriesManager` and `UserStoryDto` respectively. This maintains clear separation of concerns and follows the established project pattern.
- **Alternative considered**: Generalize `UserStoriesManager` to a generic `WorkItemManager<T>`. Rejected because OData filters, fields, and API endpoints are specific to each work item type, and trying to generalize too early adds unnecessary complexity.

### 3. Add EF Core Migration for database schema update
- **Rationale**: The project manages database schemas using EF Core migrations. We must register `Feature` in `AppDbContext` and run a migration to ensure the schema is in place before the sync runs.
- **Alternative considered**: Manual script execution. Rejected because migrations keep the code and schema in sync automatically.

## Risks / Trade-offs

- **StoryPoints in Features**
  - *Risk*: Features in standard Azure DevOps Scrum templates do not always have a "StoryPoints" field, or it might be named differently.
  - *Mitigation*: The OData endpoint returns `StoryPoints` as `null` if it's not defined/populated on Features. Since both `FeatureDto` and `Feature` declare `StoryPoints` as `double?` (nullable), this will deserialize and persist cleanly as `null` without throwing exceptions.
