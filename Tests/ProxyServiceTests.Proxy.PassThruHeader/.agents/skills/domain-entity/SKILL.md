---
name: domain-entity
description: Use when writing or changing the body of a constructor or operation on a domain entity or aggregate in the Domain project, including the generated stubs that throw NotImplementedException.
template-id: Intent.Entities.DomainEntitySkill
contentHash: EB1503D2FD3964D89A3DF90F215BC6F31032438592E6832169E14E064B45D12D
---
# Domain Entity

Fill in a constructor or operation on a domain entity. The class, its properties, and its constructor and operation signatures come from the Domain designer; only the bodies are yours.

## Rules

- Properties belong to the model. To add or change a property, constructor or operation, or a signature, change it in the Domain designer and regenerate.
- A modelled constructor or operation is generated with `[IntentManaged(Mode.Fully, Body = Mode.Merge)]` and a stub: `// IntentInitialGen`, a TODO, and `throw new NotImplementedException(...)`. When you write the body, replace the stub and change the attribute to `Body = Mode.Ignore`, so the next run leaves it alone.
- A constructor leaves the entity valid. An operation enforces the invariants it touches and updates related state together.
- Depend only on Domain types. No Application, infrastructure or framework type belongs in an entity.
- For a rule violation, throw the exception type the codebase already uses for domain rules.
- Private helper methods you add to the entity are kept.
