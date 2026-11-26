---
name: application-service-implementation
description: Implements business logic in traditional (non-CQRS) application services. MUST load before creating or editing a traditional application service class in the Application layer e.g. *Service.cs under Implementation.
template-id: Intent.Application.ServiceImplementations.ServiceImplementationSkillTemplate
contentHash: 77A0944A815F502F83896AA8AD2A38EAB0A88E64B075CC8FC2A5F95F745A0915
---
# Traditional Service Implementation

Fill in the operation bodies of an existing application service. Their signatures are generated; only the bodies are yours.

## Rules

- Mark each operation you write with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`.
- Change only the operations the task needs. Leave the rest of the file as it is, and keep unrelated refactors out.
- Start by reading the service, its interface and DTOs, the repository interfaces and the domain types it touches, and work out each operation's flow from its name, parameters, return type and nearby features.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a service.
- Let the domain model drive the code: use its aggregates, value objects, statuses and business terms rather than names of your own.
- Load aggregates through their repository interface and change them through their domain behaviour, not by setting state ad hoc. Put durable business rules on the entity or a domain service when nearby code does that.
- Before changing state, check what nearby services check: that the entity exists, and that the caller may act on it (ownership, eligibility, status).
- Look for similar services, repository methods and domain operations before writing, and follow the nearest similar service for result types, error handling, mapping and events. Don't invent a new result or exception style where one exists.
- In read operations, filter, sort and page in the query the repository runs, never in memory after loading everything, and change no state. Apply the same business filters and visibility rules as nearby services (for example ownership, tenant, status or soft-delete), and load only what the response needs.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`FindByIdAsync`, `ListByCriteriaAsync`, `ExistsBy…Async`), not for storage, and keep storage details such as includes, joins and SQL out of the service.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the service are fine when they make the flow easier to read.
- Before finishing, review your code against every rule in this skill, the sections below included.
