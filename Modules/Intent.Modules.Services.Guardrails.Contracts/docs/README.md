# Intent.Services.Guardrails.Contracts

Guards the Services designer's model in existing C# code. Each modelled element is merged into its existing file, so every modelled type and member keeps existing. Methods are always generated with empty bodies, so merging never adds code to an implementation.

Install it into an application whose existing code has been modelled in the Services designer, by hand, with Intent's C# importer, or with the AI skill below. Set the Default Type Modifiers module setting to the code's convention, and apply the Type Configuration and Async Configuration stereotypes wherever the code differs from the defaults described below.

## AI skill: reverse-engineer-services

The module generates `skills/reverse-engineer-services/SKILL.md` into the application's agents folder (the `AI.Context.Skills` output). An AI agent working in Intent Architect uses it to:

- model existing commands, queries, DTOs and services in the Services designer, mirroring the code's folders and record/class kinds;
- model integration events, integration commands, eventing DTOs and their handlers when the code has messaging;
- install the CQRS, WebApi, Security, DomainInteractions, Eventing and EventInteractions modules when the code calls for them;
- capture each endpoint's HTTP settings and security, and one primary Domain entity action per command or query;
- run the Software Factory until it changes nothing but the Intent file headers and the code's formatting, then adopt the existing files.

Run it after `reverse-engineer-domain` (from Intent.Domain.Guardrails.Contracts) when the code has a domain layer.

## Templates

| Template                        | Model                         | Emits                                                                                                                                                         | Role                                                    |
| ------------------------------- | ----------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------- |
| ServiceContract                 | Service                       | Concrete type (never an interface) with its operations, empty bodies                                                                                          | `Guardrails.Contracts.Services.Service`                 |
| CommandContract                 | Command                       | Type with its fields as properties; no `IRequest` or other interfaces                                                                                         | `Guardrails.Contracts.Services.Command`                 |
| QueryContract                   | Query                         | Type with its fields as properties; no `IRequest` or other interfaces                                                                                         | `Guardrails.Contracts.Services.Query`                   |
| DtoContract                     | DTO                           | Type with fields as properties and its base DTO                                                                                                               | `Guardrails.Contracts.Services.Dto`                     |
| EnumContract                    | Enum                          | `enum` with its literals and values                                                                                                                           | `Guardrails.Contracts.Services.Enum`                    |
| DomainEventHandlerContract      | Domain Event Handler          | Class only, no members. Handle methods and handler interfaces stay in the code. Generated only when `Intent.Modelers.Domain.Events` is installed              | `Guardrails.Contracts.Services.DomainEventHandler`      |
| IntegrationEventContract        | Message ("Integration Event") | Type with its properties; no base type or interfaces. Generated only when `Intent.Modelers.Eventing` is installed                                             | `Guardrails.Contracts.Services.IntegrationEvent`        |
| IntegrationCommandContract      | Integration Command           | Type with its properties; no base type or interfaces. Generated only when `Intent.Modelers.Eventing` is installed                                             | `Guardrails.Contracts.Services.IntegrationCommand`      |
| EventingDtoContract             | Eventing DTO                  | Type with fields as properties and its base Eventing DTO. Generated only when `Intent.Modelers.Eventing` is installed                                         | `Guardrails.Contracts.Services.EventingDto`             |
| IntegrationEventHandlerContract | Integration Event Handler     | Class only, no members. Handle methods and handler interfaces stay in the code. Generated only when `Intent.Modelers.Services.EventInteractions` is installed | `Guardrails.Contracts.Services.IntegrationEventHandler` |

Put an Output Anchor carrying these roles at the Application project's root. Designer folders then supply the rest of each file's path. Integration events, integration commands and eventing DTOs often live in a separate Contracts project; give their roles an Output Anchor there instead.

## Integration eventing

Integration events, integration commands, eventing DTOs and integration event handlers are guarded the same way as commands, DTOs and domain event handlers. The module has no dependency on `Intent.Modelers.Eventing` or `Intent.Modelers.Services.EventInteractions`: the four templates generate nothing until those modelers are installed and the elements are modelled.

- **Only this application's own packages.** A subscriber normally references the publisher's eventing package, so its Services designer shows the publisher's messages, eventing DTOs and enums too. The eventing templates, and EnumContract, generate only elements in packages the application owns.
- **Names** are used verbatim; no `Event` suffix is added.
- **Handlers are class-only.** Handle methods and handler interfaces depend on the message broker (`IConsumer<T>`, `IHandleMessages<T>`, …), so Merge keeps the existing ones. The Subscribe Integration Event and Subscribe Integration Command associations document the subscription; the template doesn't read them.

Not supported yet:

- **Positional records** (`record OrderPlaced(Guid OrderId)`): the template emits property-style members, which would be merged as a second set.
- **Property initializers on messages:** the Eventing designer has no Value on a Message or Integration Command property. Eventing DTO fields do have one.
- **A subscriber's own copy of a message** that is modelled in another application's package: it is never generated in the subscriber.
- **Accessors:** every property is emitted as `{ get; set; }`, as for DTOs. What Merge does to an existing `init` or get-only accessor hasn't been verified, so review the Software Factory diff for those properties.

## Type Configuration stereotype

Every template except EnumContract emits a `class` by default. Apply **Type Configuration** to a Service, Command, Query or DTO and set **Type** to `record` to emit a `record` instead, for example for a DTO that is a record in code. An empty Type, or no stereotype, means `class`.

- The Type must match the kind of the existing type on disk. Otherwise the merge adds a second declaration with the same name and the project won't compile.
- A DTO and its base DTO must have the same Type Configuration, because C# doesn't allow a record and a class to inherit from each other. A mismatch raises an error on the DTO.
- Integration events, integration commands and eventing DTOs have their own **Type Configuration** with the same Type and Modifiers, because the module doesn't depend on the Eventing modeler. An eventing DTO and its base eventing DTO must also have the same Type.
- Structs are not supported, because Intent's C# file builder can't declare them.

## Type modifiers

Under Merge mode the generated declaration's modifiers replace the existing ones, so they must match the code.

- **Default Type Modifiers** (module setting, in Services Guardrails Contracts Settings) applies to every class, record and enum contract: `public` (the default) or `internal`, each optionally followed by `sealed`, `partial` or `sealed partial`. Set it to the codebase's own convention, e.g. `internal sealed`. The Domain contract module has its own setting.
- **Type Configuration → Modifiers** replaces the setting for one element, e.g. `public`, `internal static` or `public sealed partial`. Use one of `public` or `internal`, plus any of `sealed`, `abstract`, `static` and `partial`, in any order; they are written in the usual C# order. A type declared without an access modifier is `internal`, so model it as `internal`.
- **Domain Event Handlers and Integration Event Handlers** share a Type Configuration with Modifiers only, because a handler is always a class.
- **abstract:** Is Abstract on a DTO or eventing DTO always adds `abstract`.
- **sealed from the setting** is left off an abstract DTO or eventing DTO, and off one that another DTO (or eventing DTO) inherits from.
- **Errors:** Modifiers set on an element are used as given. These raise an error on the element: `sealed` or `static` on an abstract or inherited type, `static` on a record, two access modifiers, more than one of `sealed`, `abstract` and `static`, or a keyword the builder can't write (`file`, `unsafe`, …).
- **Enums** take only the setting's access modifier (`internal sealed` gives `internal enum`). Where an enum differs, apply **Enum Configuration** and set **Access Modifier** to `public` or `internal`. An enum declared without an access modifier is `internal`.

## File Config stereotype

Each element is generated into `<ElementName>.cs` in its folder, with the namespace derived from its folders. Apply **File Config** to a Service, Command, Query, DTO, Enum, Domain Event Handler, Integration Event, Integration Command, Eventing DTO or Integration Event Handler only where the code differs. Both properties are optional; set just the one you need.

- **Namespace:** replaces the derived namespace, for a file whose namespace doesn't follow its folders (e.g. `Basislink.DTO` in a sub-folder). It is used as given, with no casing changes, and references to the type get the matching `using`.
- **No Namespace:** tick it for a type declared in the global namespace (no `namespace` declaration in the file). Leave Namespace empty; setting both is an error. References to the type need no `using`.
- **Filename** (with or without `.cs`; a file name only, as the folder stays the element's own) covers the cases below.

- **File named differently from its type:** `psMembersDTO.cs` holds `PsMembersDTO`, so set `Filename = psMembersDTO`.
- **File holding several types:** give each type that isn't the file's namesake the file's name, e.g. `OrderLineDto` with `Filename = OrderDto` joins `OrderDto.cs`. All contract elements that end up at the same path are generated into one file. The element whose name matches the file name writes it (otherwise the first by name), and the other elements' templates write no file of their own. References to every type in the file still resolve, with the right `using`.
- **Changing it later:** if a type already had its own generated file and you then point it at another file, or change its Filename, the Software Factory offers to delete the old file. Review that before applying.
- **Shared files and namespaces:** every type in one file must end up with the same namespace. If the file's namespace isn't the derived one, give each of its types the same File Config Namespace.
- **Invalid values:** a Filename with a folder in it (`/` or `\`) or invalid file-name characters, or a Namespace that isn't a valid C# namespace, raises an error on the element.

## Collection Type stereotype

Collections are generated as `List<T>` by default. Where the code uses a different collection, apply **Collection Type** to the DTO field (also on commands and queries), the operation parameter, the operation itself (its return type), or an integration event, integration command or eventing DTO property, and choose the **Type**:

| Type                                                                                     | Emits                                                                                                               |
| ---------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| IEnumerable, ICollection, IList, IReadOnlyCollection, IReadOnlyList, List, HashSet, ISet | `IEnumerable<T>`, `ICollection<T>`, … (`System.Collections.Generic`)                                                |
| IQueryable                                                                               | `IQueryable<T>` (`System.Linq`)                                                                                     |
| Array                                                                                    | `T[]`                                                                                                               |
| Custom                                                                                   | **Custom Format**, with `{0}` for the element type, e.g. `System.Collections.ObjectModel.ObservableCollection<{0}>` |

The Type must match the code. Under Merge mode, the generated signature replaces the existing one, so a wrong collection type rewrites the method. On an async operation the return type is wrapped after the collection is applied, giving `Task<IEnumerable<T>>`. The stereotype is ignored on a type that isn't a collection.

## Parameter Modifier stereotype

Apply **Parameter Modifier** to a service operation parameter where the code declares a modifier, and set **Modifier** to `ref`, `out`, `in`, `ref readonly` or `params`. It must match the code: under Merge mode the generated signature replaces the existing one, so a missing or wrong modifier rewrites the method. For `params`, model the parameter as a collection and set Collection Type to `Array` (`params string[] tags`). `this` (extension methods) is not supported.

## Default values

Set the Value of a service operation parameter to give it a default (`string area = "Basislink"`), and the Value of a DTO, command, query or eventing DTO field to give its property an initializer (`public string Area { get; set; } = "Basislink";`). The Value is formatted the way Intent's DTO templates format it:

- **Strings** are quoted, unless the value is already quoted, is `null` or `default`, or looks like an expression (contains `.` or `(`, or starts with `_`). `Basislink` and `"Basislink"` both give `"Basislink"`.
- **Enums:** a literal name gets its enum type (`Info` gives `EventTypeEnum.Info`).
- **Dates:** a bare `DateTime`, `Date` or `DateTimeOffset` member gets its type (`Now` gives `DateTime.Now`).
- **`@`** passes the rest through verbatim (`@DefaultArea` gives `DefaultArea`).
- Anything else (`null`, `false`, `0`, `string.Empty`) is used as written.

The default must match the code: under Merge mode the generated signature replaces the existing one, so a missing Value drops the default. A `ref`, `out` or `params` parameter can't have a default, and giving one a Value raises an error on the parameter.

## Generated shape

```csharp
[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]

namespace Acme.Application.Orders
{
    public record OrderDto
    {
        public Guid Id { get; set; }
        public List<OrderLineDto> Lines { get; set; }
    }
}
```

Every file carries both directives: plain `Mode.Merge` for the file, plus a default for constructors and methods that fully manages their signatures and merges their attributes and comments. When merged into an existing file:

- A modelled member that is **missing** is added with the generated signature and an empty body. A new method that returns a value won't compile until its body is written.
- A modelled member that **already exists** is merged too: its signature (modifiers, return type, parameters) is brought in line with the model, and its body is merged with the empty generated body, so the existing statements stay as they are. Review the Software Factory diff before applying.
- Hand-written members that were never modelled are kept. A member that Intent itself added can be removed again when it is deleted from the model, depending on the application's RoslynWeaver "Merge Behaviour" setting.
- **Formatting:** keep RoslynWeaver's "Auto Format by Default" setting on. It lays out every merged file to the C# style rules (indentation, spacing, line breaks), including inside existing method bodies, so the first merge can reformat many lines. It never changes what the code does. With it off, the members the Software Factory rewrites lose their indentation.

## Conventions

- Every name is used exactly as it appears in the designer, with no casing changes: types, properties, methods, parameters, enum literals and namespaces. `OrderService` stays `OrderService` (no interface is generated), and a field modelled as `customerId` stays `customerId`. The designer mirrors the code, so its casing is the codebase's own convention.
- Collections are `List<T>`, unless Collection Type says otherwise. They are written by their short name with a `using` (`IEnumerable<T>`, not `System.Collections.Generic.IEnumerable<T>`), even in a file that also imports `System.Collections`.
- An operation is async when its name ends in `Async`, unless the **Async Configuration** stereotype says otherwise. An async operation gets the `async` keyword, a `Task` or `Task<T>` return type (a modelled `Task`/`ValueTask` is not wrapped again) and a `CancellationToken cancellationToken = default` parameter. On the stereotype, **Is Async** explicitly sets async or sync, and **Include Cancellation Token** turns the extra parameter off. A new async method's empty body has no `await`, so the compiler warns (CS1998).
- Fields typed as Domain enums, entities, value objects or data contracts resolve through the Domain contract roles, with no reference to the Domain contract module.
- Consumer projects need the `Intent.RoslynWeaver.Attributes` NuGet package.
