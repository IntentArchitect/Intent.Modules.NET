---
name: ef-repository
description: Use when adding or changing a query or persistence method on a repository, in an I*Repository.cs interface in the Domain project or its EF Core implementation in Infrastructure.
template-id: Intent.EntityFrameworkCore.Repositories.EFRepositorySkillTemplate
contentHash: 368B28933AAB845DDACC25BF0FD33C4D0253D19417104B5BBE4D66EAA9DD55D6
---
# EF Repository Extension

Add a query or persistence method to a repository: its interface in the Domain project and its EF Core implementation in Infrastructure.

## Rules

- Read the base repository interface first, and use an existing method when one fits. Add a method for an aggregate result or a query the base methods can't express.
- If a designer will use the method, for example mapped from a command or query, model it as a repository operation in the Domain designer and regenerate, so the designer can map to it. Otherwise add it by hand to the interface, then to the implementation.
- Put no `[IntentManaged]` attribute on a method you add to the interface. The generated methods are `Mode.Fully`, and a `Mode.Fully` method the model doesn't produce is deleted on the next run.
- Mark a method you add to the implementation with `[IntentIgnore]`.
- Do the joins, filtering and aggregation in the repository, so callers get the result rather than data to finish in memory.
- Never return a tuple. For a composite result, declare a record in the interface's file, below the interface, so it stays in the Domain layer, and mark it `[IntentIgnore]` or the next run removes it.
- Name the method for the domain intent (`GetForUpdateAsync`, `ExistsBy…Async`) and take a `CancellationToken`.
