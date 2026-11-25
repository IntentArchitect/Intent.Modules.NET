# Check classification: module-building checks against Common.Drift

Made on 2026-11-12. It covers the checks in the two module-building implementations:
- eShopCustom: `D:DevTempeShopCustomintentmodules`
- the Java/Vue spike: `D:DevSpikesSamples.eShop.Demo.Java`

Each check was sorted against Common.Drift's capabilities as they were on that date. The new engine is not limited by those capabilities (see `plan.md`).

## Categories

| Category | Meaning |
|---|---|
| A1 | Covered by an existing Common.Drift fact, and works without writing code |
| A2 | Covered only by Common.Drift's structure comparison. That comparison merges C# contract templates into the code, so it writes code and works for C# only. |
| B | Expressible as a new fact kind plus a recipe. A new kind on an existing designer needs the vocabulary opened; a new designer contribution is already allowed. |
| C | Needs a rule in code. Some of these are really designer validation or model-only rules. |
| D | Conflicts with a Common.Drift design decision |

## Totals

| Module | Checks | A1 | A2 | B | C | D |
|---|---|---|---|---|---|---|
| eShopCustom Domain | 73 | 12 | 9 | 31 | 6 | 15 |
| eShopCustom Services | 87 | 20 | 9 | 23 | 24 | 11 |
| eShopCustom Front End (Blazor) | 74 | 11 | 0 | 30 | 16 | 17 |
| eShopCustom Architecture | 9 | 1 | 0 | 1 | 3 | 4 |
| Java Domain (JPA) | 32 | 7 | 9 | 9 | 7 | 0 |
| Java Services (Spring MVC) | 21 | 7 | 7 | 2 | 4 | 1 |
| Java Front End (Vue) | 19 | 0 | 0 | 12 | 5 | 2 |
| Java Architecture | 6 | 0 | 0 | 2 | 4 | 0 |
| **Total** | **321** | **58** | **34** | **110** | **69** | **50** |

About 19 of the 69 C checks are designer validation or model-only rules.

## Reading it against the plan

- **Completeness is not required** (plan, principle 1). So the D checks that report unmodelled code as drift don't carry over as they are: code→model interactions, and lists of code the model can't express.
- **Scope is required** (plan, block 5). So the adoption-scope checks among the D items stay relevant.
- **Each section below is the full output of the agent that audited that module.** Line references point into each module's engine source.


---

# eShopCustom: Domain module

#### Summary
**Files read.**
- Everything under `D:\Dev\Temp\eShopCustom\intent\modules\eShopCustom.Modules.Domain.CSharp\eShopCustom.Modules.Domain.CSharp\`:
  - `Drift\DomainDrift.cs`, all 2,060 lines (cited as DD below);
  - `Drift\CSharpReader.cs` (CR), `Drift\DomainModelReader.cs` (MR), `Drift\DriftModel.cs`, `Drift\CodeModel.cs` and `Drift\AdoptionScope.cs` (AS);
  - `FactoryExtensions\DomainDriftReporter.cs` (Rep);
  - `Api\*StereotypeExtensions.cs`, through their definitions and options.
- The test project `...\eShopCustom.Modules.Domain.CSharp.Tests\`: every test name, plus which rule ids the tests mention.
- `D:\Dev\Temp\eShopCustom\GUARDRAILS.mdx` and `D:\Dev\Temp\eShopCustom\brownfield\PROGRESS.md`.
- For CD: `Intent.Modules.Common.Drift\docs\README.md`, its `CONTEXT.md`, `Intent.Modules.Domain.Guardrails.Contracts\docs\README.md`, its `Drift\DomainDrift.cs` (the 17 fact kinds), and the EF Core and domain-events parts of its `Templates\DriftRecipeGuides.cs`.

**Total checks: 73**, from 57 rule ids. Live designer validation is left out, because it produces no drift findings.

| Category | Count | Notes |
| --- | --- | --- |
| A1 | 12 | |
| A2 | 9 | |
| B | 31 | All 31 would be new kinds on the existing Domain designer; none needs a new designer |
| C | 6 | |
| D | 15 | |

A partly covered check is classified by its uncovered part, and the table says what is covered.

**Takeaways.**
- **What already works describe-only is the classic EF facts:** table name (including EF's naming convention), schema, column name and type, the relationship facts, and origins the code raises directly. Only 12 checks fall here.
- **The biggest group (31) is plain recipe material that CD can't express today, for three reasons:**
  - **Closed vocabulary:** the Domain designer has one contributor, and the model side is read only from stock stereotypes. 11 of the B checks read the model side from this module's own stereotypes (Plain Class, Backing Field, HiLo, Enum Storage, References, On Delete).
  - **Missing elements count as pending:** CD's structure rules treat a modelled type or member that's missing from the code as a change the Software Factory will write, not as drift. Describe-only, nothing writes it, so 7 "modelled X must exist" checks need fact kinds whose missing code is reported as can't-be-verified.
  - **Index facts:** CD reads only named indexes, and eShop's single-column indexes have no name.
- **The 9 A2 checks** (kind, abstract, member types and signatures, enum literals, unmodelled public members) work only through the code-writing contract templates. That is the core gap for a describe-only target.
- **The 6 C checks are genuine plug-ins:** the SeedWork base-class contract, configurations being applied in a DbContext, EF Core nullability (which depends on the project's nullable setting), the key inherited from `Entity`, and raises followed through private helpers.
- **The 15 D checks are policy, not missing vocabulary:**
  - the adoption scope (Scoped/Strict, Not yet adopted, Backlog only in adopted files);
  - failing on model features the check can't compare;
  - a warning for every piece of code the check doesn't read;
  - code→model gaps CD deliberately never reports (raises the model doesn't link, non-public members).
- **Test gap:** 14 rule ids are never mentioned in a test: `association.missing`, `association.type`, `constructor.missing`, `constructor.signature`, `operation.missing`, `persistence.ignored`, `repository.entity`, `repository.location`, `repository.missing`, `scope.source-empty`, `scope.sources`, `type.abstract`, `type.inheritance` and `type.kind`.

#### Checks
| # | Check | Dir | Cat | CD equivalent, or proposed fact kind / plug-in | Stack-specific? (what) | Where |
|---|---|---|---|---|---|---|
| 1 | Drift Scope configured: the stereotype is on the package, it has Sources, and each Source holds .cs files (`scope.missing` / `.sources` / `.source-empty`, Wrong) | meta | D | Nearest: no recipe file means the check is off (a warning, not an error); `files` globs | No | DD `Execute` L120-142; AS L83 |
| 2 | Each Adopted entry matches a file, folder, glob or type name (`scope.stale`) | meta | D | Nearest: the "exception matches nothing" warning | No | AS ctor L35-78; DD L144-149 |
| 3 | Strict: a file under Sources that is neither adopted nor excluded is Backlog (`scope.strict`) | scope | D | None | No | DD `ReportScope` L1630-1641 |
| 4 | Not-yet-adopted summary count (warning) | scope | D | None | No | DD L1642-1647 |
| 5 | The reader starts and each file parses (`domain.read`) | meta | A1 | Built into the engine (a failing check is a Code error), not a fact kind | Roslyn bundled in the module | Rep L50-62; CR `Read` L58-70 |
| 6 | Any model feature the check can't compare fails the run: other stereotypes, child elements, generic parameters, Table or folder Schema, an auto-generated PK, multi-column Index, misplaced On Delete, event generalization or Map From Class (`unsupported.model`, Error) | meta | D | None: CD ignores model features outside its kinds | Lists of checked stereotypes | MR L27-32, L81-102, L118-120, L192-204, L243-261; DD L419-422, L523-526, L893-896 |
| 7 | SeedWork types exist with the expected kind when the model relies on them: `Entity` (class), `IAggregateRoot` (interface), `ValueObject` (class), `IRepository` (interface) (`seedwork.missing`) | M→C (implied) | C | Plug-in: a cross-file existence check that depends on the whole model | eShop SeedWork | DD `CheckSeedWork` L237-245, `RequireSeedWork` L247-267 |
| 8 | SeedWork members: `Entity` declares a non-private `Id`, and `AddDomainEvent(INotification)` when any origin is modelled (`seedwork.contract`) | M→C (implied) | C | Plug-in: a base-class contract whose model side is a convention, not an element's value | SeedWork, MediatR | DD L216-235 |
| 9 | Package Schema ↔ `HasDefaultSchema`, both ways, including a Schema with no DbContext (`persistence.schema`) | both | A1 | `table.schema` (the package's Schema is inherited by each class; the recipe reads `HasDefaultSchema`). Compared per class with Table/View, not per package | EF Core | DD `CheckDbContexts` L276-317 |
| 10 | Every modelled class's configuration is applied: `ApplyConfiguration(new X())` or `ApplyConfigurationsFromAssembly` (`persistence.applied`) | M→C | C | Plug-in: finds the configuration class by its generic argument, then looks for it in another file. It would be B only under a naming convention (`configuration.applied`) | EF Core | DD L319-328, L362-377, `CreatedTypeName` L404 |
| 11 | DbContext coverage warning (DbSets, statements it doesn't read, other members), and an inline `Entity<T>` block shadowed by a configuration class | C→M | D | None: this is coverage disclosure, which CD rejected | EF Core | DD L330-359 |
| 12 | A modelled Class, Value Object, Enum or Domain Event has a code type of that name under Sources (`type.missing`) | M→C | B | New kind `type.kind` with no `absent`. CD's structure comparison treats missing as pending | No | DD `CheckType` L424-441 |
| 13 | Type kind matches: enum vs class/record (`type.kind`) | M→C | A2 | Structure: kind. Describe-only: `type.kind` | No | DD L425-434, L1988 |
| 14 | The type's file is `Sources[0]/<model folders>/<Name>.cs` (`type.location`) | M→C | B | New kind plus a `{Folder}` placeholder | House convention | DD L451-456; AS `ExpectedPath` L108 |
| 15 | A type declared more than once: only one is compared, with a warning | meta | A1 | Engine: a find that ends at several nodes is can't-be-verified (CD is stricter: it fails) | No | DD L445-449 |
| 16 | Base type follows the element kind: a Value Object derives `ValueObject`; a Class with no generalization or Plain Class derives `Entity` (`type.base`) | M→C | B | New kind `type.base`, model value from the element kind and Plain Class; needs the vocabulary opened | SeedWork | DD L471-479, L688-693 |
| 17 | A Plain Class doesn't derive `Entity` or implement `IAggregateRoot` (`type.plain`) | M→C | B | Same `type.base` kind (module stereotype) | SeedWork | DD `CheckClass` L656-664 |
| 18 | Abstract matches (`type.abstract`) | both | A2 | Structure: modifiers | No | DD L665-670, L695-700 |
| 19 | Generalization ↔ the code's base among modelled classes, both ways (`type.inheritance`) | both | B | M→C is A2 (bases the model names). CD never reports a modelled base the code adds ("extra code bases are fine") → `type.base` | No | DD L675-687 |
| 20 | Aggregate root (no composition owns it) ↔ implements `IAggregateRoot`, both ways (`type.aggregate-root`) | both | B | New kind `type.aggregateRoot`, model value derived from the associations | SeedWork marker | DD L702-714 |
| 21 | An `IEntityTypeConfiguration<T>` or inline `Entity<T>` for an unmodelled class (`type.unmodelled`) | C→M | A1 | An `own` convention; facts locate the modelled ones | EF Core | DD `CheckUnmodelledTypes` L1538-1548, L1580-1590 |
| 22 | An unmodelled enum, `INotification` type, `Entity`/`ValueObject`/modelled-class subtype, or `IRepository<T>` interface in adopted code (`type.unmodelled`) | C→M | A2 | `own` conventions only count a type as described when it is generated or a fact located it. Describe-only, this needs the type-locating kinds in #12 | SeedWork, MediatR | DD L1550-1578, L1592-1616 |
| 23 | An adopted type of no recognised kind gets a "not checked" warning | C→M | D | None | No | DD L1623-1625 |
| 24 | Enum literals exist; an unmodelled literal is Backlog (`enum.literal`) | both | A2 | Structure: enum literals | No | DD `CheckEnum` L748-757, L790-798 |
| 25 | Enum literal values, both ways (`enum.value`) | both | A2 | Structure: literal value | No | DD L759-778 |
| 26 | Enum literal order (`enum.order`) | M→C | B | New kind `enum.literals`, a comma list like `key` | No | DD L781-788 |
| 27 | A Domain Event implements `INotification` (`event.base`) | M→C | B | `type.base`, model value from the element kind | MediatR | DD `CheckDomainEvent` L509-514 |
| 28 | Domain Event properties: each public property or positional parameter exists, with matching type and collection flag | M→C | B | Types are A2 (DomainEventContract). Missing is pending in CD → needs a member kind | No | DD L516-541; CR L172-177 |
| 29 | An unmodelled public event property (`member.unmodelled`) | C→M | A2 | Structure: unmodelled public member | No | DD L558-563 |
| 30 | A modelled Domain Event Origin is raised: `new X(...)` with `AddDomainEvent` in the member, or in private methods it calls, transitively (`event.origin`, Wrong) | M→C | C | Direct raises and a helper named after the event are A1 (`intent.raises`, domain-events guide). Following private calls of any name, transitively, needs a plug-in | SeedWork `AddDomainEvent` | DD `Raised` L582-611, L623-628; CR L505-531 |
| 31 | A raise in adopted code with no origin link (`event.origin`, Backlog) | C→M | D | CD never reports interactions the code has and the model lacks | — | DD L630-644 |
| 32 | An `AddDomainEvent` whose event can't be identified gets a warning | meta | A1 | An `unreadable` read case (CD fails it as can't-be-verified) | — | DD L646-651 |
| 33 | A modelled Class or VO attribute has a non-private property, or a shadow FK (`attribute.missing`) | M→C | B | A member kind with no `absent`; CD treats missing as pending | No | DD `CheckAttribute` L937-963 |
| 34 | Attribute type and collection flag vs the property type, through the designer-primitive→C# table (`attribute.type`) | both | A2 | Structure: property type. Describe-only it needs an `attribute.type` kind and a type table | C# primitives | DD L966-971, `TypeMatches` L1673-1690 |
| 35 | Nullability with EF Core semantics: a value type needs `?`; with nullable reference types on (csproj or `#nullable`), `?`; with them off, `[Required]` or `.IsRequired()` (`attribute.nullability`; properties, fields, events) | both | C | Partly A1 (`column.required` for non-PK string/binary) and A2 (value-type `?`). Working out the nullable setting from the .csproj and `#nullable` directives, and other reference types, needs a plug-in | EF Core, C# nullable reference types | DD L972-981, `Nullability` L1652-1669, L1022-1044, L542-550; CR L42-57, L86-118 |
| 36 | Inherited key: a PK attribute the class doesn't declare must be `Id`, of `Entity.Id`'s type, and not nullable | M→C | C | `key` (A1) says which attribute is the key; finding `Id` on the SeedWork base in another file needs a plug-in | SeedWork | DD L910-935, L165-170 |
| 37 | A foreign key as a shadow property `Property<T>("X")`: type and nullability | M→C | B | `attribute.type` kind with a find alternative on `Property<…>("{Attribute}")` | EF Core | DD L939-956, L1856-1862 |
| 38 | A Value Object attribute has `OwnsOne(x => x.Attr)` (`persistence.owned`) | M→C | B | New kind `column.owned` | EF Core | DD L983-990 |
| 39 | A modelled attribute isn't `Ignore()`d (`persistence.ignored`) | M→C | B | New kind `column.mapped`: `Ignore` gives false, `absent` true | EF Core | DD L992-997 |
| 40 | Backing Field: the named field exists, with the attribute's type (`attribute.field-missing` / `.type`) | M→C | B | New kind plus a `{BackingField}` placeholder taken from the module stereotype | C# | DD `CheckBackingField` L1006-1021 |
| 41 | Backing Field is mapped with `Property("_f")` (`persistence.field-unmapped`) | M→C | B | New kind `column.backingField` | EF Core | DD L1046-1052 |
| 42 | Backing Field column: Column Name or the attribute name ↔ `HasColumnName` or the field name | both | B | `column.name` can't find `Property("_f")` without the `{BackingField}` placeholder | EF Core | DD L1054-1061 |
| 43 | A field mapped by `Property("_f")` with no Backing Field attribute (`member.unmodelled`) | C→M | B | An `own` convention on `Property("…")` chains, once the Backing Field kinds (#40-42) locate the modelled ones | EF Core | DD L861-878 |
| 44 | HiLo Sequence ↔ `UseHiLo("seq")`, both ways | both | B | New kind `column.hilo` (module stereotype) | EF Core | DD `CheckPersistence` L1085-1089 |
| 45 | Column Name ↔ `HasColumnName`, both ways | both | A1 | `column.name` | EF Core | DD L1091-1098 |
| 46 | Column Type ↔ `HasColumnType`, both ways | both | A1 | `column.type` | EF Core | DD L1100-1104 |
| 47 | PK Data source "User supplied" ↔ `ValueGeneratedNever()`, both ways | both | B | New kind `key.generated` (CD doesn't read the stock Primary Key's Data source) | EF Core | DD L1106-1122 |
| 48 | A single-column Index (IsUnique) ↔ `HasIndex(x=>x.P)` or `HasIndex("P")` with `.IsUnique()`, both ways | both | B | CD's `index.*` kinds cover only named indexes; unnamed ones need a new kind or a wider model reader | EF Core | DD L1124-1144 |
| 49 | Max length ↔ `HasMaxLength`, `[MaxLength]` or `[StringLength]`, for string, binary and enum (Enum Storage Max Length) | both | B | Strings are A1 (`column.maxLength`); binary and enum need an extension | EF Core, DataAnnotations | DD L1146-1167 |
| 50 | `HasMaxLength` on a type other than string or binary is a no-op, so it gets a warning | C→M | D | None: a code-only lint with no model counterpart | EF Core | DD L1149-1156 |
| 51 | Enum Storage As String/Number ↔ `HasConversion<string>()` | both | B | New kind `column.enumStorage` (module stereotype) | EF Core | DD L1169-1184 |
| 52 | Table Name ↔ `ToTable`, both ways; with no `ToTable`, EF's convention name (the DbSet property's name, else the class name) | both | A1 | `table.name` with find alternatives (`ToTable`, then `DbSet<{Entity}>`) and `absent` `{Entity}` | EF Core conventions | DD `CheckTable` L717-743, `ConventionTable` L389-401 |
| 53 | Each EF configuration statement the reader doesn't interpret gets a warning: unknown calls, chain tails, non-chain statements, an owned type's own config, inverse navigations, complex selectors, a missing Configure | C→M | D | None | EF Core | DD `EntityFacts.Interpret` L1831-1983, L492-498; CR L262-267 |
| 54 | A navigable association end has a property (`association.missing`) | M→C | A1 | `relationship.multiplicity` (its find falls back to the property; nothing found is can't-be-verified) | No | DD `CheckNavigation` L1216-1222 |
| 55 | Navigation property's target and collection flag (`association.type`) | M→C | A1 | `relationship.multiplicity` read cases on the declaration (also A2) | No | DD L1225-1232 |
| 56 | A reference navigation's optionality: its FK's nullability, else the property's (`association.nullability`) | M→C | A1 | `relationship.multiplicity`. The `[Required]` case with nullable reference types off is as in #35 | EF Core | DD L1242-1279 |
| 57 | The attribute `HasForeignKey` names must carry Foreign Key (`association.foreign-key`) | C→M (Wrong) | A1 | `relationship.foreignKey` | EF Core | DD L821-833 |
| 58 | On Delete (on the navigable end or the References attribute) ↔ `OnDelete(DeleteBehavior.X)`, both ways | both | B | New kind `relationship.onDelete` (module stereotype) | EF Core | DD `CheckDeleteBehavior` L1326-1349 |
| 59 | References (FK attribute and Target) ↔ `HasOne<T>().WithMany().HasForeignKey(x=>x.Fk)` with no navigation, both ways | both | B | New kind `reference.target`, plus `own` for unmodelled ones (module stereotype) | EF Core | DD `CheckReferences` L1286-1323 |
| 60 | A modelled constructor has a matching non-private constructor (`constructor.missing`) | M→C | B | A member kind; CD treats missing as pending | No | DD L1370-1382 |
| 61 | Constructor signature: parameter count, names, types, value-type nullability, defaults | M→C | A2 | Structure: constructor parameters | C# | DD L1384-1393, `ParameterDifference` L1692-1725 |
| 62 | An unmodelled non-private constructor with parameters (`member.unmodelled`) | C→M | D | A2 only for public constructors in a type with a modelled constructor | EF (the parameterless constructor is exempt) | DD L1396-1404 |
| 63 | A modelled operation has a method of that name (`operation.missing`) | M→C | B | A member kind; CD treats missing as pending | No | DD `CheckOperations` L1413-1423 |
| 64 | Operation signature: static, return type (`Task`/`ValueTask` unwrapped), parameters | M→C | A2 | Structure: methods, with Async Configuration | C# async | DD L1432-1455, `ReturnDifference` L1727-1764 |
| 65 | An unmodelled public, internal or protected method that isn't an override or operator (`member.unmodelled`) | C→M | D | A2 for public only | No | DD L1458-1466 |
| 66 | An unmodelled non-private instance property, with an association hint when it targets a modelled Class (`member.unmodelled`) | C→M | D | A2 for public only | No | DD L835-853 |
| 67 | A C# attribute that is neither in the checked list nor the ignored list gets a warning | C→M | D | None | DataAnnotations, System.Text.Json | DD `ReportUnknownAttributes` L1766-1777 |
| 68 | Members the designer can't model get warnings: Value Object constructors, repository interface properties, public methods on events, value conversion on a non-enum | C→M | D | None: CD doesn't check what an element can't state | EF Core | DD L1354-1367, L1514-1520, L565-570, L1185-1188 |
| 69 | Repository `X` has an interface `IX` under Sources (`repository.missing`) | M→C | B | New kind (CD's RepositoryContract is the concrete class, with no interface) | The `I` prefix convention | DD `CheckRepository` L1473-1482 |
| 70 | The repository interface's file mirrors the model folder (`repository.location`) | M→C | B | As #14 | No | DD L1484-1489 |
| 71 | The Repository has a type, and `IX` derives `IRepository<T>` of it (`repository.entity`) | both | B | New kind `repository.entity` | SeedWork | DD L1491-1502 |
| 72 | A class `X : IX` exists somewhere under Sources (`repository.implementation`) | M→C | B | A type kind; CD treats missing as pending | No | DD L1504-1510 |
| 73 | Repository Operations ↔ `IX`'s methods, both ways: missing, signature, unmodelled | both | B | A2 compares only the concrete class, so the interface needs kinds | C# async | DD L1512-1513 → `CheckOperations(isRepository)` |

#### D items explained
- **Adoption scope (#1-4, plus the gate on every C→M row).**
  - CD has no concept of an adopted subset that grows chunk by chunk:
    - modelled facts are always checked;
    - `own` conventions report unmodelled code everywhere their `files` reach;
    - exceptions are permanent opt-outs, not a progress ledger.
  - The module's split has no CD equivalent:
    - "modelled elements are checked everywhere, Backlog only in Adopted files";
    - Strict ("every file under Sources is adopted or excluded");
    - the Not-yet-adopted counts;
    - stale Adopted entries as errors.
  - `files` globs plus exceptions only approximate Scoped.
- **Failing on model features it can't compare (#6).** CD's vocabulary is closed per designer and reads only stock stereotypes. A stereotype outside its kinds is ignored silently, and only "a kind has no recipe" warns. This module fails the run on any model feature it doesn't compare. That would be an engine policy change (report stereotypes outside a declared vocabulary), not a fact kind.
- **Coverage-disclosure warnings and a code lint (#11, 23, 50, 53, 67, 68).**
  - CD's rule is that what can't be modelled isn't checked, and nothing should need configuring around it. The user also rejected a per-run coverage summary.
  - This module warns about every EF call, attribute, member and DbContext statement it doesn't read.
  - #50 (a no-op `HasMaxLength`) is a code-only lint with no model side at all.
- **Code→model interaction gaps (#31).** By design, CD's `intent.*` facts check only what the model shows: a raise the code has and the model doesn't link is never reported. Here it is Backlog and fails the run.
- **Non-public unmodelled members (#62, 65, 66).**
  - CD's decision of 2026-10-28: only unmodelled public members are reported, and constructors only in types with a modelled constructor. Modelling a non-public member is a judgement call.
  - This module reports internal and protected methods, all non-private properties, and every non-private constructor with parameters.
- **Not counted as D, but a design conflict for describe-only:** CD's structure rule that a modelled type or member missing from the code is "pending, the Software Factory will write it". For #12, 28, 33, 60, 63, 69 and 72, describe-only needs the opposite. That is why they are B (facts with no `absent` are reported as can't-be-verified), not A2.

#### Stack knowledge baked into the engine
To reuse the module on another codebase, all of the following would have to become data or a plug-in:
- **SeedWork names:**
  - the constants `Entity`, `ValueObject`, `IAggregateRoot`, `IRepository`, `INotification` and `DbContext` (DD L21-30);
  - the set `SeedWorkTypes` (+`IUnitOfWork`, `Enumeration`) (L33-36);
  - the method `AddDomainEvent(INotification)`, and the member `Entity.Id`, whose type stands in for every key (L165-170).
- **Recognising a raise:** `new X(...)` inside a member that calls `AddDomainEvent`, followed into same-class private methods (CR L505-531, DD L582-611).
- **C# type tables:**
  - `PrimitiveToCSharp` (for example `date` maps to `DateOnly` or `DateTime`, `binary` to `byte[]`);
  - `ValueTypeNames`, `CollectionTypeNames` and `TaskTypeNames` (DD L38-71);
  - `ClrToKeyword` (CR L23-36);
  - aliases and qualified names reduced to their last segment.
- **EF Core vocabulary in `EntityFacts.Interpret` (L1831-1983):**
  - configuration calls: `ToTable`, `Ignore`, `Property` / `Property<T>`, `UseHiLo`, `HasColumnType`, `HasColumnName`, `HasMaxLength`, `IsRequired`, `HasConversion<string>`, `ValueGeneratedNever`;
  - relationships and indexes: `OwnsOne`/`OwnsMany`, `HasOne`/`HasMany`, `WithOne`/`WithMany`, `HasForeignKey`, `OnDelete(DeleteBehavior.X)`, `HasIndex`, `IsUnique`;
  - context and configuration plumbing: `IEntityTypeConfiguration<T>.Configure`, `OnModelCreating`, `HasDefaultSchema`, `ApplyConfiguration(new X())`, `ApplyConfigurationsFromAssembly`, `DbSet<T>`, inline `modelBuilder.Entity<T>(...)` (CR L254-349).
- **EF Core semantics:**
  - the conventional table name (the DbSet property name, else the class name);
  - private fields aren't mapped by convention;
  - max length applies only to strings and binary data;
  - with nullable reference types off, a reference type is nullable unless `[Required]` or `.IsRequired()`;
  - the nullable setting comes from csproj `<Nullable>` and `#nullable` directives;
  - the parameterless constructor is exempt (EF needs it).
- **Attribute lists:** checked `Required`, `MaxLength`, `StringLength`; ignored `Json*`, `DataMember`, `Obsolete`, … (DD L74-81).
- **House conventions:**
  - files at `Sources[0]/<model folders>/<Name>.cs`;
  - a Repository `X` means an interface `IX : IRepository<T>` plus a class `X : IX`;
  - an operation's `Task`/`ValueTask` return is unwrapped;
  - overrides and operators are never modelled;
  - a Domain Event has public properties only.
- **Model side:**
  - typed APIs with compile-time dependencies on Intent.Metadata.RDBMS, Domain, Domain.Events, Repositories and ValueObjects (CD reads stereotypes by name);
  - hard-coded lists `CheckedAttributeStereotypes` and `CheckedClassStereotypes` (MR L27-32);
  - Enum Storage values `String`/`Number`; On Delete values are EF's `DeleteBehavior` names.
- **Reporter policy:** Backlog and model-side Unsupported are errors; code-side Unsupported and Not yet adopted are warnings (Rep L112-119). Only `.cs` and `.csproj` are read, and `bin`/`obj` are skipped.

#### CD capabilities this module lacks, or has an equivalent for
**Lacks:**
- A recipe probe, ValidateDriftRecipes, and the wrong-element check. Its reads are compiled code keyed by member name, so it has no recipes to misread, but a new code style means changing the engine.
- Generated AI guidance: the always-on rule, and the `sync-model`, `configure-drift-check` and `validate-drift-recipes` skills, with their technology guides. It has designer AI rules instead, and every finding's message states the fix ("Set the Table stereotype's Name to `X`").
- Type or `Type.Member` exceptions. It has only file-level Exclude globs, which drop files from reading altogether.
- Reading non-C# files.

**Has an equivalent:**
- `unreadable` / can't-be-verified ≈ Unsupported (#32, 15, 53). It is weaker: code-side Unsupported only warns, whereas CD fails the run on a fact it can't verify.
- Element-anchored findings (ids mapped to `IElement`).
- CI handling, through the same Intent.Common.DriftReporting hub.

**Not applicable:**
- The model-first baseline and the "accepted view" of pending changes, because nothing is ever generated.

**Has, and CD doesn't:**
- The adoption scope (Scoped/Strict, progress counts).
- Failing on model features it can't compare.
- Live designer validation for its own stereotypes (GUARDRAILS §1: HiLo only on a PK, On Delete only with References, …).
- Fix-prescribing messages.

**Testing:** 100 unit tests, including real-code tests over every Ordering and Webhooks file (`RealCodeTests`), plus a manual "Test it yourself" table of breaks in GUARDRAILS. CD has adversarial re-test rounds, golden renders and guide tests. This module has no adversarial suite, and 14 rule ids have no test that mentions them.

---

# eShopCustom: Services module

#### Summary
**Files read:**
- The engine `eShopCustom.Modules.Services.CSharp\Drift\`: `ServicesDrift.cs`, `CSharpReader.cs`, `ProtoReader.cs`, `ServicesModelReader.cs`, `DriftModel.cs`, `CodeModel.cs`, `AdoptionScope.cs`.
- `FactoryExtensions\ServicesDriftReporter.cs`.
- The module's own stereotypes in `Api\*StereotypeExtensions.cs`, by grep.
- `D:\Dev\Temp\eShopCustom\GUARDRAILS.mdx` and `D:\Dev\Temp\eShopCustom\brownfield\PROGRESS.md`.
- For CD: `Intent.Modules.Common.Drift\docs\README.md` and `CONTEXT.md`, plus `Intent.Modules.Services.Guardrails.Contracts\docs\README.md` and `Drift\ServicesDrift.cs`, which defines the 14 kinds.

**Not read:**
- The test project (282 tests per PROGRESS). I listed it but didn't open it.
- The module's designer configuration. Rows 85–87 come from GUARDRAILS §1 instead.

**Total checks: 87.** A1 20, A2 9, B 23, C 24, D 11.

**Takeaways:**
- Only about a quarter of the module works today on CD's describe-only path (A1). That covers HTTP verb, route, prefix and security, binding source, and the forward direction of invokes and publishes.
- Almost every field, enum and method-signature check (A2) relies on CD's merging contract templates. Describe-only needs CD to compare structure without merging; otherwise those checks fall to C.
- All 23 B rows need new kinds on the existing Services designer, so its vocabulary must be opened. No new designer is needed. About half also need CD's model reader to read the module's own stereotypes (Identified Command, Validated, Query String Api Version, Application Service, gRPC Service, Proto Message), which CD's stock-only reader can't.
- The C rows are where the module is really a type checker: type and nullability meaning, cross-file and cross-application resolution, the .proto grammar, the exposed-type graph and binding inference. Recipes of find plus regex can't do these.
- The D rows are adoption scope, reporting what the code does but the model lacks, and listing what can't be modelled on both sides. These are deliberate design choices the module makes and CD rejects.

#### Checks
| # | Check | Dir (M→C, C→M, both, scope, meta) | Cat | CD equivalent, or proposed fact kind / plug-in | Stack-specific? (what) | Where (method or line) |
|---|---|---|---|---|---|---|
| 1 | The package has a Drift Scope with Sources (`services.scope.missing`/`.sources`) | scope | D | No adoption scope. The check stays off until the recipe file exists, and that's only a warning | No | Execute L289-303 |
| 2 | Each Source holds files (`services.scope.source-empty`) | scope | D | A recipe `files` glob that matches nothing isn't reported | No | Execute L306 |
| 3 | Each Adopted entry matches something (`services.scope.stale`) | scope | D | Closest is the "exception matches nothing" warning | No | Execute L313; AdoptionScope ctor |
| 4 | Strict: every file under Sources that isn't adopted or excluded is a Backlog (`services.scope.strict`) | scope | D | `own` conventions only find code shaped like the convention; no list of all files | No | ReportScope L4083 |
| 5 | Scoped: a not-yet-adopted summary, plus exposed types in files not adopted (`services.scope.not-yet-adopted`, `services.exposure.not-adopted`) | scope | D | None; CD has no NotYetAdopted category | No | ReportScope L4097; CheckExposure L3860 |
| 6 | Every C→M rule fires only for adopted files | scope | D | `own` and structure checks apply everywhere; exceptions are per recipe | No | `adopted` guards throughout |
| 7 | A file can't be read, or Roslyn fails to load (`services.read`) | meta | A1 | A check failure or recipe problem is logged as a Code error | .NET (bundles Roslyn) | ServicesDriftReporter L50-62 |
| 8 | Model content the check can't compare is a model-side Unsupported error (unknown stereotypes or children, mappings, field defaults, generic ops, Return Type Mediatype, several or generic Generalizations, deprecated Version, Header/Query name with another Source, Perform Invocation mappings or targets, router stereotypes on other Service kinds, several kind stereotypes, Serialization Settings on a Proto Message) | meta | D | CD silently ignores model content outside its vocabulary; closest is the "kind with no recipe" warning | Partly (stereotype names) | ModelReader.Unchecked; NotComparedOn L2500-2551; CheckService L425 |
| 9 | Code the model can't express is listed as code-side Unsupported warnings (OpenAPI calls and attributes, non-binding attributes, extra group or endpoint calls, NewVersionedApi, calls before MapGroup, mapping methods never called or duplicated, mixed prefixes or authorization, DTO/Command constructors, methods, fields, statics and attributes, error or several success results, OPTIONS/HEAD/multi-verb, stray Map*, JSON options, exposed interfaces or external types, JsonSerializerContext, a catch-all for other types, interface properties, proto constructs it doesn't read, streaming, unsigned/well-known/enum proto types, a second command handler) | code | D | Conflicts with "if it can't be modelled, it isn't checked" | Yes (Minimal APIs, OpenAPI, STJ, proto3) | Code-side Unsupported calls; CheckUnmodelledTypes L3998; CheckOtherCode L4065; CheckProtoFiles L3393 |
| 10 | A fact written in a form the reader can't read gives a warning and turns off Wrong and both-ways checks (non-literal prefix or route, lambda or non-method handler, Send/publish argument it can't resolve, RequireAuthorization or HasApiVersion argument it can't read, unknown reader, untyped holder path, lambda returning `var`) | both | A1 | `unreadable` read case gives "can't be verified" (an error in CD, a warning here) | Yes (C# forms) | CheckServiceRoute L601; CheckOperations L642; CheckInvocations L2272; ToRequirement L1239; Versions L1338; CheckReturn L898 |
| 11 | Each modelled element has a code type of the right kind (`*.missing`, `services.service.kind`, `services.dto.kind`): Service is a static router class; DTO, Command and Eventing DTO are class/record/struct; Enum is an enum; Message subclasses IntegrationEvent; handlers implement the handler interface; App Service is I<Name> or the class; Hosted and gRPC are classes; Proto Message is a .proto message | M→C | B | New existence kind per element on Services (vocabulary opened): recipe find plus `absent`. Today a missing type counts as pending (model-first) and only shows as "can't be verified" on its other facts | Yes (type shapes) | CheckService L455; CheckDto L1527; CheckCommand L3471; CheckEnum L3655; CheckMessage L1785; HandlerClass L1828; CheckApplicationService L2572; CheckHostedService L2825; CheckGrpcService L2970; CheckProtoMessage L3272 |
| 12 | The type's file is in the folder that mirrors its model folders under Sources[0] (every `*.location`) | both | B | New `element.folder` kind; needs a full model-folder-path placeholder, since CD's `{Folder}` is only the last folder | No (a convention) | DirectoryOf calls in each Check* |
| 13 | Service Route = MapGroup prefix; a builder router has no Route; an unmodelled prefix is a Backlog (`services.service.route`) | both | A1 | `service.route` with `absent`. Not covered: the Backlog for a Service without Http Service Settings (B) | Yes (MapGroup) | CheckServiceRoute L586 |
| 14 | Calls chained on mapping-method calls in other files (Program.cs `.RequireAuthorization()`) apply to the group returned | mechanism | C | Plug-in: resolves call sites across files | Yes (Minimal APIs) | CheckService L521-549; CSharpReader.MappingCall |
| 15 | Secured↔RequireAuthorization, Unsecured↔AllowAnonymous, roles and policies including `new AuthorizeAttribute{Roles,Policy}`, on groups and endpoints, both ways (`services.security.secured`/`.requirement`/`.unsecured`) | both | A1 | `security`, `security.roles`, `security.policy` (inherited) | Yes (ASP.NET authorization) | CheckSecurity L1267; ToRequirement L1211; CheckOperation L815 |
| 16 | gRPC class and override security from [Authorize]/[AllowAnonymous], plus calls chained on MapGrpcService<X>() | both | B | Extend `security*` past Http Settings endpoints; the chained-call part is C | Yes (gRPC, ASP.NET authorization) | CheckGrpcService L3009-3022; CheckGrpcOperation L3148 |
| 17 | API versions: Query String Api Version Versions ↔ HasApiVersion on the group and MapToApiVersion/HasApiVersion on the endpoint; versions are the union over groups; an endpoint inherits its group's; a narrower group is a Backlog; 1 = v1 = 1.0 (`services.version`) | both | B | New `service.versions` and `operation.versions` kinds, read from a module stereotype (not stock); union, inheritance and normalization are partly C | Yes (Asp.Versioning) | CheckVersions L1354; CheckOperation L817-848; Versions L1319 |
| 18 | Versioning scheme ↔ AddApiVersioning's ApiVersionReader: the query reader's parameter ↔ Parameter; URL segment, or the default reader plus `{x:apiVersion}`; Api Version Settings requires URL segment (`services.version.scheme`) | both | C | Plug-in: one global setting across files, inferred per router from the route | Yes (Asp.Versioning) | ResolveScheme L1405; SchemeFor L1441; CheckScheme L1473; CSharpReader.Versioning L1014 |
| 19 | Operation ↔ endpoint, by handler name, `.WithName` lambda, or an unnamed lambda by verb and route (`services.operation.missing`) | M→C | A1 | Recipe find by `{Operation}`; a missing endpoint makes its facts "can't be verified" | Yes (Minimal APIs) | CheckOperations L700-733 |
| 20 | An Operation without Http Settings that the code maps (`services.operation.http`) | both | A1 | An `own` endpoint convention reports the Map call as not modelled | Yes | CheckOperation L777 |
| 21 | Verb ↔ MapGet etc., or MapMethods with one verb (`services.operation.verb`) | both | A1 | `operation.verb` | Yes (Map*) | CheckOperation L785 |
| 22 | Route compared segment by segment: constraints dropped, parameters by position, `?` kept (`services.operation.route`) | both | A1 | `operation.route` with replace transforms | Yes (route template syntax) | CheckOperation L792; SameRoute L4289 |
| 23 | Endpoint return ↔ the Operation's type: Task unwrapped; the success in Results<>, Ok<T>, Created<T> or plain T; file results are Stream; collections, generics and nullability compared (`services.operation.return`) | both | C | Plug-in: type meaning. CD has no return kind, and structure comparison would compare `Results<…>` literally | Yes (TypedResults) | CheckReturn L896; AnalyseReturn L4216 |
| 24 | Success Response Code ↔ the success result: 200, 201, 202 or 204 (`services.operation.status`) | both | B | New `operation.status` kind, read from stock Http Settings | Yes (TypedResults) | CheckReturn L953 |
| 25 | An endpoint or router that isn't modelled (`services.operation.unmodelled`, `services.service.unmodelled`) | C→M | A1 | `own` conventions; limiting it to adopted files is D | Yes | CheckOperations L740-753; CheckUnmodelledTypes L3880 |
| 26 | A modelled Parameter exists on the handler (`services.parameter.missing`) | M→C | A1 | `parameter.source` can't be verified. Not covered: parameters without Parameter Settings (B) | No | CheckParameters L988 |
| 27 | A modelled Parameter is injected, not bound from the request (`services.parameter.injected`) | M→C | C | Plug-in: works out injected types (interfaces, DbContext, App Service classes, registrations, holders) | Yes (DI conventions) | CheckParameters L997; Bind L1073 |
| 28 | Binding source: attribute, then route template, then simple type = Query and complex = Body; the model's Default is inferred the same way (`services.parameter.source`) | both | A1 | `parameter.source` with `in`; inferring Default is C | Yes (Minimal API binding) | Bind L1073; InferModelSource L1155 |
| 29 | Bound name: [From*(Name=)] ↔ Header Name, Query String Name or the Parameter name (`services.parameter.binding-name`) | both | B | New `parameter.name` kind, read from stock Parameter Settings | Yes | CheckParameters L1013-1033 |
| 30 | Parameter type and nullability (`services.parameter.type`/`.nullability`) | both | C | Plug-in: type meaning plus the nullable context | Yes (C# types, NRT) | CheckParameters L1035-1046 |
| 31 | Parameter default value (`services.parameter.default`) | both | B | New `parameter.default` kind | No | CheckParameters L1048 |
| 32 | A request-bound handler parameter that isn't modelled (`services.parameter.unmodelled`) | C→M | C | Plug-in: has to exclude injected parameters | Yes | CheckParameters L1061 |
| 33 | A Perform Invocation to a Command is Wrong if the Command isn't sent (`services.operation.invocation`, `services.integration-event-handler.send`) | M→C | A1 | `intent.invokes` | Yes (MediatR) | CheckInvocations L2318; CheckHandlerSends L2035 |
| 34 | A Send with no Perform Invocation is a Backlog (same rules) | C→M | D | CD never reports interactions the model lacks | Yes (MediatR) | CheckInvocations L2290; CheckHandlerSends L2015 |
| 35 | Resolving Send and publish arguments: `new X`, IdentifiedCommand<X,R> unwrapped, locals with earlier assignments, casts, conditionals, parameters; delegation through methods of the same type | mechanism | C | Plug-in: CD reads only the handler's own text | Yes (C#) | CSharpReader.Request L520; Delegation L868; LocalCalls L577 |
| 36 | Sending an Identified Command bare is Wrong; a wrapped send of a non-Identified Command is a Backlog (`services.operation.identified`, `services.integration-event-handler.identified`) | both | C | Plug-in: compares how the send is written with the target Command's module stereotype | Yes (eShop idempotency) | CheckInvocations L2297-2310; CheckHandlerSends L2022 |
| 37 | A Perform Invocation to an App Service Operation is Wrong if it isn't called (`services.operation.call`, `services.integration-event-handler.call`) | M→C | A1 | `intent.invokes`; working out receiver types from parameters, holders and injected members is C | Yes | CheckServiceCalls L2408; CheckHandlerCalls L2078; ApplicationServiceFor L2421 |
| 38 | An App Service call with no Perform Invocation is a Backlog | C→M | D | CD never reports interactions the model lacks | No | CheckServiceCalls L2395; CheckHandlerCalls L2070 |
| 39 | A Publish Integration Event is published, from operations, Commands, DEH/IEH subscriptions, hosted ops and gRPC ops (`services.*.publish`) | M→C | A1 | `intent.publishes`; finding outbox methods across files is C | Yes (eShop EventBus and outbox) | CheckPublishes L2102; OutboxPublishMethods L243; Publishes L463 |
| 40 | A publish with no Publish Integration Event is a Backlog | C→M | D | CD never reports interactions the model lacks | Yes | CheckPublishes L2120 |
| 41 | A Command's interactions are those of its IRequestHandler<X,…>.Handle(X) | mechanism | A1 | Recipe find on the handler class | Yes (MediatR) | CheckCommand L3526 |
| 42 | DTO Abstract ↔ `abstract` (`services.dto.abstract`, eventing-dto) | both | A2 | Contract template modifiers | No | CheckDto L1555 |
| 43 | A DTO that implements IRequest must be a Command (`services.dto.command`) | C→M | B | New `dto.base-list` kind (IRequest `absent`) | Yes (MediatR) | CheckDto L1562 |
| 44 | Generalization ↔ a base class under Sources, Wrong or Backlog (`services.dto.base`) | both | A2 | Contract base type; the Backlog direction isn't covered (C) | No | CheckDtoBase L1668 |
| 45 | Generic parameters (`services.dto.generic`) | both | A2 | Contract generics | No | CheckDto L1580 |
| 46 | Fields ↔ properties or positional parameters: missing, type, collection, nullability (`*.field.missing`/`.type`/`.nullability`) | both | A2 | Contract structure; describe-only would be C (type meaning) | Yes (C# types, NRT) | CheckMembers L1608; CheckField L1701 |
| 47 | A public property that isn't modelled (`*.field.unmodelled`) | C→M | A2 | Structure comparison; an `own` property convention could also do it | No | CheckMembers L1660 |
| 48 | Wire name ↔ Serialization Settings (camelCase by default, [JsonPropertyName]); DTOs and Commands only (`*.field.wire-name`) | both | B | New `field.wire-name` kind from stock Serialization Settings, `{Field\|camel}` default | Yes (System.Text.Json) | CheckField L1735; ModelReader.WireName |
| 49 | Enum literals: name, value, unmodelled (`services.enum.literal.*`) | both | A2 | EnumContract | No | CheckEnum L3647 |
| 50 | A Command implements IRequest or IRequest<R> (`services.command.request`) | M→C | B | New `command.base-list` kind | Yes (MediatR) | CheckCommand L3496 |
| 51 | Command type ↔ R (Unit means none), with nullability (`services.command.return`) | both | C | Plug-in: type meaning | Yes (MediatR) | CheckCommandType L3540 |
| 52 | Exactly one IRequestHandler<X,R> with the same R; a second one is a warning (`services.command.handler`) | M→C | C | Plug-in: counts across files and compares types | Yes (MediatR) | CheckHandler L3564 |
| 53 | Identified Command ↔ an IdentifiedCommandHandler<X,R> subclass (`services.command.identified`) | both | B | New kind; model side from a module stereotype | Yes (eShop) | CheckIdentifiedAndValidated L3605 |
| 54 | An Identified Command's Validated ↔ AbstractValidator<IdentifiedCommand<X,R>> (`services.command.identified-validated`) | both | B | New kind, module stereotype | Yes (FluentValidation) | CheckIdentifiedAndValidated L3618 |
| 55 | Validated ↔ AbstractValidator<X> (`services.command.validated`) | both | B | New kind, module stereotype | Yes (FluentValidation) | CheckIdentifiedAndValidated L3631 |
| 56 | An IRequest type, or a handler or validator of a request, that isn't modelled (`services.command.unmodelled`, `.handler.unmodelled`) | C→M | A1 | `own` conventions | Yes (MediatR, FluentValidation) | CheckUnmodelledTypes L3971; CheckServesCommand L4009 |
| 57 | Every type exposed by endpoints, commands and messages, followed transitively, is modelled (`services.exposure.unmodelled`) | C→M | C | Plug-in: walks the type graph | Yes (C# types) | ExposeEndpoint L3729; FollowExposed L3817; CheckExposure L3829 |
| 58 | A Message derives IntegrationEvent (`services.message.base`) | M→C | B | New `message.base` kind | Yes (eShop EventBus) | CheckMessage L1786 |
| 59 | An IntegrationEvent subclass that isn't modelled (`services.message.unmodelled`) | C→M | A1 | `own` | Yes | CheckUnmodelledTypes L3957 |
| 60 | Domain Event Handler Association ↔ INotificationHandler<E>, across partial parts (`services.domain-event-handler.event`) | both | B | New `subscription.event` kind; CD reads only the interactions from a subscription, not the subscription itself | Yes (MediatR) | CheckDomainEventHandler L1869-1898 |
| 61 | Subscribe Integration Event ↔ IIntegrationEventHandler<M> (`services.integration-event-handler.message`) | both | B | New `subscription.message` kind | Yes (eShop EventBus) | CheckIntegrationEventHandler L1926-1964 |
| 62 | The subscription is wired by AddSubscription<M,H>() under Sources; wiring that isn't modelled is a Backlog (`services.integration-event-handler.subscription`) | both | B | New kind: a chain find over a files glob, plus `absent` | Yes (eShop EventBus) | CheckIntegrationEventHandler L1936; CheckCodeSubscriptions L2213 |
| 63 | A DEH or IEH class that isn't modelled (`*.unmodelled`) | C→M | A1 | `own` | Yes | UnmodelledHandler L4046 |
| 64 | Another application's Message: a local IntegrationEvent copy exists, its properties are a subset of the publisher's with the same types, and enum-as-string gives a warning (`services.message.copy`, `services.eventing-dto.copy`) | both | C | Plug-in: reads another application's model and compares as a subset | Yes (eShop EventBus, STJ enums) | CheckForeignCopies L2144; CheckCopy L2171 |
| 65 | App Service: a class implements I<Name> and is the Implementation; several is a warning (`services.appservice.implementation`) | M→C | B | New kind from the module stereotype (Implementation) | Yes (I<Name> convention) | CheckApplicationService L2600-2622 |
| 66 | With no interface, the Service describes the class's public instance methods across partial parts | mechanism | C | Plug-in | Yes (C# partials) | DescribedClass L2456; CheckClassApplicationService L2654 |
| 67 | Operation ↔ method: it exists; parameters by position (name, type, nullability, default; CancellationToken and ServerCallContext left out); overloads chosen by parameter names; Task<T> = T (`.operation.missing`/`.parameters`/`.return` for appservice, hosted-service and grpc) | both | A2 | ServiceContract (a concrete class, never the interface; the token comes from Async Configuration) | Yes (Task, CancellationToken, gRPC) | CheckServiceOperation L2705; ParameterDifference L2758; CheckMethodSignature L2901 |
| 68 | An interface method or public class method that isn't modelled (`services.appservice.operation.unmodelled`) | C→M | A2 | Structure comparison | No | CheckApplicationService L2635; CheckClassApplicationService L2693 |
| 69 | An App Service that isn't modelled: an I<X> with exactly one implementation, or a class registered as itself (`services.appservice.unmodelled`) | C→M | C | Plug-in: finds interface/implementation pairs and looks up registrations | Yes (MS DI) | CheckUnmodelledTypes L3933-3949, L3990; ServiceClassRegistration L2438 |
| 70 | A Hosted Service derives BackgroundService (`services.hosted-service.base`) | M→C | B | New `hosted.base` kind, module stereotype | Yes (.NET hosting) | CheckHostedService L2835 |
| 71 | It's registered with AddHostedService<X>() (`services.hosted-service.registration`) | M→C | B | New kind: find over a files glob, plus `absent` | Yes | CheckHostedService L2850 |
| 72 | Operation ↔ an override of ExecuteAsync, StartAsync or StopAsync (`services.hosted-service.operation.missing`) | M→C | A2 | Contract methods plus the override modifier | Yes | CheckHostedService L2864 |
| 73 | An override that isn't modelled (`services.hosted-service.operation.unmodelled`) | C→M | A1 | `own` method convention (the structure check skips protected members) | Yes | CheckHostedService L2891 |
| 74 | A BackgroundService that isn't modelled (`services.hosted-service.unmodelled`) | C→M | A1 | `own` | Yes | CheckUnmodelledTypes L3908 |
| 75 | The Proto File exists (relative to Sources[0]) and declares the Proto Service (`services.grpc.proto`) | M→C | B | New kind: regex over the .proto, module stereotype | Yes (proto3) | ProtoServiceOf L3063 |
| 76 | The gRPC class derives <ProtoService>Base (`services.grpc.base`) | M→C | B | New kind | Yes (Grpc.AspNetCore) | CheckGrpcService L2989 |
| 77 | It's mapped with MapGrpcService<X>() (`services.grpc.registration`) | M→C | B | New kind: find over a files glob, plus `absent` | Yes | CheckGrpcService L2998 |
| 78 | A gRPC Operation ↔ an rpc in the proto service, plus an override (`services.grpc.operation.missing`) | M→C | C | Plug-in: .proto block structure; the override part is A2 | Yes (proto3) | CheckGrpcOperation L3115-3131 |
| 79 | Its one Parameter is the request Proto Message DTO, and its type is the response (`services.grpc.operation.rpc`) | both | C | Plug-in: parses the proto and matches the typed element | Yes (proto3) | CheckRpc L3163 |
| 80 | An rpc, override, proto service, or `<X>Base`/MapGrpcService class that isn't modelled (`services.grpc.operation.unmodelled`, `services.grpc.unmodelled`) | C→M | C | Plug-in: parses the proto | Yes | CheckGrpcService L3032-3059; CheckProtoFiles L3416; CheckUnmodelledTypes L3917 |
| 81 | A Proto Message DTO isn't abstract, generic or generalized (`services.proto-message.shape`) | meta | C | Model lint; belongs in live validation | Yes | CheckProtoMessage L3258 |
| 82 | Proto Message ↔ a .proto `message` (`services.proto-message.missing`) | M→C | C | Plug-in; existence alone could be B (regex) | Yes (proto3) | CheckProtoMessage L3272 |
| 83 | Field ↔ proto field: name by protoc PascalCase, scalar type table, message type matched by element, repeated = collection (`.field.missing`/`.type`) | both | C | Plug-in: proto grammar plus a type table | Yes (proto3, protoc) | CheckProtoField L3347 |
| 84 | A proto field or message that isn't modelled (`.field.unmodelled`, `services.proto-message.unmodelled`) | C→M | C | Plug-in | Yes | CheckProtoMessage L3327; CheckProtoFiles L3409 |
| 85 | PascalCase and valid identifiers on Service, Operation, DTO, DTO-Field and Command (base rules restated) | meta | C | Designer validation script, not drift | Partly (C# identifiers) | GUARDRAILS.mdx §1 (designer config not read) |
| 86 | An Operation has Http Settings with a Verb; a Route has no leading slash and no constraints | meta | C | Designer validation script | No | GUARDRAILS.mdx §1 |
| 87 | Query String Api Version isn't combined with Api Version Settings, and names its Versions | meta | C | Designer validation script | No | GUARDRAILS.mdx §1 |

#### D items explained
- **Adoption scope (rows 1–6).** The module brings code in step by step. Drift Scope (Mode Scoped or Strict, Sources, Adopted, Exclude) decides which files the C→M rules apply to. Files not yet adopted are reported as NotYetAdopted (a warning), and Strict turns every file not adopted into a Backlog. CD assumes the whole codebase is described from the start. Its `own` conventions and exceptions apply everywhere, and it has no idea of an adopted subset, a stale adoption, an empty source or a not-yet-adopted category. The closest CD feature is exceptions, which map only roughly to Exclude. Adding scope to CD would change its core model: it would need a new finding state and per-file gating in every C→M kind.
- **Reporting what the code does but the model lacks (rows 34, 38, 40).** The module reports a Send, App Service call or publish that has no Perform Invocation or Publish Integration Event as a Backlog. It types the receiver from syntax to tell real calls from incidental ones. CD deliberately never reports an interaction the model lacks, because a recipe can't tell incidental calls apart. Its always-on agent rule tells the AI to add them to the model instead. Matching the module here goes against that decision, not just the vocabulary.
- **Model-side Unsupported fails the run (row 8).** The module's rule is "never skipped silently": any model content it can't compare is an error, so the model can't claim more than the check verifies. CD's vocabulary is closed, so it ignores everything outside it. The only signal is the "kind with no recipe" warning.
- **Listing what the code does that can't be modelled (row 9).** The module lists OpenAPI metadata, extra attributes, constructors, JSON options, unread proto constructs and so on as code-side Unsupported warnings. That makes the gap between model and code visible. CD decided on 2026-10-23: "if it can't be modelled, it isn't checked; nothing should need configuring around it." The module's inventory is the opposite approach.

#### Stack knowledge baked into the engine
**Minimal APIs**
- MapGet, MapPost, MapPut, MapDelete, MapPatch, MapMethods (with `HttpMethods.X`), MapGroup and MapFallback.
- The router shape: a static method on a static class whose builder parameter it maps.
- `.WithName`; NewVersionedApi.
- The GroupCalls and EndpointCalls tables.
- Route constraint and optional syntax.
- The binding attributes (FromServices, FromKeyedServices, FromHeader, FromRoute, FromQuery, FromBody, FromForm with `Name=`) and AsParameters.
- The InjectedTypes and FormInterfaces tables; the `DbContext` suffix; the simple-versus-complex binding rule.

**Results**
- SuccessResults (Ok, Created, Accepted, NoContent), Results<…>, UntypedResults, and FileResults (which map to Stream).
- Parsing of Success Response Code.

**OpenAPI**
- The OpenApiCalls and OpenApiAttributes tables (inventoried, not compared).

**ASP.NET authorization**
- RequireAuthorization argument forms, `new AuthorizeAttribute{Roles,Policy}`, AllowAnonymous, and [Authorize]/[AllowAnonymous] mapped to calls.

**Asp.Versioning**
- AddApiVersioning, ApiVersionReader, QueryStringApiVersionReader (0 or 1 literal arguments), UrlSegmentApiVersionReader, the default `api-version`.
- HasApiVersion and MapToApiVersion argument forms (1.0, "1.0", (1,0), `new ApiVersion`); version normalization; `{x:apiVersion}`.

**System.Text.Json**
- camelCase by default, the snake/kebab/Pascal policies, [JsonPropertyName].
- DataContract and DataMember are ignored.
- ConfigureHttpJsonOptions, Configure<JsonOptions>, JsonSerializerContext.
- The JsonStringEnumConverter assumption for enum copies.

**MediatR and FluentValidation**
- IRequest, IRequest<R>, IBaseRequest, IRequestHandler, Handle, Send, Unit, INotificationHandler.
- AbstractValidator<X>.

**eShop-specific (not a framework)**
- IdentifiedCommand<X,R> and IdentifiedCommandHandler<X,R>.
- IntegrationEvent, IIntegrationEventHandler<M>, AddSubscription<M,H>(), IEventBus.PublishAsync, AddAndSaveEventAsync.
- The outbox regex `^I?[A-Z]\w*IntegrationEventService$`.
- IsImplied (IdentifiedCommand, IdentifiedCommandHandler, outbox).

**.NET hosting and DI**
- BackgroundService, AddHostedService, ExecuteAsync/StartAsync/StopAsync, AddSingleton/Scoped/Transient.
- The I<Name>↔<Name> pairing and the IsInterfaceName heuristic.

**gRPC and proto3**
- MapGrpcService<X>, `<Service>Base`, ServerCallContext.
- A hand-written proto3 grammar (package, service, rpc, stream, message, field labels). It lists enums, nested messages, oneof, map, extend and group as unread.
- The ProtoScalars table, `google.protobuf.*`, protoc PascalCase naming.

**C# type knowledge**
- The PrimitiveToCSharp, ValueTypeNames, SimpleTypeNames, FrameworkTypes and CollectionTypeNames tables, Dictionary types, TaskTypeNames (Task, ValueTask), ClrToKeyword.
- Nullable from the csproj `<Nullable>` and `#nullable`.
- Names reduced to their last segment, with aliases resolved.
- Matching across partial classes.

**Model-side constants**
- Hard-coded names for stock stereotypes and properties: Http Service Settings, Http Settings, Parameter Settings, Secured, Unsecured, Api Version Settings, Serialization Settings.
- Hard-coded association names: Perform Invocation, Publish Integration Event, Subscribe Integration Event, Domain Event Handler Association.
- Parameter Source value names.
- The module's own stereotypes: Drift Scope, Application Service, Hosted Service, gRPC Service, Query String Api Version, Proto Message, Identified Command, Validated.

**Conventions**
- Folders mirror the model under Sources[0].
- The reporter reads only `.cs`, `.proto` and `.csproj`, and skips bin and obj.

To reuse the engine on another codebase, at least these would have to become data or a plug-in: the eShop group, the MediatR names, the result and binding tables, and the versioning reader names.

#### CD capabilities this module lacks, or has an equivalent for
- **Recipe probe and ValidateDriftRecipes:** the module has neither, because nothing is configurable. Its stand-in is 282 xunit tests with fixtures, plus RealCodeTests and the "clean without the rule, fails with it" practice described in PROGRESS.
- **Wrong-element check:** the module lacks it. It needs it less, because it matches by name in a syntax index rather than through a configured find.
- **Unreadable / can't be verified:** the module has an equivalent that is weaker. A code-side Unsupported warning turns off Wrongs, but it is a warning, not an error.
- **Model-first baseline:** the module has no equivalent and takes the opposite stance. A modelled type that is missing from the code is Wrong, which is right for describe-only.
- **AI skills and guides** (sync-model, configure-drift-check, validate-drift-recipes, technology guides, the always-on rule): the module lacks them. It has designer AI rules and GUARDRAILS.mdx, and the hub's agent rule is disabled.
- **Adversarial (ClientA) and golden tests:** the module's equivalent is the GUARDRAILS "Test it yourself" table of about 49 breaks, each proven to bite, plus the unit tests.
- **Exceptions:** the module's equivalent is Drift Scope Exclude, which works at file level only.
- **`own` conventions:** the module's equivalent is hard-coded conventions per element kind.
- **Entity-action and Send Integration Command facts** (`intent.creates`/`updates`/`deletes`/`queries`/`sends`): the module has no equivalent rule.
- **Reusable engine across stacks and designers:** the module lacks it. It is single-stack and single-codebase, and all knowledge is in constants.
- **Things the module has that CD lacks:**
  - adoption scope (Scoped or Strict);
  - model-side Unsupported;
  - a full list of code it can't model;
  - live designer validation;
  - reading Messages from another application;
  - a real .proto parser;
  - typing of interaction receivers.
- **Shared by both:** element anchoring and the DriftReporting hub (Area "Services", Order 30). Neither enforces drift on CI: the module by decision, and CD only through `--param ensure-no-outstanding-changes`.

---

# eShopCustom: Front End (Blazor) and Architecture modules

### Front End (eShopCustom.Modules.FrontEnd.Blazor)

#### Summary
**Files read.** Base folder: `D:\Dev\Temp\eShopCustom\intent\modules\eShopCustom.Modules.FrontEnd.Blazor\`.
- Read in full: `Drift\FrontEndDrift.cs`, `CSharpReader.cs`, `RazorReader.cs`, `FrontEndModelReader.cs`, `DriftModel.cs`, `CodeModel.cs`, `AdoptionScope.cs`; `FactoryExtensions\FrontEndDriftReporter.cs`; the three `Api\*StereotypeExtensions.cs`; `CONTEXT.md`; `modelers\eShopCustom Front End Drift.designer.settings` (live validation and AI rules).
- Tests (`FrontEndDriftTests.cs`, `RealCodeTests.cs`): method names only.
- Context: `GUARDRAILS.mdx` and `brownfield\PROGRESS.md`.
- CD side: `Intent.Modules.Common.Drift` README and CONTEXT (partly), `Intent.Modules.UI.Guardrails.Contracts` README and CONTEXT, and a grep of `Drift\UserInterfaceDesigner1.cs`.

**Totals.** N = 74: A1 11, A2 0, B 30, C 16, D 17. Severity: Wrong, Backlog and model-side Unsupported are errors. Code-side Unsupported and Not-yet-adopted are warnings.

**Takeaways.**
- **What CD covers.** CD covers 11 of the 74 checks today: component kind, page route, security, the model→code presence of each call, navigation and child, and unmodelled components through `own`. Even there CD loses detail this module has: a set of several `@page` routes, URIs built through helpers, scoping to a Component Operation, and resolving the injected receiver of a call.
- **The B checks.** All 30 are **new kinds on the existing User Interface designer**. Today only Intent.UI.Guardrails.Contracts may contribute that designer. Its CONTEXT leaves out Properties, Component Operations, Event Emitters and Model Definitions on purpose, and its reader doesn't read Service Proxies at all. Either Intent adds these kinds, or the UI vocabulary must be opened to a second contributor. State Store and Backend Operation are this module's own stereotypes.
- **The C checks.** These 16 are the module's distinctive value: they prove a client method hits the backend endpoint the model names (route template, `api-version` from a registration in another file, headers, HTTP vs gRPC). They also compare types and signatures under the nullable context, and resolve same-named components and clients across packages by Razor and C# rules.
- **The D checks.** These 17 are policy, not missing vocabulary: the adoption scope lives in the model (Strict, Not-yet-adopted), code-only calls, navigations and renders fail the run, and code the model can't state always gets a warning.
- **Where CD is ahead.** CD covers Calls to Commands and Queries (this module lists them as not checked), dialogs (`intent.opens`), the children of a Layout, and any UI stack.

**Assumptions.**
- Live designer validation is classed C: it is outside CD's scope, not a drift check.
- CD's `intent.invokes` reads a Call to a Service Proxy or State Store Operation, with `{InvokedOwner}` as the parent's name. I checked this in `UserInterfaceDesigner1.cs` (lines 84 and 107-110).

#### Checks
Line numbers are in `FrontEndDrift.cs` unless another file is named.

| # | Check | Dir | Cat | CD equivalent, or proposed fact kind / plug-in | Stack-specific? (what) | Where |
|---|---|---|---|---|---|---|
| 1 | Package has a Drift Scope with Sources (`scope.missing`, `scope.sources`) | scope | D | None: CD scopes by the recipe's `files` globs, and no recipe file just turns the check off | No | `Execute` L145-159 |
| 2 | Each Source holds .razor/.cs files (`scope.source-empty`) | scope | D | Nearest: ValidateDriftRecipes showing a recipe that reads nothing | No | L162-167, `AdoptionScope.EmptySources` |
| 3 | Each Adopted entry (glob, file, folder, type or component name) matches something (`scope.stale`) | scope | D | Nearest: CD's stale-exception warning | Type-name entries use the C# index | L169-174, `AdoptionScope` ctor |
| 4 | Strict: every file under Sources is adopted or excluded, else one Backlog per file (`scope.strict`) | scope | D | None (CD has exceptions, no Strict mode) | No | `ReportScope` L1811-1818 |
| 5 | Not-yet-adopted file count per package, and undescribed code outside Adopted downgraded to notes (`scope.not-yet-adopted`, `Undescribed`/`NotYet`) | scope | D | None (no adoption counts) | No | L1820-1825, L1834-1848 |
| 6 | Roslyn failing to load, or a file that can't be parsed, is an error (`frontend.read`) | meta | A1 | Engine: the check failing is a Code error on the recipe file | Roslyn | `FrontEndDriftReporter.Report` L55-67 |
| 7 | Any model feature the check can't compare is an error: other stereotypes or elements, Component View contents, association mappings, Navigation parameters, Calls to Commands/Queries, Api Version Settings, a mapping it can't follow, Query/Route/Bindable on a Model Definition property (`unsupported.model`) | meta | C | None: CD ignores model parts outside its vocabulary; this would be a lint in the reader. CD does compare Calls to Commands/Queries | No | `ReportUnchecked` L1800, `UnsupportedModel` L1850, `FrontEndModelReader` notes, L828-831, L957-960, L1170-1200 |
| 8 | Each Component, Page and Layout has `Name.razor` under Sources (`component.missing`) | M→C | A1 | `component.kind` ("is modelled, but not in the code") | `.razor` file naming | `CheckComponent` L214-223 |
| 9 | The model folder mirrors the file's folder under Sources[0] (`component.folder`) | M→C | B | Partly A1 through `{Folder}` in the find; the full path needs a `{FolderPath}` placeholder or a `component.folder` kind (UI designer) | No | L231-236 |
| 10 | Layout if and only if it derives `LayoutComponentBase` (`@inherits` or code-behind base) (`frontend.layout`) | both | A1 | `component.kind` = Layout | Blazor `LayoutComponentBase` | L238-249 |
| 11 | Page stereotype if and only if `@page` (`page.missing`, `page.unmodelled`) | both | A1 | `component.kind` = Page | Blazor `@page` | `CheckPage` L275-286 |
| 12 | Route = the set of `@page` routes as written; route-parameter names compared case-insensitively (`page.route`) | both | A1 | `page.route` (without the leading `/`); several directives and case-insensitive names may need a list read or a `lower` filter | Blazor routing | L287-297, `RouteKey` L269 |
| 13 | Page Title = the literal `<PageTitle>`; a stated Title against a computed one is Wrong (`page.title`) | both | B | New kind `page.title` (UI designer): regex on `<PageTitle>`, an `@` expression read as `unreadable` | Blazor `<PageTitle>` | L299-338 |
| 14 | Secured if and only if `[Authorize]` (`@attribute` or code-behind); Roles set and Policy set match (`frontend.security`) | both | A1 | `security`, `security.roles`, `security.policy` | ASP.NET `[Authorize]` | `CheckSecurity` L341-383 |
| 15 | An adopted .razor with no Component/Layout/Page is Backlog; the fix names the kind and Route (`component.unmodelled`) | C→M | A1 | `own` entries (the Blazor guide's `page`/`component`); gating by Adopted is D | Blazor | `CheckAdoptedCode` L1419-1436 |
| 16 | Each Property is a `[Parameter]`/`[SupplyParameterFromQuery]`/`[CascadingParameter]` property; a form-bound or plain one is Wrong (`property.missing`) | M→C | B | New kind `parameter` (UI designer; CD left Properties out by decision) | Blazor parameter attributes | `CheckProperties` L490-505 |
| 17 | Property type, collection and nullability, with the nullable context from `<Nullable>`/`#nullable` (`property.type`) | M→C | C | A comparer that normalises types (primitive map, `List<T>`/`T[]`, nullable context); the raw type text alone would be B | C# types, `.csproj` | L507-512, `TypeMatches` L1728, `Nullability` L1723 |
| 18 | Query Parameter if and only if `[SupplyParameterFromQuery]` (`property.query`) | both | B | New kind `parameter.query` (presence, `default: false`) | Blazor | L514-522 |
| 19 | Route Parameter if and only if a `{name}` in a `@page` route, case-insensitive (`property.route`) | both | B | New kind `parameter.route` (regex over the component read as one place) | Blazor routing | L530-538 |
| 20 | Bindable if and only if `[Parameter] EventCallback<T> NameChanged` (`property.bindable`) | both | B | New kind `parameter.bindable` (regex on `{Property}Changed`) | Blazor `@bind-` convention | L540-548 |
| 21 | A code `[Parameter]` with no Property is Backlog or a note; the fix names Query/Route/Bindable (`property.unmodelled`) | C→M | B | `own` entry on `parameter` | Blazor | L567-588 |
| 22 | Each Event Emitter is a `[Parameter] EventCallback` of that name (`event.missing`) | M→C | B | New kind `event` (UI designer) | Blazor `EventCallback` | `CheckEventEmitters` L594-602 |
| 23 | Event Emitter type = the T of `EventCallback<T>` (`event.type`) | M→C | C | Type comparer as in #17 | Blazor | L604-610 |
| 24 | A code `EventCallback` parameter with no emitter is Backlog, except a Bindable's `XChanged` (`event.unmodelled`) | C→M | B | `own` on `event`; the Bindable exemption needs the model (partly C) | Blazor | L613-626 |
| 25 | Each Component Operation is a method of that name (`operation.missing`) | M→C | B | New kind `component.operation` (CD left these out by decision) | No | `CheckOperations` L631-641 |
| 26 | Component Operation signature: return type (`Task` unwrapped), parameter count, names, types, nullability (`operation.signature`) | M→C | C | Signature comparer. A2-style, but A2 needs a template that writes code, so it isn't describe-only | C# `Task`/`ValueTask` | L643-645, `CompareSignature` L1663 |
| 27 | Each Composition's target is rendered as `<Target>` (`composition.missing`) | M→C | A1 | `intent.uses` | Razor PascalCase tags | `CheckCompositions` L653-665 |
| 28 | A rendered, modelled component with no Composition is Backlog (`composition.unmodelled`) | C→M | D | CD never reports a child the code has and the model lacks | Razor | L680-690 |
| 29 | A rendered component that isn't modelled yet is a not-yet-adopted note (`composition.not-modelled`) | C→M | D | As #28, plus adoption state | Razor | L691-696 |
| 30 | A Composition points into the package of the component the tag resolves to: qualifier, `@namespace`, `_Imports.razor`, `<RootNamespace>`, `@using`, own package (`composition.package`) | both | C | Razor name-resolution plug-in; CD has no target-package placeholder | Razor namespace rules, `.csproj` | `CheckCompositionPackage` L705-718, `Rendered`/`Visible` L1606-1654, `CSharpReader.RazorNamespaces` |
| 31 | A Navigation must target a Page, one with a Route (`navigation.target`) | meta | C | Model-only rule; live validation fits better (in CD an empty `{Route}` only misreads) | No | `CheckNavigations` L760-766 |
| 32 | Each Navigation is an `href` (on `a`, `area` or a component) or a `NavigateTo` whose URI matches the target's `@page`; one from a Component Operation must be made in that method or the methods it calls (`navigation.missing`) | M→C | A1 | `intent.navigates`. Partly C (URI evaluator following constants and helpers in other types, route-template matching) and partly D (scoping to a Component Operation) | Razor `href`, `NavigationManager` | L768-779, `CodeLinks` L723, `RouteMatches` L81, `CSharpReader.Evaluator` |
| 33 | Code navigating to a modelled page with no Navigation is Backlog (`navigation.unmodelled`) | C→M | D | CD never reports it | Blazor | L782-798 |
| 34 | Code navigating to an unmodelled page is a note (`navigation.not-modelled`) | C→M | D | As #33 | Blazor | L799-804 |
| 35 | Each Call (to a Service Proxy or State Store Operation) is a call through an injected member of that type (by name, `I<Name>` or the State Store Interface), in its Component Operation or the methods that one calls; overloads told apart by argument count; calls in markup count (`call.missing`) | M→C | A1 | `intent.invokes`. Partly C (receiver resolution, argument count, delegation) and partly D (scoping to a Component Operation) | Blazor `@inject`/`[Inject]` | `CheckCalls` L825-849, `CheckInjections` L427, `Reach` L900 |
| 36 | A code call through a modelled proxy or store with no Call is Backlog (`call.unmodelled`) | C→M | D | CD never reports it | Blazor | L851-866 |
| 37 | A code call through an unmodelled client is a note (`call.not-modelled`) | C→M | D | As #36 | Blazor | L867-872 |
| 38 | A Call goes through the class of its target's package; the injected class is resolved by namespace, usings, then package (`call.package`) | both | C | C# name-resolution plug-in | C#/Razor | `CheckCallPackage` L882-897, `Injected` L1613 |
| 39 | Each Model Definition is a record, class or struct of that name, not an interface or enum (`definition.missing`) | M→C | B | New kind `definition` (CD's UI reader reads no Model Definitions) | C# | `CheckDefinition` L924-936 |
| 40 | Model folder mirrors the declaring file's folder (`definition.folder`) | M→C | B | `{FolderPath}` placeholder or a `definition.folder` kind | No | L939-944 |
| 41 | Generic type parameters match, in order (`definition.generic`) | M→C | B | New kind `definition.generic` (regex on `class X<...>`) | C# | L946-951 |
| 42 | Each Property is a public instance property or positional parameter (`definition.property.missing`) | M→C | B | New kind `definition.property` | C# records | L962-969 |
| 43 | Property type, collection and nullability (`definition.property.type`) | M→C | C | Type comparer (#17); A2 only through a template that writes code | C# | L971-976 |
| 44 | A public property with no Property is Backlog (`definition.property.unmodelled`) | C→M | B | `own` on `definition.property` | C# | L979-984 |
| 45 | Each Operation is a public method (`definition.operation.missing`) | M→C | B | New kind `definition.operation` | C# | L998-1008 |
| 46 | Operation signature (`definition.operation.signature`) | M→C | C | Signature comparer (#26) | C# | L1010-1011 |
| 47 | A public method with no Operation is Backlog (`definition.operation.unmodelled`) | C→M | B | `own` on `definition.operation` | C# | L1014-1019 |
| 48 | A State Store (module stereotype) implements its Interface (`state-store.interface`) | M→C | B | New kind `state-store.interface`, model side read from this module's stereotype | C# | L1021-1028 |
| 49 | An adopted record, or a class or struct with only properties, with no Model Definition is Backlog (`definition.unmodelled`) | C→M | B | `own` finding records and data classes; the "only properties" test is partly C | C# | `CheckAdoptedCode` L1473-1478, `IsDataType` L1502 |
| 50 | Each Service Proxy is a class of that name (`proxy.missing`) | M→C | B | New kind `proxy` (CD's UI reader doesn't read Service Proxies) | C# | `CheckProxy` L1069-1076 |
| 51 | Folder mirror (`proxy.folder`) | M→C | B | `{FolderPath}` placeholder | No | L1079-1084 |
| 52 | Each Operation is a public instance method (`proxy.operation.missing`) | M→C | B | New kind `proxy.operation` | C# | L1104-1111 |
| 53 | Operation signature; `CancellationToken` never modelled; overloads matched by parameter names, then count (`proxy.operation.signature`) | M→C | C | Signature comparer (#26) | C# | L1113-1115, `Overload` L1656 |
| 54 | A public method with no Operation (overload-aware) is Backlog; the fix names the request it sends (`proxy.operation.unmodelled`) | C→M | B | `own` on `proxy.operation`; counting overloads is partly C | C# | L1126-1136 |
| 55 | An adopted API client (an `HttpClient` member or a generated gRPC client member) with no Service Proxy is Backlog (`proxy.unmodelled`) | C→M | B | `own` finding a class with an `HttpClient` member; a gRPC client known from `AddGrpcClient<T>` in another file is partly C | HttpClient, gRPC | `CheckAdoptedCode` L1446-1459, `IsApiClient` L1507 |
| 56 | Each proxy Operation has a Backend Operation; the fix names the request it sends (`proxy.mapping`) | M→C | B | Part of a `proxy.request` kind: model `none` against the code's request | No | `CheckTarget` L1180-1189 |
| 57 | The Backend Operation belongs to the Service the proxy is mapped to (`proxy.service`) | meta | C | Model-only rule; live validation fits better | No | L1202-1207 |
| 58 | Transport matches: an HTTP client's target has Http Settings and isn't a gRPC rpc; a gRPC client's target is a gRPC Service Operation (`proxy.target`) | both | C | Classifying the client from its members and registrations, across model and code | HttpClient, gRPC | L1215-1223, `CheckRpc` L1285-1291 |
| 59 | The Operation's method, or the methods it calls, sends a request or makes an rpc call (`proxy.request`) | M→C | B | New kind `proxy.request` (find the request call in the method); following delegation is partly C | HttpClient, gRPC | L1225-1234, L1293-1300 |
| 60 | Request verb = the backend Http Settings Verb (`proxy.verb`) | M→C | B | New kind `proxy.verb`: fixed-value read cases (`GetFromJsonAsync` = GET, ...), model value from the linked backend element; an `HttpRequestMessage` local is partly C | HttpClient method names | L1241-1250, `CSharpReader.RequestVerbs` |
| 61 | Request path matches the Service route + Operation route segment by segment (holes against parameters, optional, catch-all); the path is built by the URI evaluator (`proxy.route`) | M→C | C | URI evaluator + route-template matcher | HTTP routing | L1254-1270, `RouteMatches` L81 |
| 62 | The `api-version` sent (in the URI query, else `.AddApiVersion(x)` on the `AddHttpClient<T>` registration in another file) is one of the backend's Query String Api Version Versions, under its Parameter; `2` = `2.0` (`proxy.version`) | M→C | C | Registration lookup across files + normalisation; a literal `api-version=` alone would be B | Asp.Versioning | `CheckVersion` L1363-1412, `CSharpReader.Registrations` |
| 63 | Headers the request message sets against the backend's From Header parameters: a required one not sent is Wrong, an extra one a warning (`proxy.header`) | both | C | Set comparison with a nullable exemption, tracking the request-message local | `HttpRequestMessage.Headers` | `CheckHeaders` L1328-1361 |
| 64 | The gRPC call is `<Rpc>Async` or `<Rpc>` of the Backend Operation (`proxy.rpc`) | M→C | B | New kind `proxy.rpc` (regex `\.{Rpc}(Async)?\(`); following delegation is partly C | gRPC codegen naming | L1307-1312 |
| 65 | The gRPC client's `.proto` service (`Basket.BasketClient`) = the gRPC Service's Proto Service (`proxy.grpc-service`) | M→C | B | New kind `proxy.grpc-service` (regex on the member type) | gRPC codegen naming | L1314-1320 |
| 66 | Warning: component constructs the model can't state (other directives, code-behind attributes, interfaces or base class, framework or library tags, `<PageTitle>` on a non-page or computed) | code | D | None: CD is silent about code that no fact or `own` names | Razor/Blazor | `CheckDirectives` L386, `CheckTags` L462, L302-329 |
| 67 | Warning: parameters the model can't state (`[SupplyParameterFromForm]`, `[CascadingParameter]`, `RenderFragment`, `[EditorRequired]`/`required`, a query name override) | code | D | As #66 | Blazor | L524-528, L550-578 |
| 68 | Warning: injections not compared (a service that isn't a proxy or store, with its calls listed; a proxy or store injected but never called) | code | D | As #66 | Blazor DI | `CheckInjections` L427-460 |
| 69 | Warning: what a Layout renders (the designer has no Composition from a Layout) | code | A1 | CD's `intent.uses` reads Display Components in a Layout's header, body, sider and footer, so CD covers it | Blazor | L667-678 |
| 70 | Warning: a navigation whose target can't be built, or that goes to a route no page under the Sources serves | code | D | CD has `unreadable` only for a modelled fact | Blazor | `CodeLinks` L735-745 |
| 71 | Warning: contract and client details not compared (property attributes and `required`, public or primary constructors, an interface missing an Operation, a State Store's calls through proxies, an `HttpClient` base address with a path) | code | D | As #66 | C#, HttpClient | L987-995, L1029-1060, L1094-1097, L1117-1121 |
| 72 | Warning: request readings that can't be verified (several requests or rpc calls, an unreadable verb, an absolute or computed URI, computed or unknown headers, a computed `api-version`) | code | B | With the proxy kinds: `unreadable` cases give "can't be verified" (an error in CD, a warning here); several matches give "narrow the find" | HttpClient, gRPC | L1236-1260, L1302-1305, L1330-1344, L1374-1378 |
| 73 | Warning: adopted code kinds the model can't state (a static helper, an enum, a `JsonSerializerContext`, any other service class with its proxy and store calls, an unmodelled interface) | code | D | In CD these files are simply not `own`ed, or are exceptions | C#, System.Text.Json | `CheckAdoptedCode` L1460-1496 |
| 74 | Live: Component, Layout, Model Definition and Service Proxy names are valid PascalCase C# identifiers; a Page Route starts with `/` | meta (live) | C | Outside CD (it has no designer validation). CD models routes *without* the leading `/` | C# identifiers | `modelers\eShopCustom Front End Drift.designer.settings`, `validateFunctionOverride` |

#### D items explained
- **#1–5, adoption scope.** The module holds Sources, Adopted, Exclude and Mode in a model stereotype. It grows coverage chunk by chunk with green runs, reports stale entries, Strict Backlog and Not-yet-adopted counts, and decides whether undescribed code is Backlog (error) or a note (warning). CD has no adoption state: recipe `files` globs plus exceptions decide what is read, and what is read is always checked.
- **#28, 29, 33, 34, 36, 37, code→model interactions.** CD's UI design says a call, navigation or child the code has and the model lacks is "never reported; model it when you add it". Here they fail the run: under Strict, every interaction must be modelled in both directions.
- **#66–68, 70, 71, 73, "the model can't state it" warnings.** Every CD finding is an error, and code that no fact or `own` names is silent (or listed as an exception). This module's rule that nothing goes unreported produces a non-failing inventory row for each such construct. CD has no row class for that.
- **Partly D in #32 and #35.** This module requires a Call or Navigation from a Component Operation to be made in that method or the methods it calls (the agreed modelling: "calls start at the method that makes them"). CD's UI decision is that where an interaction is modelled doesn't matter: anything inside a component counts as the component's.

#### Stack knowledge baked into the engine
- **Razor without a compiler.**
  - Directives: page, inject, inherits, layout, rendermode, attribute, implements, using, namespace, typeparam, preservewhitespace.
  - `@code`/`@functions` blocks are lexed as C#; Razor and HTML comments are blanked.
  - A PascalCase tag is a component, by its last name when qualified.
  - `href` counts on `a`, `area` and components, not on `link` or `base`. `<PageTitle>` is read.
  - Implicit and explicit `@` expressions in attribute values; `@@` and email addresses stay literal.
  - `_Imports.razor` is never a component; `.xaml` isn't read.
- **Razor namespaces.** `@namespace`, then the nearest `_Imports.razor` (its `@namespace`, and `@using`s from the folder up to the project), then the `.csproj` `<RootNamespace>` or file name, with folder names turned into identifiers.
- **Blazor.**
  - Attributes: `[Parameter]`, `[SupplyParameterFromQuery(Name)]`, `[CascadingParameter]`, `[SupplyParameterFromForm]`, `[EditorRequired]`, `[Inject]`, `[Authorize(Roles, Policy)]`/`[AllowAnonymous]`.
  - Types and conventions: `EventCallback<T>` and the bindable `XChanged`, `RenderFragment`, `LayoutComponentBase`/`ComponentBase`.
  - Navigation: `NavigationManager.NavigateTo`; `GetUriWithQueryParameter(s)` is the same page.
  - Route templates (`{x:int}`, `{x?}`, `{*rest}`), with route parameters bound case-insensitively.
  - `NavigationManager`, `ILogger` and `ILoggerFactory` are injections that need no model.
- **HttpClient.**
  - A verb table (`GetFromJsonAsync`, `PostAsJsonAsync`, ...).
  - `SendAsync(new HttpRequestMessage(HttpMethod.X, uri))`, directly or through a local, with `Method`/`RequestUri` initialisers.
  - Headers set with `Headers.Add`/`TryAddWithoutValidation`.
  - Registrations `AddHttpClient<T>` and `AddHttpClient<TI, T>` with `BaseAddress`.
  - Asp.Versioning: `.AddApiVersion(x)`, the default `api-version` query parameter, and `2` read as `2.0`.
- **gRPC.** `AddGrpcClient<T>`, the `X.XClient` naming of generated clients, `<Rpc>Async`/`<Rpc>`.
- **C# (Roslyn syntax only).**
  - Partial classes and `X.razor.cs` merged; using aliases resolved; CLR names mapped to keywords.
  - Primitive map; collection wrappers; `Task`/`ValueTask` unwrapped; `CancellationToken` dropped.
  - Nullable context from `.csproj` `<Nullable>` and `#nullable`.
  - Records, positional parameters and primary constructors; `JsonSerializerContext`.
- **URI evaluator.**
  - Literals, interpolation, `+`, `string.Concat`, `string.Empty`, `nameof`, `Trim*`, `new Uri`.
  - Locals assigned once, and fields, constants and properties that nothing reassigns.
  - Follows methods, including static helpers in other types, with parameters bound, up to depth 12.
- **Files read.** `.razor`, `.cs` and `.csproj`; `bin`, `obj` and `wwwroot` are skipped.
- **Intent side.**
  - Stock UI stereotype ids: Page, Route, Secured, Composable, Route Parameter, Query Parameter, Bindable.
  - Element type names, and the Layout part names (assumed, not verified).
  - Services stereotypes read by name: Http Settings, Http Service Settings, Query String Api Version, Api Version Settings, Parameter Settings/From Header, gRPC Service/Proto Service.
  - The module's Backend Operation, State Store and Drift Scope.

### Architecture (eShopCustom.Modules.Architecture)

#### Summary
**Files read.** Base folder: `D:\Dev\Temp\eShopCustom\intent\modules\eShopCustom.Modules.Architecture\`. All read in full: `Drift\ArchitectureDrift.cs`, `FactoryExtensions\ArchitectureDriftReporter.cs`, `Api\*.cs` (Layer, Layer Settings, May Depend On, Architecture Root Folder, Architecture Scope), `ApiMetadataProviderExtensions.cs`, `eShopCustom.Modules.Architecture.Tests\ArchitectureDriftTests.cs`, `CONTEXT.md`, `modelers\eShopCustom Architecture.designer.settings`.

**Totals.** N = 9: A1 1, A2 0, B 1, C 3, D 4.

**Takeaways.**
- CD has no architecture vocabulary at all. Contributing the Codebase Structure designer is already allowed, but only one of the nine checks (a Layer's Path holds files) fits "find, read with a regex, compare".
- The core rule (#8, imports vs May Depend On) needs namespace ownership across every file and longest-Path layer membership, so it is a plug-in. It also runs code→model, a direction CD reports only through `own`.
- Four checks are about the Architecture Scope (D), and two are model-only or live validation (C).
- The model (Layers, Paths, arrows) is language-neutral. Only `ImportReader` is C#-specific, so a plug-in with an import reader per language would generalise.

#### Checks
Line numbers are in `ArchitectureDrift.cs` unless another file is named.

| # | Check | Dir | Cat | CD equivalent, or proposed fact kind / plug-in | Stack-specific? (what) | Where |
|---|---|---|---|---|---|---|
| 1 | Roslyn failing to load is one error (`architecture.read`) | meta | A1 | Engine: the check failing is a Code error | Roslyn | `Build` L111-115 |
| 2 | Each Layer has a Path (`architecture.layer.path`; live validation states it too) | meta | C | Model-only rule; live validation already enforces it | No | L130-135; Layer `validateFunction` |
| 3 | A Layer's Path holds at least one C# file (`architecture.layer.empty`) | M→C | B | **New designer contribution** (Codebase Structure): a kind such as `layer` with find `file:{Path}/**/*.cs`, reported as "modelled, but not in the code" | `.cs` | L136-141 |
| 4 | A Layer's Path is inside the Architecture Scope's Sources (`architecture.layer.outside-scope`) | scope | D | No adoption scope in CD | No | L143-148 |
| 5 | The root folder has an Architecture Scope, else a warning (`architecture.scope.missing`) | scope | D | None | No | L151-155 |
| 6 | Each Scope Source holds C# files (`architecture.scope.source-empty`) | scope | D | Nearest: recipe validation showing a find with no hits | `.cs` | L158-166 |
| 7 | Files under Sources that no Layer covers: one warning per Source, with a count (`architecture.file.unlayered`) | scope / C→M | D | Nearest: an `own` convention, which would make each file an error, with no summary | No | L168-173 |
| 8 | A `using`, `global using`, `using static` or alias in a Layer's file that names a namespace only other Layers' files declare needs a May Depend On arrow to one of them; the longest Path wins; namespaces no Layer declares are ignored (`architecture.import`) | C→M | C | Import-graph plug-in (namespace ownership across files, layer membership), on a new Codebase Structure contribution | C# namespaces and usings | L177-225, `Owners` L231-241, `ImportReader` L54-96 |
| 9 | Live: Layer Path is relative, uses forward slashes, has no leading or trailing slash and no `..`, and is unique among Layers; no May Depend On to itself | meta (live) | C | Outside CD | No | `modelers\eShopCustom Architecture.designer.settings`, Layer `validateFunction` |

#### D items explained
- **#4–7, Architecture Scope.** The application's owned folders, and their excludes, are a stereotype on the Codebase Structure root folder. They decide which files are read, whether a Layer sits outside them, and how many files are reported as in no Layer. CD has no adoption scope and no count-summary row; it reads its recipe globs and reports each item.

#### Stack knowledge baked into the engine
- Only `.cs` files are read, with Roslyn syntax trees.
- Block and file-scoped namespaces, with nested ones joined.
- `UsingDirectiveSyntax`: plain, `global`, `static` and alias forms, with `global::` stripped.
- `using static` and aliases fall back one segment to the type's namespace.
- Fully qualified names used inline, and implicit access to a parent namespace, aren't seen.
- Default excludes are `**/bin/**` and `**/obj/**`, and the reader skips `bin` and `obj`.
- The model is read from the module's Layer element, the Layer Settings Path, the May Depend On association, and Architecture Scope on Codebase Structure's Root Folder.

#### CD capabilities these modules lack, or have an equivalent for
- **Recipe probe:** lacking. The nearest equivalent is one fixture test per rule (clean without the break, failing with it) plus real-code tests that plant breaks (Front End 114 tests, Architecture 10). These test the engine, not a consumer's configuration, and there is no per-consumer configuration to probe.
- **Wrong-element check:** lacking as such. Resolution is by name and folder, and Razor/C# name resolution tells same-named components and clients of two packages apart (#30, #38). That is a stronger equivalent, but specific to the stack.
- **Unreadable / can't be verified:** an equivalent exists, but as warnings that don't fail the run (Front End #70, #72), where CD's row is an error.
- **Exceptions with stale-exception warnings:** the equivalent is Drift Scope Exclude plus the stale-Adopted error (#3). Stale Exclude entries aren't reported in either module.
- **`own` conventions:** the equivalent is hard-coded Backlog detectors (Front End #15, #21, #24, #44, #47, #49, #54, #55), gated by Adopted.
- **Baseline memory (`<Designer>.model.json`):** lacking. A change made in the model first fails until the code follows (CD's UI check does the same for a component modelled before its code).
- **ValidateDriftRecipes task and `/validate-drift-recipes`:** lacking; there are no recipes.
- **AI skills and always-on rule** (`/sync-model`, `/configure-drift-check`, `reverse-engineer-ui`): lacking. What these modules have instead: AI rules text on each designer and element type, `GUARDRAILS.mdx`, `CONTEXT.md`, and a model fix named in every finding message. The hub's agent-rule output is disabled here.
- **Technology neutrality and guides** (Blazor, Angular, React): lacking. The modules cover Blazor and C# only.
- **Adversarial and guide tests:** an equivalent exists: fixture tests per rule, plus real eShop code tested both clean and with planted breaks.
- **Report hub:** the same. Both modules report through Intent.Common.DriftReporting as Areas, with the same error and warning model.
- **What these modules have and CD lacks:**
  - an adoption scope held in the model, with Strict and Not-yet-adopted;
  - errors for model features the check can't compare;
  - warnings that list code the model can't state;
  - live designer validation;
  - UI-to-UI reference packages across applications (HybridApp → WebAppComponents).
- **What CD has and the Front End module lacks:**
  - Calls to Commands and Queries (here they are only "not checked");
  - dialogs (`intent.opens`);
  - Layout children read through Display Component.

---

# Java/Vue spike: Domain, Services, Front End and Architecture modules

#### Stack
eShop.Backend is a Java 17 / Spring Boot 3.3 app built with Maven. It uses Spring MVC `@RestController`s, Spring Data JPA over Jakarta Persistence (H2 and PostgreSQL), Bean Validation, and Spring Security as an OAuth2 resource server with `@PreAuthorize`. eShop.FrontEnd is Vue 3 and TypeScript with vue-router 4, Pinia 3 and zod, built with Vite and tested with Vitest. All four modules read code with hand-written regular expressions plus brace and parenthesis matching. None uses a Java or TypeScript parser:
- **Domain** strips comments and strings, then parses each top-level Java type into records (fields, methods, annotations) before comparing.
- **Services** parses controllers and records, and builds a regex call graph of Spring beans.
- **FrontEnd and Architecture** run their regexes over the `.vue`, `.ts` and `.java` text inside the comparison itself.

Reading the model is separate from comparing. Each reporter turns its designer into plain records, looking up stereotypes and associations by name, then calls a pure static `Build` or `Check(model, files)` that the xunit tests drive. Reading the code and comparing it both happen inside that engine, and all the technology knowledge is compiled into it. Each reporter derives from `DriftReporterBase` in Intent.Common.DriftReporting, the same hub CD uses, and logs every Wrong and Backlog line as an Error in its own Area.

Across the four modules there are 78 checks: A1 14, A2 16, B 25, C 20, D 3. A partly covered check is rated by the part that isn't covered and marked "part A1". 10 of the 20 C checks only look at the model, so they belong in designer validation rather than drift detection. In short, CD's file-plus-regex path can carry the fact-style checks on Java and Vue: 14 work today and 25 more need new kinds or a new designer contribution. None of the 16 checks CD does through its C#-only structure comparison carries over: 9 of them could become B kinds and 7 need code.

##### Domain: Summary
Total 32: A1 7, A2 9, B 9, C 7, D 0 (5 of the C are model-only rules). The JPA mapping facts CD already has carry over to Java using `file:` finds and regexes anchored on `{Attribute\|camel}` or `{TargetEnd\|camel}`: column name, length, precision and scale, relationship multiplicity (including composition and required ends), and unmodelled entities through `own`. Everything CD checks on C# types through the structure comparison is unavailable here. That covers kind, abstract, extends, attribute types, signatures, navigation, enum literals and unmodelled members; 6 of those 9 could become Domain kinds and 3 need code. The other B rows need the Domain vocabulary opened up: this module's own Repository and Version stereotypes, join columns, a `{Folder}` placeholder, and nullability beyond strings. Key names also need a way to match camelCase in the code with PascalCase in the model.

##### Domain: Checks
| # | Check | Dir | Cat | CD equivalent, or proposed fact kind / plug-in | Stack-specific? (what) | Where |
|---|---|---|---|---|---|---|
| 1 | Data Contracts must not be used | meta | C | Model-only rule; belongs in a designer validate function, outside CD | No | `Build` L128 |
| 2 | Each Class/Enum has a Java type of that name under `…/domain` | M→C | B (part A1) | Persisted classes: any per-class fact whose `file:` find fails is can't-verify (`key`, `table.*`). Enums and unpersisted Embeddables have no Domain kind, so new `type.kind` (new kind on existing designer) | `src/main/java/**/domain` layout | `Locate` L114 |
| 3 | The type is in the sub-package that matches its designer folder | M→C | B | A `{Folder}` placeholder in the type's `file:` find; Domain kinds have none (Services and UI do); new placeholder on existing designer | Java package = folder | `Locate` L121 |
| 4 | Class is `@Entity`/`@Embeddable`/`@MappedSuperclass` (not an enum or helper); Enum is a Java `enum` | M→C | A2 | C# structure compares kind. Java: new `type.kind` read by regex on the type header (B) | JPA annotations | `Build` L137-138, L316 |
| 5 | Abstractness matches | M→C | A2 | C# structure (modifiers). Java: new `type.abstract` (B) | Java `abstract` | L139 |
| 6 | Generalization ↔ `extends`; an unmodelled `extends <domain class>` is Backlog | both | A2 | C# structure checks only that the model's base types are there (extra code bases are fine). Java: new `type.extends`, parent name or empty, compared both ways (B) | Java `extends` | L140-143 |
| 7 | Table stereotype Name ↔ `@Table(name)` | both | B (part A1) | M→C: `table.name`, regex `@Table\(name="([^"]*)"`. C→M (`@Table` on a class with no Table stereotype) is never stated: widen the kind to every persisted class | JPA `@Table` | L146-150 |
| 8 | Stereotype properties the check can't verify must be removed (Table Schema, Schema, View, Check Constraint, Index element; Column Type/Order/Collation, SQL Data Type, Computed Value, Default Constraint) | meta | C | Model-only. CD instead compares `table.schema`, `table.kind`, `column.type`, `column.textType`, `column.default` (readable from `@Table(schema)` and `@Column(columnDefinition)`), ignores the rest, and only warns for a kind with no recipe | Spike's view of JPA | `Build` L145, `CheckDb` L251; reporter `ReadModel` L90-92, `ToDb` L125-129 |
| 9 | Own Repository stereotype ↔ `interface <X>Repository extends JpaRepository<X,…>`; a JpaRepository whose class lacks the stereotype is Backlog | both | B | New `repository` kind on Domain that reads a non-stock stereotype (vocabulary opened); C→M through `own` (`file: **/*Repository.java with JpaRepository<`). The C# analogue is Repository elements compared by structure | Spring Data JPA | L152-157, L349-352 |
| 10 | Model names PascalCase (Attribute, Operation, association ends); live: Class, Enum, Attribute PascalCase, Attribute has a type | meta | C | Model-only; designer validate functions | No | L162, L200, L258; `Domain.designer.settings` |
| 11 | Attribute ↔ non-static, non-transient field `camel(Name)` exists | M→C | B | C# structure never reports a member the code lacks (treated as a pending addition). Java: new `attribute.type` whose read fails becomes can't-verify | `transient`/`@Transient` | L163-164 |
| 12 | Attribute type ↔ field type (type map; `List`/`Set`/`[]` = `[*]`; `Optional`) | M→C | A2 | C# structure (property types). Java: new `attribute.type`, one read case per Java type, e.g. `\bBigDecimal\s+{Attribute\|camel}\b` → `decimal` (B) | Java↔Intent type map | L170, `Matches` L565 |
| 13 | A field typed by a domain class must be an Association, not an Attribute | M→C | C | Needs to know which Java types are entities (cross-file); could mostly be a model-only rule (an Attribute typed by a Class) | JPA entity set | L169 |
| 14 | Nullability: a primitive can't be nullable, a boxed type can't be non-nullable, a reference is non-null exactly when `@Id`/`nullable=false`/`@NotNull` | M→C | B (part A1) | `column.required` covers only string/binary non-key attributes (A1, with cases for `nullable=false` and `@NotNull`). Other types: widen `column.required` or add `attribute.nullable` with primitive/boxed/annotation cases | Primitives vs boxed; Bean Validation | L172-176 |
| 15 | `@Column(name)` ↔ Column Name | both | A1 | `column.name` (regex anchored on `{Attribute\|camel}`); persisted classes only | JPA | `CheckDb` L215-219 |
| 16 | `@Column(length)` ↔ Text Constraints MaxLength | both | A1 | `column.maxLength` (string attributes only; the spike reads an unset length as 255, CD as empty) | JPA default 255 | L221-225 |
| 17 | `@Column(precision, scale)` ↔ Decimal Constraints | both | A1 | `column.precision`, `column.scale` | JPA | L227-235 |
| 18 | `@Id` ↔ Primary Key; Data source ↔ `@GeneratedValue` | both | B (part A1) | `key` works only through fixed-value cases: values compare ordinally and read-value `case` is only upper or lower, so field `id` can't read as `Id`. Data source needs new `key.generated` | JPA | L237-245 |
| 19 | Own Version stereotype ↔ `@Version` | both | B | New kind that reads a non-stock stereotype | JPA `@Version` | L247-249 |
| 20 | Operation/Constructor ↔ non-private method/constructor with the same name and signature | M→C | A2 | C# structure (signatures). Java: C (parameter-list parsing, overloads, and `int`/`Integer` both accepted, so there is no single expected text) | Java syntax | `CheckMembers` L195-208 |
| 21 | Unmodelled non-private methods/constructors (except the no-arg constructor, field accessors, `equals`/`hashCode`/`toString`) | C→M | A2 | C# structure, public members only. Java: C (text ownership is per file; accessor exclusion) | JavaBeans accessors, JPA no-arg constructor | L184-192 |
| 22 | The association's target end must be navigable | meta | C | Model-only | No | L257 |
| 23 | Owning field ↔ end: type `X` or `List<X>`, and a JPA relationship annotation that matches the multiplicities | M→C | A1 | `relationship.multiplicity`: one read case per annotation form, anchored on `{TargetEnd\|camel}` in `{Source}.java` | JPA relationship annotations | `CheckEnd` L276-295 |
| 24 | Composition (source end `1`) ↔ `cascade=ALL`/`orphanRemoval=true`/`@Embedded` | both | A1 | Same recipe: the cascade case gives `1->*`/`1->1`, otherwise `0..1->…` | JPA cascade | L296-304 |
| 25 | A single end is `1` ↔ `optional=false`/`@JoinColumn(nullable=false)`/`@NotNull` | both | A1 | Same recipe: `*->1` vs `*->0..1` | JPA | L305-309 |
| 26 | Source end navigable ↔ back-reference field on the target class | both | A2 | C# structure (navigation property). Java: new `relationship.navigable` read from `{Target}.java` (B) | No | L261-268 |
| 27 | `@JoinColumn(name)` ↔ the end's Foreign Key Column Name | both | B | `relationship.foreignKey` holds foreign-key attribute names, which Java doesn't have; new `relationship.joinColumn` from the stock Foreign Key stereotype | JPA | L279-283 |
| 28 | Enum literals in Java order, none missing; extra Java literals are Backlog | both | A2 | C# structure (enum literals). Java: new `enum.literals`, an ordered comma list read by regex over the enum body plus `replace` (B) | Ordinal persistence | L317-324 |
| 29 | A literal's Value must be empty or equal its ordinal | meta | C | Model-only | Ordinal storage | L322 |
| 30 | Indexes as column sets: Index stereotypes (including unnamed) ↔ `@Column(unique)`, `@Table(uniqueConstraints/indexes)`, both ways | both | C (part A1) | `index.unique`/`index.columns` cover named indexes only and compare attribute names, while JPA lists column names (snake_case, `<end>_id`) | Spring physical naming, JPA index annotations | L331-346, `CodeIndexes` L609 |
| 31 | Unmodelled `@Entity`/`@Embeddable`/`@MappedSuperclass`/enum in the domain package | C→M | A1 | `own` (`file: …/domain/**/*.java with @Entity\|…`). A file only counts as modelled when some fact located it, so enums need #28's kind first | JPA | L353-357 |
| 32 | Unmodelled non-static fields of a modelled class | C→M | A2 | C# structure (public properties; fields are never reported). Java's mapped fields are private, so C (sub-file ownership) | JPA field mapping | L358-366 |

##### Services: Summary
Total 21: A1 7, A2 7, B 2, C 4, D 1 (2 of the C are model-only). Controllers, handlers, verbs, routes, parameter binding and security map onto CD's Services kinds as whole-file regex recipes anchored on `{Service\|trimEnd:Service}Controller` and `{Operation\|camel}`. The `class`, `method` and `statement` finds, `in`, `part` and `inherit` only work on the C# index. Return types, parameter types and optionality, DTO shapes, and unmodelled handlers, parameters and record components are C#-structure-only in CD (A2): 3 could become new kinds and 4 need code. The Domain Interactions trace (controller → service beans → helpers → JpaRepository) needs a plug-in, and its Backlog half is a D.

##### Services: Checks
| # | Check | Dir | Cat | CD equivalent, or proposed fact kind / plug-in | Stack-specific? (what) | Where |
|---|---|---|---|---|---|---|
| 1 | Service named `<X>Service` (engine and live); live: Operation has Http Settings with Verb and Route, Route has no leading `/` and no `?&=`; DTO-Field PascalCase | meta | C | Model-only; designer validate functions | `<X>Service`↔`<X>Controller` convention | `Build` L71-75; `Services.designer.settings` |
| 2 | Service ↔ `@RestController` class `<X>Controller` | M→C | A1 | Endpoint facts' `file: **/{Service\|trimEnd:Service}Controller.java` find fails, so they're can't-verify | Spring MVC | L76-83 |
| 3 | Unmodelled `@RestController` | C→M | A1 | `own` (`file: **/*Controller.java with @RestController`) | Spring MVC | L103-106 |
| 4 | Operation ↔ handler `camel(Name)` with a `*Mapping` | M→C | A1 | `operation.verb` read case anchored on `{Operation\|camel}\(`; no match and no `default` gives can't-verify | Spring MVC | L88-92 |
| 5 | Unmodelled handler methods | C→M | A2 | C# structure (unmodelled public methods). Java: C (text ownership is per file) | `*Mapping` | L98-101 |
| 6 | Verb + full route (class `@RequestMapping` prefix + method path) ↔ Http Settings (+ Service route); route parameters compared by position | M→C | A1 | `operation.verb`, `operation.route`, `service.route` (`$1/$2` value template, regex `replace`). CD compares parameter names, not positions | Spring MVC | `CheckOperation` L140-143 |
| 7 | Success code (`Http.created`→201, `noContent()`→204, else 200) ↔ Success Response Code; an unmodelled non-200 is Backlog | both | B | New `operation.successCode` on Services (vocabulary opened) | Project's own `Http` helper, `ResponseEntity` | L145-149, `StatusOf` L287 |
| 8 | Return type ↔ Operation type (unwrap `ResponseEntity<T>`; `ResponseEntity<Object>` traced into the app-service method; untraceable is Backlog) | M→C | A2 | C# structure (signature). Java: C (cross-file trace through the injected service class); the declared type alone could be a B `operation.returns` | `ResponseEntity`, project `Http` | L151-155, `ReturnsOf` L292 |
| 9 | Parameter source (`@PathVariable`/`@RequestParam`/`@RequestBody`/`@RequestHeader`; Default inferred from the route or a DTO type) | M→C | A1 | `parameter.source` (Default inference not replicated, so the model must state Source) | Spring MVC | L158-173, `ParamsOf` L316 |
| 10 | Parameter type (including `Http.parseEnum` binding a String to an enum) | M→C | A2 | C# signature. Java: new `parameter.type` (B) | Project's `Http.parseEnum` | L174-175 |
| 11 | Parameter optionality (`required=false`) ↔ nullable | M→C | A2 | C# signature (`T?`). Java: new `parameter.required` (B) | Spring `required=false` | L176-177 |
| 12 | Unmodelled bound handler parameters | C→M | A2 | C# signature. Java: C | Spring binding annotations | L179-182 |
| 13 | Security: `@PreAuthorize(hasRole/hasAnyRole/isAuthenticated/permitAll)`, `@Secured`, `@RolesAllowed`, method overriding class ↔ Secured/Unsecured + roles | M→C | A1 | `security`, `security.roles`; the class level as a later read case in the same file; `unreadable` for other SpEL expressions | Spring Security | L184-187, `SecurityOf` L349 |
| 14 | A modelled Policy is always Wrong (the code reading has no policy) | M→C | A1 | `security.policy` with an empty `absent` | Spring Security | L184-185 |
| 15 | Entity Actions match the code: the target has a repository or owner, the code reaches it, read vs write, Create ↔ `new X(`, Delete ↔ `repo.delete…` | M→C | C (part A1) | `intent.creates/updates/deletes/queries` exist but only read the located file; this code lives in application services and helpers, so it needs a call-graph plug-in | Spring beans, JpaRepository, JPA cascade ownership | `CheckInteractions` L197-229, `Uses` L490 |
| 16 | An aggregate the code reaches through a repository has no Entity Action | C→M | D | CD never reports interactions the code performs and the model lacks | Spring Data | L215-216 |
| 17 | A Query beside an Update or Delete on the same entity | meta | C | Model-only | No | L201-205 |
| 18 | DTO ↔ Java `record` of that name | M→C | B | CD has no Services DTO kind; a new `dto.field.type` whose find fails gives can't-verify | Java records | L110-115 |
| 19 | DTO-Field ↔ record component and type (primitive ↔ non-nullable, boxed ↔ nullable) | M→C | A2 | C# structure (DTO properties). Java: new `dto.field.type` (B) | Java type map | L116-122 |
| 20 | Unmodelled record components | C→M | A2 | C# structure (unmodelled public properties). Java: C | Java records | L123-126 |
| 21 | Every record the API exposes (bodies, returns, transitively through components; non-generic) is a DTO | C→M | C | Reachability; an `own` on all records would also flag internal and generic ones | Java records and generics | L129-132, `Exposed` L377 |

##### FrontEnd: Summary
Total 19: A1 0, A2 0, B 12, C 5, D 2 (1 of the C is model-only). The module defines its own Front End designer, and no CD contribution reads it. So every model→code check becomes a new designer contribution (already allowed) that reads `.vue`/`.ts` text. Four of them (element file, page route, renders, navigates) mirror stock UI kinds. They would be A1 if the storefront were remodelled in the stock User Interface designer, but that would lose Store, Composable, Utility, Proxy and Contract. Completeness inside a file (route records, proxy methods, contract properties, exported types) needs code, and unmodelled links are D. Check 11 compares against the backend application's Services routes, so the contribution's model reader would have to reach across applications.

##### FrontEnd: Checks
| # | Check | Dir | Cat | CD equivalent, or proposed fact kind / plug-in | Stack-specific? (what) | Where |
|---|---|---|---|---|---|---|
| 1 | Model-only: a Page needs a Route (live: it starts with `/`); a Proxy Operation needs exactly one Calls (engine: more than one is Wrong, none is Backlog); live: Proxy Operation is camelCase, Field and Parameter names are valid TS identifiers | meta | C | Designer validate functions | TS identifier rules | `Build` L80, L130, L143; `Front End.designer.settings` |
| 2 | Each Layout/Page/Component/Store/Composable/Utility/Proxy owns `<folders>/<Name>.vue` or `.ts` (case-insensitive), or its Implementation > Path | M→C | B | New designer contribution: a per-element `file:` find with `{Folder}`; stock UI `component.kind` is the analogue for Page/Component/Layout | Vue/TS file layout | L65-74, `FileOf` L234 |
| 3 | Contract ↔ `export interface` or `export type <Name>` in a file in its folder | M→C | B | Contribution kind: `file: src/{Folder}/*.ts with export (interface\|type) {Contract}\b` | TypeScript | L67-72 |
| 4 | Page Route Path ↔ a `{ path, component }` route record | M→C | B | Close to stock `page.route` (A1 if remodelled in the stock designer) | vue-router | L76-86 |
| 5 | Renders ↔ `<Name` or `<kebab-name` inside `<template>` | M→C | B | Close to stock `intent.uses` with the `kebab` filter | Vue templates | L94, `Renders` L241 |
| 6 | Navigates To ↔ `to=` or `router.push`/`replace` to a path matching the route (`:id` matches a literal or `${…}`) | M→C | B | Close to stock `intent.navigates` with `{RouteStart}` | vue-router | L95, `Navigates` L253 |
| 7 | Uses ↔ a relative value import (not `import type`) of the target's file; for a Proxy Operation also a `.op(` call | M→C | B | Contribution kind; the import is matched by file name in a regex, not by resolved path | ES modules | L96, `UsesTarget` L224, `Imports` L260 |
| 8 | Proxy Operation ↔ method `name(` in the Proxy's file | M→C | B | Contribution kind; a failed find gives can't-verify | TS client object | L105-110, `Method` L264 |
| 9 | `Promise<T>` return ↔ Returns (TS type map, `\| null`, `T[]`); an unmodelled non-void return is Backlog | both | B | Contribution kind; its model reader renders the expected TS type | TypeScript | L112-117, `Matches` L311 |
| 10 | Parameters by position, name and type; extra code parameters are Backlog | both | B | Contribution kind whose value is the canonical parameter list | TypeScript | L119-128 |
| 11 | The method's `request('VERB', path)` ↔ the called backend Operation's verb and Service + Operation route (`[controller]` expanded, normalised) | M→C | B | Contribution kind; model value computed from the backend's Http Settings; regex `replace` turns `${…}` into `{}` | Storefront's `request<T>()` helper | L131-142, `EndpointRoute` L49 |
| 12 | Every `src` `.ts`/`.vue` file (except tests, `.d.ts`, `test/`, `main.ts`, `router.ts`) is owned by an element | C→M | B | CD `own` + `exceptions`, once every element type has a file-level fact | Vite/Vue layout | L149-152 |
| 13 | A route record points at an unmodelled Page | C→M | C | Sub-file (`router.ts` is one place to CD); mostly caught by #12 on the page's file | vue-router | L153-156 |
| 14 | Renders/Navigates/Uses links the code makes and the model lacks | C→M | D | CD's UI check never reports code-has/model-lacks links | Vue, ES imports | L157-171 |
| 15 | A Proxy Operation the code calls that no Uses link models | C→M | D | Same as #14 | ES imports | L172-176 |
| 16 | Unmodelled Proxy methods (`name(…): Promise<`) | C→M | C | Sub-file completeness | TypeScript | L178-185 |
| 17 | Contract Field ↔ property and TS type (`?`/`\| null`/`\| undefined` = nullable, `T[]` = many) | M→C | B | Contribution kind anchored on `export interface {Contract}\b…{Field}` | TypeScript | L186-194 |
| 18 | Unmodelled Contract properties | C→M | C | Set completeness inside one type | TypeScript | L195 |
| 19 | Unmodelled exported interfaces or types in files that hold Contracts | C→M | C | Sub-file | TypeScript | L197-204 |

##### Architecture: Summary
Total 6: A1 0, A2 0, B 2, C 4, D 0 (2 of the C are model-only). CD has no architecture vocabulary. The layer import rule and the empty-layer check need a plug-in for the import graph, path resolution, longest-Path ownership and counting. File Conventions and File Exceptions fit a Codebase Structure designer contribution plus CD's `own` and `exceptions`. The exceptions would then move from the model into the recipe, and `{Singular}` needs a new filter. File Conventions and Exceptions are in the engine and its tests but GUARDRAILS.mdx doesn't describe them.

##### Architecture: Checks
| # | Check | Dir | Cat | CD equivalent, or proposed fact kind / plug-in | Stack-specific? (what) | Where |
|---|---|---|---|---|---|---|
| 1 | Live: Layer Path is set, relative, forward slashes, no `..`, unique; Layer name unique; no self arrow; a File Convention names Designer, Element Type and a File Pattern with known tokens; a File Exception is a relative glob | meta | C | Designer validate functions | No | `Codebase Structure.designer.settings` |
| 2 | An import into another Layer needs a May Depend On arrow (Java `import`/`import static`/`.*` resolved through package → source root; TS relative `from`, `import type`, side-effect and dynamic imports; longest-Path ownership; libraries ignored) | M→C | C | Import graph, path resolution and longest-prefix ownership across many files; no CD kind | Java and ES import syntax | `Check` L91-108, `Imports` L208, `Owner` L204 |
| 3 | A Layer whose Path holds no source files (stale model) | M→C | C | Counting; a `file:` find with several hits is can't-verify | `.java`/`.ts`/`.vue`, test exclusions | L86-89 |
| 4 | Each element a File Convention selects (designer, element type, stereotype, In Folder) has `<Layer Path>/<pattern>` (`{Name}`, `{name}`, `{Singular}`, `{singular}`, `{Folder}`, Remove Suffix) | M→C | B | New Codebase Structure contribution: one fact per selected element with a computed-path `file:` find; needs a `singular` filter and a model reader that reaches other designers | English singular | L113-143, `FileFor` L232 |
| 5 | A File Convention matches no model elements | meta | C | Model-only | No | L121-127 |
| 6 | Every source file in a Layer is some convention's file or matches a File Exception glob | C→M | B | CD `own` + `exceptions` over #4's facts; the exceptions move from the model into the recipe | Test-file conventions | L145-152 |

#### D items explained
- **Services 16 (an aggregate the code reaches has no Entity Action).** CD's Services contribution only states the Entity Actions the model has. It deliberately never reports interactions the code performs and the model lacks, so the model needn't list every repository call. The spike makes this Backlog, which fails the run. Supporting it would mean reversing that rule, and here it would also need the call-graph plug-in (C) to know what the code does.
- **FrontEnd 14 and 15 (unmodelled Renders/Navigates/Uses links, and unmodelled proxy calls).** CD's UI contribution says a link "the code has and the model doesn't is never reported, so model it when you add it". Facts exist only per model link. The only thing that finds code without a model fact is `own`, and in text files `own` works per whole file, so it can't see individual links.
- **Design tensions, not rated D, that matter for describe-only Java:**
  - CD's structure comparison treats "the model has it, the code doesn't" as a pending Software Factory addition and never a row. With nothing generating code, every "no Java type, field or method" check (Domain 2, 11, 20; Services 2, 4, 18; FrontEnd 2, 8) must show up as a fact that can't be verified.
  - CD reports an unmodelled member only when it's public, and never reports fields (user decision 2026-10-28). The spike reports non-private methods (Domain 21) and private JPA fields (Domain 32).
  - CD only warns about a modelled claim it has no recipe for. The spike fails on stereotype properties it can't verify (Domain 8).
  - CD compares values ordinally, and a read value's `case` can only be upper or lower. This codebase's rule is PascalCase in the model and camelCase in Java, so any kind whose value is a list of member names read from Java (`key`, `relationship.foreignKey`, `index.columns`) can't match without a fixed-value case per name or a new `pascal` read case.

#### Stack knowledge baked into the engines
- **Domain:**
  - Java syntax: annotations with balanced parentheses, records, enums, text blocks, modifiers, `extends`.
  - Layout: `src/main/java` and a `/domain/` package.
  - JPA (`jakarta.`/`javax.persistence`): `@Entity`, `@Embeddable`, `@MappedSuperclass`, `@Transient`, `@Table`, `@Column(name, length, precision, scale, unique, nullable)`, `@Id`, `@GeneratedValue`, `@Version`, `@JoinColumn`, `@OneToMany` and the other relationship annotations, `@Embedded`, `@EmbeddedId`, `@ElementCollection`, `cascade`, `orphanRemoval`, `optional`, `@UniqueConstraint`, `@Index(columnList)`.
  - Defaults and conventions: JPA's default length 255, Spring Boot's snake_case physical naming, `<end>_id` join columns.
  - Bean Validation `@NotNull`/`@NonNull`, and Spring Data `JpaRepository<X,…>`.
  - The Java↔Intent type map, primitive vs boxed nullability, JavaBeans accessors, `equals`/`hashCode`/`toString`, and ordinal enum storage.
- **Services:**
  - Spring MVC: `@RestController`, class-level `@RequestMapping`, `@Get/Post/Put/Delete/PatchMapping`, `@PathVariable`/`@RequestParam`/`@RequestBody`/`@RequestHeader` with `name`/`value`/`required=false`.
  - Responses: `ResponseEntity<T|Object|Void>`, `ResponseEntity.ok` and `noContent`, and this codebase's own `Http.created` and `Http.parseEnum` helpers.
  - Spring Security: a subset of `@PreAuthorize` SpEL, `@Secured`, `@RolesAllowed`, and the `ROLE_` prefix.
  - Java records as DTOs, with generic records treated as Type-Definitions.
  - Call-graph heuristics: Spring bean annotations, Spring Data method prefixes (`save…`, `delete…`), getter-name and `add`/`remove`/`clear` mutation heuristics, and JPA cascade ownership.
  - The `<X>Service`↔`<X>Controller` naming rule.
- **FrontEnd:**
  - Vue 3 SFC `<template>` with PascalCase or kebab-case tags.
  - vue-router: `{ path, component }` records, `<RouterLink to>`/`:to`, `router.push`/`replace`, `:param` segments.
  - ES module relative imports, with `import type` excluded.
  - The storefront's own `request<T>('VERB', path)` helper and `Promise<T>` client methods.
  - TypeScript: `export interface`/`type`, the type map, `| null`/`| undefined`/`?`/`T[]`.
  - File conventions: Vite/Vitest tests excluded, `main.ts` and `router.ts` exempt, and the `[controller]` route token from Intent's WebApi.
- **Architecture:**
  - Java `package` and `import` statements (including `static` and wildcards), resolved against a source root.
  - ES relative `from`, side-effect and dynamic imports.
  - `.java`/`.ts`/`.vue` and test-folder conventions, and English singularisation.
  - The Layers, arrows, conventions and exceptions themselves are modelled, not baked in.

#### Does it have an adoption scope? How is "not yet modelled" handled?
No. Wrong and Backlog are both `DriftSeverity.Error` ("User decision: every finding fails the run"), so any unmodelled entity, field, controller, handler, record, source file or link fails every run. There is no Scoped/Strict mode, no "not yet adopted" count, and no switch to turn a check on. Domain, Services and FrontEnd run whenever they're installed, and Architecture skips only when there are no Layers.

What gets checked is fixed in code:
- **Domain** reads files under `/domain/` and checks only JPA types and enums. It lists other types as "Not checked" in its legacy report text, which the reporter no longer writes.
- **Services** checks every `@RestController` and every record the API exposes.
- **FrontEnd** checks every `src` `.ts`/`.vue` file except tests, `.d.ts`, `test/`, `main.ts` and `router.ts`.
- **Architecture** checks only files under a Layer Path, so `EShopApplication.java` goes unchecked.

The only modelled scoping is Architecture's File Exceptions. "Not yet modelled" is therefore always Backlog that fails the run: each area has to be modelled completely before a run goes green.

#### CD capabilities these modules lack, or have an equivalent for
- **Technology-neutral engine with a committed recipe file:** none. Every convention is compiled C#, so changing one means editing the engine, rebuilding and force-reinstalling the module.
- **Recipe probe, wrong-element check, ValidateDriftRecipes task and skill:** none (there are no recipes). The closest equivalents are xunit tests per engine that plant each kind of drift, and two tests against the real code: Services `The_real_backend_parses_completely` and Domain `The_eShop_backend_domain_modelled_by_the_conventions_is_clean`. No adversarial run is recorded. On the CD side, its wrong-element check covers C# files only, so for Java and Vue recipes only the probe and the samples would catch a regex reading another element's code.
- **Can't-verify, `unreadable`, `unstated`:** mostly missing. Code the regexes can't read either shows up as "not found" (Wrong) or is silently skipped: a handler without `*Mapping`, a parameter without a binding annotation, a multi-line TS signature. Partial equivalents: an untraceable `ResponseEntity<Object>` becomes Backlog, an unreadable `@PreAuthorize` becomes a role named after the expression so it never matches by accident, and messages state the JPA defaults inline.
- **Comment stripping:** only Domain strips comments and strings. Services, FrontEnd and Architecture match raw text, so GUARDRAILS.mdx's "comments and strings are stripped first" is true only for Domain.
- **`own`, exceptions, and the warning for an exception that matches nothing:** scope is hard-coded, and FrontEnd's `Unmodelled` list is a fixed exception. Architecture's File Exceptions are modelled globs, the closest equivalent, but nothing warns about one that matches nothing.
- **Identity and anchoring:** findings are keyed by element name across folders, an ambiguity the spike acknowledges. Only Domain (file) and Architecture (file and line) point at code. CD keys facts by element id and anchors every row at file:line.
- **Off until configured, baseline, pending model-first changes, accepted view:** none. Only CD's off switch would matter describe-only; the rest exists for code generation.
- **AI guidance:** designer AI rules per element type, plus GUARDRAILS.mdx. There is no always-on sync rule, no sync-model, configure-drift-check or validate-drift-recipes skill, and no tested technology guides.
- **What the spike has that CD lacks:**
  - Live designer validation.
  - Architecture layering, import rules and file conventions.
  - A check across applications (FrontEnd Calls ↔ the backend's routes).
  - JPA semantics: composition ↔ cascade, nullability from primitive vs boxed, ordinal enums, unnamed indexes.
- **Shared with CD:** both report through Intent.Common.DriftReporting: an Area per check, Error findings, rule ids, one failed run with the report staged, and a CI break through `ensure-no-outstanding-changes`. But `azure-pipelines.yml` only runs `mvn verify` and Vitest, not the Software Factory, so drift isn't enforced on CI.
