# Rules (blocks 3 and 4)

A rule reads a claim's value from the code graph and returns a [reading](claims-and-findings.md#the-reading). There are four kinds, and all return the same reading ([R45](requirements.md)):

| Kind      | For                                                                                          | Written as                                                   | Phase                          |
| --------- | -------------------------------------------------------------------------------------------- | ------------------------------------------------------------ | ------------------------------ |
| Structure | Types, members, signatures and enums, for any language with a language profile (block 3)     | built in, configured by conventions and the language profile | 1                              |
| Recipe    | A value at one place in the code: find it, then read it                                      | JSON, a superset of Common.Drift's                           | 1                              |
| Query     | A value that needs joining code with code, or code with the model, such as a layer's imports | JSON pipeline                                                | 3                              |
| Plug-in   | Anything else                                                                                | C#, in a pack                                                | 1 (interfaces), 4 (first uses) |

## Choosing a rule

A pack's rules file maps claim kinds to rules for one designer, optionally narrowed by element (`for`, as in [vocabulary](vocabulary.md#a-vocabulary-entry)). Exactly one rule must apply to each claim:

- none gives `noRule` for that claim;
- two from different packs are a `config/ruleConflict` error, unless one declares `"replaces": "<pack>/<kind>"`, which is how a house pack overrides a stack pack's recipe for its codebase.

## Structure (block 3)

Structure claims are compared from the code graph, describe-only: nothing is generated and no template is involved ([R41](requirements.md)). Common.Drift compared each type a Guardrails contract template generated with its file; that tied structure to C# and to the templates. The engine instead locates each element's code and compares what the element can state ([R44](requirements.md)).

### Locating an element's code

1. **Types.** The pack's conventions give, per element type, the code type kinds allowed (`Class` → `class` or `record`; `Enum` → `enum`) and optional location rules (a namespace, folder or name pattern, such as `*Dto` in `src/Contracts/**`). The element's type is the in-scope type node named as the language profile's naming says, among those kinds, within those locations. None gives `notFound` (so `type.exists` is contradicted), and several give `ambiguous` with every candidate ([R43](requirements.md)).
2. **Members.** Inside the located type: an attribute is a property (or, in Java, a field with its accessors); an operation is a method, chosen among overloads by its parameter types; a constructor is a constructor; a parameter is a parameter by name; an enum literal is an enum member.

### What is compared

| Kind                                                        | Model                                          | Code                                                       | Comparer                                       |
| ----------------------------------------------------------- | ---------------------------------------------- | ---------------------------------------------------------- | ---------------------------------------------- |
| `type.exists`                                               | the element                                    | its type node                                              | `presence`                                     |
| `type.kind`                                                 | the element type's code kind, from conventions | `typeKind`                                                 | `exact`                                        |
| `type.abstract`                                             | `abstract` on the element                      | the `abstract` modifier                                    | `exact`                                        |
| `type.generalization`                                       | the generalization's target                    | the base class, by name, resolved where possible           | `type`                                         |
| `type.generics`                                             | the element's generic type parameters          | `typeParameters`                                           | `list`                                         |
| `member.exists`                                             | the attribute, operation or constructor        | the member node                                            | `presence`                                     |
| `member.type`                                               | the attribute's type reference                 | the property's type ref                                    | `type`                                         |
| `member.nullable`                                           | the type reference's nullable                  | the type ref's `nullable`                                  | `exact` (an `unknown` reading is `unresolved`) |
| `member.static`                                             | `static` on the element                        | the `static` modifier                                      | `exact`                                        |
| `operation.returns`                                         | the operation's type reference                 | the return type, unwrapped of the profile's async wrappers | `type`                                         |
| `operation.parameters`                                      | the parameters' names and types, in order      | the method's parameters                                    | `list`                                         |
| `parameter.type`, `parameter.nullable`, `parameter.default` | the parameter's type reference and default     | the parameter node                                         | `type`, `exact`, `exact`                       |
| `enum.literals`                                             | the literals, in model order                   | the enum members                                           | `subsequence`                                  |
| `enum.literal.value`                                        | the literal's value, where modelled            | the member's `value`                                       | `number`                                       |

All but `parameter.default` and `enum.literal.value` are `stated: always`: a class with no `abstract` modifier states that it isn't abstract. A modelled member that isn't there is contradicted ([R8](requirements.md)). An unmodelled member is never a finding unless the `unmodelled.member` opt-in rule is on.

Four more structure kinds take their expected value from vocabulary and conventions rather than from the element alone: `type.location` (where the type's file must be), `type.base` (a base type the model implies, such as `IAggregateRoot` for an aggregate root), `type.marker` (a named marker, present exactly when modelled) and `file.exists` (a file an element stands for, such as a Vue component). [claim-catalog.md](claim-catalog.md#structure) lists every structure kind.

### Language profiles

A language profile is pack data ([R42](requirements.md)). Its schema is [`schemas/language-profile.schema.json`](schemas/language-profile.schema.json).

| Section       | Says                                                    | C# example                                           | Java example                              |
| ------------- | ------------------------------------------------------- | ---------------------------------------------------- | ----------------------------------------- |
| `types`       | Each model type's names in code                         | `datetime` → `DateTime`, `System.DateTime`           | `datetime` → `LocalDateTime`, `Instant`   |
| `collections` | The generic types and forms that mean "a collection of" | `List<T>`, `ICollection<T>`, `IEnumerable<T>`, `T[]` | `List<T>`, `Set<T>`, `Collection<T>`      |
| `async`       | Wrappers unwrapped from return types                    | `Task<T>`, `ValueTask<T>`; `Task` means void         | `CompletableFuture<T>`, `Mono<T>`         |
| `nullable`    | How a type ref says nullable                            | `T?` in a nullable context; `Nullable<T>`            | `@Nullable`; a boxed type for value types |
| `naming`      | From an element name to a type, member and file name    | type `{name}`, property `{name                       | pascal}`                                  |
| `typeKinds`   | The code kinds the language has                         | `class`, `record`, `struct`, `interface`, `enum`     | `class`, `record`, `interface`, `enum`    |

The `type` comparer maps the model's type through `types`, `collections` and `nullable`, then compares with the code's type ref: model `OrderItem[]` matches C# `ICollection<OrderItem>` and Java `List<OrderItem>`; model `Guid?` matches C# `Guid?` and Java `@Nullable UUID`. A type the profile doesn't list is compared by name.

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

Two behaviours change, both listed in [conversion.md](conversion.md#semantic-changes):

- `default` and `absent` are refused for a `whenSet` kind (a `config/silenceForWhenSet` error). The model states those values only where the code does (decision 4).
- A find that ends at several places is `ambiguous` and names every candidate.

### Additions

| Addition                       | Example                                                   | Why                                                                                    |
| ------------------------------ | --------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| `type:Name` step               | `type:{Entity}`                                           | `class:` reads as C#; `type:` names any language's types. `class:` stays as an alias.  |
| `annotation:Name` step         | `type:{Entity} > annotation:Table`                        | C# attributes, Java annotations and decorators as nodes, not regular expressions       |
| `call:Name` step               | `method:Configure > call:ToTable`                         | A call, whatever the chain around it                                                   |
| `markup:Tag`, `directive:Name` | `file:{Folder}/{Component}.razor > directive:page`        | Markup without regular expressions                                                     |
| framework node steps           | `ef.entity:{Entity}`                                      | Framework readers' nodes                                                               |
| `base` step                    | `type:{Entity} > base`                                    | The base types, nearest first; needs `resolution`                                      |
| `node` read case               | `{ "node": "arguments[0].literal" }`                      | Read a node field, with no regular expression                                          |
| `map`                          | `"map": { "nvarchar": "NVARCHAR", "varchar": "VARCHAR" }` | A value table applied after `replace`                                                  |
| `pascal`, `camel` for `case`   | `"case": "pascal"`                                        | Case conversion for read values, beside `upper` and `lower`                            |
| `inherit` as a list            | `"inherit": ["method", "class"]`                          | Common.Drift already accepted it; now documented                                       |
| `compare`                      | `"compare": "ignoreCase"`                                 | A recipe-specific comparer, such as SQL types that differ only in case in one codebase |

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

Code is **located** by a claim when the claim's reading came from it or from code inside it, as Common.Drift's ownership is.

An exception is a row of `Rule`, `Match` and `Reason` on the `Drift Check` stereotype. `Match` is a file glob (relative to the output folder), a type name, a `Type.Member`, or the name a finding gives. An exception that matches nothing is an `optin/staleException` warning; one with no reason is a `config/exceptionReason` error.

## Recipe validation

Validation runs on demand, from the model, the code and the packs, never during a Software Factory run ([R50](requirements.md)). It keeps Common.Drift's checks and extends them to every rule kind:

- **The probe**: change, in memory, the code each reading came from, read again, and warn when the same value comes back.
- **The wrong-element check**: for each kind and owner, warn when a reading for one element came from code naming a sibling's own placeholder (`own`).
- **Counts by source**: per kind, how many readings came from code, from inheritance and from silence. Silence readings are listed so a recipe that verifies everything from `absent` stands out.

Phase 1 builds it; its acceptance cases live with phase 1, not in this suite.
