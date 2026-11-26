# Context: Intent.Entities

## Purpose

Generates the domain entities, their contracts and state classes from the Domain designer, and a `domain-entity` AI
skill for filling in the bodies of modelled constructors and operations.

## Architectural Decisions

- **The `domain-entity` skill is trimmed, not dropped (5.3.6).** In November 2026 it was cut to the rules an agent
  cannot know without this module: properties and signatures belong to the Domain designer; a modelled constructor or
  operation is generated `[IntentManaged(Mode.Fully, Body = Mode.Merge)]` with a stub (`// IntentInitialGen`, a TODO,
  `throw new NotImplementedException(...)`), and a written body replaces the stub and switches to
  `Body = Mode.Ignore`; the entity stays valid and enforces its invariants; Domain types only; the codebase's own
  exception type; private helpers are kept. Removed: generic DDD advice. Dropping the skill entirely was considered
  and rejected by the developer: the `Mode.Merge` → `Mode.Ignore` switch is Intent-specific and agents get it wrong
  without being told. The reasoning behind the trimmed shape is in `Intent.Application.MediatR`'s `CONTEXT.md`,
  "Handler skills: trimmed text, no pointer rule".
- **No pointer rule; the description carries the trigger.** No module ships glob-scoped rules that point at a skill
  (see the MediatR section named above), and no file glob picks out entity files reliably anyway (they sit in the
  Domain project under any folder name and share it with value objects, enums and contracts). Its description names the exact situation: the body of a
  constructor or operation on an entity or aggregate in the Domain project, including the stubs that throw
  `NotImplementedException`. Keep that specificity if the description is reworded.

## Invariants & Constraints

- **The skill describes the stub `DomainEntityTemplate` generates.** If the stub's attribute, `// IntentInitialGen`
  marker or exception changes, the skill text must change with it.
