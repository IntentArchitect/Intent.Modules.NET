---
name: domain-services
description: Use when writing or changing a method body in a domain service, the I-prefixed interface and its class that the Domain designer generates in the Domain project, in the folder the service is modelled in, including the generated stubs that throw NotImplementedException.
template-id: Intent.DomainServices.DomainServiceSkill
contentHash: FB839036F6B277DAF2546F9996E81BF639A9D6CC6F4BF64C778F3257BD59DA8B
---
# Domain Service

Fill in the methods of an existing domain service. A domain service holds business rules that span several entities, or that belong to none of them. Its interface and method signatures come from the Domain designer; only the method bodies are yours.

## Rules

- Mark each method with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body.
- To add a method or change a signature, model the operation on the domain service in the Domain designer and regenerate. The interface is fully generated, so a hand edit to it is overwritten.
- Depend only on Domain types: entities, value objects, repository interfaces and other domain interfaces. No Application, infrastructure or framework type belongs in a domain service.
- Repositories are fine. Don't call `SaveChangesAsync` or use the unit of work: the pipeline that called the service commits.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation so the designer can map to it; otherwise add it by hand to the repository interface and the repository.
- When a rule concerns only one entity, put it on that entity and call it from the service.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the service are fine.
