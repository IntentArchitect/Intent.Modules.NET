# Intent.Domain.Guardrails.Contracts

Guards the Domain designer's model in existing C# code. Each modelled element is merged into its existing file, so every modelled type and member keeps existing. Constructors and methods are always generated with empty bodies, so merging never adds code to an implementation.

Install it into an application whose existing code has been modelled in the Domain designer, by hand, with Intent's C# importer, or with the AI skill below. Set the Default Type Modifiers module setting to the code's convention, and apply the Type Configuration and Async Configuration stereotypes wherever the code differs from the defaults described below.

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
| DomainEventContract   | Domain Event   | Type with its properties and base Domain Event. Generated only when `Intent.Modelers.Domain.Events` is installed | `Guardrails.Contracts.Domain.DomainEvent` |

Put an Output Anchor carrying these roles at the root of the project that holds each kind of type. The Repository role usually goes on the project with the concrete repository classes, often Infrastructure. Designer folders then supply the rest of each file's path.

## Type Configuration stereotype

Every template except EnumContract emits a `class` by default. Apply **Type Configuration** to a Class, Value Object, Repository, Data Contract, Domain Service or Domain Event and set **Type** to `record` to emit a `record` instead. An empty Type, or no stereotype, means `class`.

- The Type must match the kind of the existing type on disk. Otherwise the merge adds a second declaration with the same name and the project won't compile.
- A record can only inherit from a record, and a class cannot inherit from a record. A mismatch between an element and its base type raises an error on the element.
- Structs are not supported, because Intent's C# file builder can't declare them.

## Type modifiers

Under Merge mode the generated declaration's modifiers replace the existing ones, so they must match the code.

- **Default Type Modifiers** (module setting, in Domain Guardrails Contracts Settings) applies to every class, record and enum contract: `public` (the default) or `internal`, each optionally followed by `sealed`, `partial` or `sealed partial`. Set it to the codebase's own convention, e.g. `internal sealed`.
- **Type Configuration → Modifiers** replaces the setting for one element, e.g. `public`, `internal static` or `public abstract partial`. Use one of `public` or `internal`, plus any of `sealed`, `abstract`, `static` and `partial`, in any order; they are written in the usual C# order. A type declared without an access modifier is `internal`, so model it as `internal`.
- **abstract:** Is Abstract on a Class or Domain Event, or an abstract operation, always adds `abstract`.
- **sealed from the setting** is left off an abstract type, and off a Class, Data Contract or Domain Event that another modelled element inherits from.
- **Errors:** Modifiers set on an element are used as given. These raise an error on the element: `sealed` or `static` on an abstract or inherited type, `static` on a record, two access modifiers, more than one of `sealed`, `abstract` and `static`, or a keyword the builder can't write (`file`, `unsafe`, …).
- **Enums** take only the setting's access modifier (`internal sealed` gives `internal enum`). Where an enum differs, apply **Enum Configuration** and set **Access Modifier** to `public` or `internal`. An enum declared without an access modifier is `internal`.

## File Config stereotype

Each element is generated into `<ElementName>.cs` in its folder, with the namespace derived from its folders. Apply **File Config** to a Class, Value Object, Repository, Data Contract, Domain Service, Enum or Domain Event only where the code differs. Both properties are optional; set just the one you need.

- **Namespace:** replaces the derived namespace, for a file whose namespace doesn't follow its folders. It is used as given, with no casing changes, and references to the type get the matching `using`.
- **No Namespace:** tick it for a type declared in the global namespace (no `namespace` declaration in the file). Leave Namespace empty; setting both is an error. References to the type need no `using`.
- **Filename** (with or without `.cs`; a file name only, as the folder stays the element's own):
  - **File named differently from its type**, e.g. `orderEntity.cs` holds `Order`: set `Filename = orderEntity`.
  - **File holding several types:** give each type that isn't the file's namesake the file's name. All contract elements that end up at the same path are generated into one file. The element whose name matches the file name writes it (otherwise the first by name), and the other elements' templates write no file of their own. References to every type in the file still resolve.
- **Shared files and namespaces:** every type in one file must end up with the same namespace. If the file's namespace isn't the derived one, give each of its types the same Namespace.
- **Changing it later:** if a type already had its own generated file and you then point it at another file, or change its Filename, the Software Factory offers to delete the old file. Review that before applying.
- **Invalid values:** a Filename with a folder in it or invalid file-name characters, or a Namespace that isn't a valid C# namespace, raises an error on the element.

## Collection Type stereotype

By default, collection properties are generated as `ICollection<T>`, and operation parameters and return types as `IEnumerable<T>`. Where the code uses a different collection, apply **Collection Type** to the attribute, the association end (navigation property), the constructor or operation parameter, or the operation itself (its return type), and choose the **Type**:

| Type | Emits |
|---|---|
| IEnumerable, ICollection, IList, IReadOnlyCollection, IReadOnlyList, List, HashSet, ISet | `IEnumerable<T>`, `ICollection<T>`, … (`System.Collections.Generic`) |
| IQueryable | `IQueryable<T>` (`System.Linq`) |
| Array | `T[]` |
| Custom | **Custom Format**, with `{0}` for the element type, e.g. `System.Collections.ObjectModel.ObservableCollection<{0}>` |

The Type must match the code. Under Merge mode, the generated signature replaces the existing one, so a wrong collection type rewrites the member. On an async operation the return type is wrapped after the collection is applied, giving `Task<IEnumerable<T>>`. The stereotype is ignored on a type that isn't a collection.

## Parameter Modifier stereotype

Apply **Parameter Modifier** to a constructor or operation parameter where the code declares a modifier, and set **Modifier** to `ref`, `out`, `in`, `ref readonly` or `params`. It must match the code: under Merge mode the generated signature replaces the existing one, so a missing or wrong modifier rewrites the method. For `params`, model the parameter as a collection and set Collection Type to `Array` (`params string[] tags`). `this` (extension methods) is not supported.

## Generated shape

```csharp
[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]

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

Every file carries both directives: plain `Mode.Merge` for the file, plus a default for constructors and methods that fully manages their signatures and merges their attributes and comments. When merged into an existing file:

- A modelled member that is **missing** is added with the generated signature and an empty body. A new method that returns a value won't compile until its body is written.
- A modelled member that **already exists** is merged too: its signature (modifiers, return type, parameters) is brought in line with the model, and its body is merged with the empty generated body, so the existing statements stay as they are. Review the Software Factory diff before applying.
- Hand-written members that were never modelled are kept. A member that Intent itself added can be removed again when it is deleted from the model, depending on the application's RoslynWeaver "Merge Behaviour" setting.

## Conventions

- Every name is used exactly as it appears in the designer, with no casing changes: types, properties, navigation properties, constructors, methods, parameters, enum literals and namespaces. `OrderRepository` stays `OrderRepository` (no interface or `I` prefix is generated), and a property modelled as `orderDate` stays `orderDate`. The designer mirrors the code, so its casing is the codebase's own convention.
- Collections are `ICollection<T>` on properties and `IEnumerable<T>` on operation parameters and return types, unless Collection Type says otherwise. They are written by their short name with a `using` (`IEnumerable<T>`, not `System.Collections.Generic.IEnumerable<T>`), even in a file that also imports `System.Collections`.
- An operation is async when its name ends in `Async`, unless the **Async Configuration** stereotype says otherwise. An async operation gets the `async` keyword, a `Task` or `Task<T>` return type (a modelled `Task`/`ValueTask` is not wrapped again) and a `CancellationToken cancellationToken = default` parameter. On the stereotype, **Is Async** explicitly sets async or sync, and **Include Cancellation Token** turns the extra parameter off. Abstract operations never get `async`. A new async method's empty body has no `await`, so the compiler warns (CS1998).
- Types modelled in the Domain designer resolve through this module's roles, so the right `using`s are added across folders and projects.
- Consumer projects need the `Intent.RoslynWeaver.Attributes` NuGet package.
