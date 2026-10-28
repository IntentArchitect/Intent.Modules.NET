# Intent.Domain.Guardrails.Contracts

Guards the Domain designer's model in existing C# code. Each modelled element is merged into its existing file, so every modelled type and member keeps existing. Constructors and methods are always generated with empty bodies, so merging never adds code to an implementation.

Install it into an application whose existing code has been modelled in the Domain designer, by hand, with Intent's C# importer, or with the AI skill below. Set the Default Type Modifiers module setting to the code's convention, and apply the Type Configuration and Async Configuration stereotypes wherever the code differs from the defaults described below.

It needs Intent Architect 5.4.0-beta.6 or later. Where the application already has them, installing it also brings Intent.VisualStudio.Projects up to 4.1.12-pre.1 and Intent.Modelers.CodebaseStructure up to 1.3.0-pre.0, the versions the AI skill relies on.

## AI skill: reverse-engineer-domain

The module generates `skills/reverse-engineer-domain/SKILL.md` into the application's agents folder (the `AI.Context.Skills` output). An AI agent working in Intent Architect uses it to:

- model an existing domain layer (entities, value objects, enums, repositories) in the Domain designer;
- install `Intent.Metadata.RDBMS` or `Intent.Metadata.DocumentDB` to match the code's persistence;
- align the Template Outputs with the existing folders, and apply Async Configuration to non-conventional operations;
- run the Software Factory until it changes nothing but the Intent file headers and the code's formatting, then adopt the existing files;
- configure the drift check: write the recipes that read the code's database facts, until the Software Factory reports no drift (see Drift check).

## Drift check

Once its recipe file exists (below), every Software Factory run compares the Domain model with the code on disk, and writes the result to `.cache/guardrails/<application>/DomainDriftReport.mdx`, in the folder that holds the solution's `.isln` (which every Intent solution's `.gitignore` already ignores). The report is only written when it needs attention, and the last one is deleted when the run starts, so a run that writes one always lists it in the Changes pane. A run with no drift writes no report and logs one line saying so. Intent applies none of a failing run's changes by itself, the report included; apply the ones you expect by hand.

Until the recipe file exists, the check is off: nothing is compared, the report says how to set it up, and the run only logs a warning. Installing the module therefore never blocks a run, including the one that generates the **configure-drift-check** skill that writes the file.

The report is an MDX document with up to two tables, each left out when empty, and no timestamps:

- **Model and code drift**: one row per difference, with the model element (live links to it and the elements it belongs to, such as a class and its attribute, or a Service and its operation), the code file (a link to the line that states it; when the code doesn't state it, to where it would: the nearest place the recipe reached, such as the entity's configuration, else the element's own file) and what the drift is. The model must never be wrong, so every row fails the run, and a CI run fails too. The Software Factory shows one error, which says how many drifts there are and names the report in the run's changes. It is on the first row's model element, so Intent Architect offers to fix it with AI; when no row is about a model element, it is a friendly error instead. On CI, where no report is written, the error lists every row. Intent Architect versions that don't yet show logged friendly and element errors show it as a plain error, prefixed with the exception type. A row says that a database fact or a generated type differs between the model and the code (kind, modifiers, base types, a property's type, a method's signature); that a fact can't be verified (a recipe finds no code for it, can't read its value, or the code states it in a form the recipe marks as unreadable, such as a value from a constant); that code isn't modelled (what a recipe's `own` list finds, such as every `Entity<T>` configuration, every index or every type following an element type's convention, or a member of a generated type, with no exception); that a modelled type or member isn't in the code; or that the recipe file is wrong.
- **Warnings**: a kind of fact the recipe file has no recipe for (for example one a module upgrade adds), an exception that matches nothing, or the check being off. They only warn.

For example:

| Model element | Code file | Drift |
| --- | --- | --- |
| `sapPlants` › `differential` (links) | `Basislink.Data/Model/BasislinkContext.cs:1117` (link) | `sapPlants.differential` text type: model `VARCHAR`, code `CHAR` |
| `basisCalcLog` › `test` (links) | `Basislink.Data/Model/BasislinkContext.cs:256` (link) | `basisCalcLog.test` required: model `true`, code `false` (no `IsRequired()` for `test` in EF Core, which maps a string as nullable) |

- **Facts.** The model side is fixed: Intent's own database stereotypes (`Table`, `View`, `Schema`, `Primary Key`, `Column`, `Text Constraints`, `Decimal Constraints`, `Default Constraint`, `Foreign Key`, `Index`, Document DB `Field Setting`) and the associations and `Index` elements of classes, read by name, so neither Intent.Metadata.RDBMS nor Intent.Metadata.DocumentDB is a dependency. The facts are table and view name and schema, primary key, column name and type, text type, max length, required, precision and scale, default, relationship multiplicity and foreign key (between two persisted classes, or to a class a persisted one owns), and each named index's columns and uniqueness (one index per `UniqueKey`, or an `Index` element with `Use Default Name` off). Sort direction, filters and included columns aren't compared yet.
- **Recipes.** The code side is described once per codebase, in `guardrails/domain.drift.json` beside the application's `.application.config`: for each kind of fact, where it is in the code (a find path over classes, methods, properties, statements, call chains or files) and how to read its value (regular expressions over the code, with defaults). The module knows no database technology; the recipes do, so EF Core configurations, a database-first `OnModelCreating`, Mongo class maps or a Cosmos container's JSON can all be read. A recipe's `unstated` says, in the codebase's own terms, what the code saying nothing means (no `IsRequired()` in EF Core, which maps a string as nullable), and a drift row whose value comes from the recipe's `default` or `absent` quotes it. A read case marked `unreadable` reports a form no recipe can read as can't be verified, rather than letting the recipe's default stand in. Commit the recipe file. The generated **configure-drift-check** skill documents the format and every fact.
- **Structure.** Each type this module's templates generate is compared with its file on disk, but only on what its element can state, so nothing the designer can't express is ever drift or needs an exception. Every type is compared on its modifiers and the base types the model names. A Class is also compared on its kind (class or record), generic parameters, Primary Constructor or positional parameters, properties, methods and constructors. Data Contracts and Domain Events are compared on their kind, generic parameters and properties, and Value Objects on their kind and properties; for all three, parameters on the type are compared only as a positional record. Repositories and Domain Services are compared on their kind and methods, so the primary constructor that holds their injected dependencies isn't drift. Spellings that mean the same type agree (`Nullable<int>` and `int?`, `System.String` and `string`, a qualified name and its last part). A member the model doesn't describe is reported only when it's public: whether a protected, internal or private member belongs in the model is a judgement call, so leaving one out is never drift, and once modelled it's compared like any other. Fields, events and indexers are never reported: no designer models one.
- **New types, by convention.** New code that should have been modelled is found by the conventions the recipe file records, one `own` entry per element type whose code follows one: a folder and name pattern, a base class, an interface, an attribute or a member (e.g. every class deriving from `Entity`). The reverse-engineer and configure-drift-check skills record a convention only where the modelled code has one; an element type without one gets no entry, and its new types aren't detected. Types the templates generate, and types in any file a template of the run writes (another designer's included), count as modelled.
- **Exceptions.** The recipe file's `exceptions` list code that may stay unmodelled: a file pattern, a type, a `Type.Member` or the name a convention gives. A type or `Type.Member` exception covers that declaration, and a type its members' declarations, but not code a convention finds inside a member, such as each `Entity<T>` configuration in `OnModelCreating`: that is exempt only by its own name or a file pattern. An exception never covers a contradicted or unverifiable fact.
- **Checking the recipes, on demand.** The drift report doesn't check the recipes. The module's `Intent.Domain.Guardrails.Contracts.ValidateDriftRecipes` module task (argument `{ "applicationId": "…" }`, optionally with `"fact"`, `"element"` and `"file"`) reads every fact through its recipe, against the code on disk and the saved model (nothing from a Software Factory run), and returns JSON: per kind of fact, how many facts were read from the code, from the recipe's `default` or `absent` value, couldn't be verified or contradict the model, with samples; read cases that matched nothing; what each convention finds, with samples; the recipe probe's warnings; and the read cases that read some facts from code that names another element of the same kind and not their own (another attribute's configuration, another action's attributes). The probe takes one fact per find path and read case that read a value from the code, changes the code the case matched in memory (never on disk), and reads the fact again; if the same case still reads the same value, that case isn't reading the code it seems to. Neither check proves that a recipe reads the right code, so the samples are what show it. With `fact` (a kind of fact), `element` (an element and those it holds: `Order` also selects `Order.Notes`) or `file` (a code file, by its path or its end), it covers only the facts selected, leaves out the conventions, and lists every reading in full: the model and code values, where the value came from, the find path and read case, how far it was inherited and the text it matched. The generated **validate-drift-recipes** skill runs the task for each installed Guardrails designer and has the AI check what it read against the code, for every recipe or for one recipe, element or file. The recipe memory earlier builds kept, `.cache/guardrails/<application>/Domain.recipe.json`, is deleted by the next run.
- **A model-first change isn't drift, unless the code changed too.** The check remembers, per machine (`.cache/guardrails/<application>/Domain.model.json`), what each generated type and member looked like in both the model and the code when they last agreed. A difference only the model has made since (an addition, a change or a removal) is left to the Software Factory until its change is applied, however many runs that takes. A difference the code made (a property deleted, a type changed) is reported, and so is one both made, since applying the model's change would overwrite the code's: its row ends by saying the model and the code have both changed. Something the model has that neither the memory nor the code has is left to the Software Factory only if its file hasn't been written since the memory was saved: on a machine whose memory predates a member that a pull added to both, deleting it by hand is reported. A type the model removes is left to the Software Factory to delete only while its code still has exactly the members it had when they last agreed, at any access level, so a member added by hand is reported rather than deleted with the file. Property accessors (`private set`, `init`) aren't compared: the model can't state them, and the merge keeps the code's. On a machine's first run, or on CI, there is nothing to remember, so a model-first change is reported until applied. Facts (columns, keys, indexes) are compared on every run, since the templates never write them. A fact about a type or member the model has just added, or about a method whose parameters it changed, is compared on the same run, against the code as it will be once the run's changes are applied: the addition itself isn't drift, but a gap it leaves (a missing `IsRequired()` or `HasMaxLength`) is. While the code it was read from isn't on disk yet, its row points at the file without a line.
- **On CI** (`TF_BUILD` set) the check runs and fails the run, but the report isn't written.
- **Upgrading from an earlier build** that wrote `DomainDriftReport.md` into the codebase: the first run proposes deleting that file; apply the deletion. Any `guardrails/*.sync.json` belong to the old drift check and can be deleted.

The module also generates four files for AI agents:

- `guardrails-model-sync.md` into the application's rules folder (the `AI.Context.Instructions` output): an always-on rule to update the model in the same change as the code, run the Software Factory and resolve the drift it reports.
- `skills/sync-model/SKILL.md` (the `AI.Context.Skills` output): the `/sync-model` skill, which resolves each Wrong line by updating the model, the code, a recipe or the exceptions.
- `skills/configure-drift-check/SKILL.md`: the `/configure-drift-check` skill, which writes and extends the recipe files for whatever technology the code uses.
- `skills/validate-drift-recipes/SKILL.md`: the `/validate-drift-recipes` skill, which runs each designer's ValidateDriftRecipes module task and checks what each recipe reads against the code.

All four cover every Guardrails designer in the application. When Intent.Services.Guardrails.Contracts is installed too, this module writes them.

Limits: recipes read the code's text, not its meaning, so a value built in a helper method can't be read; it is reported as can't be verified, never passed.

## Templates

| Template              | Model          | Emits                                                                                                            | Role                                        |
| --------------------- | -------------- | ---------------------------------------------------------------------------------------------------------------- | ------------------------------------------- |
| EntityContract        | Class          | Type with base type, attributes as properties, navigable associations, constructors and operations               | `Guardrails.Contracts.Domain.Entity`        |
| EnumContract          | Enum           | `enum` with its literals and values                                                                              | `Guardrails.Contracts.Domain.Enum`          |
| ValueObjectContract   | Value Object   | Type with attributes as properties (no equality implementation)                                                  | `Guardrails.Contracts.Domain.ValueObject`   |
| RepositoryContract    | Repository     | Concrete type with its operations, empty bodies (no interface is generated)                                      | `Guardrails.Contracts.Domain.Repository`    |
| DomainContract        | Data Contract  | Type with attributes as properties and its base contract                                                         | `Guardrails.Contracts.Domain.DataContract`  |
| DomainServiceContract | Domain Service | Type with its operations, empty bodies                                                                           | `Guardrails.Contracts.Domain.DomainService` |
| DomainEventContract   | Domain Event   | Type with its properties and base Domain Event. Generated only when `Intent.Modelers.Domain.Events` is installed | `Guardrails.Contracts.Domain.DomainEvent`   |

Put an Output Anchor carrying these roles at the root of the project that holds each kind of type. The Repository role usually goes on the project with the concrete repository classes, often Infrastructure. Designer folders then supply the rest of each file's path.

## Type Configuration stereotype

Every template except EnumContract emits a `class` by default. Apply **Type Configuration** to a Class, Value Object, Repository, Data Contract, Domain Service or Domain Event and set **Type** to `record` to emit a `record` instead. An empty Type, or no stereotype, means `class`.

- The Type must match the kind of the existing type on disk. Otherwise the merge adds a second declaration with the same name and the project won't compile.
- A record can only inherit from a record, and a class cannot inherit from a record. A record and a positional record can inherit from each other. A mismatch between an element and its base type raises an error on the element.
- **Type = positional record** emits a record whose attributes are its primary constructor's positional parameters, in model order: `public record Money(decimal Amount, string Currency);`. An attribute's Value becomes the parameter's default. It applies to Class, Value Object, Data Contract and Domain Event, not to a Repository or Domain Service. A Class keeps its navigation properties, constructors and operations in the body. A positional type whose base declares parameters on the type must include them among its own; they're passed on by name (`record Child(Guid Id, string Name) : Base(Id)`).
- Structs are not supported, because Intent's C# file builder can't declare them.

## Primary Constructor stereotype

Apply **Primary Constructor** to a Class Constructor to declare its parameters on the type itself (C# 12): `public class OrderService(IOrderRepository repository)` or `public record Money(decimal Amount, string Currency)`. The parameters are written as for any constructor (names verbatim, Parameter Modifier, defaults from Values), and a primary constructor has no body.

- At most one constructor per Class can carry it, and not on a Class whose Type is positional record. Either raises an error on the Class.
- On a **record**, the parameters are the record's properties, so attributes with the same names aren't declared again; the other attributes stay body properties. On a **class**, the parameters are plain parameters and every attribute stays a property.
- When the base Class also declares parameters on the type, they're passed on by name (`class Child(Guid id, ILogger logger) : Base(logger)`), so this constructor must include them.

## Type modifiers

Under Merge mode the generated declaration's modifiers replace the existing ones, so they must match the code.

- **Default Type Modifiers** (module setting, in Domain Guardrails Contracts Settings) applies to every class, record and enum contract: `public` (the default) or `internal`, each optionally followed by `sealed`, `partial` or `sealed partial`. For code that declares types without an access modifier, choose `sealed`, `partial`, `sealed partial`, or `none` for no modifiers at all. Set it to the codebase's own convention, e.g. `internal sealed` or `sealed`.
- **Type Configuration → Modifiers** replaces the setting for one element, e.g. `public`, `sealed`, `internal static` or `public abstract partial`. Use at most one of `public` or `internal`, plus any of `sealed`, `abstract`, `static` and `partial`, in any order; they are written in the usual C# order. Leave the access modifier out when the code has none (`sealed` gives `sealed class PriceCalculator`), and use `none` on its own when the declaration has no modifiers at all (`record OrderPlaced`), because an empty Modifiers means the setting applies.
- **abstract:** Is Abstract on a Class or Domain Event, or an abstract operation, always adds `abstract`.
- **sealed from the setting** is left off an abstract type, and off a Class, Data Contract or Domain Event that another modelled element inherits from.
- **Errors:** Modifiers set on an element are used as given. These raise an error on the element: `sealed` or `static` on an abstract or inherited type, `static` on a record, two access modifiers, more than one of `sealed`, `abstract` and `static`, `none` with any other keyword, or a keyword the builder can't write (`file`, `unsafe`, …).
- **Enums** take only the setting's access modifier: `internal sealed` gives `internal enum`, and a setting without one, such as `sealed` or `none`, gives `enum`. Where an enum differs, apply **Enum Configuration** and set **Access Modifier** to `public`, `internal`, or `none` for an enum declared without one.

## File Config stereotype

Each element is generated into `<ElementName>.cs` in its folder, with the namespace derived from its folders. Apply **File Config** to a Class, Value Object, Repository, Data Contract, Domain Service, Enum or Domain Event only where the code differs. Both properties are optional; set just the one you need.

- **Namespace:** replaces the derived namespace, for a file whose namespace doesn't follow its folders. It is used as given, with no casing changes, and references to the type get the matching `using`.
- **No Namespace:** tick it for a type declared in the global namespace (no `namespace` declaration in the file). Leave Namespace empty; setting both is an error. References to the type need no `using`.
- **Filename** (with or without `.cs`; a file name only, as the folder stays the element's own):
  - **File named differently from its type**, e.g. `orderEntity.cs` holds `Order`: set `Filename = orderEntity`.
  - **File name that starts or ends with a space**, e.g. `abc .cs`: include `.cs`. With `.cs` the name is used exactly as written; without it, surrounding spaces are trimmed.
  - **File holding several types:** give each type that isn't the file's namesake the file's name. All contract elements that end up at the same path are generated into one file. The element whose name matches the file name writes it (otherwise the first by name), and the other elements' templates write no file of their own. References to every type in the file still resolve.
- **Shared files and namespaces:** every type in one file must end up with the same namespace. If the file's namespace isn't the derived one, give each of its types the same Namespace.
- **Changing it later:** if a type already had its own generated file and you then point it at another file, or change its Filename, the Software Factory offers to delete the old file. Review that before applying.
- **Invalid values:** a Filename with a folder in it or invalid file-name characters, or a Namespace that isn't a valid C# namespace, raises an error on the element.

## Collection Type stereotype

By default, collection properties are generated as `ICollection<T>`, and operation parameters and return types as `IEnumerable<T>`. Where the code uses a different collection, apply **Collection Type** to the attribute, the association end (navigation property), the constructor or operation parameter, or the operation itself (its return type), and choose the **Type**:

| Type                                                                                     | Emits                                                                                                               |
| ---------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| IEnumerable, ICollection, IList, IReadOnlyCollection, IReadOnlyList, List, HashSet, ISet | `IEnumerable<T>`, `ICollection<T>`, … (`System.Collections.Generic`)                                                |
| IQueryable                                                                               | `IQueryable<T>` (`System.Linq`)                                                                                     |
| Array                                                                                    | `T[]`                                                                                                               |
| Custom                                                                                   | **Custom Format**, with `{0}` for the element type, e.g. `System.Collections.ObjectModel.ObservableCollection<{0}>` |

The Type must match the code. Under Merge mode, the generated signature replaces the existing one, so a wrong collection type rewrites the member. On an async operation the return type is wrapped after the collection is applied, giving `Task<IEnumerable<T>>`. The stereotype is ignored on a type that isn't a collection.

## Property Configuration stereotype

Apply **Property Configuration** to an attribute, a navigable association end or a Domain Event property whose declaration isn't plain `public`. Properties are `public` without it.

- **Required:** tick it when the code declares the property `required`, giving `public required string Name { get; set; }`.
- **Access Modifier:** `public` (the default), `protected`, `internal`, `private`, `protected internal`, or `none` for a property declared without one. For example, `protected` with Required gives `protected required string Name { get; set; }`.
- **Why it matters:** under Merge mode a property's modifiers follow the generated ones. On the first merge into an existing file every existing modifier is replaced, so an unmodelled `required` would be dropped and an unmodelled `protected` or `internal` would become `public`.
- **Accessors need nothing:** Merge keeps an existing `init`, `private set` or get-only accessor as written, so `public required string Name { get; init; }` stays as it is.
- **Positional records:** their fields are constructor parameters, so both options are ignored on them.

## Operation Configuration and Constructor Configuration stereotypes

Constructor and method signatures are fully managed under Merge, so their modifiers replace the existing ones and must match the code.

- **Operation Configuration** (on an Operation): operations are `public` without it.
  - **Access Modifier:** `public` (the default), `protected`, `internal`, `private`, `protected internal`, or `none` for a method declared without one.
  - **Virtual**, **Override**, **Sealed** and **New** checkboxes. Sealed is only valid with Override (`public sealed override`).
  - `static` and `abstract` come from the operation's own **Is Static** and **Is Abstract**, and `async` from **Async Configuration**.
  - Examples: Access Modifier `protected` + Override gives `protected override void Apply(…)`; `internal` with Is Static gives `internal static Order Create(…)`.
- **Constructor Configuration** (on a Class Constructor): constructors are `public` without it. **Access Modifier** is `public`, `protected`, `internal`, `private` or `protected internal`, e.g. `protected Order()` for a constructor only EF Core uses.
  - A constructor declared without an access modifier is private, so model it as `private`; the generated code then spells it out.
  - It's ignored on a **Primary Constructor**, which has no access modifier of its own.
- **Errors:** a combination C# rejects raises an error on the element: `static` with `virtual`, `override`, `sealed` or `abstract`; `virtual` with `override` or `abstract`; `sealed` without `override`, or with `abstract`; `new` with `override`; or `virtual`, `abstract` or `override` on a `private` (or `none`) method.
- `private protected` can't be reproduced.

## Parameter Modifier stereotype

Apply **Parameter Modifier** to a constructor or operation parameter where the code declares a modifier, and set **Modifier** to `ref`, `out`, `in`, `ref readonly` or `params`. It must match the code: under Merge mode the generated signature replaces the existing one, so a missing or wrong modifier rewrites the method. For `params`, model the parameter as a collection and set Collection Type to `Array` (`params string[] tags`). `this` (extension methods) is not supported.

## Default values

Set the Value of a constructor or operation parameter to give it a default (`string area = "Basislink"`), and the Value of an attribute or Domain Event property to give it an initializer (`public string Area { get; set; } = "Basislink";`). The Value is formatted the way Intent's DTO templates format it:

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
- **Formatting:** keep RoslynWeaver's "Auto Format by Default" setting on. It lays out every merged file to the C# style rules (indentation, spacing, line breaks), including inside existing method bodies, so the first merge can reformat many lines. It never changes what the code does. With it off, the members the Software Factory rewrites lose their indentation.

## Conventions

- Every name is used exactly as it appears in the designer, with no casing changes: types, properties, navigation properties, constructors, methods, parameters, enum literals and namespaces. `OrderRepository` stays `OrderRepository` (no interface or `I` prefix is generated), and a property modelled as `orderDate` stays `orderDate`. The designer mirrors the code, so its casing is the codebase's own convention.
- Collections are `ICollection<T>` on properties and `IEnumerable<T>` on operation parameters and return types, unless Collection Type says otherwise. They are written by their short name with a `using` (`IEnumerable<T>`, not `System.Collections.Generic.IEnumerable<T>`), even in a file that also imports `System.Collections`.
- An operation is async when its name ends in `Async`, unless the **Async Configuration** stereotype says otherwise. An async operation gets the `async` keyword, a `Task` or `Task<T>` return type (a modelled `Task`/`ValueTask` is not wrapped again) and a `CancellationToken cancellationToken = default` parameter. Abstract operations never get `async`. A new async method's empty body has no `await`, so the compiler warns (CS1998). The stereotype's properties:
  - **Async Mode** (default `async`): `async`; `no async keyword`, the same return type and token without `async`, for a method that returns its task directly (`public Task<Order> FindAsync(Guid id, CancellationToken ct) => _set.FindAsync(id, ct);`); or `sync`, the return type exactly as modelled with no token.
  - **Task Type**: `Task`, `ValueTask` or `void`. Empty means `Task`, or `ValueTask` when the return type is modelled as one; an explicit `Task` or `ValueTask` converts a modelled `Task`/`ValueTask` to the other. `void` writes `async void` (`public async void OnChanged(...)`), e.g. for an event handler; it needs Async Mode `async` and an operation with no return type, and anything else raises an error on the operation.
  - **Include Cancellation Token** (default ticked) adds the token parameter.
  - **Cancellation Token Name**: the token's name when the code doesn't call it `cancellationToken`, e.g. `ct`. Method signatures are fully managed, so any other name would otherwise be renamed.
  - **Cancellation Token Default** (default ticked) writes `= default`; untick it for a token without a default.
  - A `CancellationToken` that isn't the last parameter can't be reproduced.
- Types modelled in the Domain designer resolve through this module's roles, so the right `using`s are added across folders and projects.
- Consumer projects need the `Intent.RoslynWeaver.Attributes` NuGet package.
