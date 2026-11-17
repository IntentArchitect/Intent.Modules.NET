# Context: Intent.HotChocolate.GraphQL

## Purpose

Generates the Hot Chocolate GraphQL query, mutation and subscription types from GraphQL-enabled models. The dispatch modules (`Intent.HotChocolate.GraphQL.Dispatch.Services`, `.Dispatch.MediatR`) supply implicit resolver models and wire the method bodies; the hosting modules (`.AspNetCore`, `.AzureFunctions`) register the types.

## Architectural Decisions

- **Mutations without a return type are skipped, not given one (2026-11-14, user decision).** Hot Chocolate doesn't expose a method returning only `Task`, rejects `[UseMutationConvention]` on it ("must return a value"), and fails at startup when the `Mutation` root has no fields. `MutationTypeTemplate` leaves such resolvers out with a warning, and its `CanRunTemplate` is false when no resolver has a return type, so no empty `*Mutations` type extension is generated. Because void methods are never generated, the Services dispatch's `[UseMutationConvention]` never lands on one.
  - Rejected: generating a `bool` (or similar) return value for void operations. It adds an API shape the model doesn't show and changes existing apps' schemas.
  - Rejected: guarding each source (the Services and MediatR implicit registrations, the designer's own mutation types) separately. The template is the one place every source passes through.

## Module Interactions

- **`Intent.HotChocolate.GraphQL.AspNetCore` / `.AzureFunctions`** — register only mutation type instances whose `CanRunTemplate()` is true, and emit no `AddMutationType()` when none run. Hosting versions older than AspNetCore 6.1.0 / AzureFunctions 2.1.0 register every instance, so combined with this version they reference a mutation type that is no longer generated; update them together. Their floor on this module (5.1.0-pre.1) makes the hosting fix bring this one with it.
