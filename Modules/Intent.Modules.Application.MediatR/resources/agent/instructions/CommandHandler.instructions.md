---
description: Instructions for implementing a Command Handler in the Intent Application layer using MediatR.
appliesTo:
  - "**/*CommandHandler.cs"
---

## Implementation Rules:
- ALWAYS follow the architectural guidelines as and when they become apparent.
- NEVER modify the method signature of the Handle method.
- ALWAYS ensure that the `IntentManaged` attribute indicates that the body of the method must be in `Mode.Ignore` (e.g. `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`).
- Only ever inject in dependencies from the Domain or Application layers.
- Never introduce dependencies on infrastructural nuget packages (e.g. Entity Framework, Dapper, etc.) directly in the handler. If data access is required, use the appropriate repository in the Domain layer and inject that into the handler.
- Follow the user's modeled intentions as best as possible.

## Architectural Guidelines:
- Follow the Single Responsibility Principle. The handler should only be responsible for handling the command and delegating work to other services or components as necessary.
- Use Dependency Injection to inject any required services or repositories into the handler's constructor.
- Ensure that the handler is focused on orchestrating the retrieval of data and does not contain complex data manipulation. Place complex data manipulation logic in the infrastructure layer (e.g. in a repository) if possible.