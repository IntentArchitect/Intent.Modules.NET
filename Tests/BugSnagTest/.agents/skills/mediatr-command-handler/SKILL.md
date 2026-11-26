---
name: mediatr-command-handler
description: Implements business logic in MediatR command handler Handle methods. MUST load before creating or editing any command handler e.g. *CommandHandler.cs.
template-id: Intent.Application.MediatR.CommandHandlerSkillTemplate
contentHash: E62420DFF1D328F070D99F0A5ECD702F115584F0AE8CBDEDA5E015C604CD3B89
---
# MediatR Command Handler

Fill in the `Handle` method of an existing command handler. Its signature is generated; only the body is yours.

## Rules

- Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Load aggregates through their repository interface and change them through their domain behaviour, not by setting state ad hoc. Put durable business rules on the entity or a domain service when nearby code does that.
- Follow the nearest similar handler for result types, error handling and events. Publish an event only where nearby code already does.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`GetForUpdateAsync`, `ExistsBy…Async`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make the flow easier to read.
