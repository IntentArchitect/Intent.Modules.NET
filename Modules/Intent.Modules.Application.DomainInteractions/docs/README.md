# Intent.Application.DomainInteractions

## What This Module Does
Provides generation strategies for domain interaction implementations (e.g., find, create/update, projection patterns) defined using the Domain Interactions modelling specialization. It standardizes how application layer code orchestrates domain entity access and mutation beyond basic CRUD.

## Generated Behaviour (Conceptual)
- Interaction registration extension (via factory extension) wiring interaction strategies into the software factory pipeline.
- Implementations for modeled interaction types (e.g., ProjectTo, FindById, CreateOrUpdate) guided by domain model metadata.

## Interoperability
Detects related modelling modules: Domain, Services, Domain Interactions, Event Interactions, CRUD MediatR – ensuring presence of necessary metadata and base abstractions.

## Typical Workflow
1. Model a Domain Interaction (e.g., a projection aggregating several entities).
2. Run generation; module produces interaction implementation code and registers it.
3. Application services or request handlers invoke the interaction class/method instead of constructing repository queries manually.
4. Adjust/extend generated code via merge mode for specialized logic (validation, caching, authorization checks).

## Customization Points
- Extend generated interaction partial classes with business rules.
- Introduce caching wrappers around expensive projections.
- Inject additional domain services (e.g., calculators, policy evaluators).

## Related Modules
- `Intent.Application.MediatR.CRUD`
- `Intent.Application.ServiceImplementations.Conventions.CRUD`
- `Intent.Entities.Repositories.Api`
- `Intent.Eventing.Contracts` (for interactions emitting events)
