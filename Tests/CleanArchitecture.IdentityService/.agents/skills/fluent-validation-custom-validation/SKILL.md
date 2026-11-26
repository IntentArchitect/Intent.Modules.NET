---
name: fluent-validation-custom-validation
description: Use when writing or changing a custom or async validation rule in a FluentValidation validator (*Validator.cs).
template-id: Intent.Application.FluentValidation.CustomValidationSkillTemplate
contentHash: 7A317167C63982EEFF51807ED440AAF7D4C3C2165CEA66D6B29733B2B1EA4A82
---
# FluentValidation Custom Validation

Fill in a custom validation method in an existing validator. Its signature is generated; only the body is yours.

## Rules

- Mark the custom validation method with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body. Don't change its signature.
- Implement the rule the codebase states: XML documentation, comments, the domain model or tests. If nothing states it, ask the user what the rule is rather than writing a check that always passes.
- Report a failure with `validationContext.AddFailure(...)`, in the wording nearby validators use. Don't throw.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a validator.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation so the designer can map to it; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`ExistsBy…Async`, `IsAvailableAsync`), not for storage.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the validator are fine.
