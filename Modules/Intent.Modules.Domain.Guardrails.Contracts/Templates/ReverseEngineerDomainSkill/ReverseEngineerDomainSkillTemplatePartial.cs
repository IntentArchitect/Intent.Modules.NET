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
description: Reverse-engineer an existing C# domain layer (entities, value objects, enums, repositories, EF Core configuration) into the Intent Architect Domain designer so the model mirrors the code and the Intent.Domain.Guardrails.Contracts module adopts the existing files in place, installing Intent.Metadata.RDBMS or Intent.Metadata.DocumentDB to match the code's persistence technology. Use when the user has installed the Domain Guardrails module (or asks to "align / model / reverse-engineer the domain") on a brownfield codebase.
short-description: Model an existing domain layer in the Domain designer and adopt its files with the Domain Guardrails module.
argument-hint: "[application name]"
---

# Reverse-engineer the Domain

Goal: the Domain designer describes the code **as it is**, and a Software Factory (SF) run against the existing files changes **nothing but the Intent file headers**. You are aligning the model to the code, not the code to the model. Business logic, method bodies and EF configuration code stay hand-written.

Load **scripting-the-model**, **exploring-the-model** and **changing-the-model** before you start, and **generating-and-applying-code** / **resolve-destructive-changes** before the first SF run. Follow them for _how_ to script, lay out and generate; this skill only adds what is specific to reverse-engineering.

## 0. Preconditions

1. `listInstalledModules(appId)` (via `run_solution_query`) must include `Intent.Domain.Guardrails.Contracts`. If it doesn't, stop and ask.
2. **Detect the persistence technology and install the matching metadata module.** Running this skill is the user's authorization to install the metadata modules that apply. Detect from the code: grep the Infrastructure/DbContext/DI code and the csproj `PackageReference`s.

   | Evidence in the code                                                                                                                                                                                                                                                       | Install (if not already installed)                                                |
   | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------- |
   | EF Core relational: `DbContext` + `IEntityTypeConfiguration` / `OnModelCreating`, `Microsoft.EntityFrameworkCore.SqlServer` / `Npgsql` / `Sqlite` / `MySql`, `UseSqlServer(...)` etc. (an `UseInMemoryDatabase` stand-in for a relationally configured model still counts) | `Intent.Metadata.RDBMS`                                                           |
   | Document store: `MongoDB.Driver`, `Microsoft.Azure.Cosmos`, EF Core Cosmos (`UseCosmos`), `[BsonId]`, containers/collections instead of tables                                                                                                                             | `Intent.Metadata.DocumentDB`                                                      |
   | Both                                                                                                                                                                                                                                                                       | Both. Use RDBMS on the relational package and DocumentDB on the document package. |
   | Neither (no persistence)                                                                                                                                                                                                                                                   | None                                                                              |

   - Always also make sure `Intent.Modelers.Domain.Repositories`, `.ValueObjects` and `.Services` are installed when the code has custom repositories, value objects or domain services.
   - Get the exact current version with `searchAvailableModules(id)`, then call `install_or_update_modules` and state in one line what you installed and why.
   - After installing, a designer tab may open a dialog that blocks tools. Ask the user to close it, then continue.
   - Ask only when the evidence is ambiguous (e.g. a custom data layer you can't classify).
3. **Know the installed template behaviour before you model.** The module is evolving. Decompile it rather than guess. It is cached under `<solution>/intent/.cache/modules/<id>.<version>/lib/*.dll`; run `dnx ilspycmd --yes -- -p -o <tmp> <dll>`, which needs no global install. For each template, confirm:
   - **File path:** `GetFolderPath()` makes the file path = Template Output location + the designer folder path.
   - **Class name:** whether it is `Model.Name` verbatim (no `I`, no suffix).
   - **Repository output:** whether repositories generate the **interface** (older pre-release: name the element `ICustomerRepository`, output in the Domain project) or the **implementation class** (newer: name it `CustomerRepository`, output in the Infrastructure `Repositories` folder).
   - **Async operations:** read `IsAsync()`, `IncludeCancellationToken()` and `GetOperationReturnType()` in `ContractBuilderExtensions`, then model to what you find (see §3 "Async operations"). In the current module an operation is async when its name ends in `Async` (ordinal, so case-sensitive), unless its `Async Configuration` stereotype says otherwise. The older `Asynchronous` stereotype is no longer read.
   - **Merge directive:** whether the template output carries `[assembly: DefaultIntentManaged(...)]` (see §6).

## 1. Extract the code (read-only, in parallel)

Dispatch read-only `Explore` sub-agents in **one** message, with non-overlapping slices. Ask for verbatim-accurate specs, not summaries:

- **Domain project, every `.cs` file plus the EF `IEntityTypeConfiguration`s and the `DbContext`.** For each type, report:
  - Path, namespace and kind (aggregate root / owned entity / value object / enum / repository interface / helper).
  - Every property: exact type, nullability, collection-ness and setter.
  - Constructors and public methods (signatures only).
  - Enum literals with explicit values.
  - Repository interfaces: their base interface and every extra method signature.
- **From the EF configuration, per property and relationship:**
  - `OwnsOne` / `OwnsMany` / `HasOne` / `HasMany`.
  - FK properties and shared primary keys (`HasForeignKey<T>(x => x.Id)`).
  - Keys: `HasKey` (single or composite, with key types), `HasPrincipalKey` (FKs onto a non-primary key), `HasNoKey`, and which classes the `DbContext` doesn't map. These decide the key mode in §4a.
  - Delete behaviour.
  - `HasMaxLength`, `HasColumnType("decimal(p,s)")`, `HasDefaultValue`, `HasIndex(...).IsUnique()`, row-version and concurrency tokens.

## 2. Align the Codebase Structure

Template Outputs from a fresh install land under whatever anchor was available, often the Api project. Move each Guardrails Template Output with `el.setParent(...)` so that **output folder + designer folder path = the existing file's folder**. Create `Folder`s in the project as needed (`Folder Options: Namespace Provider = true` keeps namespaces matching). Typical layout:

| Template                                                                 | Put the Template Output in                                                                      |
| ------------------------------------------------------------------------ | ----------------------------------------------------------------------------------------------- |
| EntityContract                                                           | the folder that holds entity sub-folders (e.g. `Domain/Entities`)                               |
| EnumContract, ValueObjectContract, DomainContract, DomainServiceContract | wherever their files' parent folders hang (often the Domain project root)                       |
| RepositoryContract                                                       | interface variant: `Domain/Repositories`. Implementation variant: `Infrastructure/Repositories` |

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
  - **File named differently from its type:** set `Filename` to the file's name, with or without `.cs` (the folder stays the element's own).
  - **File holding several types:** model every type as its own element in that file's folder, and set `Filename` to the file's name on each type the file is *not* named after. Elements that end up at the same path are generated into one file, written by the element the file is named after.
  - **Set it before the first SF run.** If a type already had its own file generated and you then point it at another file, the SF offers to delete the old file. Treat that as a destructive change.
- **Delete placeholders** such as a default `MyEntity` if the code doesn't have them.
- **Attributes:** use the exact names and types (`string`, `guid`, `datetime`, `decimal`, `long`, `binary`, enums). Set `isNullable` for `T?`. Do **not** model collections of entities as attributes; use associations.
- **Collections:** the template emits `ICollection<T>` for properties and `IEnumerable<T>` for operation parameters and return types. Wherever the code uses anything else (`List<T>`, `IList<T>`, `IReadOnlyCollection<T>`, `IQueryable<T>`, `T[]`, …), apply the **`Collection Type`** stereotype to that attribute, association end, parameter or operation (for its return type) and set `Type` to match (`Custom` + `Custom Format` with `{0}` for anything else). Otherwise Merge mode rewrites the existing signature.
- **Parameter modifiers:** where a constructor or operation parameter is declared `ref`, `out`, `in`, `ref readonly` or `params`, apply the **`Parameter Modifier`** stereotype and set `Modifier` to match (for `params`, also set `Collection Type = Array`). Otherwise Merge mode drops the modifier from the existing signature. An extension method (`this` parameter) can't be reproduced; leave it unmodelled and report it.
- **Value objects:** model them as `Value Object` elements, used as attribute types on the owner (e.g. `DeliveryAddress: AddressSnapshot?`).
- **Associations:** always set both multiplicities and the target navigation name to the code's property name.
  - Composition (owned, `OwnsOne` / `OwnsMany`, or a cascade-owned 1:1): `sourceMultiplicity: "1"`.
  - Reference to another aggregate (`HasOne(...).WithMany()`): `sourceMultiplicity: "0..*"`, `targetMultiplicity: "1"` (or `"0..1"` when it is optional).
  - Unidirectional unless the code navigates both ways.
- **Primary / foreign keys:** do **not** add `Id` / `XxxId` attributes unless RDBMS is installed (§4). Merge mode keeps them in code either way.
- **Repositories:** add one only where the code has a custom repository. Set its type to the entity, and model **only the extra methods** beyond the generic base, named exactly as in code (`ExistsWithEmailAsync`, …). Set their return types and async settings by "Async operations" below; for a conventional `…Async` method that is `T`, without `Task`. Put the repository in the same designer folder as its entity's code folder.
- **Comments:** copy XML-doc summaries into element comments where they help.
- **Constructors and methods:** model only behaviour you need. Unmodelled members survive in Merge mode.

### Async operations

This applies to every modelled operation, whether it belongs to a repository, a domain service or an entity. Settle each one before the first SF run. Merge mode applies the generated signature (modifiers, return type and parameters) to the existing method and keeps only its body. A mismatch here breaks the build, e.g. `async` stripped from a method whose body still `await`s.

**What the template emits.** An operation is async when its name ends in `Async`, unless the operation's `Async Configuration` stereotype says otherwise. An async operation gets:

- the `async` keyword, except on abstract operations, which still get the return type and parameter below;
- a `Task` return type when none is modelled, otherwise `Task<T>`. A modelled `Task` / `Task<T>` is not wrapped again;
- a trailing `CancellationToken cancellationToken = default` parameter.

A sync operation's return type is emitted exactly as modelled, with no token. A brand-new async method gets an empty body with no `await`, so the compiler warns CS1998. In reverse-engineering that is just another inserted member (§6), so fix the model.

The stereotype has two properties:

- `Is Async` (default `true`) explicitly sets async or sync.
- `Include Cancellation Token` (default `true`) turns the extra parameter off. It only applies when `Is Async` is on.

Applying the stereotype with its defaults therefore makes the operation async.

**Conventional code: no stereotype.** The method is named `…Async`, is declared `async`, returns `Task` / `Task<T>` and ends with `CancellationToken cancellationToken = default`. Model the name as-is, set the return type to `T` (or none for `Task`), and add nothing.

**Non-conventional code: set the stereotype explicitly.** Check every method against the convention. Wherever it differs, apply `Async Configuration` and set **both** properties explicitly. Don't rely on the defaults or on the name:

| Code                                                                                                             | Model                                                                             |
| ---------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------- |
| `async Task<T> GetNextReference(…, CancellationToken cancellationToken = default)`: async, but no `Async` suffix | `Is Async = true`, `Include Cancellation Token = true`, return type `T`           |
| `bool ExistsAsync(…)`: named `…Async` but synchronous                                                            | `Is Async = false`, return type `bool`                                            |
| `async Task<T> LoadAsync(…)` with no `CancellationToken` parameter                                               | `Is Async = true`, `Include Cancellation Token = false`, return type `T`          |
| `Task<T> FindAsync(…) => _inner.FindAsync(…)`: returns a task without `async`, and takes no `CancellationToken`  | `Is Async = false`, return type `Task<T>` (the designer has `Task` and `Task<T>`) |

```js
const st = op.ensureStereotype("Async Configuration");
st.setProperty("Is Async", true);
st.setProperty("Include Cancellation Token", false);
```

**Shapes the template can't reproduce.** Leave these methods unmodelled; Merge mode keeps them untouched. Say so in your report. The designer has no `CancellationToken` or `ValueTask` type, so none of them can be modelled by hand:

- a `CancellationToken` that is named differently (e.g. `ct`), not optional, or not the last parameter;
- a method that returns a task without being `async` and takes a `CancellationToken`;
- `ValueTask` / `ValueTask<T>` return types.

## 4. Persistence alignment

### 4a. RDBMS (relational solution; `Intent.Metadata.RDBMS` installed in §0)

1. **Mark the package.** Apply `Relational Database` to the Domain package (`pkg.ensureStereotype("Relational Database")`).
2. **Choose the key mode before modelling classes and associations.** The application setting `Database Settings` → `Key Creation Mode` defaults to `Explicitly add PKs and FKs automatically` (value `explicit`). In that mode the designer gives every class an `Id` primary key of the default `Key Type` (usually `guid`), creates and removes FK attributes from associations, and silently reverts FK attributes you add or delete by hand. That only reproduces code where every entity has a single surrogate key and every FK points at it.
   - **Keep `explicit`** when the code follows that convention: one `Id` per entity, FKs named `XxxId` onto it.
   - **Switch to `Manually add PKs and FKs` (value `manual`)** when it doesn't. Typical signs: EF database-first code; `int`, `string` or composite keys (`HasKey(x => new { x.A, x.B })`); FKs onto a non-primary key (`HasPrincipalKey`, e.g. `bids.memberNo` → `members.memberno`, `silo` → `sapPlants.code`); keyless entities (`HasNoKey`, views); classes the `DbContext` doesn't map. Read the setting with `getApplicationSettings({ search: "Key Creation Mode" })` (via `run_solution_query`), change it with `update_application_settings`, and state in one line that you changed it and why.
   - Switch before you create classes or associations. If keys were already added in `explicit` mode, delete every one that isn't in the code.
3. **Automatic keys (`explicit` mode only).** The designer adds PKs and FKs only from change events, so existing elements aren't back-filled. Touch each association (set a multiplicity to something else and back) to fire them. Then fix up the result:
   - **Casing:** rename generated keys to match the code (they may come out as `id` / `customerId`).
   - **Missing primary keys:** add `Id: guid` with the `Primary Key` stereotype, plus `setOrder(0)`, to classes with no associations (e.g. `Product`, a counter table).
   - **Shared-PK 1:1** (code `HasForeignKey<Child>(x => x.Id)`): make the child's `Id` both `Primary Key` and `Foreign Key(Association = <association id>)`. Delete any `ParentId` FK the designer added that isn't in the code.
   - **Owned 1:1 child with no children of its own:** the designer removes an explicit PK, because the key is implied and shared with the owner. Accept that; the code's `Id` survives in Merge mode.
4. **Manual keys (`manual` mode only).** Nothing is added for you; model exactly the keys the code has:
   - **Primary keys:** add the key attribute with the code's own name and type (`memberno: int`, `code: string`) and apply `Primary Key`. For a composite key, apply `Primary Key` to each part and order the parts as the code's `HasKey` does (`setOrder`).
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
   - possibly the merge directive
   - plus one `PackageReference Intent.RoslynWeaver.Attributes` per csproj.
2. **Diagnose anything else:**
   - **`create`** means a path or name mismatch. Fix the folder, Template Output location or element name, or add `File Config` for a type that lives in another file or a differently named one. Never apply it.
   - **Inserted members** mean the model has something the code doesn't (wrong name, type or nullability). Fix the model.
   - **A changed signature on a modelled operation** (`async` added or removed, a `CancellationToken` appended or dropped, the return type wrapped in `Task<…>`) means the async rule didn't match the code. Set `Async Configuration` on that operation (§3 "Async operations"). If its shape can't be reproduced, remove the operation from the model. Never hand-edit the signature.
   - **Deleted or changed lines** mean drift. Use **resolve-destructive-changes**. Never apply destructive changes.
   - **A template exception** ("The parent has not been set for this type …") is a module bug. Report it to the user. Only as a last resort, and only _before_ that template has ever been applied, disable that Template Output (`Template Output Settings.Is Enabled = false`). Disabling it after an apply makes the SF propose deleting every file it previously owned.
3. **Merge directive (critical).** Every adopted file must carry `[assembly: DefaultIntentManaged(Mode.Merge)]`. If the template didn't write it, add exactly that line above the `IntentTemplate` attribute in each adopted file **before any further model change**. Without it the next SF run treats the file as fully managed and strips constructors, XML docs, attributes, base types, `virtual` and initialisers. Use plain `Mode.Merge`: a `Targets = Methods | Constructors | Properties`-scoped variant leaves the class declaration itself fully managed.
4. **Apply** only the header-only files you reviewed (`apply_staged_file_changes` with explicit paths).
5. **Verify:** `dotnet build`, the test suite, and a final SF run that proposes **zero** changes.

## Gotchas

- **Saving.** A package created by script (`createPackage`) is not seen by the SF until the designer is saved. Save it explicitly with `saveOnSuccess: true`.
- **Blocking dialogs.** After a module install, a designer tab may show a dialog that blocks every tool ("A dialog is currently showing…"). Ask the user to resolve it, then retry.
- **Concurrent edits.** The user may be editing the model while you work. Elements you didn't create (new classes, attributes, changed multiplicities) are theirs. Don't modify them and don't apply their generated output. Report them instead.
- **No hand edits.** Beyond the merge directive, never hand-edit adopted files to make the diff clean. Fix the model.
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
