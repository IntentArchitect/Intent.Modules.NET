# Rule audit: can the engine do every house rule?

2026-11-13. Every rule in eShopCustom's four modules (`D:\Dev\Temp\eShopCustom\intent\modules`) and the Java spike's four (`D:\Dev\Spikes\Samples.eShop.Demo.Java`: Java backend, Vue/TypeScript front end), each checked against what the engine reads. The question was whether an AI agent could write each rule as a module rule on the engine's model and code reading, with no parser of its own.

## Answer

No rule needs a different design. Every model-to-code rule fits a claim (a source states it, a reader answers it), and every code-to-model, package or cross-element rule fits a rule. What's missing is facts the engine doesn't read yet, plus two pieces of shared logic every house module rebuilt (sections C and D).

| Slice | Rules | On the engine now | Writable today | Need engine work | Not drift |
| --- | --- | --- | --- | --- | --- |
| eShopCustom Domain + Architecture | 78 | 45 | 20 | 5 | 8 |
| eShopCustom Services | ~90 | ~15 | ~30 | ~25 | the rest (warnings about code the model can't state) |
| eShopCustom FrontEnd (Blazor) | 52 groups | 2 | ~15 | ~20 | the rest |
| Java spike backend (Java) | 65 | 0 (no Java language) | most, once a Java language fills today's graph | ~15 | ~10 model-only |
| Java spike FrontEnd (Vue + TS) | 25 | 0 (no TS language) | 6, once a TS language exists | 15 | 4 model-only |

"Not drift" is a warning about code the model can't express, or a model-only check that belongs to designer validation. The engine doesn't report either, by design.

## A. Facts the engine reads wrongly or not at all (small, every slice)

| # | Gap | Needed by |
| --- | --- | --- |
| A1 | **Bug:** a property's or field's access is never set, so it always reads `public`, though this module's CONTEXT says every member's access is read. Also Java package-private, and an exported flag on TS types | unmodelled-property rules, DTO fields, exposure, Java fields |
| A2 | **Bug:** a class's primary-constructor parameters are read as properties (a record's are, correctly) | DTO/Model Definition properties |
| A3 | Modifiers beyond static and abstract: override, virtual, transient, final, default | unmodelled methods, hosted/gRPC overrides, Java fields |
| A4 | A parameter's default value | parameter defaults (Domain, Services) |
| A5 | An enum member's value | enum literal values (Domain, Services) |
| A6 | A type's own type parameters (`PagedResult<T>`) | generic DTOs, Model Definitions, exposure |
| A7 | Union types (TS `string \| number`) | TS contracts |
| A8 | A nested type's containing type | Java nested records |

## B. Reading inside bodies (small to medium)

| # | Gap | Needed by | Size |
| --- | --- | --- | --- |
| B1 | An argument's literal value (string, number, interpolation parts), not just its text | Domain package Schema, Services roles/policies/versions, Java `Http.parseEnum`, FrontEnd URIs | small |
| B2 | Type arguments on a call (`AddSubscription<M, H>()`, `AddScoped<T>()`) | subscriptions, registrations | small |
| B3 | Call chains: a call made on a call (`MapGroup(..).RequireAuthorization()`, `find(id).getLines().add(..)`) | Services security/versions, Java entity actions | small–medium |
| B4 | Call targets across types: which method a call reaches, its return type as the value, method references as calls | Java entity actions through beans, `AddDomainEvent(Helper())` | medium |
| B5 | Creation arguments and object initializers | FrontEnd verbs and headers | small–medium |
| B6 | Structured annotation values: arrays and nested annotations (`cascade = {ALL}`, `@UniqueConstraint(..)`) | Java JPA and security | small |
| B7 | A lambda as its own unit: parameters with attributes, its own calls, assignments | Minimal API lambda endpoints, options lambdas | medium–large |
| B8 | Top-level code as a unit: Program.cs; TS module-level functions and consts; Vue `<script setup>` | registrations, subscriptions, TS clients | medium |
| B9 | Import detail and resolution: resolved file, type-only, wildcard or static, using aliases | TS/Vue uses, Java imports, alias type references | small |
| B10 | String evaluation: a URI built through fields, interpolation and helper methods | Blazor and TS proxies, navigation | large |

## C. Engine plumbing and C# left in the core (small)

| # | Gap |
| --- | --- |
| C1 | Member names compare exactly, so Java's `name` never matches a model's `Name`. The language profile needs a naming convention |
| C2 | C# assumptions in the core: `operation.returns` unwraps `Task`/`ValueTask` by name, the type comparer ignores generic arguments, the Repository convention is `I{Name}`. They belong in the profile or a stack |
| C3 | One language profile per run (the language with the most files), so a C# + TS application compares one language's types with the other's profile |
| C4 | `RuleContext` has no model helpers (`Links`, `Stereotype`) and no type comparison; `ModelContext` can't reach another application's referenced packages |
| C5 | A claim can't sit on a package (Domain package Schema) |
| C6 | The Minimal API reading, scope model and type comparer are internal, so a module repeats them |
| C7 | The route comparer drops `?` after a constraint (`{x:int?}`) and compares parameters by name, not position |
| C8 | Nullable context per file, not per `#nullable` region |

## D. Shared logic every house module rebuilt (medium, a decision each)

- **D1. "Code the model doesn't describe."** Every house module reports code ahead of the model as Backlog: an unmodelled type, member, endpoint or link. The spec's opt-in `unmodelled.*` rules aren't built, so each module would rewrite "what no claim located" itself, which is the shared logic modules aren't meant to carry.
- **D2. Read for context, but don't check.** Rules need facts from files that mustn't be checked themselves: Program.cs registrations and subscriptions, the DbContext. The engine reads only in-scope files, and making them In makes other checks report their types. Scope needs a third choice beside In and Out.
- **D3. Finding shape.** A claim's finding is always an error on the element. House modules used warnings and anchored Backlog on the code. This is decided with D1.

## E. New languages and file kinds (large)

| # | Language | Needed by | Note |
| --- | --- | --- | --- |
| E1 | Java | the whole Java backend | The spike reads Java with regexes, so there's no parser to reuse. The Java language needs a real one |
| E2 | TypeScript + Vue single-file components | the Java spike's front end | |
| E3 | Razor | the Blazor front end | Merged with the code-behind into one type; `@page`, `@inject`, `@code` |
| E4 | Markup nodes (elements, attributes, bindings, routes) | Razor and Vue alike | The spec's `markup` and `directive` nodes; one neutral shape for Razor, Vue and Angular |
| E5 | `.proto` | gRPC services and messages | |
| E6 | Project and config files (`RootNamespace`, `pom.xml`, `application.yml`) | namespaces; Spring naming strategies | small |

Framework knowledge stays in stacks, as EF Core and ASP.NET Core do: JPA and Spring MVC read the same `intent.rdbms` and `intent.http` claims; Blazor and Vue would turn markup into shared UI facts; HttpClient and fetch would give an outbound request fact.

## Order

Java is proven before anything else is built on the engine (user decision, 2026-11-13).

1. **A1–A6 and C1–C4.** Small. They unblock Java and most writable rules, each with acceptance cases. Done 2026-11-13: A1–A6, B6, C1 and C2, and an ANTLR spike that parses every Java eShop backend file. C3 and C4 are left; C3 matters once one run reads two languages, as the Java eShop's Java and TS will.
2. **The Java language (E1)** with a `java` profile, then the stock structure checks on the Java spike: a clean baseline, then planted drift.
3. **JPA and Spring MVC stacks**, then one Java house rule on the same API as the C# ones. Java's entity actions are its equivalent of Services' sends, and they need B4.
4. **D1 and D2**, the shared "code the model doesn't describe" rules and the read-only scope tier.
5. **B1–B9, then E2–E5, then B10**, as the remaining rules need them.
