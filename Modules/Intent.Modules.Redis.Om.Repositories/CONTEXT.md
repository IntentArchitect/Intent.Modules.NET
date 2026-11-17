# Context: Intent.Redis.Om.Repositories

## Purpose

Generates Redis OM documents, repositories and the start-up index creation for Domain aggregates persisted in Redis Stack.

## Architectural Decisions

- **The document id is always `[Indexed]`, decided in the template (2026-11-14).** `IndexCreationService` creates a search index for every aggregate root document at start-up, and Redis rejects `FT.CREATE` with an empty `SCHEMA` (`Fields arguments are missing`), so a document with no indexed field stops the app from starting. Repositories query with `RedisCollection` (`FT.SEARCH`), so the index is needed anyway. `RedisOmDocumentTemplate` adds `[Indexed]` next to `[RedisIdField]` whatever the model says, and skips the stereotype's own `[Indexed]` on the id so it is never emitted twice.
  - Rejected: fixing only the `Redis OM Key Automation` macro. It runs `on-created` and adds `Indexed` only when the attribute already has `Primary Key`, but the Domain key automation adds `Primary Key` after the attribute exists. A macro fix would also leave existing models broken until someone edits them again; the template fix repairs them on the next Software Factory run.
  - The macro still applies the stereotype where it can, so the designer shows the id as indexed. The template does not depend on it.
