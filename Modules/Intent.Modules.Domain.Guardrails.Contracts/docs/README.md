# Intent.Domain.Guardrails.Contracts

Guards the Domain designer's model in existing C# code. Each modelled element is merged into its existing file, so every modelled type and member keeps existing. Constructors and methods are always generated with empty bodies, so merging never adds code to an implementation.

Install it into an application whose existing code has been modelled in the Domain designer, by hand, with Intent's C# importer, or with the AI skill below. Apply the Type Configuration and Async Configuration stereotypes wherever the code differs from the defaults described below.

## AI skill: reverse-engineer-domain

The module generates `skills/reverse-engineer-domain/SKILL.md` into the application's agents folder (the `AI.Context.Skills` output). An AI agent working in Intent Architect uses it to:

- model an existing domain layer (entities, value objects, enums, repositories) in the Domain designer;
- install `Intent.Metadata.RDBMS` or `Intent.Metadata.DocumentDB` to match the code's persistence;
- align the Template Outputs with the existing folders, and apply Async Configuration to non-conventional operations;
- run the Software Factory until it changes nothing but the Intent file headers, then adopt the existing files.

## Templates

| Template              | Model          | Emits                                                                                       | Role                                        |
| --------------------- | -------------- | ------------------------------------------------------------------------------------------- | ------------------------------------------- |
| EntityContract        | Class          | Type with base type, attributes as properties, navigable associations, constructors and operations | `Guardrails.Contracts.Domain.Entity`        |
| EnumContract          | Enum           | `enum` with its literals and values                                                         | `Guardrails.Contracts.Domain.Enum`          |
| ValueObjectContract   | Value Object   | Type with attributes as properties (no equality implementation)                             | `Guardrails.Contracts.Domain.ValueObject`   |
| RepositoryContract    | Repository     | Concrete type with its operations, empty bodies (no interface is generated)                 | `Guardrails.Contracts.Domain.Repository`    |
| DomainContract        | Data Contract  | Type with attributes as properties and its base contract                                    | `Guardrails.Contracts.Domain.DataContract`  |
| DomainServiceContract | Domain Service | Type with its operations, empty bodies                                                      | `Guardrails.Contracts.Domain.DomainService` |

Put an Output Anchor carrying these roles at the root of the project that holds each kind of type. The Repository role usually goes on the project with the concrete repository classes, often Infrastructure. Designer folders then supply the rest of each file's path.

## Type Configuration stereotype

Every template except EnumContract emits a `class` by default. Apply **Type Configuration** to a Class, Value Object, Repository, Data Contract or Domain Service and set **Type** to `record` to emit a `record` instead. An empty Type, or no stereotype, means `class`.

- The Type must match the kind of the existing type on disk. Otherwise the merge adds a second declaration with the same name and the project won't compile.
- A record can only inherit from a record, and a class cannot inherit from a record. A mismatch between an element and its base type raises an error on the element.
- Structs are not supported, because Intent's C# file builder can't declare them.

## Generated shape

```csharp
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Acme.Domain.Orders
{
    public class Order : AggregateBase
    {
        public Order(Guid customerId)
        {
        }

        public Guid Id { get; set; }
        public ICollection<OrderLine> OrderLines { get; set; }

        public void Cancel(string reason)
        {
        }
    }
}
```

When merged into an existing file under `Mode.Merge`:

- A modelled member that is **missing** is added with the generated signature and an empty body. A new method that returns a value won't compile until its body is written.
- A modelled member that **already exists** is merged too: its signature (modifiers, return type, parameters) is brought in line with the model, and its body is merged with the empty generated body, so the existing statements stay as they are. Review the Software Factory diff before applying.
- Hand-written members that were never modelled are kept. A member that Intent itself added can be removed again when it is deleted from the model, depending on the application's RoslynWeaver "Merge Behaviour" setting.

## Conventions

- Every name is used exactly as it appears in the designer, with no casing changes: types, properties, navigation properties, constructors, methods, parameters, enum literals and namespaces. `OrderRepository` stays `OrderRepository` (no interface or `I` prefix is generated), and a property modelled as `orderDate` stays `orderDate`. The designer mirrors the code, so its casing is the codebase's own convention.
- Collections are `ICollection<T>` on properties and `IEnumerable<T>` on operation parameters and return types.
- An operation is async when its name ends in `Async`, unless the **Async Configuration** stereotype says otherwise. An async operation gets the `async` keyword, a `Task` or `Task<T>` return type (a modelled `Task`/`ValueTask` is not wrapped again) and a `CancellationToken cancellationToken = default` parameter. On the stereotype, **Is Async** explicitly sets async or sync, and **Include Cancellation Token** turns the extra parameter off. Abstract operations never get `async`. A new async method's empty body has no `await`, so the compiler warns (CS1998).
- Types modelled in the Domain designer resolve through this module's roles, so the right `using`s are added across folders and projects.
- Consumer projects need the `Intent.RoslynWeaver.Attributes` NuGet package.
