## ADDED Requirements

### Requirement: Retrieve Features from Azure DevOps
The system SHALL retrieve Feature work items from the Azure DevOps Analytics OData feed using an OData query.
- The OData query MUST select: `WorkItemId,Title,WorkItemType,State,StoryPoints,LeadTimeDays,CycleTimeDays,CreatedDate,ResolvedDate,AreaSK,IterationSK,ActivatedDate,ClosedDate,CompletedDate,ParentWorkItemId,TagNames`.
- The OData query MUST filter by `WorkItemType eq 'Feature'` and `State ne 'Removed'` and by project's AreaPath, and respect the sync date and excluded tags filters.

#### Scenario: Retrieve Features from OData Feed successfully
- **WHEN** a synchronization for Features is requested with a valid TFS URI, PAT, Project Key, Area Path, and Sync Date
- **THEN** the system queries the OData feed with the specified filters and deserializes the response into a list of Feature entities

### Requirement: Persist Features to SQL Server
The system SHALL persist Features into a dedicated database table named `Features` in SQL Server.
- Each Feature record MUST be uniquely identified by a composite key of `WorkItemId` and `ProjectKey`.
- The table MUST support upserting existing records by updating fields when matching composite keys are found.

#### Scenario: Upsert Feature entities into the database
- **WHEN** a list of fetched Feature entities is provided to the repository
- **THEN** the repository upserts all Features in the database, updating matching records and inserting new ones

### Requirement: Delete Features based on sync mode
The system SHALL support cleaning up Feature records in the database based on the selected sync and delete modes.
- If the sync date is changed, the system SHALL support deleting Feature records on or after the new sync date.
- The system SHALL support deleting non-closed Features.
- The system SHALL support deleting all Features for a project.

#### Scenario: Delete Features by date range
- **WHEN** the system is requested to delete Features from a date range
- **THEN** the system deletes all Feature records with a `CreatedDate` on or after that date for the given `ProjectKey`

#### Scenario: Delete non-closed Features
- **WHEN** the system is requested to delete non-closed Features
- **THEN** the system deletes all Feature records whose `State` is not 'Closed' for the given `ProjectKey`

#### Scenario: Delete all Features
- **WHEN** the system is requested to delete all Features for a project
- **THEN** the system deletes all Feature records matching the given `ProjectKey`
