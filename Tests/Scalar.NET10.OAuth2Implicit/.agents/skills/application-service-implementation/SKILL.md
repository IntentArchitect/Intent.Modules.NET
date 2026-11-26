---
name: application-service-implementation
description: Implements business logic in traditional (non-CQRS) application services. MUST load before creating or editing a traditional application service class in the Application layer e.g. *Service.cs under Implementation.
template-id: Intent.Application.ServiceImplementations.ServiceImplementationSkillTemplate
contentHash: 863A790432AC1C72262969FB9700BECA4BA39C7BE26D80C63F1F5313C9F44D83
---
# Traditional Service Implementation

Fill in the operation bodies of an existing application service. Their signatures are generated; only the bodies are yours.

## Rules

- Mark each operation you write with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a service.
- Load aggregates through their repository interface and change them through their domain behaviour, not by setting state ad hoc. Put durable business rules on the entity or a domain service when nearby code does that.
- Follow the nearest similar service for result types, error handling, mapping and events.
- In read operations, filter, sort and page in the query the repository runs, never in memory after loading everything, and change no state.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`FindByIdAsync`, `ListByCriteriaAsync`, `ExistsBy…Async`), not for storage, and keep storage details such as includes, joins and SQL out of the service.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the service are fine when they make the flow easier to read.
