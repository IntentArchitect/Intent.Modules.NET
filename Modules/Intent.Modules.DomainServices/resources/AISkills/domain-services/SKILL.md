---
name: domain-services
description: guidance for implementing missing c# domain service functionality in clean architecture solutions. use when an ai coding agent must complete methods in domain-level service classes where domain entities live in a separate project with no infrastructure dependencies, service interfaces belong in the domain, and implementations must express business logic that does not naturally fit inside a single entity.
---

# Domain Services Implementation

## Purpose

Use this skill to implement missing C# Domain Service methods in a Clean Architecture codebase while keeping the Domain project dependency-free.

A Domain Service here means a domain-level class that expresses business rules that do not belong cleanly inside one Entity or Value Object. It is not an application service, infrastructure service, repository implementation, API handler, or orchestration layer.

## Core rules

- Keep the Domain project independent. Do not add dependencies on application, infrastructure, persistence, web, logging, HTTP, EF Core, configuration, or external SDK packages.
- Keep interfaces used by the Domain Service in the Domain project.
- Use constructor injection only for domain abstractions that already exist in the Domain layer, or that clearly belong there.
- Implement business logic in terms of Entities, Value Objects, domain enums, domain interfaces, and domain exceptions/results already present in the codebase.
- Do not perform database queries, file access, network calls, message publishing, transactions, caching, or framework-specific behavior inside the Domain Service.
- Prefer deterministic, side-effect-light logic. Mutating domain objects is acceptable when that is the established domain model style.
- Do not create an anemic pass-through service. If the behavior clearly belongs on one Entity or Value Object, move or suggest moving it there instead.
- Preserve existing naming, nullability style, guard clauses, result/error style, exception conventions, and test patterns.

## Implementation workflow

1. Inspect the Domain Service class, its interface, related entities/value objects, domain exceptions/results, and any existing tests.
2. Identify each unimplemented method and write down the business invariant it should enforce.
3. Determine where the logic belongs:
   - Single aggregate/entity behavior: implement or delegate to that entity when possible.
   - Cross-entity, cross-value-object, or policy logic: implement in the Domain Service.
   - Infrastructure/app orchestration: do not implement in the Domain Service; surface a clear domain abstraction or leave orchestration outside the domain.
4. Implement the smallest complete domain-only solution.
5. Add or update focused unit tests for the business rules and edge cases.
6. Check that the Domain project still has no new external dependencies.

## DI guidance

Use dependency injection only for domain concepts, for example:

```csharp
public sealed class PricingDomainService : IPricingDomainService
{
    private readonly IDiscountPolicy _discountPolicy;

    public PricingDomainService(IDiscountPolicy discountPolicy)
    {
        _discountPolicy = discountPolicy ?? throw new ArgumentNullException(nameof(discountPolicy));
    }
}
```

Valid injected dependencies are domain interfaces such as policies, calculators, clocks already abstracted as domain concepts, rule providers, or specifications.

Invalid injected dependencies include `DbContext`, repositories that hide persistence orchestration for the method, HTTP clients, loggers, application services, mediator, unit of work, configuration, queues, or infrastructure clients.

## Coding style

- Match the existing project style before introducing new patterns.
- Use clear guard clauses for invalid inputs when consistent with the codebase.
- Keep methods readable; extract private helpers for repeated domain calculations or rule checks.
- Prefer Value Objects over primitive obsession when the codebase already has them.
- Prefer existing domain result types over throwing if the project uses result-based validation.
- Make async methods only when the existing domain abstraction requires async. Do not introduce async for purely in-memory domain logic.

## Output expected from the AI coding agent

When completing code, provide:

1. The implemented Domain Service method(s).
2. Any minimal domain-only helper types or private helpers needed.
3. Unit tests or test cases for important business paths.
4. A brief note if any requested behavior actually belongs outside the Domain layer.

Keep the response focused on code changes, not Clean Architecture theory.

## Red flags to call out

Call out the issue before coding when the missing implementation appears to require:

- Application workflow orchestration.
- Logic that clearly belongs inside a single aggregate/entity.

In those cases, suggest a domain interface only if it represents a true domain concept; otherwise keep the behavior in the Application or Infrastructure layer.
