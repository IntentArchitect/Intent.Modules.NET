# CONTEXT.md — Intent.AspNetCore.Controllers.Dispatch.Wolverine

This document contains durable architectural decisions, constraints, and patterns for the Wolverine controller dispatch module.

## 🏗️ Architectural Constraints & Rules

### 1. Wolverine Controller Dispatch Mapping
- This module intercepts ASP.NET Core controller templates and replaces MediatR's `IMediator` dispatch calls with Wolverine's `IMessageBus` dispatch.
- When generating the instantiation statement for command/query payloads inside actions, it checks if the target command or query model template defines a constructor.
  - If a constructor exists, it maps route/query parameters to constructor arguments:
    ```csharp
    new GetItemByIdQuery(id: id)
    ```
  - If no constructor exists, it falls back to object initializer mapping:
    ```csharp
    new GetItemByIdQuery { Id = id }
    ```

### 2. Dependency Rules
- To maintain package-boundary integrity, this module project references the NuGet package `Intent.Modules.AspNetCore.Controllers` rather than a local `<ProjectReference>`.
