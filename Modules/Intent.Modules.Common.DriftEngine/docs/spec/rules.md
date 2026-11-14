# Rules (blocks 3 and 4)

A rule reads a claim's value from the code graph and returns a [reading](claims-and-findings.md#the-reading). There are four kinds, and all return the same reading ([R45](requirements.md)):

| Kind      | For                                                                                          | Written as                                                   | Phase                          |
| --------- | -------------------------------------------------------------------------------------------- | ------------------------------------------------------------ | ------------------------------ |
| Structure | Types, members, signatures and enums, for any language with a language profile (block 3)     | built in, configured by conventions and the language profile | 1                              |
| Recipe    | A value at one place in the code: find it, then read it                                      | JSON, a superset of Common.Drift's                           | 1                              |
| Query     | A value that needs joining code with code, or code with the model, such as a layer's imports | JSON pipeline                                                | 3                              |
| Plug-in   | Anything else                                                                                | C#, in a pack                                                | 1 (interfaces), 4 (first uses) |

## Choosing a rule

A pack's rules file maps claim kinds to rules for one designer, optionally narrowed by element (`for`, as in [vocabulary](vocabulary.md#a-vocabulary-entry)). A kind may map to one rule or to a list of rules, each narrowed by its own `for`, so one pack can read a kind differently for two element types. Exactly one rule must apply to each claim:

- none gives `noRule` for that claim;
- two from one pack are a `config/ruleConflict` error, and so are two from different packs, unless one declares `"replaces": "<pack>/<kind>"`, which is how a house pack overrides a stack pack's recipe for its codebase.

A rule is keyed by kind, not by the pack whose vocabulary produced the claim. A framework pack's rule therefore reads claims a module's or a house pack's vocabulary states, such as On Delete from eShopCustom's own stereotype ([packs.md](packs.md#who-ships-what)).

## Structure (block 3)

Structure claims are compared from the code graph, describe-only: nothing is generated and no template is involved ([R41](requirements.md)). Common.Drift compared each type a Guardrails contract template generated with its file; that tied structure to C# and to the templates. The engine instead locates each element's code and compares what the element can state ([R44](requirements.md)).

### Locating an element's code

1. **Types.** The pack's conventions give, per element type, the code type kinds allowed (`Class` → `class` or `record`; `Enum` → `enum`), an optional name pattern (`I{Name}` for a Repository's interface) and optional locations (globs such as `src/Contracts/**`). The element's type is the in-scope type node with that name (by default the language profile's naming), of any kind, within those locations. None gives `notFound` (so `type.exists` is contradicted), and several give `ambiguous` with every candidate ([R43](requirements.md)). The allowed kinds are `type.kind`'s expected value, so a type of the wrong kind is found and reported as the wrong kind, not as missing.
2. **Members.** Inside the located type: an attribute is a property (or, in Java, a field with its accessors); a constructor is a constructor; a parameter is a parameter by name; an enum literal is an enum member. An attribute with a `member.backingField` claim is its property, or, when the type has no property of that name, the field the claim names.
3. **Operations.** An operation is the method of its name. When several overloads have that name, the one whose parameter names are the claim's `parameterNames`, in order, is chosen, and of several with those names, the one whose parameter types match too (basislink5 has `Update(BidsDTO bid)` beside `Update(bids bid)`); failing that, the one with as many parameters; failing that, `ambiguous`. A single method of that name is the operation's method whatever its parameters, so a wrong parameter list is a contradicted `operation.parameters`, not a missing member.

The packs' conventions also name **injected parameters**: parameters the framework supplies, which are part of no operation's signature, such as `CancellationToken`, `ServerCallContext` and anything marked `[FromServices]`. `operation.parameters` and the `parameter.*` kinds skip them.

#### Which conventions apply

Conventions are keyed by element type, and each may be narrowed by a `where` expression, so an application service and a hosted service can be located differently. When two packs give conventions for the same element type that both apply to an element, the pack that requires the other wins: a house pack's conventions override a stack pack's, and a stack pack's override the core's. Two packs neither of which requires the other are a `config/conventionConflict` error for that element type.

### What is compared

| Kind                                                        | Model                                          | Code                                                       | Comparer                                       |
| ----------------------------------------------------------- | ---------------------------------------------- | ---------------------------------------------------------- | ---------------------------------------------- |
| `type.exists`                                               | the element                                    | its type node                                              | `presence`                                     |
| `type.kind`                                                 | the code kinds the conventions allow           | `typeKind`                                                 | `within`                                       |
| `type.abstract`                                             | `abstract` on the element                      | the `abstract` modifier                                    | `exact`                                        |
| `type.generalization`                                       | the generalization's target                    | the base class, by name, resolved where possible           | `type`                                         |
| `type.generics`                                             | the element's generic type parameters          | `typeParameters`                                           | `list`                                         |
| `member.exists`                                             | the attribute, operation or constructor        | the member node                                            | `presence`                                     |
| `member.type`                                               | the attribute's type reference                 | the property's type ref                                    | `type`                                         |
| `member.nullable`                                           | the type reference's nullable                  | the type ref's `nullable`                                  | `exact` (an `unknown` reading is `unresolved`) |
| `member.static`                                             | `static` on the element                        | the `static` modifier                                      | `exact`                                        |
| `operation.returns`                                         | the operation's type reference                 | the return type, unwrapped of the profile's async wrappers and the conventions' result wrappers | `type`                    |
| `operation.parameters`                                      | the parameters, in order, each `name: type`    | the method's parameters, less injected ones                | `list` of `type`                               |
| `parameter.type`, `parameter.nullable`, `parameter.default` | the parameter's type reference and default     | the parameter node                                         | `type`, `exact`, `exact`                       |
| `enum.literals`                                             | the literals, in model order                   | the enum members                                           | `subsequence`                                  |
| `enum.literal.value`                                        | the literal's value, where modelled            | the member's `value`                                       | `number`                                       |

All but `parameter.default` and `enum.literal.value` are `stated: always`: a class with no `abstract` modifier states that it isn't abstract. A modelled member that isn't there is contradicted ([R8](requirements.md)). An unmodelled member is never a finding unless the `unmodelled.member` opt-in rule is on.

Four more structure kinds take their expected value from vocabulary and conventions rather than from the element alone: `type.location` (where the type's file must be), `type.base` (a base type the model implies, such as `IAggregateRoot` for an aggregate root), `type.marker` (a named marker, present exactly when modelled) and `file.exists` (a file an element stands for, such as a Vue component). [claim-catalog.md](claim-catalog.md#structure) lists every structure kind.

### Language profiles

A language profile is pack data ([R42](requirements.md)). Its schema is [`schemas/language-profile.schema.json`](schemas/language-profile.schema.json). One run may read several languages, and each file's code is compared under its own language's profile: an element's type is looked for under the name each language gives it, and a member, a type reference or a parameter list is read as its own language writes it.

| Section       | Says                                                    | C# example                                           | Java example                              |
| ------------- | ------------------------------------------------------- | ---------------------------------------------------- | ----------------------------------------- |
| `types`       | Each model type's names in code                         | `datetime` → `DateTime`, `System.DateTime`           | `datetime` → `LocalDateTime`, `Instant`   |
| `collections` | The generic types and forms that mean "a collection of" | `List<T>`, `ICollection<T>`, `IEnumerable<T>`, `T[]` | `List<T>`, `Set<T>`, `Collection<T>`      |
| `async`       | Wrappers unwrapped from return types                    | `Task<T>`, `ValueTask<T>`; `Task` means void         | `CompletableFuture<T>`, `Mono<T>`         |
| `nullable`    | How a type ref says nullable                            | `T?` in a nullable context; `Nullable<T>`            | `@Nullable`; a boxed type for value types |
| `naming`      | From an element name to a type, member and file name    | interface `I{Name}`; the rest as modelled            | field, method `{Name\|camel}`             |
| `typeKinds`   | The code kinds the language has                         | `class`, `record`, `struct`, `interface`, `enum`     | `class`, `record`, `interface`, `enum`    |

The `type` comparer maps the model's type through `types` and `collections`, then compares with the code's type ref: model `OrderItem[]` matches C# `ICollection<OrderItem>` and Java `List<OrderItem>`. Nullability isn't part of it: model `guid?` against C# `Guid` is one finding, from `member.nullable` or `parameter.nullable`, never a second from `member.type`. A model type that is a modelled element matches the code type that element is located as: where the reader resolves names, the code's type ref must resolve to that node, so a same-named type in another namespace doesn't match; where it can't resolve, the names are compared. A library type, or a primitive the profile doesn't list, is compared by name.

Result wrappers beyond async ones belong to frameworks, so the conventions of a framework pack list them, with `T` for the result and `…` for any further type arguments: ASP.NET Core's `ActionResult<T>`, `Ok<T>`, `Results<Ok<T>, …>`; Spring's `ResponseEntity<T>`. `operation.returns` unwraps them as it unwraps `Task<T>`.

## Recipes

A recipe finds one place in the code and reads a value from it. The format is Common.Drift's, unchanged, with additions ([R46](requirements.md)). A pack's recipes for one designer are a file shaped like Common.Drift's recipe file:

```json
{
  "designer": "Domain",
  "files": "src/**/*.cs",
  "facts": {
    "column.maxLength": {
      "find": "class:{Entity}Configuration > chain:Property(x=>x.{Attribute})",
      "read": [{ "match": "\\.HasMaxLength\\((\\d+)\\)" }]
    }
  }
}
```

`files` narrows a recipe within scope; it can never widen it ([scope.md](scope.md#what-scope-decides)). `own` and `exceptions` aren't part of a pack's recipe file: they become opt-in rules and the model's exceptions ([conversion.md](conversion.md)).

### What carries over unchanged

Every recipe property (`find`, `read`, `default`, `absent`, `unstated`, `inherit`, `replace`, `case`, `files`, `note`), every read case property (`match`, `value`, `part`, `in`, `case`, `unreadable`), every find step (`class:`, `method:` with its overload form, `property:`, `parameter:`, `statement:`, `chain:`, `file:`, `<kind> with <regex>`, a bare `<kind>`), every placeholder filter (`camel`, `pascal`, `kebab`, `lower`, `upper`, `trimStart:`, `trimEnd:`, `replace:`), .NET regular expressions, and `//` comments. Each reads the same canonical text it read in Common.Drift ([code-reader.md](code-reader.md#canonical-text)).

A `<kind> with <regex>` step keeps the nodes of that kind whose canonical text the regular expression matches anywhere, as Common.Drift's does; `^` and `$` anchor to the start and end of the node's text. A markup element's text includes its children's, so for `markup` the step keeps only the innermost matching elements: an element is dropped when an element inside it matches too.

A read case gives text. For a `type` kind, that text is read as a type written in the file's language and resolved where the file's reader resolves names; a `node` read case that names a type ref field (`node: "type"`, `node: "returnType"`) gives that type ref as it is.

Two behaviours change, both listed in [conversion.md](conversion.md#semantic-changes):

- `default` and `absent` are refused for a `whenSet` kind (a `config/silenceForWhenSet` error). The model states those values only where the code does (decision 4).
- A find that ends at several places is `ambiguous` and names every candidate.

### Additions

| Addition                       | Example                                                   | Why                                                                                    |
| ------------------------------ | --------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| `type:Name` step               | `type:{Entity}`                                           | `class:` reads as C#; `type:` names any language's types. `class:` stays as an alias.  |
| `annotation:Name` step         | `type:{Entity} > annotation:Table`                        | C# attributes, Java annotations and decorators as nodes, not regular expressions       |
| `call:Name` step               | `method:Configure > call:ToTable`                         | A call, whatever the chain around it                                                   |
| `markup:Tag`                   | `type:{Component} > markup:PageTitle`                     | Markup without regular expressions. A Razor directive is the code it compiles to: `@page` is `annotation:Route` |
| framework node steps           | `ef.entity:{Entity}`                                      | Framework readers' nodes                                                               |
| `base` step                    | `type:{Entity} > base`                                    | The base types, nearest first; needs `resolution`                                      |
| `node` read case               | `{ "node": "arguments[0].literal" }`                      | Read a node field, with no regular expression                                          |
| `map`                          | `"map": { "nvarchar": "NVARCHAR", "varchar": "VARCHAR" }` | A value table applied after `replace`                                                  |
| `pascal`, `camel` for `case`   | `"case": "pascal"`                                        | Case conversion for read values, beside `upper` and `lower`                            |
| `inherit` as a list            | `"inherit": ["method", "class"]`                          | Common.Drift already accepted it; now documented                                       |
| `compare`                      | `"compare": "ignoreCase"`                                 | A recipe-specific comparer, such as SQL types that differ only in case in one codebase |
| `matches: "any"`               | `"matches": "any"`                                        | For `presence` and `bool` kinds: a find that ends at several places reads its first, in path and line order, rather than `ambiguous`. A Layer holding many files is populated by any of them |

## Queries

A query is a JSON pipeline over code nodes and model elements. It reads values that need a join, which no single find can reach ([R47](requirements.md)). Its schema is [`schemas/query.schema.json`](schemas/query.schema.json).

| Step       | Does                                                                                                                   |
| ---------- | ---------------------------------------------------------------------------------------------------------------------- |
| `from`     | Starts rows from code nodes of a kind (`code:import`) or model elements of a specialization (`model:Layer`)            |
| `where`    | Keeps rows for which an expression is true                                                                             |
| `let`      | Adds fields from expressions                                                                                           |
| `flatMap`  | One row per item of a list field                                                                                       |
| `resolve`  | Follows a type ref or call to its declaration; needs `resolution`, else `unresolved`                                   |
| `ownerOf`  | Finds the model element whose path (an expression, such as a stereotype's Path) is the longest prefix of a file's path |
| `distinct` | Removes duplicate rows                                                                                                 |
| `group`    | Groups by fields, collecting others into lists                                                                         |
| `reading`  | Ends the query: for each claim, by its bindings, the value read and where each part of it is                           |

Expressions are [vocabulary's](vocabulary.md#expressions), plus the fields of code nodes.

How rows become readings:

- With `item`, the rows that reach one claim are collected into its list or set value, each item with its own `at`.
- With `value`, the rows that reach one claim must agree; rows with different values make the claim `ambiguous`, naming each.
- A claim no row reaches takes the query rule's `absent` value when it has one, marked as silence; `"absent": ""` with `item` gives an empty list or set, so a Layer that imports nothing verifies under `within`. Without `absent` the claim is `notFound`. As for recipes, `absent` is refused for a `whenSet` kind (`config/silenceForWhenSet`).

Example, for `layer.dependencies` (phase 3): each Layer's model value is the set of Layers it may depend on, and the code's is the set of Layers its files import from.

```json
"layer.dependencies": {
  "query": [
    { "from": "code:import" },
    { "ownerOf": { "path": "file", "among": "model:Layer", "by": "stereotype('Layer Settings').Path", "as": "fromLayer" } },
    { "resolve": { "target": "target", "as": "imported" } },
    { "ownerOf": { "path": "imported.file", "among": "model:Layer", "by": "stereotype('Layer Settings').Path", "as": "toLayer" } },
    { "where": "exists(fromLayer) && exists(toLayer) && fromLayer != toLayer" },
    { "reading": { "claim": { "Layer": "fromLayer.name" }, "item": "toLayer.name", "at": "span" } }
  ],
  "compare": "within"
}
```

The `within` comparer verifies when every Layer the code imports from is one the model allows. When it isn't, the finding names each offending import and its line:

> Layer dependencies of `Domain`: the code depends on `Infrastructure` at `src/Domain/Order.cs:3`, which the model doesn't allow. Remove the import, or add a May Depend On from `Domain` to `Infrastructure`.

eShopCustom's planted break 28 (`using` Infrastructure in `Order.cs`) is that case. A C# `using` names a namespace, not a type, so for C# `resolve` maps the namespace to the files declaring it; the Java spike's planted case imports a class.

## Plug-ins

A plug-in is a .NET assembly in a pack, implementing one or more of these interfaces ([R48](requirements.md)). Phase 1 fixes their exact signatures; this is their contract.

| Interface          | Given                                                    | Returns                                                       |
| ------------------ | -------------------------------------------------------- | ------------------------------------------------------------- |
| `ICodeReader`      | in-scope files of its languages, and the project nodes   | graph nodes and edges, and its capabilities                   |
| `IFrameworkReader` | committed artifacts its patterns match                   | framework nodes, located in the artifacts                     |
| `IClaimReader`     | one claim (kind, element, bindings) and the scoped graph | a reading                                                     |
| `IValueComparer`   | the expected value and a reading                         | verified or contradicted, with the text to show for each side |
| `IModelFunction`   | a model element and arguments                            | a value; it declares the stereotypes and properties it reads  |
| `IOptInRule`       | the scoped graph and the claims' readings                | the code it finds unmodelled                                  |

The rules a plug-in must follow:

- It sees only in-scope code, through the graph. It never opens files itself.
- It never writes anything: no files, no model.
- It throws only for its own bugs. A throw is caught and becomes `ruleFailed` on each claim the plug-in was reading, plus one `config/pluginFailed` finding.

Any rule, not only a plug-in, fails as `ruleFailed` when it returns something malformed: a value that doesn't fit the kind's value type (text for a `bool` or `number` kind), or a node field path that doesn't exist on the node it reached. What can be checked without code (a recipe that doesn't parse, a query step naming an unknown field of a model element, a read case's bad regular expression) is checked when the pack loads, as a `config/ruleInvalid` error, and the rule doesn't run.

## Opt-in "must be modelled" rules

By principle 1, code the model doesn't mention is never drift. A team that wants some kind of code always modelled turns a rule on in the model's `Drift Check` stereotype ([scope.md](scope.md#the-drift-check-stereotype)). Each rule applies within scope only, and each finding can be excepted, with a reason ([R49](requirements.md)).

| Rule                     | Reports                                                                                                                      | Replaces                                 |
| ------------------------ | ---------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------- |
| `unmodelled.type`        | An in-scope type that follows an element type's convention (`type with :Entity\\b` in `src/Domain/**`) and no claim located  | Common.Drift's `own` conventions         |
| `unmodelled.member`      | A public member of a located type, of a kind its element type can state, that no claim located                               | Common.Drift's unmodelled public members |
| `unmodelled.interaction` | A call the model's interactions can state (a repository's `Add` in a handler) that no `intent.*` claim located               | eShopCustom's code-only interactions     |
| `unmodelled.exposed`     | An in-scope type an endpoint exposes (a request or response type) that no element describes                                  | eShopCustom Services' exposure check     |
| `unmodelled.file`        | An in-scope file in which no claim located anything                                                                          | eShopCustom's Strict scope mode          |
| `unmodelled.value`       | An explicit value the code states for a `whenSet` kind that the model leaves unset (a `ToTable` with no modelled table name) | decision 6                               |

Code is **located** when some claim's reading came from it or from code inside it, as Common.Drift's ownership is. An element's type is located by its `type.exists` reading, so "the type an element is located as" and "code a claim located" are the same idea.

What each rule looks at, precisely:

- **`unmodelled.type`** reports in-scope types the conventions' `find` reaches (per element type, narrowed by `where`) that no claim located.
- **`unmodelled.member`** looks inside located types only, as do the conventions' `members` entries. It reports the public properties, fields, methods, constructors and events of a kind the element type can state (its type convention's `states`: a Class's properties, fields, constructors and methods, a DTO's properties and fields) that no claim located. An override of a modelled base type's member, a record's primary constructor, a located field's accessors (the language profile's `accessors`, as Java's `getName`) and the members every type has (`Equals`, `GetHashCode`, `ToString`; Java's `equals`, `hashCode`, `toString`) are never reported. An override of a framework's base (a hosted service's `StopAsync`) is the type's own, and is. A `members` convention's find path may end inside a member, at a parameter or a call; the hit is unmodelled when no claim's reading came from it. Parameters aren't members of their own: a parameter list is `operation.parameters`.
- **`unmodelled.interaction`** reports the calls the conventions' `interactions` reach inside located methods that no `intent.*` claim located.
- **`unmodelled.exposed`** follows what an endpoint exposes: each located operation's parameter types and unwrapped return type, then, transitively, the types of those types' public properties. It reports each in-scope type on the way that no element describes. Library types and generic type parameters are skipped.
- **`unmodelled.file`** reports in-scope files in which no claim located anything.
- **`unmodelled.value`** runs a `whenSet` kind's rule for each element of the specialization the kind's vocabulary entry selects (its `for`, ignoring `where` and `when`) that has no claim of that kind, and reports each value read from code. Silence is never a value, so a framework convention is never reported.

A finding's subject is where the code is: the line a node starts on (its first attribute or annotation, if it has any), or line 1 for a file. It is in the Area of the designer whose convention or element found it (`Domain (DriftEngine)`), and names the element whose type holds the code, if any; `unmodelled.file`'s is in `Drift Scope (DriftEngine)`. A rule reports nothing in a file a `Context` scope row decides: that code is read, not checked ([scope.md](scope.md#what-scope-decides)).

A module adds an opt-in rule as an `IOptInRule` (`OptInRules` on its extension): given the run, it returns the code it finds unmodelled, using what the claims located (`RuleContext.IsLocated`), and the engine applies everything else above the same way as for its own rules. A module's rule of the same id replaces the engine's. A rule turned on whose convention has a find path the engine can't read is a `config/ruleInvalid` error, and the rule doesn't run.

An exception is a row of `Rule`, `Match` and `Reason` on the `Drift Check` stereotype. `Match` is a file glob (relative to the output folder), a type name, a `Type.Member`, or the name a finding gives. An exception that matches nothing is an `optin/staleException` warning; one with no reason is a `config/exceptionReason` error. Both name the Root Folder that holds the row, and the row.

Each opt-in rule has one default severity, an error. A team that wants some of a rule's findings as warnings overrides the rule's severity ([claims-and-findings.md](claims-and-findings.md#overrides)); eShopCustom's notes-only checks become errors unless it does.

Opt-in rules are policy (block 5) and arrive in phase 1 with scope. Each case's phase is the latest phase of the packs and kinds it relies on: a rule over Domain conventions is phase 1, one over EF Core configuration phase 4.

## Recipe validation

Validation runs on demand, from the model, the code and the packs, never during a Software Factory run ([R50](requirements.md)). It keeps Common.Drift's checks and extends them to every rule kind:

- **The probe**: change, in memory, the code each reading came from, read again, and warn when the same value comes back.
- **The wrong-element check**: for each kind and owner, warn when a reading for one element came from code naming a sibling's own placeholder (`own`).
- **Counts by source**: per kind, how many readings came from code, from inheritance and from silence. Silence readings are listed so a recipe that verifies everything from `absent` stands out.

Phase 1 builds it; its acceptance cases live with phase 1, not in this suite.
