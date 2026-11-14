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
| A7 | Union types (TS `string \| number`). **Done 2026-11-14** with E2: `TypeRefNode.Union` and `Literal`; `null` and `undefined` members only make it nullable | TS contracts |
| A8 | A nested type's containing type | Java nested records |

## B. Reading inside bodies (small to medium)

| # | Gap | Needed by | Size |
| --- | --- | --- | --- |
| B1 | An argument's literal value (string, number, interpolation parts), not just its text. **Done 2026-11-14** for C# and Java: `ValueNode.Literal`, also on annotation arguments and initializers | Domain package Schema, Services roles/policies/versions, Java `Http.parseEnum`, FrontEnd URIs | small |
| B2 | Type arguments on a call (`AddSubscription<M, H>()`, `AddScoped<T>()`). **Done 2026-11-14** for C# and Java: `CallNode.TypeArguments` | subscriptions, registrations | small |
| B3 | Call chains: a call made on a call (`MapGroup(..).RequireAuthorization()`, `find(id).getLines().add(..)`). **Done 2026-11-13** for C# and Java: `CallNode.On` | Services security/versions, Java entity actions | small–medium |
| B4 | Call targets across types: which method a call reaches, its return type as the value, method references as calls. **Done 2026-11-13** for C# and Java: `CallNode.Target`, `CodeGraph.Targets` | Java entity actions through beans, `AddDomainEvent(Helper())` | medium |
| B5 | Creation arguments and object initializers. **Done 2026-11-14** for C# and Java: `MemberNode.Creations`, and a value's `Creations` through locals | FrontEnd verbs and headers | small–medium |
| B6 | Structured annotation values: arrays and nested annotations (`cascade = {ALL}`, `@UniqueConstraint(..)`) | Java JPA and security | small |
| B7 | A lambda as its own unit: parameters with attributes, its own calls, assignments. **Done 2026-11-14** for C# and Java: `ValueNode.Function`, a `FunctionNode` as a member's body is | Minimal API lambda endpoints, options lambdas | medium–large |
| B8 | Top-level code as a unit: Program.cs; TS module-level functions and consts; Vue `<script setup>`. **Done 2026-11-14** for C#: `FileNode.TopLevel`; and with E2 for TS and Vue: a module's functions and variables are its `module` type's members, what it runs (a `<script setup>`'s code) its `TopLevel` | registrations, subscriptions, TS clients | medium |
| B9 | Import detail and resolution: resolved file, type-only, wildcard or static, using aliases. **Done 2026-11-14** for C# and Java: `ImportNode.Wildcard`, `CodeGraph.TypeOf(import)`, and a type name resolved through aliases and imports; and with E2 for TypeScript: one import per binding, with `Module`, the resolved `File` and `TypeOnly` | TS/Vue uses, Java imports, alias type references | small |
| B10 | String evaluation: a URI built through fields, interpolation and helper methods. **Done 2026-11-14** for C# and TypeScript: `CodeGraph.StringOf`, one evaluation over facts every language gives a value (`Joined`, `Given`, `Parameter`, `Field`, `Call`, `Trimmed`; `FunctionNode.Changes`). Checked by hand against the code on every request and navigation URI of eShopCustom's front ends and the Java spike's API client. Markup values and Java aren't evaluated yet | Blazor and TS proxies, navigation | large |

## C. Engine plumbing and C# left in the core (small)

| # | Gap |
| --- | --- |
| C1 | Member names compare exactly, so Java's `name` never matches a model's `Name`. The language profile needs a naming convention |
| C2 | C# assumptions in the core: `operation.returns` unwraps `Task`/`ValueTask` by name, the type comparer ignores generic arguments, the Repository convention is `I{Name}`. They belong in the profile or a stack |
| C3 | One language profile per run (the language with the most files), so a C# + TS application compares one language's types with the other's profile. **Done 2026-11-14**: each file's code under its own language's profile |
| C4 | `RuleContext` has no model helpers (`Links`, `Stereotype`) and no type comparison; `ModelContext` can't reach another application's referenced packages. **Done 2026-11-14**: `RuleContext.Model` and `TypeMatches`; `ModelContext.References` and `ReferencedElements` |
| C5 | A claim can't sit on a package (Domain package Schema) |
| C6 | The Minimal API reading, scope model and type comparer are internal, so a module repeats them |
| C7 | The route comparer drops `?` after a constraint (`{x:int?}`) and compares parameters by name, not position |
| C8 | Nullable context per file, not per `#nullable` region |

## D. Shared logic every house module rebuilt (medium, a decision each)

- **D1. "Code the model doesn't describe."** Every house module reports code ahead of the model as Backlog: an unmodelled type, member, endpoint or link. The spec's opt-in `unmodelled.*` rules aren't built, so each module would rewrite "what no claim located" itself, which is the shared logic modules aren't meant to carry.
- **D2. Read for context, but don't check.** Rules need facts from files that mustn't be checked themselves: Program.cs registrations and subscriptions, the DbContext. The engine reads only in-scope files, and making them In makes other checks report their types. Scope needs a third choice beside In and Out.
- **D3. Finding shape.** A claim's finding is always an error on the element. House modules used warnings and anchored Backlog on the code. This is decided with D1.

Done 2026-11-14. D1: the Root Folder's `Drift Check` turns on `unmodelled.file`, `unmodelled.type` and `unmodelled.member`, or a module's own `IOptInRule`, with exceptions; "located" is what the claims located, so no module rebuilds it. `unmodelled.type` and a designer's `members` conventions find code by find paths over canonical text. D2: a `Context` scope row is read, never reported on. D3: an opt-in finding is anchored on the code, with its element where there is one, and severity overrides make any non-info finding a warning or an error. Left: `unmodelled.interaction`, `.exposed` and `.value`, the `chain` find step, and locating a Minimal API router as its Service. The `markup` find step came with E4.

## E. New languages and file kinds (large)

| # | Language | Needed by | Note |
| --- | --- | --- | --- |
| E1 | Java | the whole Java backend | The spike reads Java with regexes, so there's no parser to reuse. The Java language needs a real one |
| E2 | TypeScript + Vue single-file components | the Java spike's front end | **Done 2026-11-14**: TypeScript's own parser on Node.js; modules, `module` and `object` types, Vue components with props, emits and models, bodies and call targets, imports through `tsconfig.json`. It reads the Java spike's 32 front-end files in under a second with no failures. A `.vue` file's `<template>` is E4's |
| E3 | Razor | the Blazor front end | **Done 2026-11-14**: a `.razor` component is the C# the Razor compiler (6.0) generates for it, one `component` type with its code-behind, in its project's root namespace and folders or its `@namespace`. `@page`, `@layout` and `@attribute` are its attributes, `@inherits` and `@implements` its bases, `@typeparam` its type parameters, `@inject` a property, `@code` its members with bodies and call targets, `@using` its imports, and its `_Imports.razor` files' apply to it. Each is placed where it is written. It reads eShopCustom's 40 `.razor` files with no failures. Markup is E4's |
| E4 | Markup nodes (elements, attributes, bindings, routes) | Razor and Vue alike | **Done 2026-11-14**: `FileNode.Markup`, one shape for Razor and Vue: elements with their tag, the component it names (resolved like any type), attributes with Razor's and Vue's directives split alike, and content and plain values as literals with code as holes. Razor's from its compiler's parse before its passes; Vue's from a reader that matches Vue's own compiler line for line on the Java spike's 8 components. The `markup` find step. There's no `directive` node: a Razor directive is the code it compiles to |
| E5 | `.proto` | gRPC services and messages | **Done 2026-11-14**: the `intent.protobuf` language, its own parser of proto2, proto3 and editions. Messages, enums and services are types, fields properties with their numbers, rpcs methods with `stream` as a type over the message, options annotations, imports resolved to in-scope files, and nullability as field presence. It reads 72 real files, Google's `descriptor.proto` among them, as protobuf-net's parser does, line for line. gRPC's conventions (protoc's naming) are `intent.grpc`'s, still to come |
| E6 | Project and config files (`RootNamespace`, `pom.xml`, `application.yml`) | namespaces; Spring naming strategies | small. A `.csproj`'s `RootNamespace` is read for Razor components' namespaces (E3) |

Framework knowledge stays in stacks, as EF Core and ASP.NET Core do: JPA and Spring MVC read the same `intent.rdbms` and `intent.http` claims; Blazor and Vue would turn markup into shared UI facts; HttpClient and fetch would give an outbound request fact.

## Order

Java is proven before anything else is built on the engine (user decision, 2026-11-13).

1. **A1–A6 and C1–C4.** Small. They unblock Java and most writable rules, each with acceptance cases. Done 2026-11-13: A1–A6, B6, C1 and C2, and an ANTLR spike that parses every Java eShop backend file. C3 and C4 are left; C3 matters once one run reads two languages, as the Java eShop's Java and TS will.
2. **The Java language (E1)** with a `java` profile, then the stock structure checks on the Java spike: a clean baseline, then planted drift.
3. **JPA and Spring MVC stacks**, then one Java house rule on the same API as the C# ones. Java's entity actions are its equivalent of Services' sends, and they need B4. Done 2026-11-13: E1 with the `java` profile, both stacks, Java calls and arguments, B4 and B3, and the house rule: the Java eShop's `Http` helpers as a refiner (`IMemberRefiner`), which the proof showed the engine was missing, with Spring's own `ResponseEntity` builders read by the stack. The Java Services module's entity actions, the equivalent of Services' sends, moved onto the engine 2026-11-14 (`Drift/EntityActions.cs`: a claim source over each Operation's Entity Action links and a claim reader over the graph's call targets, chains, references and creations), and its ~200 lines of regex reading are gone. The same tests pass on the engine's graph, and a planted delete in a query handler is reported at the handler. The one rule it had that reads no code (a Query beside an Update of the same entity) was dropped: a rule on the model alone is the designer's to validate, not drift.
4. **D1 and D2**, the shared "code the model doesn't describe" rules and the read-only scope tier. Done 2026-11-14, with D3.
5. **B1–B9, then E2–E5, then B10**, as the remaining rules need them. B1–B9 done 2026-11-14 for C# and Java, with C3 and C4; E2 done 2026-11-14, with A7 and B8 and B9 for TypeScript; E3, E4 and E5 done 2026-11-14, and B10 for C# and TypeScript.
