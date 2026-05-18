---
name: EF Core Guidelines
description: Guidelines for using Entity Framework Core with SQL Server.
---

# Entity Framework Core Guidelines

This skill guides the implementation and use of Entity Framework Core and SQL Server in this project.

## 1. DbContext Injection
- It is **allowed** to inject the `DbContext` (e.g., `AppDbContext`) directly into business services. Creating a generic *Repository* pattern on top of EF Core is not mandatory.
- EF Core already implements the *Repository* pattern (via `DbSet`) and *Unit of Work* (via `SaveChanges`).

## 2. Performance & Queries
- Use `.AsNoTracking()` for read-only queries (where entities will not be modified).
- Pay close attention to the N+1 query problem. Use `.Include()` or projections (`.Select()`) appropriately.

## 3. Testability with DbContext
- Since we allow direct DbContext injection, mocking the `DbContext` and its `DbSet<T>` directly with Moq is not practical or recommended.
- See the `qa-testing-guidelines` skill for how the QA should handle this using the In-Memory provider.

## 4. Migrations
- Before adding a new *Migration*, the DEV must analyze the existing models and database schema to avoid redundancies or data loss.
