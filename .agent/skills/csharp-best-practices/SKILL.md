---
name: C# Best Practices
description: Architecture, Clean Code, and testability guidelines for C# code.
---

# C# Best Practices & Testability

This skill defines the golden rules for writing C# code in this project, focusing on maintainability and, above all, code testability.

## 1. Dependency Injection
- **Golden Rule:** All external dependencies (I/O, network, database, third-party services) **must** be injected via the constructor.
- Never instantiate infrastructure classes with the `new` keyword inside business logic (e.g., `new HttpClient()`, `new SqlConnection()`).
- Code must always be prepared to receive *mocks* of its dependencies.

## 2. Dependency Inversion Principle (SOLID)
- Depend on abstractions (Interfaces), not concrete implementations, for calls to external APIs.
- Example: Create and inject an `IAzureDevOpsClient` instead of coupling code directly to the DevOps HTTP logic.

## 3. State Management & Statics
- Avoid using static classes and methods for operations involving state or I/O (such as `DateTime.Now`). For clocks, use abstractions like `TimeProvider` (.NET 8+) or `ISystemClock`.

## 4. The DEV's Role Toward QA
- The DEV has architectural freedom to decide *how* to resolve a testability issue raised by the QA (e.g., creating a new interface, separating responsibilities).
- If the QA raises an alert that a class is not testable due to strong coupling, the DEV must refactor the code immediately before marking the work as complete.
