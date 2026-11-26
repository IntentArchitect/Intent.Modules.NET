# Context: Intent.DomainServices

## Purpose

Generates domain service interfaces and implementations from the Domain designer, and a `domain-services` AI skill for
filling in their method bodies.

## Architectural Decisions

- **The `domain-services` skill keeps only Intent-specific rules (1.2.3).** Kept: `Body = Mode.Ignore` on a body you
  write; signatures change in the Domain designer, since the interface is fully generated; Domain types only;
  repositories are allowed; the repository-method rule (model it when a designer will map to it, otherwise hand-add
  it); a single-entity rule belongs on the entity; the `cancellationToken`; private helpers are fine. Removed: generic
  DDD advice. The reasoning behind the trimmed shape is in `Intent.Application.MediatR`'s `CONTEXT.md`, "Handler
  skills: trimmed text, no pointer rule".
- **A domain service never saves.** Developer ruling: it may use repositories but not the unit of work, because the
  pipeline that called it (a command handler or service operation) commits. Before 1.2.3 the skill listed both
  repositories and the unit of work as invalid dependencies; it now allows repositories. `Intent.UnitTesting`'s
  `unit-test-guidelines` skill no longer asks domain-service tests to verify a save. Keep the two consistent.
- **No pointer rule; the description carries the trigger.** No module ships glob-scoped rules that point at a skill
  (see the MediatR section named above), and domain service files have no reliable name pattern for one anyway. The
  description names the situation: a method body in a domain service, the I-prefixed interface
  and its class in the Domain project, in the folder the service is modelled in, including the stubs that throw
  `NotImplementedException`. An earlier wording named a "Services folder", which isn't generated: the files go in the
  modelled folder (found in the 2026-11-26 module spike).

## Module Interactions

- **`Intent.UnitTesting`** — its `unit-test-guidelines` skill covers domain-service tests and must not contradict
  the no-save rule above.
