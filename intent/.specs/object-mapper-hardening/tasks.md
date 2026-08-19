# Implementation Plan — Object Mapper Hardening

Six waves. The first two are the **Golden Sample hard gate** — the design's "Wave 0" split in half so each Flavour Application's hand-written code stays inside one coding agent's workload. **No module code is touched until both are green.** Waves 3–5 build the module changes; Wave 6 throws the hand-written mappings away and proves the module reproduces them.

## Scope change — cursor paging is DEFERRED (user decision, Wave 1)

**R4.3 and the cursor-paged half of R8.9 are deferred and unverified.** They are NOT satisfied by this spec and must not be ticked as if they were.

Reason: `CursorPagedListInterfaceTemplate.CanRunTemplate()` gates on some installed module fulfilling `TemplateRoles.Application.Common.CursorPagedList`, and the only module in this repository that does is `Intent.Modules.Azure.TableStorage` — a persistence provider whose repository templates would compete with EF Core's for the same roles. Installing it was ruled out. This is a pre-existing gap in Pagination + EF Core and is unrelated to the Object Mapper module.

Consequences across the plan:
- Neither Flavour Application models a cursor-paged query. `ObjectMapping.Strict` had `GetOrdersCursorPaged` modelled and then removed in Wave 1; `ObjectMapping.Lenient` (T2.3) must never model one.
- The four surviving queries are `GetOrderById`, `GetOrders`, `GetOrderOrNull`, `GetOrdersPaged`. **Offset pagination is unaffected and stays fully covered** — R4.1, R4.2, R4.4, R4.5 and the offset half of R8.9 are in scope as written.
- T5.2 (`ImplementPagedMappingStatement`) is exercised on the offset-paged path only.
- Wherever a task below says "cursor-paged", read it as struck out.

## Tasks

- [x] T1. Golden Sample — `ObjectMapping.Strict`
  - [x] T1.1 [module] (satisfies: R8.1, R8.3, R8.11) — Create the `ObjectMapping.Strict` application and install its module set
    - Create fresh from the same architecture as `ObjectMappingTest`; **do not copy** the application folder, its `.deviations`, managed-file records or designer metadata (R8.11)
    - Install exactly the module set in `Tests/ObjectMappingTest/modules.config` — Clean Architecture, MediatR CQRS, EF Core, Domain Interactions, Pagination, AspNetCore Api with controllers — **excluding** `Intent.Application.Dtos.ObjectMapping`, which is installed in Wave 6 (T6.1)
    - Add the `ObjectMapping.Strict.Application.Tests` xUnit project; folder name must match the project name exactly, with no relative-location override (R8.3)
  - [x] T1.2 [model] (satisfies: R6.1, R8.2) — Model the Domain package (8 entities, 2 nullable hops)
    - Entities per design A.1: `Order` (root, `OrderNumber: string`, `Status: OrderStatus`, `Notes: string?`, `CustomerId: Guid`, parameterless operation `GetDisplayLabel(): string`), `Customer` (`Name: string`, `Tier: CustomerTier`), `Address` (`Line1`, `City`, `PostalCode`), `OrderLine` (`ProductName: string`, `Quantity: int`), `Coupon` (`Code: string`, `PercentOff: int`, `Kind: CouponKind`), `Tag` (`Name: string`), `PaymentMethod` (`Label: string`), `CardPayment : PaymentMethod` (`CardLast4: string`)
    - Associations: `Order → Customer` **required** many-to-one (this is what proves R1.8 — the non-guarded hop), `Order → Coupon` **optional** to-one (**Nullable Hop #1**), `Order *— OrderLine` composition to-many, `Order ↔ Tag` many-to-many, `Order → PaymentMethod` to-many, `Customer → Address` **optional** to-one (**Nullable Hop #2**), `CardPayment` generalizes `PaymentMethod`
    - Enums: `OrderStatus`, `CustomerTier`, `CouponKind`
    - Element names must be identical to the Lenient application's (T2.2) — R8.2 and R8.10 both depend on this
  - [x] T1.3 [model] (satisfies: R6.1, R2.5, R8.2, R8.9) — Model the Services package: DTOs pinning all 21 Mapping Shapes, plus the queries
    - `OrderDto` mapped from `Order`, fields exactly as design A.2 — one field per shape, in declared order: `OrderNumber`(1), `Notes`(2), `CustomerName`(3), `Coupon: CouponDto?`(4), `Customer: CustomerDto`(5), `Lines: List<OrderLineDto>`(6), `CustomerId`(7), `CouponId`(8), `LineIds`(9), `ProductNames`(10), `Status`(11), `StatusView: OrderStatusDto`(12), `DisplayLabel`(13), `Payments: List<PaymentMethodDto>`(16), `CustomerCity`(17), `CouponPercentOff`(18), `CouponKind`(19), `TagNames: List<string>?`(20), `SrcFormLabel`(21a), `ProjectFromFormLabel`(21b)
    - R2.5: `SrcFormLabel` authored as `src.orderNumber + " / " + src.status`, `ProjectFromFormLabel` as the same expression in the `projectFrom.` Prefix Form — the two must later normalise byte-identically
    - `CardPaymentDto` mapped from `CardPayment` carries shapes 14 (inherited `Label` through the generalization hop) and 15 (`CardLast4` on the derived type); `CouponDto`, `CustomerDto`, `OrderLineDto`, `PaymentMethodDto` as nested targets
    - Queries modelled via Domain Interactions: `GetOrderById` (single), `GetOrders` (collection), `GetOrderOrNull` (nullable single), `GetOrdersPaged` (paged) — the offset-paged query is what R8.9 needs present end to end. ~~`GetOrdersCursorPaged` (cursor-paged)~~ **deferred** (see Scope change)
    - Leave `OrderDto` with at least one unmapped field to pin R6.4 (omitted from the initializer, no warning)
  - [x] T1.4 [code] (satisfies: R1.2, R6.1, R9.1, R9.2, R9.4) — Hand-write `OrderDtoMappingExtensions` exactly as design A.3
    - `MapToOrderDto` body is one object initializer, one entry per mapped field in declared order; `MapToOrderDtoList` is expression-bodied; plain `static class`, no base type
    - Strict form on the four R1-governed entries: `Coupon!.Id`, `Customer.Address!.City`, `Coupon!.PercentOff`, `Coupon!.Kind` — the `!` sits on the **Nullable Hop**, not the field, and `Customer` stays a plain `.`
    - Collection entries end `?? []` **except** `TagNames` (nullable target — shape 20, R6.8), which ends bare
  - [x] T1.5 [code] (satisfies: R6.1, R6.5, R6.7) — Hand-write the remaining 5 Mapping Extension Classes
    - `CustomerDto`, `CouponDto`, `OrderLineDto`, `PaymentMethodDto`, `CardPaymentDto` mapping extensions, same conventions as T1.4
    - `CardPaymentDto`'s method is typed to the derived `CardPayment` and maps inherited `Label` alongside `CardLast4` (R6.5)
  - [x] T1.6 [code] (satisfies: R3.2, R3.3, R3.4, R3.5, R4.1; ~~R4.3~~ deferred) — Hand-write the Golden Sample Call Sites in the four query handlers
    - `return order.MapToOrderDto();` · `return orders.MapToOrderDtoList();` · `return order?.MapToOrderDto();` · `return orders.MapToPagedResult(x => x.MapToOrderDto());` (~~`MapToCursorPagedResult`~~ deferred)
    - Remove any `IMapper` constructor parameter / field the CRUD scaffolding injected — no handler may reference `AutoMapper.IMapper` (R3.5)
  - [x] T1.7 [code] (satisfies: R6.1, R8.4, R1.6) — Write the Mapping Shape test suite
    - At least one assertion per Mapping Shape 1–21 that fails if the expression is wrong (R8.4); cover R6.3 (null source collection → empty list), R6.8 (`TagNames` → `null`, not empty), R6.4 (unmapped field left at CLR default)
    - R1.6 Strict runtime assertions: a null `Coupon` / null `Address` makes the mapping throw `NullReferenceException` and return no partially populated DTO
  - [x] T1.8 [code] (satisfies: R8.8, R8.9, R4.2, R4.4) — Write the Call Site and pagination test suite
    - R8.8: per handler returning a mapped DTO, reflect over the handler type and assert **no** constructor parameter is an `AutoMapper.IMapper`, plus a behavioural assertion that the returned DTO is fully populated (this is the design's D4 pair)
    - R8.9/R4.2/R4.4: the offset-paged query end to end — populated page with total count / page number / page size intact, and an empty page that does not throw and keeps its metadata. ~~cursor-paged~~ deferred, so the cursor half of R8.9 is unverified
  - [x] T1.9 [code] (satisfies: R8.6) — Snapshot the green Strict baseline into the spec folder
    - Copy the six hand-written `Mappings/*.cs` files and the five handler bodies into `intent/.specs/object-mapper-hardening/baseline/strict/` **after** the suite passes — this is the only comparison target Wave 6's parity diff has once T6.2 deletes them

- [x] T2. Golden Sample — `ObjectMapping.Lenient`
  - [x] T2.1 [module] (satisfies: R8.1, R8.3, R8.11) — Create the `ObjectMapping.Lenient` application and install its module set
    - Identical to T1.1 in every respect except the name; created fresh, inheriting nothing from `ObjectMappingTest` **or** from `ObjectMapping.Strict`
    - `ObjectMapping.Lenient.Application.Tests`, folder name matching the project name exactly
  - [x] T2.2 [model] (satisfies: R6.1, R8.2) — Model the Domain package, element-for-element identical to T1.2
    - Same entities, attributes, operations, enums, association ends and multiplicities, **same names** — a duplicated model, not a shared package (R8.2). Any divergence here becomes the model drift R8.10 exists to catch
  - [x] T2.3 [model] (satisfies: R6.1, R2.5, R8.2, R8.9) — Model the Services package, element-for-element identical to T1.3
    - Same DTOs, same field order, same two Prefix Form expressions, same **four** queries including the offset-paged one. **Do NOT model a cursor-paged query** (see Scope change)
    - The two Prefix Form expressions must be authored **by the user in the designer UI** — `setBasicMapping` takes element ids only, so an expression mapping cannot be created from the scripting API. Author them PascalCase (`src.OrderNumber + " / " + src.Status` and `projectFrom.OrderNumber + " / " + projectFrom.Status`), because the Domain attributes are `OrderNumber`/`Status`
  - [x] T2.4 [code] (satisfies: R1.3, R6.1, R9.1, R9.2, R9.8) — Hand-write `OrderDtoMappingExtensions` in the Lenient form (design A.4)
    - Byte-identical to T1.4 **except** the four R1-governed entries, which take the null-conditional-plus-`?? default!` form: `Coupon?.Id ?? default!`, `Customer.Address?.City ?? default!`, `Coupon?.PercentOff ?? default!`, `Coupon?.Kind ?? default!`
    - R9.8: each guard is a single expression inside its initializer entry — no local variable, no helper. `default!` is emitted uniformly, including on the `int` and enum targets (design D5 / assumption a2)
  - [x] T2.5 [code] (satisfies: R6.1, R6.5, R6.7) — Hand-write the remaining 5 Mapping Extension Classes
    - Same five classes as T1.5; these contain no R1-governed entry, so they must be byte-identical to Strict's modulo the application-name prefix
  - [x] T2.6 [code] (satisfies: R3.2, R3.3, R3.4, R3.5, R4.1; ~~R4.3~~ deferred) — Hand-write the Golden Sample Call Sites in the four query handlers
    - Identical statements to T1.6 — the null-handling setting does not reach Call Sites, only mapping expressions
  - [x] T2.7 [code] (satisfies: R6.1, R8.4, R1.7) — Write the Mapping Shape test suite
    - Mirrors T1.7 shape for shape, but R1.7 replaces R1.6: a null `Coupon` / null `Address` yields a DTO whose affected field holds `default` for its type — `Guid.Empty`, `null`, `0`, the enum's zero value — with **every other field populated normally**
  - [x] T2.8 [code] (satisfies: R8.8, R8.9, R4.2, R4.4) — Write the Call Site and pagination test suite
    - Mirrors T1.8: reflection assertion that no `IMapper` is injected, behavioural assertion the DTO is populated, and populated/empty **offset**-paged journeys (~~cursor-paged~~ deferred)
  - [x] T2.9 [code] (satisfies: R8.6) — Snapshot the green Lenient baseline into the spec folder
    - Same as T1.9, into `intent/.specs/object-mapper-hardening/baseline/lenient/`

- [x] T3. Module metamodel — the `Null Path Handling` setting
  - [x] T3.1 [model] (satisfies: R1.1) — Add the `Object Mapping` Settings Group and the `Null Path Handling` select setting
    - In `Intent.Modules.Application.Dtos.ObjectMapping`'s Module Builder package — **this is the first modelled element this module has ever had**; the package is currently empty and the template is declared straight into the `.imodspec`
    - New Settings Group named `Object Mapping` (a group, not a `groupExtension` onto an existing one), containing one `select` setting titled `Null Path Handling`
    - Options `strict` (description `Strict`) and `lenient` (description `Lenient`), `defaultValue` `strict`, `isRequired` false, hint per design B.1. Precedent for the shape: the `<setting type="select">` blocks in `Intent.EntityFrameworkCore.imodspec`
    - Generation produces the `<moduleSettings>` block plus a settings accessor class into the module's currently-empty `Settings/` folder, read at template time as `ExecutionContext.Settings`
    - The Template element stays unmodelled (design assumption a1) — only the Settings Group is added
  - [x] T3.2 [code] (satisfies: R3.1, R8.12) — Add the missing `interoperability` detect and bump the module version
    - `Intent.Application.Dtos.ObjectMapping.imodspec` currently has **no** `interoperability` block; add `detect id="Intent.Application.DomainInteractions"` mirroring what `Intent.Application.Dtos.Mapperly` declares — without it, R3.1 is only satisfiable by the user separately knowing to install a second module
    - Bump the version consistently across the `.imodspec`, the `.csproj` and the designer's Module Settings (assumption a5/a7), with a release-notes entry opened under the new version

- [x] T4. Object Mapper expression builder
  - [x] T4.1 [code] (satisfies: R1.2, R1.3, R1.4, R1.5, R1.8, R1.9, R9.8) — Per-hop null handling in `MappingHelper.BuildPath`
    - `BuildPath` today picks the separator from the previous hop's nullability alone and emits `?.` unconditionally. Replace with the design B.2 matrix, decided **per hop**: non-nullable hop → `.` always (R1.8); nullable hop + nullable target → `?.` under both settings (R1.4); nullable hop + non-nullable target + `Strict` → `!.` (R1.2); nullable hop + non-nullable target + `Lenient` → `?.` **and the whole initializer entry gains a trailing `?? default!`** (R1.3)
    - Setting read from `ExecutionContext.Settings` via the accessor T3.1 generates; absent value means `Strict` (R1.1)
    - R1.5: never fail Software Factory execution over a nullable-into-non-nullable path — the designer's own validation is the warning mechanism
    - R1.9: zero errors and zero new nullability warnings for every matrix combination, which is what the trailing `!` on `default!` buys
  - [x] T4.2 [code] (satisfies: R2.1, R2.2, R2.3, R2.4, R2.5, R2.6, R2.7, R2.8) — Prefix normalisation in `BuildEntryExpression`'s expression branch
    - Replace the PascalCase-only implementation with the AutoMapper algorithm at `Intent.Modules.Application.Dtos.AutoMapper/Templates/MappingHelper.cs:121`, plus `projectFrom.` as a second recognised Prefix Form
    - `src.` is **rewritten** to `projectFrom.`, not prepended (R2.2); an already-`projectFrom.` expression is left alone (R2.3); anything else gets `projectFrom.` prepended to the expression **as a whole**, exactly as AutoMapper does, including the cases AutoMapper gets wrong (R2.4) and with no design-time diagnostic (R2.8)
    - **Rewrite every occurrence of the recognised prefix token, not just the leading one** — `src.Amount > 0 ? src.Amount : 0` has a second `src.` mid-expression, and R2.5's byte-identical guarantee fails on any multi-reference expression otherwise. This is the one deliberate step beyond AutoMapper's literal implementation (assumption a4)
    - `PascalCasePropertyAccesses` still runs over the remainder (R2.6); an unresolvable member access passes through unchanged (R2.7)
  - [x] T4.3 [code] (satisfies: R6.2, R6.3, R6.4, R6.6, R6.7, R6.8) — Collection nullability across the three collection call sites
    - Every collection projection today ends `?? []`. Emit `?? []` **only** when the DTO field is non-nullable (R6.3) and end bare when it is nullable (R6.8) — this is the distinction shape 20 exposes
    - Applies at all three emitting sites: the nested-DTO branch, `BuildMultiplePkExpression`, and the collection-with-trailing-property branch
    - Confirm the existing behaviour still holds for R6.2 (always materialize to `List<T>`), R6.4 (unmapped field omitted from the initializer, no warning), R6.6 (exactly one Mapping Method + one List Mapping Method per mapped DTO; none at all for a DTO with no mapping) and R6.7 (source order preserved)
  - [x] T4.4 [code] (satisfies: R7.1, R7.2, R7.3, R7.4) — Diagnostics and namespace derivation
    - `GetEntityTypeName` throws a bare `System.Exception`; replace with an `ElementException` scoped to `Model.InternalElement` naming the DTO and its unresolved source, so the designer highlights the element (R7.1). Plain prose only — `ElementException` does not render Markdown
    - Leave the `InvalidOperationException` on an unhandled multiplicity pair as is — that is a module bug, correctly a developer exception (R7.2)
    - Replace `this.GetNamespace().Replace(".Mappings", "")` with derivation from the output structure that does no substring surgery — today it corrupts any application with an unrelated `Mappings` path segment (R7.3), while keeping the class resolvable from the DTO's own namespace without an explicit `using` (R7.4)
  - [x] T4.5 [code] (satisfies: R5.1, R5.2, R5.3) — Delete the AutoMapper stand-down guard from `CanRunTemplate`
    - Remove the clause that suppresses generation when `Intent.Application.Dtos.AutoMapper` is installed; emit no error or warning about another Mapping Provider being present (R5.3)
    - Keep `Model.Mapping != null` — that is R6.6's "no class for a DTO with no mapping", not a stand-down
  - [x] T4.6 [code] (satisfies: R9.1, R9.2, R9.3, R9.4, R9.5, R9.6, R9.7) — Conform the template's output shape and managed mode
    - Assert by inspection against the T1.9 baseline: one object initializer as the whole body, one entry per mapped field in declared field order (R9.1); expression-bodied List Mapping Method (R9.2); no locals, helpers, nested types or types outside the class (R9.3); plain `static class`, no base or interface (R9.4); no NuGet reference added to the consuming application (R9.5); no reflection, dynamic dispatch or runtime expression trees (R9.7)
    - **R9.6 is the trap:** `MappingExtensionsTemplate` is declared `[IntentManaged(Mode.Fully, Body = Mode.Merge)]`. The emitted file must be **fully** managed with no merge-managed region, or a hand edit survives regeneration and quietly passes the test it should fail. T6.2's delete-then-regenerate is what finally proves this

- [x] T5. Domain Interactions call sites
  - [x] T5.1 [code] (satisfies: R3.1, R3.2, R3.3, R3.4, R3.5, R3.6, R3.7, R3.8) — Add `ObjectMappingMappingStrategy`
    - Fourth implementation of the existing `IMappingStrategy` extension point, alongside `AutoMapperMappingStrategy` and `MapperlyMappingStrategy` — no new abstraction
    - `IsMatch` keys off `ExecutionContext.InstalledModules` containing `Intent.Application.Dtos.ObjectMapping`
    - `ImplementMappingStatement` emits one statement covering all three non-paged shapes: `?` when the return type is nullable (R3.4), `List` suffix when it is a collection (R3.3), never an argument (R3.2/R3.5)
    - The `TryGetTypeName("Intent.Application.Dtos.EntityDtoMappingExtensions", …)` call is **load-bearing here**, unlike in AutoMapper's strategy: it registers the `using` (R3.6) **and** its false result is the guard that stops a Call Site referencing a Mapping Extension Class that was never generated (R3.7)
    - `HasProjectTo()` returns `false` (R3.8) — that is what makes the existing `ElementException` in `QueryActionContext` fire with a clear message under `ProjectTo`, rather than generating a query that cannot translate
  - [x] T5.2 [code] (satisfies: R4.1, R4.2, R4.4, R4.5; ~~R4.3~~ deferred) — Implement `ImplementPagedMappingStatement` on the same strategy
    - `return {entity}.{mappingMethod}(x => x.MapTo{Dto}());` — the projection is the strategy's only contribution; page metadata comes from the pagination module untouched (R4.2). **Exercised on the offset-paged path only** — no Flavour Application models a cursor-paged query, so R4.3 stays unverified
    - Never leave a paged return unmapped or defaulted (R4.5); an empty match returns an empty page with metadata intact (R4.4)
  - [x] T5.3 [code] (satisfies: R3.1, R8.12) — Register the strategy and bump `Intent.Application.DomainInteractions`
    - One registration line beside the existing two in `DomainInteractionRegistration.cs:34`. No priority needed — `IsMatch` keys off the installed module and the three strategies are mutually exclusive in a well-formed application; the two-providers case is R5's accepted risk and the existing provider already logs it
    - Version-bump the module (currently `1.2.7`) across `.imodspec`, `.csproj` and designer Module Settings, with release notes — R3 and R4 are unsatisfiable in either Flavour Application until this ships

- [x] T6. Dogfood & Software Factory parity
  - [x] T6.1 [module] (satisfies: R5.1, R8.6) — Install the rebuilt modules into both Flavour Applications
    - Build and reinstall `Intent.Application.Dtos.ObjectMapping` (at T3.2's version) and `Intent.Application.DomainInteractions` (at T5.3's version) into both `ObjectMapping.Strict` and `ObjectMapping.Lenient`
    - Pin `Null Path Handling` to `Strict` in the Strict application and `Lenient` in the Lenient application (R8.1) — the setting exists from T3.1
  - [x] T6.2 [code] (satisfies: R8.5, R8.6, R8.7, R9.6, R1.9) — Delete the hand-written Golden Sample, regenerate, and diff against the baseline
    - Delete all six hand-written `Mappings/*.cs` files **and** the hand-written handler Call Site bodies in both applications, then run the Software Factory. Every one must be reproduced (R8.6)
    - Diff the regenerated output against the T1.9 / T2.9 baselines — **zero unintended diffs**. Any divergence is fixed in the module, never in the baseline
    - Run the Software Factory a **second** consecutive time: it must propose zero changes (R8.6). A surviving merge-managed body here is the R8.7 / R9.6 failure — the file must be fully managed
    - Both solutions build with exit code 0 and both suites pass unchanged at solution level (R8.5). **If a test has to change, the module is what gets fixed** — the suites were authored against the Golden Sample precisely so they are an independent oracle
  - [x] T6.3 [code] (satisfies: R8.10) — Write the flavour-equivalence script
    - Normalises the application-name prefix out of both applications' generated `Mappings` folders and diffs them; exits non-zero on any difference outside the four R1-governed entries — `CouponId`, `CustomerCity`, `CouponPercentOff`, `CouponKind`
    - Lives in the repository, outside both applications' output, so no code-management directive is needed and neither solution depends on the other (design D3)
  - [x] T6.4 [code] (satisfies: R8.12) — Update the module's README and release notes
    - `docs/README.md` and `release-notes.md` in `Intent.Modules.Application.Dtos.ObjectMapping`, covering `Null Path Handling` (both values, the default, and the runtime consequence of each), the `src.` / `projectFrom.` Prefix Form normalisation, the Call Site behaviour Domain Interactions now generates, and the `IQueryable` projection trade-off implied by `HasProjectTo() == false`
    - Same change as the behaviour, not a follow-up

## Task Dependency Graph

```json
{
  "waves": [
    { "id": 1, "tasks": ["T1.1", "T1.2", "T1.3", "T1.4", "T1.5", "T1.6", "T1.7", "T1.8", "T1.9"], "label": "Golden Sample — Strict (HARD GATE)" },
    { "id": 2, "tasks": ["T2.1", "T2.2", "T2.3", "T2.4", "T2.5", "T2.6", "T2.7", "T2.8", "T2.9"], "label": "Golden Sample — Lenient (HARD GATE)" },
    { "id": 3, "tasks": ["T3.1", "T3.2"], "label": "Module metamodel — Null Path Handling setting" },
    { "id": 4, "tasks": ["T4.1", "T4.2", "T4.3", "T4.4", "T4.5", "T4.6"], "label": "Object Mapper expression builder" },
    { "id": 5, "tasks": ["T5.1", "T5.2", "T5.3"], "label": "Domain Interactions call sites" },
    { "id": 6, "tasks": ["T6.1", "T6.2", "T6.3", "T6.4"], "label": "Dogfood & SF parity" }
  ]
}
```

### Why the waves fall here

**Waves 1 and 2 are a single hard gate split for size.** The design specifies one Wave 0 covering both Flavour Applications, but that is ~12 implementation points of hand-written C# — two applications' worth of mapping classes, Call Sites and xUnit suites — and one coding agent implements a whole wave. Splitting per application keeps each wave at ~6 points. **Wave 3 does not start until both waves are green**: no module code is touched while a Golden Sample is red, which is the discipline the gate exists to enforce.

**Wave 3 before Wave 4** is a hard ordering constraint, not a preference. The Lenient branch in `BuildPath` (T4.1) reads the settings accessor, and that accessor does not exist until the Software Factory has run over the Settings Group T3.1 models.

**Wave 5 after Wave 4** follows R3's dependency on R6: a Call Site cannot invoke a Mapping Method that is not being generated correctly, so the strategy is written against a builder that already emits the right thing.

**Wave 6 last, by construction.** R8 depends on everything, and R9 is verified there rather than built — it constrains how R1, R2 and R6 may be implemented (checked in T4.6) and is finally proven by the delete-and-regenerate parity diff.

**No forward references.** Every element each wave names exists by the time that wave runs: Waves 1 and 2 create their own applications, entities and DTOs before mapping against them (Domain modelled before Services in both); Wave 3 touches only the module's own Module Builder package; Wave 4 reads a setting Wave 3 created; Wave 5 calls Mapping Methods Wave 4 corrected; Wave 6 installs into applications Waves 1 and 2 built and diffs against baselines they snapshotted.

**One deliberate throwaway.** The Wave 1 and 2 hand-written mapping files are the only code in this plan written to be deleted. T1.9 and T2.9 snapshot them into the spec folder first — without that, T6.2's parity diff has nothing to compare against.
