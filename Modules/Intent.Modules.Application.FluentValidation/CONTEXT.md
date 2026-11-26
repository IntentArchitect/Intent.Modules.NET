# Context: Intent.Application.FluentValidation

## Purpose

Provides the `Validations` stereotype and the shared validator-generation logic other modules use, and a
`fluent-validation-custom-validation` AI skill for filling in custom validation methods.

## Architectural Decisions

- **The skill keeps only Intent-specific rules (3.14.0).** Kept: `Body = Mode.Ignore` on the custom validation method;
  implement the rule the codebase states, and ask rather than write a check that always passes; report with
  `AddFailure`, don't throw; Domain and Application types only; the repository-method rule (model it when a designer
  will map to it, otherwise hand-add it); the `cancellationToken`; private helpers are fine. Removed: generic
  FluentValidation tutorial text. The reasoning behind the trimmed shape is in `Intent.Application.MediatR`'s
  `CONTEXT.md`, "Handler skills: trimmed text, no pointer rule".
- **Calm description that names the files, no pointer rule (3.14.0).** 3.12.3 had made the description an imperative
  "MUST load before…". It is now a "Use when…" sentence naming `*Validator.cs`, the name of every generated validator.
  A glob-scoped rule telling agents to load the skill was built in this line and removed before release (developer
  decision): in the evaluation (skills inventory report, §17) trimmed skills loaded from their names and descriptions
  alone. The full reasoning is in the MediatR section named above.
- **The version stays 3.14.0.** The minor move was taken for the pointer rule, which no longer ships. It was not
  walked back: the version gate allows one move per line of work, and the rewritten skill and corrected
  `Intent.Common` floor still ship under it.

## Module Interactions

- **`Intent.Common`** — written against 3.11.2, the version the project compiles against. The markdown file builder
  behind the skill needs at least 3.11.1. The `.imodspec` declared 3.11.0 until 3.14.0.
