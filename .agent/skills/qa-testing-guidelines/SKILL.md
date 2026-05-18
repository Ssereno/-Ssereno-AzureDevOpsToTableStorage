---
name: QA Testing Guidelines
description: Testing rules, tools, and QA interaction guidelines with DEV code.
---

# QA Testing Guidelines

This skill governs the behavior of the QA Agent and how it ensures the **80% code coverage** target is met.

## 1. Testing Stack
- **Primary Framework:** `xUnit`
- **Mocking Library:** `Moq`
- **Assertions (Optional):** `FluentAssertions` (if available in the project)

## 2. The Moq vs. EF Core Rule
- The QA must use **Moq** to mock all interfaces injected via the constructor (e.g., `IHttpClientFactory`, `IAzureDevOpsClient`).
- **The Exception (DbContext):** Never attempt to directly mock the `DbContext` with Moq. When code injects a `DbContext`, the QA must configure the DbContext options to use EF Core's In-Memory provider (`UseInMemoryDatabase()`) or in-memory SQLite (`UseSqlite("DataSource=:memory:")`).

## 3. Socratic QA Behavior
- If the QA finds code that cannot be tested with xUnit/Moq (e.g., non-injected dependencies, strong coupling to I/O services):
  1. The QA **must not** attempt to write a fragile test.
  2. The QA **must not** tell the DEV exactly which code pattern to apply.
  3. The QA **must** pause the pipeline and report the problem to the human, stating: *"Dependency X is tightly coupled and prevents unit testing. The DEV needs to refactor the class to allow this dependency to be injected."*
- It is the DEV's responsibility to decide the best architecture to resolve the issue.
