# CONTEXT.md — Intent.Application.DomainInteractions

## Purpose

Generates the C# implementations of modelled domain interactions (Entity Actions, Processing Actions) through interaction strategies, and, from 1.3.0, owns the accelerators that create those interactions from a domain element: the CRUD accelerators and the repository-operation suggestions.

## Architectural Decisions

- **CRUD accelerators moved here from `Intent.Modelers.Services.DomainInteractions` (v1.3.0)**: `Create CRUD CQRS Operations` / `Create CRUD Traditional Service` (basic and advanced), the Domain-designer CRUD suggestions, the repository-operation suggestions, the `Crud Api` and `Repository Operation Api` scripts and their documentation topics. Rule: the designer module holds intention; anything that starts from a domain element and generates services from it belongs here. Every .NET CRUD module already depends on this module. Accepted cost: Java and Node.js CRUD modules depend only on the designer module and lose the accelerators.

## Invariants & Constraints

- **Released together with `Intent.Modelers.Services.DomainInteractions` 2.5.0.** This module depends on it at 2.5.0 (stops every accelerator appearing twice), and its `detect` installs this module at 1.3.0 when present. Publishing one without the other breaks installs or duplicates menus. 2.5.0 needs Intent Architect 4.6.0, so `supportedClientVersions` starts at 4.6.0.
- **`resources/crud-api.js` and `resources/repository-operation-api.js` are compiled output, not source.** Their TypeScript lives in the Intent.Modules repo under `DesignerMacros/src`; each bundle's `tsconfig-save-to-module.json` writes here, and only resolves when the two repos are siblings (as under `D:\Dev`, not in worktrees). Never hand-edit the `.js`; the next compile replaces it. No CI compiles them here, so a TS fix ships only when someone compiles and commits the output. `crud-api.js` still lags its TS sources (it lacks the recursion guard from Intent.Modules commit `0304849817`), so the next compile is a behaviour change that needs its own release note.
- **The accelerators read `Default Mapping Mode` by id** from `Intent.Modelers.Services.DomainInteractions` (setting group `0ecca7c5-96f7-449a-96b9-f65ba0a4e3ad`, field `14bc1e98-930b-494c-97a9-5da4e1f3a5fa`).

## Module Interactions

- **Intent.Modelers.Services.DomainInteractions** — owns the Entity Action vocabulary and the module settings the accelerators read. See the release-together invariant above.
