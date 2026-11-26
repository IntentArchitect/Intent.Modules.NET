# Context: Intent.EntityFrameworkCore.Repositories

## Purpose

Generates the EF Core implementations of the repositories and the unit of work that the Domain layer's repository
interfaces describe. It also adds repository guidance to the AI handler skills that other modules generate.

## Architectural Decisions

- **AI skill guidance goes into the shared "Persistence (EF Core)" section, at Order 1.** `AISkillsExtension` adds its
  rules to every `AI.Context.Skills.Handler` skill with `WithSection(PersistenceSectionTitle, ...)`.
  `Intent.EntityFrameworkCore` writes the loading rule into the same section at Order 0, so it leads. Keep
  `PersistenceSectionTitle` identical to that module's constant; a mismatch splits the section in two. The mapping
  modules run at Order 2, so "Mapping" follows "Persistence".
- **Query skills get nothing (4.8.7).** A query changes no state, so "do not call `SaveChangesAsync`" and "do not call
  `Update`" are noise there; a query skill's own rules already say it changes no state. A skill is recognised as a
  query skill by `QueryHandler` in its template id.
- **The save rule is worded per skill kind (4.8.7)**, recognised by template id: `DomainEventHandler` ("the handler
  runs inside the unit of work that raised the event", with no exception, because the outer unit of work commits),
  `ServiceImplementation` ("the unit of work commits after the operation returns"), and otherwise a command handler
  ("the pipeline's unit of work commits after the handler"). The operation and command-handler wording allow a call
  when a database-generated value such as a new Id must be returned. Rejected: one sentence for all kinds, which told
  domain-event handlers they could save and told service operations about a "handler".
- **The save rule depends on the unit-of-work setting.** With the application's `Automatically persist unit of work`
  setting (from `Intent.Common.UnitOfWork`) on, or absent, command handlers and services are told not to save. With it
  off, nothing commits after the handler and the generated CRUD code calls `SaveChangesAsync` in the handler itself
  (`DomainInteractions`' Update and Delete strategies), so since 2026-11-26 those skills are told to call it once after
  the last change, following the generated handlers. Previously the rule was simply left out when off ("the developer
  owns saving"), which left an agent with no save guidance at all where it most needed some. Domain event handlers are
  told not to save either way: the DbContext dispatches domain events inside `SaveChangesAsync`, before it writes, so
  the save that raised the event writes the handler's changes. The `Update` rule is written either way, because
  tracked entities are saved without it.
- **The condensed text is the evaluated text (4.8.7).** The earlier "cite the file path and an excerpt" procedure and
  the restatements around it were removed. The reasoning is recorded in `Intent.Application.MediatR`'s `CONTEXT.md`,
  "Handler skills: trimmed text, no pointer rule".
- **The `ef-repository` skill keeps only Intent-specific rules (4.8.7).** Kept: use an existing base method first;
  model the method as a repository operation when a designer will map to it, otherwise hand-add it to the interface
  and implementation; no `[IntentManaged]` on a hand-added interface method (the interface's generated methods are
  `Mode.Fully`, and a `Mode.Fully` method the model doesn't produce is deleted); `[IntentIgnore]` on a hand-added
  implementation method; aggregate in the repository; a composite result is a record in the interface's file, below
  the interface, so it stays in the Domain layer, marked `[IntentIgnore]`; never a tuple. Removed: generic C# and EF
  advice an agent already follows. The record's location was a developer ruling (Domain layer, interface's file),
  not inferred.
- **The description is calm and names the files, with no pointer rule (4.8.7).** 4.8.5 had changed the description
  to an imperative "MUST load before…" on purpose, so agents would not skip the skill. It is now a "Use when…"
  sentence naming `I*Repository.cs` and its EF Core implementation. A glob-scoped rule telling agents to load the
  skill was built in this line and removed before release (developer decision): in the evaluation (skills inventory
  report, §17) trimmed skills loaded from their names and descriptions alone. Repository guidance reached the agent
  in 4/5 skills-only runs against 5/5 with the rule, a difference within noise at n=5. If misses show up again,
  restoring the imperative description is the first lever. The full reasoning is in `Intent.Application.MediatR`'s
  `CONTEXT.md`, "Handler skills: trimmed text, no pointer rule".

## Invariants & Constraints

- **The skill-kind checks depend on template id substrings** owned by other modules: `QueryHandler` (MediatR,
  Mediator, Wolverine), `DomainEventHandler` (MediatR.DomainEvents, Mediator.DomainEvents, Wolverine.DomainEvents)
  and `ServiceImplementation` (ServiceImplementations). Renaming one of those skill templates silently gives it the
  command-handler wording.
