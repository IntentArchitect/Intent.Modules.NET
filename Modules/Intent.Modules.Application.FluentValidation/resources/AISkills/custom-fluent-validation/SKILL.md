---
name: custom-fluent-validation
description: Implements the body of a FluentValidation CustomAsync method in a C# clean-architecture project, injecting required services into the constructor if needed.
---

# Fluent Validation Custom Method Implementation

You are implementing the body of a custom FluentValidation async validation method in a C# clean-architecture project.

What you are given: A validator class (e.g. `CreateCustomerCommandValidator`) that inherits from `AbstractValidator<T>`. Inside it is one or more `private async Task` methods with the signature:

```
private async Task SomeValidationMethodAsync(
    TProperty value,
    ValidationContext<TCommand> validationContext,
    CancellationToken cancellationToken)
```

These are registered via `.CustomAsync(SomeValidationMethodAsync)` in `ConfigureValidationRules`. Their bodies currently throw `NotImplementedException`.

Your task:

1. Read the validator file the user points you to. Identify the unimplemented custom method(s) — the ones whose body only throws NotImplementedException.

2. Infer the intent from all available context:
- The method name (e.g. ValidateEmailAsync → validate email format or uniqueness)
- The property being validated and its type
- The command/DTO class — read it to understand the full shape of the object
- Domain entity classes in the Domain/Entities folder — read them to understand the data model
- Repository interfaces in the Domain/Repositories folder — check what query methods are available
- Any related domain rules, naming conventions, or sibling validators that hint at intent

3. Implement the method body. Rules:
- Do NOT change the method signature — only the body between the braces
- Use validationContext.AddFailure(...) to report failures (do not throw exceptions)
- Use async/await properly; if no async work is needed, return Task.CompletedTask
- Returning Task.CompletedTask is fine only when the method contains actual validation logic that is synchronous.
- Use cancellationToken when calling async methods
- If a validation method’s intent is not explicitly documented in code (comment, domain rule, existing validator pattern, or tests), OR the intent is not fairly obvious, the assistant MUST stop and ask 1–4 clarifying questions via ask_user_question before implementing. 
    - Treat XML docs as authoritative requirements
- You MUST NOT:
    - invent validation rules
    - add "placeholder" failures
    - implement a no-op (e.g. return; / Task.CompletedTask) merely to avoid NotImplementedException
    - remove the rule hookup (CustomAsync(...)) unless the user explicitly approves it.
- If XML documentation explicitly specifies a validation requirement, implement it as written without asking the user for confirmation. If implementing it requires a repository/service method that does not exist, add the minimal required method to the appropriate interface and implementation, inject that dependency into the validator, and proceed. Only ask the user when the comment is still ambiguous (e.g., scope of uniqueness, accepted formats), not when it is merely missing supporting infrastructure.
- If the requirement implies external access, auto-discover or extend dependencies
- If the intent is suggested by comments/naming, the assistant should attempt to confirm it by reading referenced enum/type definitions. Ask the user only if the required symbol/service cannot be found in code.
- Do not introduce new validation constraints (e.g., min length, regex, allowed characters) unless they are explicitly specified in:
    - XML docs/comments on the property/DTO/command, or
    - an existing domain rule/value object, or
    - an existing validator/test demonstrating the same rule.
- If not specified, you MUST ask clarifying questions and THEN only implement no additional rule beyond what already exists in the validator chain, IF the user provides no additional context.
- "No additional rules" is NOT an acceptable default assumption, unless specifically instructed by the user. If the user provides no additional context, you MUST ask follow-up questions to confirm whether any additional rules are needed, and if so, what they are. If the user confirms that no additional rules are needed, you may implement a no-op (e.g. return Task.CompletedTask) in that case.

4. Dependency discovery requirement: 
Before asking the user for clarification, the assistant must read the definitions of any non-primitive types involved in the rule (enums, value objects, DTOs) 
and search for relevant repository/service interfaces that could support the validation (e.g., ICustomerRepository, ICustomerReadRepository). 
Only ask the user if, after this, the rule intent is still ambiguous or required infrastructure is missing.

5. If services are needed (e.g. a repository to check uniqueness in the database):
- Add the required interface as a constructor parameter
- Store it in a private readonly field
- Update the constructor body to assign it
- Do NOT change the existing constructor signature shape — only add new parameters; keep the existing IValidatorProvider provider parameter if it is already there

6. If a STRONG intent cannot be reasonably inferred from the available code, or the intent is ambiguous, stop and ask the user:
- What the validation rule should enforce (e.g. "must be unique", "must match a regex", "must exist in the database")
- Whether any external service or repository is needed
- Any domain-specific rules that are not visible in the code
- Where requirements are not explicitly stated in code/comments/docs/tests, DO NOT implement "minimal" validation - in this case you MUST stop and ask clarifying questions first.
- Only ask if:
    - there are conflicting hints (comment says Delivery, but enum has no Delivery, etc.), or
    - implementing would require a new external dependency (repo/service) that isn’t already available and there’s no clear existing interface method to use, or
    - multiple plausible interpretations remain even after reading the referenced types.

If a CustomAsync method has no explicit requirement (XML docs/comments/tests/domain rule), do NOT make up an implemention and do NOT implement a no op implementation (unless specifically instructed to) - you MUST ask what the rule should be.
You must NEVER implement a no-op (e.g. return Task.CompletedTask) just to avoid NotImplementedException — if there is no clear intent, you MUST ask the user what the rule should be.
Do NOT guess wildly — a wrong implementation is worse than asking. If the method name and property together make the intent obvious (e.g. ValidateEmailAsync on an Email string property), proceed. If the name is ambiguous (e.g. ValidateIdNumberAsync where the rule could be format-based or uniqueness-based), ask. 
Do NOT prioritize "making the validator functional" over correctness to the intended business rules. If it is not OBVIOUS as to the intent of the validation, always seek clarification from the user by asking question(s).