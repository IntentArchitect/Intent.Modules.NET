# Intent.Services.Guardrails.Contracts

Guards the Services designer's model in existing C# code. Each modelled element is merged into its existing file, so every modelled type and member keeps existing. Methods are always generated with empty bodies, so merging never adds code to an implementation.

Install it into an application whose existing code has been modelled in the Services designer, by hand, with Intent's C# importer, or with the AI skill below. Apply the Type Configuration and Async Configuration stereotypes wherever the code differs from the defaults described below.

## AI skill: reverse-engineer-services

The module generates `skills/reverse-engineer-services/SKILL.md` into the application's agents folder (the `AI.Context.Skills` output). An AI agent working in Intent Architect uses it to:

- model existing commands, queries, DTOs and services in the Services designer, mirroring the code's folders and record/class kinds;
- install the CQRS, WebApi, Security and DomainInteractions modules when the code calls for them;
- capture each endpoint's HTTP settings and security, and one primary Domain entity action per command or query;
- run the Software Factory until it changes nothing but the Intent file headers, then adopt the existing files.

Run it after `reverse-engineer-domain` (from Intent.Domain.Guardrails.Contracts) when the code has a domain layer.

## Templates

| Template        | Model   | Emits                                                                   | Role                                    |
| --------------- | ------- | ----------------------------------------------------------------------- | --------------------------------------- |
| ServiceContract | Service | Concrete type (never an interface) with its operations, empty bodies    | `Guardrails.Contracts.Services.Service` |
| CommandContract | Command | Type with its fields as properties; no `IRequest` or other interfaces   | `Guardrails.Contracts.Services.Command` |
| QueryContract   | Query   | Type with its fields as properties; no `IRequest` or other interfaces   | `Guardrails.Contracts.Services.Query`   |
| DtoContract     | DTO     | Type with fields as properties and its base DTO                         | `Guardrails.Contracts.Services.Dto`     |
| EnumContract    | Enum    | `enum` with its literals and values                                     | `Guardrails.Contracts.Services.Enum`    |

Put an Output Anchor carrying these roles at the Application project's root. Designer folders then supply the rest of each file's path.

## Type Configuration stereotype

Every template except EnumContract emits a `class` by default. Apply **Type Configuration** to a Service, Command, Query or DTO and set **Type** to `record` to emit a `record` instead, for example for a DTO that is a record in code. An empty Type, or no stereotype, means `class`.

- The Type must match the kind of the existing type on disk. Otherwise the merge adds a second declaration with the same name and the project won't compile.
- A DTO and its base DTO must have the same Type Configuration, because C# doesn't allow a record and a class to inherit from each other. A mismatch raises an error on the DTO.
- Structs are not supported, because Intent's C# file builder can't declare them.

## Generated shape

```csharp
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Acme.Application.Orders
{
    public record OrderDto
    {
        public Guid Id { get; set; }
        public List<OrderLineDto> Lines { get; set; }
    }
}
```

When merged into an existing file under `Mode.Merge`:

- A modelled member that is **missing** is added with the generated signature and an empty body. A new method that returns a value won't compile until its body is written.
- A modelled member that **already exists** is merged too: its signature (modifiers, return type, parameters) is brought in line with the model, and its body is merged with the empty generated body, so the existing statements stay as they are. Review the Software Factory diff before applying.
- Hand-written members that were never modelled are kept. A member that Intent itself added can be removed again when it is deleted from the model, depending on the application's RoslynWeaver "Merge Behaviour" setting.

## Conventions

- Every name is used exactly as it appears in the designer, with no casing changes: types, properties, methods, parameters, enum literals and namespaces. `OrderService` stays `OrderService` (no interface is generated), and a field modelled as `customerId` stays `customerId`. The designer mirrors the code, so its casing is the codebase's own convention.
- Collections are `List<T>`.
- An operation is async when its name ends in `Async`, unless the **Async Configuration** stereotype says otherwise. An async operation gets the `async` keyword, a `Task` or `Task<T>` return type (a modelled `Task`/`ValueTask` is not wrapped again) and a `CancellationToken cancellationToken = default` parameter. On the stereotype, **Is Async** explicitly sets async or sync, and **Include Cancellation Token** turns the extra parameter off. A new async method's empty body has no `await`, so the compiler warns (CS1998).
- Fields typed as Domain enums, entities, value objects or data contracts resolve through the Domain contract roles, with no reference to the Domain contract module.
- Consumer projects need the `Intent.RoslynWeaver.Attributes` NuGet package.
