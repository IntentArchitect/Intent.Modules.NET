using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates.ReverseEngineerDomainSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ReverseEngineerDomainSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Domain.Guardrails.Contracts.ReverseEngineerDomainSkill";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ReverseEngineerDomainSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: "reverse-engineer-domain")
                .FromMarkdown("""
---
description: Reverse-engineer an existing C# domain layer (entities, value objects, enums, repositories, EF Core configuration) into the Intent Architect Domain designer so the model mirrors the code and the Intent.Domain.Guardrails.Contracts module adopts the existing files in place, installing Intent.Metadata.RDBMS or Intent.Metadata.DocumentDB to match the code's persistence technology, and configures the drift check that compares the model with the code on every Software Factory run. Use when the user has installed the Domain Guardrails module (or asks to "align / model / reverse-engineer the domain") on a brownfield codebase.
short-description: Model an existing domain layer in the Domain designer and adopt its files with the Domain Guardrails module.
argument-hint: "[application name]"
---

# Reverse-engineer the Domain

Goal: the Domain designer describes the code **as it is**, and a Software Factory (SF) run against the existing files changes **nothing but the Intent file headers and the code's formatting**: what the code does stays exactly the same. You are aligning the model to the code, not the code to the model. Business logic, method bodies and EF configuration code stay hand-written. The last step configures the drift check, so every Software Factory run compares the model with the code, and the adoption ends with the run reporting no drift.

Load **scripting-the-model**, **exploring-the-model** and **changing-the-model** before you start, and **generating-and-applying-code** / **resolve-destructive-changes** before the first SF run. Follow them for _how_ to script, lay out and generate; this skill only adds what is specific to reverse-engineering.

## 0. Preconditions

1. `listInstalledModules(appId)` (via `run_solution_query`) must include `Intent.Domain.Guardrails.Contracts`. If it doesn't, stop and ask.
2. **Detect the persistence technology and install the matching metadata module.** Running this skill is the user's authorization to install the metadata modules that apply. Detect from the code: grep the Infrastructure/DbContext/DI code and the csproj `PackageReference`s.

   | Evidence in the code | Install (if not already installed) |
   | --- | --- |
   | EF Core relational: `DbContext` + `IEntityTypeConfiguration` / `OnModelCreating`, `Microsoft.EntityFrameworkCore.SqlServer` / `Npgsql` / `Sqlite` / `MySql`, `UseSqlServer(...)` etc. (an `UseInMemoryDatabase` stand-in for a relationally configured model still counts) | `Intent.Metadata.RDBMS` |
   | Document store: `MongoDB.Driver`, `Microsoft.Azure.Cosmos`, EF Core Cosmos (`UseCosmos`), `[BsonId]`, containers/collections instead of tables | `Intent.Metadata.DocumentDB` |
   | Both | Both. Use RDBMS on the relational package and DocumentDB on the document package. |
   | Neither (no persistence) | None |

   - Always also make sure `Intent.Modelers.Domain.Repositories`, `.ValueObjects` and `.Services` are installed when the code has custom repositories, value objects or domain services.
   - **Domain events:** install `Intent.Modelers.Domain.Events` when the code has domain events. Signs: classes deriving from a `DomainEvent` base or implementing `IDomainEvent` (or a MediatR `INotification` used as a domain event), a `DomainEvents` collection on entities, or calls like `AddDomainEvent(...)` / `RaiseDomainEvent(...)`. The guardrail modules deliberately don't depend on it: without it, Domain Events can't be modelled and nothing is generated for them.
   - Get the exact current version with `searchAvailableModules(id)`, then call `install_or_update_modules` and state in one line what you installed and why.
   - Ask only when the evidence is ambiguous (e.g. a custom data layer you can't classify).
3. **What the installed templates do.** This skill ships with them, so don't decompile the module to check:
   - **File path:** Template Output location + the designer folder path.
   - **Class name:** the element's name verbatim (no `I`, no suffix).
   - **Repositories** generate the implementation class: name the element `CustomerRepository`, with its output in the Infrastructure `Repositories` folder.
   - **Merge directives:** every file carries both `[assembly: DefaultIntentManaged(...)]` lines (§6 step 3).
   - **Async operations:** an operation is async when its name ends in `Async` (case-sensitive), unless its `Async Configuration` stereotype says otherwise (§3 "Async operations").

## 1. Extract the code (read-only, in parallel)

Dispatch read-only `Explore` sub-agents in **one** message, with non-overlapping slices. Ask for verbatim-accurate specs, not summaries:

- **Domain project, every `.cs` file plus the EF `IEntityTypeConfiguration`s and the `DbContext`.** For each type, report:
  - Path, namespace and kind (aggregate root / owned entity / value object / enum / repository interface / helper).
  - Declaration modifiers exactly as written: `public`, `internal` or none, plus `sealed`, `abstract`, `static`, `partial`.
  - Every property: exact type **as spelled** (e.g. `Nullable<T>` vs `T?`, `System.Guid` vs `Guid`, partly qualified names such as `DTO.Logging.LogDTO`), nullability, collection-ness, setter and whether it is `required`.
  - Constructors and methods other than private helpers (signatures only, with parameter and return types as spelled), with their modifiers exactly as written: access (`public`, `protected`, `internal`, `private`, `protected internal` or none), plus `static`, `abstract`, `virtual`, `override`, `sealed`, `new`.
  - Whether the type declares parameters on the type itself: a positional record (`record Money(decimal Amount, string Currency)`) or a primary constructor (`class OrderService(IOrderRepository repository)`), and any base-type arguments (`: Base(Id)`).
  - Enum literals with explicit values.
  - Repository interfaces: their base interface and every extra method signature.
  - Domain events: path, namespace, kind (`class` / `record`), declaration modifiers, every property, the base type, and which entity methods or constructors raise each event, with one verbatim example of each way they raise one.
- **Nullable context:** per project, whether the csproj sets `<Nullable>` (and to what); per file, any `#nullable` directives.
- **From the EF configuration, per property and relationship:**
  - `OwnsOne` / `OwnsMany` / `HasOne` / `HasMany`.
  - FK properties and shared primary keys (`HasForeignKey<T>(x => x.Id)`).
  - Keys: `HasKey` (single or composite, with key types), `HasPrincipalKey` (FKs onto a non-primary key), `HasNoKey`, and which classes the `DbContext` doesn't map. These decide the key mode in §4a.
  - Delete behaviour.
  - `HasMaxLength`, `HasColumnType("decimal(p,s)")`, `HasDefaultValue`, `HasIndex(...).IsUnique()`, row-version and concurrency tokens.
- **How the code states each database fact:** where each class's persistence is configured (e.g. `OrderConfiguration.Configure`, the owner's helper method for an owned type, a Mongo class map registration, a Cosmos container setup or indexing policy file; in database-first code, the class's `modelBuilder.Entity<T>(…)` statement inside the context's `OnModelCreating`), and the calls that state each fact (`ToTable`, `HasKey`, `HasMaxLength`, `IsUnicode(false)`, `HasColumnType`, `IsRequired`, `HasDefaultValueSql`, `HasOne(…).WithMany(…)`). Note every style the code uses, with one verbatim example of each, and the facts it states by saying nothing (EF conventions: a table named after its `DbSet`, a key named `Id`). The recipes below are written from this.
- **The convention of each element type:** for each type of element you will model (entity, value object, enum, repository, domain service, domain event), whether all of its code shares something a new one would have too: a folder with a name pattern (every `*Repository` in `Infrastructure/Repositories`), a base class (`: Entity`), an interface (`IAggregateRoot`), an attribute, or a member. Report it with one verbatim example, or report that there is none.

**Then write the drift check's recipes, before modelling anything.** The sub-agents have just read every configuration, and a file outlives this conversation's context, so don't leave this to the end. Load **configure-drift-check** for the format and write `guardrails/domain.drift.json` from the sub-agents' reports, without re-reading the code:
- a recipe for each database fact the code states, with one `find` per style;
- `absent` or `default` for what the code states by saying nothing;
- `intent.raises` from configure-drift-check's domain-events guide, when entities raise domain events;
- an `own` entry for each kind of code the model must describe (every `Entity<T>` configuration, class map or container);
- an `own` entry for each element type that follows a convention, which finds every type following it, with `kind` naming the element type. An element type without a convention gets none: its new types can't be told apart from helpers, so they aren't detected.

Ask a sub-agent only about a style the reports don't cover. Writing the file turns the check on: from now on every Software Factory run compares the model with the code, and fails until they match. During adoption that is expected, and the report lists what isn't modelled yet. §6 step 8 finishes the check.

## 2. Align the Codebase Structure

Template Outputs from a fresh install land under whatever anchor was available, often the Api project. Move each Guardrails Template Output with `el.setParent(...)` so that **output folder + designer folder path = the existing file's folder**. Create `Folder`s in the project as needed (`Folder Options: Namespace Provider = true` keeps namespaces matching). Typical layout:

| Template | Put the Template Output in |
| --- | --- |
| EntityContract | the folder that holds entity sub-folders (e.g. `Domain/Entities`) |
| EnumContract, ValueObjectContract, DomainContract, DomainServiceContract | wherever their files' parent folders hang (often the Domain project root) |
| RepositoryContract | `Infrastructure/Repositories` (it generates the repository class, §0) |

- **One template, several locations.** A template is not limited to one Template Output. When a template's existing files live in more than one project or root folder (e.g. enums in both the DTO and Business projects), **duplicate the Template Output**, one copy per location. Give each copy a **Registration Filter** (on its `Template Output Settings` stereotype) that selects which model instances generate there.
  - The filter is a Dynamic LINQ expression over the template's model (e.g. `EnumModel`, `DTOModel`, `ServiceModel`, `ClassModel`) and must return `bool`. `Folder.Name` is usually the simplest discriminator. Use `np(...)` for anything that can be null. Example: `np(Folder.Name) == "BusinessObject"` on the Business copy and `np(Folder.Name) != "BusinessObject"` on the DTO copy.
  - **Filters must be mutually exclusive.** An instance matching two outputs fails the SF. An empty filter matches everything, so when you add a filtered copy, also give the original the complementary filter.
  - Script it: `project.addChild("Template Output", "<TemplateId>")`, then `ensureStereotype("Template Output Settings").setProperty("Registration Filter", "<expr>")`.
  - Model those elements in a designer folder that the filter selects on, and combine with `File Config` (`Filename` / `Namespace`) to land each type in its exact existing file.
  - `File Config` can't move a file to another project; a filtered Template Output can. Use Type-Definition placeholders only for types that can't be elements at all (nested types, `dynamic`, framework types).

Save the Codebase Structure designer (`saveOnSuccess: true` on a no-op script) once it is right.

## 3. Model the domain

**Ensure a package exists first.** Check with `getPackages()` on the Domain designer.

- **If there are none,** create one named `{ApplicationName}.Domain`, using the application name from the workspace context / `get_applications` (e.g. `Training.Ordering.Domain`): `await createPackage("Training.Ordering.Domain")`.
- **If one or more exist,** use the existing package and don't create another. If several exist, ask which one to use.
- **Persistence stereotypes** (§4) go on this package.
- **Saving:** a script-created package is invisible to the SF until the designer is saved (`saveOnSuccess: true`).

Create one designer `Folder` per code folder **below** the template output (e.g. `Customers`, `Orders`), so that `GetFolderPath()` reproduces the path. Put types whose file sits directly in the output folder at the package root. Then build it in phases (enums → value objects → classes → associations → repositories), one script per area, verifying `errors[]` each time:

- **Files that don't follow the convention:** each element is written to `<ElementName>.cs` in its folder's namespace. Apply the **`File Config`** stereotype only where the code differs, and set only the property you need; both are optional.
  - **Namespace doesn't follow the folders:** set `Namespace` to the file's exact namespace. Every type that shares a file needs the same `Namespace`.
  - **No namespace in the file** (types declared in the global namespace): tick `No Namespace` and leave `Namespace` empty.
  - **File named differently from its type:** set `Filename` to the file's name, with or without `.cs` (the folder stays the element's own). If the name starts or ends with a space (e.g. `abc .cs`), include `.cs`: with it the name is used exactly as written, without it surrounding spaces are trimmed.
  - **File holding several types:** model every type as its own element in that file's folder, and set `Filename` to the file's name on each type the file is *not* named after. Elements that end up at the same path are generated into one file, written by the element the file is named after.
  - **Set it before the first SF run.** If a type already had its own file generated and you then point it at another file, the SF offers to delete the old file. Treat that as a destructive change.
- **Type modifiers:** Merge mode replaces each class, record or enum declaration's modifiers with the generated ones, so they must match the code.
  - **Set the convention once, before the first SF run.** If most of the code's declarations aren't plain `public` (e.g. `internal sealed`, or `sealed` with no access modifier), set the application setting `Default Type Modifiers` (group `Domain Guardrails Contracts Settings`) to the dominant combination. Its options are `public` or `internal`, each optionally followed by `sealed`, `partial` or `sealed partial`; `sealed`, `partial` or `sealed partial` with no access modifier; and `none` for no modifiers at all. Read it with `getApplicationSettings({ search: "Default Type Modifiers" })` (via `run_solution_query`), change it with `update_application_settings`, and state in one line what you set and why.
  - **The setting's `sealed` is left off automatically** for an abstract type (`Is Abstract`, or an abstract operation) and for a type another modelled type inherits from.
  - **Exceptions:** wherever a type differs from the setting, apply `Type Configuration` and set `Modifiers` to the code's keywords exactly as written, e.g. `public`, `sealed`, `internal static` or `public abstract partial`: at most one of `public` / `internal`, plus any of `sealed`, `abstract`, `static`, `partial`, in any order. Omit the access modifier when the code has none (never write `internal` in its place), and use `none` on its own when the declaration has no modifiers at all, because an empty `Modifiers` means the setting applies. `Is Abstract` on the element still adds `abstract`. An invalid or conflicting value (e.g. `sealed` on an abstract type, or `none` with another keyword) is an error on the element.
  - **Enums** take only the setting's access modifier (`internal sealed` gives `internal enum`, and a setting without one, such as `sealed` or `none`, gives `enum`). Where an enum differs, apply `Enum Configuration` and set `Access Modifier` to `public`, `internal`, or `none` for an enum declared without one.
  - **Can't be reproduced:** `file`-scoped and `unsafe` types. Leave them unmodelled and report them. This applies only to shapes the model can't express at all. A type that is merely spelled differently is never a reason to leave a member out (§6, "Equivalent type rewrites").
- **Delete placeholders** such as a default `MyEntity` if the code doesn't have them.
- **Attributes:** use the exact names and types (`string`, `guid`, `datetime`, `decimal`, `long`, `binary`, enums). Set `isNullable` for nullable types (see "Nullability and type spelling"). Do **not** model collections of entities as attributes; use associations.
- **Collections:** the template emits `ICollection<T>` for properties and `IEnumerable<T>` for operation parameters and return types. Wherever the code uses anything else (`List<T>`, `IList<T>`, `IReadOnlyCollection<T>`, `IQueryable<T>`, `T[]`, …), apply the **`Collection Type`** stereotype to that attribute, association end, parameter or operation (for its return type) and set `Type` to match (`Custom` + `Custom Format` with `{0}` for anything else). Otherwise Merge mode rewrites the existing signature.
- **Nullability and type spelling.** Model the type, not its spelling:
  - **Nullable value types:** set `isNullable` for `T?` **and** for `Nullable<T>` / `System.Nullable<T>`. They are the same type.
  - **`string?` (or any reference type with `?`) with nullable reference types off** (the csproj has no `<Nullable>enable</Nullable>` or `annotations`, and the file no `#nullable enable`): the `?` does nothing but warn CS8632. Model a nullable `string` anyway; the template emits `string`, a change beyond the headers handled under "Equivalent type rewrites" (§6). Never drop the member.
  - **Nullable reference types on:** make the project's Nullable option in the Codebase Structure designer match the csproj, so the template emits `string?`. A `string?` → `string` diff then is real drift: fix the project setting, not the member.
  - **Other spellings of the same type** (`System.Guid`, `Int32`, `String`, or a partly or fully qualified name such as `DTO.Logging.LogDTO`): model the type itself (`guid`, `int`, `string`, `LogDTO`); the SF may rewrite the spelling (§6, "Equivalent type rewrites").
- **Parameter modifiers:** where a constructor or operation parameter is declared `ref`, `out`, `in`, `ref readonly` or `params`, apply the **`Parameter Modifier`** stereotype and set `Modifier` to match (for `params`, also set `Collection Type = Array`). Otherwise Merge mode drops the modifier from the existing signature. An extension method (`this` parameter) can't be reproduced; leave it unmodelled and report it. Like the other "leave it unmodelled" rules, this covers only shapes the model can't express, never a type spelled differently (§6, "Equivalent type rewrites").
- **Operation and constructor modifiers:** method and constructor signatures are fully managed, so their modifiers must match the code exactly; a mismatch rewrites the declaration.
  - **Operations** are `public` unless the **`Operation Configuration`** stereotype says otherwise. Set `Access Modifier` (`public`, `protected`, `internal`, `private`, `protected internal`, or `none` for a method declared without one) and tick `Virtual`, `Override`, `Sealed` (only with `Override`, for `sealed override`) or `New` as the code declares. `static` and `abstract` stay on the operation's own `Is Static` / `Is Abstract`, and `async` on `Async Configuration`.
  - **Constructors** are `public` unless the **`Constructor Configuration`** stereotype sets `Access Modifier`, e.g. `protected` for a constructor only EF Core uses. A constructor declared without an access modifier is private, so model it as `private`; the generated code then spells it out. It's ignored on a `Primary Constructor`.
  - A combination C# rejects (e.g. `static` with `virtual`, `sealed` without `override`, or a `private` `virtual`) is an error on the element. `private protected` can't be reproduced; leave that member unmodelled and report it.
- **Required properties:** where a property (an attribute, a navigation property or a Domain Event property) is declared `required`, apply the **`Property Configuration`** stereotype and tick `Required`. On its first merge, Merge mode replaces a property's modifiers with the generated ones, so an unmodelled `required` is dropped. Accessors need nothing: Merge keeps an existing `init`, `private set` or get-only accessor as written. A positional record's fields are constructor parameters, so `Required` is ignored on them.
- **Property access modifiers:** a property (an attribute, a navigation property or a Domain Event property) is `public` unless the **`Property Configuration`** stereotype sets `Access Modifier` (`public`, `protected`, `internal`, `private`, `protected internal`, or `none` for a property declared without one). Model the code's own modifier: on its first merge, Merge mode replaces a property's modifiers with the generated ones, so an unmodelled `protected` or `internal` becomes `public`. Ignored on a positional record's fields.
- **Default values and initializers:** where a constructor or operation parameter has a default (`string area = "Basislink"`, `bool forceCreate = false`, `Exception ex = null`), or an attribute's property has an initializer, set the element's Value (`el.setValue("Basislink")`). Otherwise Merge mode drops the default from the existing signature.
  - Strings are quoted for you, and an already quoted value stays as it is. `null`, `default`, numbers, `true` and `false` are used as written.
  - An enum literal name gets its enum type (`Info` becomes `EventTypeEnum.Info`), and a bare `DateTime` member gets its type (`Now` becomes `DateTime.Now`).
  - Anything that looks like an expression (contains `.` or `(`, or starts with `_`) is used as written. Prefix any other expression with `@` to pass it through verbatim (`@DefaultArea` gives `DefaultArea`).
  - A `ref`, `out` or `params` parameter can't have a default; that combination is an error on the element.
- **Value objects:** model them as `Value Object` elements, used as attribute types on the owner (e.g. `DeliveryAddress: AddressSnapshot?`).
- **Associations:** always set both multiplicities and the target navigation name to the code's property name.
  - Composition (owned, `OwnsOne` / `OwnsMany`, or a cascade-owned 1:1): `sourceMultiplicity: "1"`.
  - Reference to another aggregate (`HasOne(...).WithMany()`): `sourceMultiplicity: "0..*"`, `targetMultiplicity: "1"` (or `"0..1"` when it is optional).
  - Unidirectional unless the code navigates both ways.
  - **No navigation property** (only an FK, e.g. `int? PaymentId`): an association's target end can't be made non-navigable, so model the FK as a plain attribute and say what it references in its comment.
- **Primary / foreign keys:** do **not** add `Id` / `XxxId` attributes unless RDBMS is installed (§4). Merge mode keeps them in code either way.
- **Repositories:** add one only where the code has a custom repository. Set its type to the entity, and model **only the extra methods** beyond the generic base, named exactly as in code (`ExistsWithEmailAsync`, …). Set their return types and async settings by "Async operations" below; for a conventional `…Async` method that is `T`, without `Task`. Put the repository in the same designer folder as its entity's code folder.
- **Domain events** (when `Intent.Modelers.Domain.Events` is installed): one `Domain Event` element per event class, in the designer folder that matches its code folder, named verbatim, with one `Property` per property (exact names and types).
  - Model a base event as a Domain Event Generalization only when the base is itself a modelled domain event. A framework base or interface (`DomainEvent`, `IDomainEvent`, `INotification`) stays in the code under Merge mode.
  - Link each constructor or operation that raises an event to it with a `Domain Event Origin` association, one per event, from the member whose body raises it: the drift check verifies each link against the code. It generates nothing.
  - Domain Events take the same `File Config` and `Type Configuration` stereotypes (record events are common), and their properties take `Collection Type` and `Property Configuration`.
- **Comments:** copy XML-doc summaries into element comments where they help.
- **Which members to model:** each type's public members. Model a protected, internal or private member only when it's part of the design (a protected method subclasses override, a private constructor EF Core needs), never just because it exists: helpers and plumbing stay out. The drift check reports an unmodelled member only when it's public, and Merge keeps a member left out as written, so it needs no `[IntentIgnore]`.
- **Records and primary constructors:** set `Type Configuration` → `Type` to `record` for a record.
  - **Positional record** (`record Money(decimal Amount, string Currency)`, nothing but a parameter list): set `Type` to `positional record`. The attributes, in model order, are the positional parameters, and an attribute's Value is the parameter's default. This works on Class, Value Object, Data Contract and Domain Event.
  - **Primary constructor** (`class OrderService(IOrderRepository repository)`, or a record whose parameter list isn't all of its attributes): model a Constructor with those parameters and apply the **`Primary Constructor`** stereotype. Model it whenever the code has one. At most one per Class, and not together with `positional record`. On a record, attributes named like its parameters aren't declared again; the others stay body properties.
  - **Inheritance:** a type whose base declares parameters on the type passes them on by name (`record Child(Guid Id, string Name) : Base(Id)`), so its own parameters must include the base's.

### Async operations

This applies to every modelled operation, whether it belongs to a repository, a domain service or an entity. Settle each one before the first SF run. Merge mode applies the generated signature (modifiers, return type and parameters) to the existing method and keeps only its body. A mismatch here breaks the build, e.g. `async` stripped from a method whose body still `await`s, or added to one that returns its task directly.

**What the template emits.** An operation is async when its name ends in `Async`, unless the operation's `Async Configuration` stereotype says otherwise. An async operation gets:

- the `async` keyword, except on abstract operations, which still get the return type and parameter below;
- a `Task` return type when none is modelled, otherwise `Task<T>`. A modelled `Task` / `Task<T>` is not wrapped again;
- a trailing `CancellationToken cancellationToken = default` parameter.

A sync operation's return type is emitted exactly as modelled, with no token. A brand-new async method gets an empty body with no `await`, so the compiler warns CS1998. In reverse-engineering that is just another inserted member (§6), so fix the model.

The stereotype's properties:

- `Async Mode` (default `async`): `async` is the keyword plus a task return type; `no async keyword` is the same return type and token without the keyword, for a method that returns its task directly; `sync` is the return type exactly as modelled, with no token.
- `Task Type`: `Task`, `ValueTask` or `void`. Empty means `Task`, or `ValueTask` when the return type is modelled as one. An explicit `Task` or `ValueTask` converts a modelled `Task` / `ValueTask` to the other. `void` writes `async void`, only with `Async Mode = async` and no return type; anything else is an error on the element. Not used for `sync`.
- `Include Cancellation Token` (default `true`) adds the trailing token parameter. Not used for `sync`.
- `Cancellation Token Name`: the token's name when the code doesn't call it `cancellationToken`, e.g. `ct`. Empty means `cancellationToken`.
- `Cancellation Token Default` (default `true`) writes `= default`. Untick it when the code's token has no default.

Applying the stereotype with its defaults therefore gives the conventional async shape.

**Conventional code: no stereotype.** The method is named `…Async`, is declared `async`, returns `Task` / `Task<T>` and ends with `CancellationToken cancellationToken = default`. Model the name as-is, set the return type to `T` (or none for `Task`), and add nothing.

**Non-conventional code: set the stereotype explicitly.** Check every method against the convention. Wherever it differs, apply `Async Configuration`, set `Async Mode` explicitly, and set every other property where the code differs from its default. Don't rely on the name:

| Code | Model |
| --- | --- |
| `async Task<T> GetNextReference(…, CancellationToken cancellationToken = default)`: async, but no `Async` suffix | `Async Mode = async`, return type `T` |
| `bool ExistsAsync(…)`: named `…Async` but synchronous | `Async Mode = sync`, return type `bool` |
| `async Task<T> LoadAsync(…)` with no `CancellationToken` parameter | `Async Mode = async`, `Include Cancellation Token = false`, return type `T` |
| `Task<T> FindAsync(…, CancellationToken ct) => _inner.FindAsync(…, ct)`: returns its task without `async` | `Async Mode = no async keyword`, `Cancellation Token Name = ct`, return type `T` |
| `async ValueTask<T> GetAsync(…, CancellationToken cancellationToken)`: `ValueTask`, and a token with no default | `Async Mode = async`, `Task Type = ValueTask`, `Cancellation Token Default = false`, return type `T` |
| `async void OnChanged(…)`: `async void`, e.g. an event handler | `Async Mode = async`, `Task Type = void`, `Include Cancellation Token = false`, no return type |

```js
const st = op.ensureStereotype("Async Configuration");
st.setProperty("Async Mode", "no async keyword");
st.setProperty("Cancellation Token Name", "ct");
```

**Shapes the template can't reproduce.** The designer has no `CancellationToken` or `ValueTask` type, so the stereotype is the only way to produce them. Leave these methods unmodelled; Merge mode keeps them untouched. Say so in your report:

- a `CancellationToken` that isn't the last parameter.

A token default written another way, e.g. `= default(CancellationToken)`, is part of the fully managed signature, so expect it to be rewritten to `= default`, which means the same. Accept it, and mention it in your report.

These rules cover only shapes the model can't express. A type that is merely spelled differently is never a reason to leave a method out (§6, "Equivalent type rewrites").

## 4. Persistence alignment

### 4a. RDBMS (relational solution; `Intent.Metadata.RDBMS` installed in §0)

1. **Mark the package.** Apply `Relational Database` to the Domain package (`pkg.ensureStereotype("Relational Database")`).
2. **Choose the key mode before modelling classes and associations.** The application setting `Database Settings` → `Key Creation Mode` defaults to `Explicitly add PKs and FKs automatically` (value `explicit`). In that mode the designer gives every class an `Id` primary key of the default `Key Type` (usually `guid`), creates and removes FK attributes from associations, and silently reverts FK attributes you add or delete by hand. That only reproduces code where every entity has a single surrogate key and every FK points at it.
   - **Keep `explicit`** when the code follows that convention: one `Id` per entity, FKs named `XxxId` onto it.
   - **Switch to `Manually add PKs and FKs` (value `manual`)** when it doesn't. Typical signs: EF database-first code; `int`, `string` or composite keys (`HasKey(x => new { x.A, x.B })`); FKs onto a non-primary key (`HasPrincipalKey`, e.g. `bids.memberNo` → `members.memberno`, `silo` → `sapPlants.code`); keyless entities (`HasNoKey`, views); classes the `DbContext` doesn't map; a key declared on a base class (`Entity.Id`). Read the setting with `getApplicationSettings({ search: "Key Creation Mode" })` (via `run_solution_query`), change it with `update_application_settings`, and state in one line that you changed it and why.
   - Switch before you create classes or associations. If keys were already added in `explicit` mode, delete every one that isn't in the code.
3. **Automatic keys (`explicit` mode only).** The designer adds PKs and FKs only from change events, so existing elements aren't back-filled. Touch each association (set a multiplicity to something else and back) to fire them. Then fix up the result:
   - **Casing:** rename generated keys to match the code (they may come out as `id` / `customerId`).
   - **Missing primary keys:** add `Id: guid` with the `Primary Key` stereotype, plus `setOrder(0)`, to classes with no associations (e.g. `Product`, a counter table).
   - **Shared-PK 1:1** (code `HasForeignKey<Child>(x => x.Id)`): make the child's `Id` both `Primary Key` and `Foreign Key(Association = <association id>)`. Delete any `ParentId` FK the designer added that isn't in the code.
   - **Owned 1:1 child with no children of its own:** the designer removes an explicit PK, because the key is implied and shared with the owner. Accept that; the code's `Id` survives in Merge mode.
4. **Manual keys (`manual` mode only).** Nothing is added for you; model exactly the keys the code has:
   - **Primary keys:** add the key attribute with the code's own name and type (`memberno: int`, `code: string`) and apply `Primary Key`. For a composite key, apply `Primary Key` to each part and order the parts as the code's `HasKey` does (`setOrder`). A key declared on a base class isn't modelled, because the template would declare it again on every class; say so in the class comment.
   - **Foreign keys:** add the FK attribute with the code's name and type, apply `Foreign Key` and set its `Association` to the association it belongs to. An association with no FK property in the code gets no FK attribute.
   - **FK onto a non-primary key** (`HasPrincipalKey`): model the FK as above. Check `getStereotypeDefinitions()` for a way to name the principal key; if there is none, record it in the FK attribute's comment (e.g. `References members.memberno`).
   - **Keyless classes** (`HasNoKey`, views) and **classes the `DbContext` doesn't map**: add no key, and say why in the class comment. Merge mode keeps whatever the code has.
5. **Column constraints** (from the EF configuration):
   - `Text Constraints.MaxLength` for `HasMaxLength`. This also applies to value object attributes.
   - `Decimal Constraints` Precision/Scale for `decimal(p,s)`.
   - `Default Constraint.Value` for `HasDefaultValue`.
   - `Index` (`UniqueKey`, `IsUnique`) for unique indexes.
6. **Nothing to express in the model:** row versions, concurrency tokens and delete behaviour have no stereotype. Leave a comment instead.

### 4b. Document DB (document solution; `Intent.Metadata.DocumentDB` installed in §0)

1. Apply the package-level `Document Database` stereotype. Read the exact stereotype names with `getStereotypeDefinitions()` after installing; don't guess.
2. Model each document (collection/container) root as a Class. Model embedded sub-documents as compositions.
3. Map container/collection names, partition keys and the id field to the module's stereotypes where the code configures them.
4. Model references between documents as aggregations (`0..*` → `1`).

## 5. Lay out the diagram

Follow **changing-the-model** → diagram layout. Snowflake pattern around the most-connected class; enums and repositories on the edges; no overlaps.

## 6. Generate, verify and adopt

1. **Run the SF.** Every change must be an `overwrite` of an existing file, consisting of:
   - `using Intent.RoslynWeaver.Attributes;`
   - `[assembly: IntentTemplate(...)]`
   - possibly the two merge directives
   - one `PackageReference Intent.RoslynWeaver.Attributes` per csproj
   - and formatting from RoslynWeaver's Auto Format, anywhere in the file (step 5).

   The only other exception is an equivalent type rewrite the user accepts (step 4). While there is drift, a run also lists the drift report, `<<DriftReport>>` (in the solution's git-ignored `.cache` folder), as a new file; it isn't code and needs no review. Until step 8, its drift lines fail the run, so the Software Factory applies nothing by itself (step 6 applies by path anyway). They aren't for step 2; read its "isn't modelled" rows as a second view of what is still missing.
2. **Diagnose anything else:**
   - **`create`** means a path or name mismatch. Fix the folder, Template Output location or element name, or add `File Config` for a type that lives in another file or a differently named one. Never apply it.
   - **Inserted members** mean the model has something the code doesn't (wrong name, type or nullability). Fix the model.
   - **A changed signature on a modelled operation** (`async` added or removed, a `CancellationToken` appended or dropped, the return type wrapped in `Task<…>`) means the async rule didn't match the code. Set `Async Configuration` on that operation (§3 "Async operations"). If its async shape can't be reproduced (§3 "Shapes the template can't reproduce"), remove the operation from the model. Never remove it because a type is spelled differently (see "Equivalent type rewrites"). Never hand-edit the signature.
   - **A changed type declaration** (e.g. `internal sealed class` rewritten to `public class`, or `partial` dropped) means the modifiers don't match the code. Fix `Default Type Modifiers`, or that element's `Type Configuration` → `Modifiers` (`Enum Configuration` → `Access Modifier` for an enum; §3 "Type modifiers"). Never hand-edit the declaration.
   - **A type spelled differently on an existing member** (`Nullable<DateTime>` → `DateTime?`, `DTO.Logging.LogDTO` → `LogDTO`, `string?` → `string`): see "Equivalent type rewrites" (step 4). Don't remove the member.
   - **Deleted or changed lines**, other than formatting (step 5), mean drift. Use **resolve-destructive-changes**. Never apply destructive changes, apart from equivalent type rewrites the user accepted (step 4).
   - **A template exception** ("The parent has not been set for this type …") is a module bug. Report it to the user. Only as a last resort, and only _before_ that template has ever been applied, disable that Template Output (`Template Output Settings.Is Enabled = false`). Disabling it after an apply makes the SF propose deleting every file it previously owned.
3. **Merge directives (critical).** Every adopted file must carry both of these lines, in this order:

   ```csharp
   [assembly: DefaultIntentManaged(Mode.Merge)]
   [assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
   ```

   The current templates write both. If the template didn't, add exactly these lines above the `IntentTemplate` attribute in each adopted file **before any further model change**. Without the first, the next SF run treats the file as fully managed and strips constructors, XML docs, attributes, base types, `virtual` and initialisers. The first must stay untargeted: with only a targeted directive, the class declaration itself is fully managed. The second fully manages constructor and method signatures, and merges their attributes and comments.
4. **Equivalent type rewrites — ask, never drop.** An equivalent type rewrite is an SF diff where the only change to an existing member is how a type is spelled, and both spellings mean the same type:

   | Code | Template |
   |---|---|
   | `Nullable<DateTime>`, `System.Nullable<int>` | `DateTime?`, `int?` |
   | `System.Guid`, `Int32`, `String` | `Guid`, `int`, `string` |
   | `DTO.Logging.LogDTO` (partly or fully qualified) | `LogDTO` plus a `using` |
   | `string?` with nullable reference types off | `string` |

   - **Never remove a member from the model, or leave one out, to avoid an equivalent rewrite.** This covers properties, fields, parameters and operations. The model must mirror the code; a smaller model that keeps the diff free of type rewrites defeats the purpose.
   - **Confirm it is really equivalent:**
     - the same type after resolving usings and enclosing namespaces;
     - `Nullable<T>` vs `T?`;
     - for `string?`, that nullable reference types are off.

     Anything else is real drift, so fix the model. That includes a different type, nullability added or removed on a value type, or a changed collection type.
   - **Ask the user once, for all of them together,** with `ask_user_question`, **before** applying any affected file. List each as `File:line — before → after`. Offer:
     1. **Accept the rewrites (recommended).** Keep the members in the model and apply the files. Only the spelling changes; behaviour is identical. For `string?` with nullable reference types off, this also clears warning CS8632.
     2. **Keep the code as written; leave these members out of the model.** The files keep their spelling, but the model is incomplete. Put a comment on each affected element saying what is missing and why.
     3. **Model them, but don't adopt the affected files yet.** Apply every other file, and leave these until a RoslynWeaver build keeps the existing spelling. Until then, the SF keeps proposing these changes, so the zero-change check doesn't pass for those files.
   - **Follow the answer, and state it in the final report** with the list of affected members.
5. **Formatting changes — accept, never suppress.** RoslynWeaver's Auto Format is on by default and lays out each adopted file to the C# style rules: indentation, spaces around operators, commas and colons, trailing spaces, blank lines and line breaks. It can touch hundreds of lines, including inside hand-written method bodies (`GetFileName(link): null` → `GetFileName(link) : null`). That is expected: it changes the layout, never what the code does, and brings the code in line with its coding standards.
   - **Confirm it is formatting only:** the same tokens in the same order, with only the whitespace between them changed. String literal contents and comment text are unchanged. Anything else (a token added, removed, reordered or renamed) is not formatting; diagnose it under step 2.
   - **Apply formatting changes with the rest of the file.** They need no question to the user.
   - **Never turn off Auto Format** (the `Auto Format by Default` application setting, under `RoslynWeaver Settings`), and never suppress it any other way, to make a diff smaller or header-only. That hides harmless fixes, and with it off, the members the SF rewrites (declarations with attributes, for example) lose their indentation. If it is already off, ask the user whether to turn it back on (recommended); don't change it without asking.
   - **Reviewing a large diff:** read past the whitespace. What is left must be the header changes from step 1 or equivalent type rewrites (step 4). If the files are under git and had no other uncommitted changes, `git diff -w -- <file>` after applying shows exactly what is left; revert any file that shows more, and diagnose it under step 2.
   - **In the final report,** say how many files were reformatted, so the user expects the whitespace changes in their commit.
6. **Apply** only the files you reviewed (`apply_staged_file_changes` with explicit paths): files whose only changes are the headers and formatting, plus files whose equivalent rewrites the user accepted (step 4).
7. **Verify:** `dotnet build`, the test suite, and a final SF run that proposes **zero** changes to the code, apart from files the user chose not to adopt yet (step 4, option 3). Its drift lines are step 8's.
   - **An ambiguous name after adoption** (CS0104, e.g. `Order` from two namespaces the global usings import): add a using alias for the type the file means at the top of the file (`using Order = eShop.Ordering.Domain.AggregatesModel.OrderAggregate.Order;`). An alias outranks every namespace import, global ones included, and later runs keep it. Don't leave the file unadopted.
8. **Finish the drift check (required).** §1 wrote `guardrails/domain.drift.json`, so every run already compares the Domain model's database facts, and the types the Guardrails templates generate, with the code. Load **configure-drift-check** and do its steps 4 to 9 for the Domain designer: fix the recipes until no fact is reported as can't be verified (a kind of fact with no recipe, or a style §1 missed) and validate-drift-recipes shows every recipe reading the code, then resolve the rest. Save the designer first: the check reads the saved model.
   - No recipe file (the adoption began before §1 wrote one): do all of configure-drift-check's steps.
   - The adoption isn't finished until a run finds no drift and no warnings, so it writes no `<<DriftReport>>`, and the user has approved the model changes (configure-drift-check's step 9). Skip that approval when an orchestrating skill's **User decision** says so.
   - A contradiction it finds is a recipe misreading the code, a modelling slip or real drift. Check the code at its location first (configure-drift-check's step 6): fix a recipe that misreads it, and otherwise fix the model to match the code, never the code. Where the code looks wrong, say so in the final report; setting up the check never changes the code. A fact the model can't state the way the code does (e.g. a fixed-length `char` column, which `Text Constraints` can't express) is modelled with `Column` → `Type` instead.
   - Code the model doesn't describe (e.g. a generic repository's methods, an unmodelled `Entity<T>` configuration): model what §3 should have covered; add an exception for helpers and plumbing, asking the user about anything that isn't clearly one.

## Gotchas

- **Saving.** A package created by script (`createPackage`) is not seen by the SF until the designer is saved. Save it explicitly with `saveOnSuccess: true`.
- **Concurrent edits.** The user may be editing the model while you work. Elements you didn't create (new classes, attributes, changed multiplicities) are theirs. Don't modify them and don't apply their generated output. Report them instead.
- **No hand edits.** Beyond the merge directives and a using alias for an ambiguous name (§6 step 7), never hand-edit adopted files to make the diff clean, and never turn off Auto Format to make it smaller (§6 step 5). Fix the model.
- **Check a merge risk on the staged diff first.** Before excluding a file from generation or asking the user about something the merge might lose, stage it and read its diff. Ask only about a loss the diff shows.
""".Replace("<<DriftReport>>", ModelSyncGuidance.ReportPath(outputTarget.ExecutionContext, "Domain")));
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
