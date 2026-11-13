# Code readers (block 2)

A code reader turns the in-scope files of one language into the **code graph**: one shape for every language, so structure comparison, recipes and queries work the same over C#, Java or anything a pack adds ([R33](requirements.md)). Its schema is [`schemas/code-graph.schema.json`](schemas/code-graph.schema.json).

Readers read files. They never build, restore, load or run the codebase ([R18](requirements.md), decision 3). A framework's own view of the code comes only from artifacts committed beside it, such as EF Core's migration snapshot.

## The code graph

### Nodes

Every node has:

| Field         | Meaning                                                                                         |
| ------------- | ----------------------------------------------------------------------------------------------- |
| `id`          | Unique within a run, stable for unchanged code: the file, the node's kind and its path of names |
| `kind`        | One of the kinds below, or a framework node kind                                                |
| `name`        | Its declared name, where it has one                                                             |
| `file`        | Path relative to the application's output folder, with `/` separators                           |
| `span`        | 1-based `line`, `column`, `endLine`, `endColumn` ([R36](requirements.md)). A declaration's span starts at its first attribute or annotation, as the language's own syntax tree has it |
| `text`        | Its canonical text ([below](#canonical-text))                                                   |
| `declaration` | The canonical text of its declaration part: for a type or member, everything before its body    |
| `body`        | The canonical text of its body, where it has one                                                |
| `parent`      | The node that contains it                                                                       |
| `incomplete`  | `true` when the node lies in code the reader couldn't parse cleanly                             |

| Kind            | What it is                                                                                                         | Its own fields                                                                                       |
| --------------- | ------------------------------------------------------------------------------------------------------------------ | ---------------------------------------------------------------------------------------------------- |
| `project`       | A project file (`.csproj`, `pom.xml`, `package.json`)                                                              | `settings`: root namespace, nullable context, target framework, implicit usings, source roots        |
| `file`          | A source file                                                                                                      | `language`, `raw` (the file as written)                                                              |
| `unit`          | The files read as one place: a partial type's parts, a component's markup and code-behind ([R37](requirements.md)) | `files`                                                                                              |
| `namespace`     | A C# namespace, Java package or TypeScript module                                                                  | `qualifiedName`                                                                                      |
| `import`        | A `using`, `import` or `@using`                                                                                    | `target`, `alias`, `static`, `global`                                                                |
| `type`          | A class, interface, record, struct, enum, annotation type or component                                             | `typeKind`, `modifiers`, `typeParameters`, `supertypes` (type refs), `qualifiedName`                 |
| `field`         | A field                                                                                                            | `modifiers`, `type`                                                                                  |
| `property`      | A property, or a JavaBeans property (a field with its accessors)                                                   | `modifiers`, `type`, `accessors`                                                                     |
| `method`        | A method                                                                                                           | `modifiers`, `returnType`, `typeParameters`                                                          |
| `constructor`   | A constructor, including a primary constructor                                                                     | `modifiers`, `primary`                                                                               |
| `event`         | An event                                                                                                           | `modifiers`, `type`                                                                                  |
| `enumMember`    | An enum literal                                                                                                    | `value` (as written, when written)                                                                   |
| `parameter`     | A parameter                                                                                                        | `type`, `default`, `modifiers`, `position`                                                           |
| `typeParameter` | A generic type parameter                                                                                           | `constraints`                                                                                        |
| `annotation`    | A C# attribute, Java annotation or TypeScript decorator                                                            | `arguments`: each with `name` (if named), `text` (canonical) and `literal` (when it is one)          |
| `statement`     | A statement                                                                                                        |                                                                                                      |
| `chain`         | A whole fluent call chain                                                                                          | `calls` in order                                                                                     |
| `call`          | A method or function call                                                                                          | `callee` (the name called), `receiver` (canonical text), `receiverType` (when resolved), `arguments` |
| `creation`      | An object creation (`new X(…)`)                                                                                    | `type`, `arguments`                                                                                  |
| `markup`        | An element in markup (`.razor`, `.html`, `.vue` templates)                                                         | `tag`, `attributes`, `component` (a type ref, when the tag names a component)                        |
| `directive`     | A markup directive (`@page`, `@inject`, `@attribute`)                                                              | `directive`, `value`, `type` (a type ref, for `@inject`)                                             |

Framework readers add their own kinds, namespaced by framework: `ef.entity`, `openapi.operation`.

### Type references

A type as written: `text` (canonical), `name` (without namespace or generics), `qualifiedName` (when resolved), `arguments` (type refs), `array`, `nullable` (`true`, `false` or `unknown`) and `resolved` (the `type` node it resolves to, when the reader resolves names).

Nullability is `unknown` when the language or context can't say. A C# reference type outside a nullable context is `unknown`; inside one, `string` is `false` and `string?` is `true`. A Java reference type is `unknown` unless annotated (`@Nullable`, `@NonNull`), or unless its package declares a non-null default the Java pack's profile names (Spring's `@NonNullApi`); a primitive is `false` and a boxed type `true`. A TypeScript type under `strict` or `strictNullChecks` is `true` when optional (`?`) or a union with `null` or `undefined`, else `false`; without either setting it is `unknown`. Nullability is the type's: a framework's binding rules, such as a required `@RequestParam`, are other kinds' business.

### Edges

| Edge         | From → to                                                    |
| ------------ | ------------------------------------------------------------ |
| `contains`   | A node to each node directly inside it                       |
| `extends`    | A type to its base class                                     |
| `implements` | A type to each interface                                     |
| `imports`    | A file to each import                                        |
| `references` | A type ref to the type it resolves to                        |
| `calls`      | A call to the method it resolves to                          |
| `creates`    | A creation to the type it resolves to                        |
| `partOf`     | A file to its unit, a partial type's part to the merged type |

Edges that need resolution exist only when the reader has the capability.

## Canonical text

Rules compare canonical text, so layout and comments never change a reading.

- **C#**: exactly Common.Drift's: tokens joined with no whitespace except one space between two words; comments and code disabled by `#if` dropped; string and interpolated string contents as written ([R34](requirements.md)). `builder.ToTable( "Orders" ,  "sales" ); // legacy` reads `builder.ToTable("Orders","sales");`, and `public  sealed class Order` reads `public sealed class Order`.
- **Java**: the same rule over Java's tokens; text blocks as written.
- **Markup** (a `markup` or `directive` node's `text`): attributes in source order, whitespace between attributes collapsed to one space, text content trimmed and each inner run of whitespace reduced to one space, so `Shopping Bag | AdventureWorks` reads as written.
- **TypeScript**: the C# rule over TypeScript's tokens; template literals as written.
- **Protobuf**: the same rule over `.proto` tokens.
- **Any other file**, and a `file` node's `text` whatever its language: as Common.Drift reads text, with each run of whitespace removed, or reduced to one space between two words. Converted regular expressions over `.razor`, `.ts`, `.html` or JSON files then read what they read before. `raw` keeps the file as written.

## Readers

| Reader             | Phase | Reads                                                                        | Resolution |
| ------------------ | ----- | ---------------------------------------------------------------------------- | ---------- |
| C#                 | 1     | `.cs`, `.razor` (its `@code` and markup), `.csproj`, `Directory.Build.props` | declared   |
| Text               | 1     | any file, as `file` with `raw`                                               | none       |
| Java               | 4     | `.java`, `pom.xml`, `build.gradle`                                           | declared   |
| EF Core migrations | 4     | committed `*ModelSnapshot.cs`                                                | n/a        |
| TypeScript         | later | `.ts`, `.vue` (its `<script>` as TypeScript, its `<template>` as markup), `tsconfig.json`, `package.json` | declared |
| Protobuf           | later | `.proto`                                                                     | declared   |

A reader is a plug-in like any other ([rules.md](rules.md#plug-ins)); the C# and text readers ship in the engine's own pack, the others in their stack packs ([packs.md](packs.md#stock-packs)).

### The TypeScript reader

The Java spike's front end is Vue and TypeScript, and the Services designer's contracts end up as TypeScript there. The reader gives them the same graph as any language:

| TypeScript                                                                                          | Node                                                                                     |
| --------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| A module (a file)                                                                                   | `namespace`, named by its path without extension                                         |
| `import`                                                                                            | `import`, with `target` the module path as written and, resolved, the file               |
| `class`, `interface`, `type X = {…}`, `enum`                                                        | `type`, with `typeKind` `class`, `interface`, `typeAlias` or `enum`                      |
| A Vue single-file component                                                                         | `type` with `typeKind` `component`, named after the file; its unit is the `.vue` file   |
| A property of a class, interface or object type; a component's props (`defineProps`)                | `property`, with `type`                                                                  |
| A method; a top-level or exported `function`; a `const` holding an arrow function or function       | `method`, with `parameter` nodes and `returnType`; a top-level one's parent is the module |
| Calls, `new`, statements                                                                            | `call`, `creation`, `statement`, as for C#                                               |

Resolution is declared: relative imports, and `tsconfig.json`'s `baseUrl` and `paths`, resolve to in-scope files; a package import stays as written.

### The Protobuf reader

eShopCustom's gRPC services are described by `.proto` files beside the C# that implements them:

| Protobuf          | Node                                                                                                           |
| ----------------- | -------------------------------------------------------------------------------------------------------------- |
| `package`         | `namespace`                                                                                                    |
| `import`          | `import`                                                                                                       |
| `message`, `enum` | `type`, `typeKind` `message` or `enum`                                                                         |
| A message field   | `property`, its `type` a collection when `repeated`, nullable `true` when `optional`, else `false`; its number as `value` |
| An enum value     | `enumMember`, with `value`                                                                                     |
| `service`         | `type`, `typeKind` `service`                                                                                   |
| `rpc`             | `method`, with one `parameter` named `request` of the request type, `returnType`, and a `stream` modifier on either |

Names are kept as the `.proto` writes them. protoc's renaming for C# (`order_id` to `OrderId`, `BasketClient` for a service's client) is the gRPC pack's conventions' business, not the reader's.

### Markup references

A component used in markup, and a type injected into it, are type refs like any other, so the `type` comparer can say which component or service the code really means. In a `.razor` file:

- an element whose tag starts with a capital letter, or has a namespace qualifier, has a `component` type ref naming that tag;
- an `@inject` directive has a `type` ref naming its type.

The C# reader resolves both as Razor does: a qualified name as written; otherwise through the `@using` directives of the file and of every `_Imports.razor` in its folder and the folders above it, then the file's own namespace (its `@namespace`, or the project's root namespace plus the folder path). The TypeScript reader resolves a `.vue` file's components through its imports.

### Capabilities

Each reader declares what it can do ([R35](requirements.md)). A rule that needs a capability the reader lacks gives can't verify, `unresolved`.

| Capability     | Meaning                                                                                                                                                    | C#       | Java       | TypeScript | Protobuf | Text |
| -------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- | -------- | ---------- | ---------- | -------- | ---- |
| `declarations` | Types and members, with modifiers and type refs                                                                                                            | yes      | yes        | yes        | yes      | no   |
| `bodies`       | Statements, chains, calls and creations                                                                                                                    | yes      | yes        | yes        | no       | no   |
| `resolution`   | `declared`: names resolve to in-scope declarations through imports, namespaces and the project's global usings. Never to library types: there is no build. | declared | declared   | declared   | declared | none |
| `nullable`     | Knows the nullable context                                                                                                                                 | yes      | by profile | by `tsconfig.json` | yes | no |
| `callTargets`  | Resolves a call to its method when the receiver's declared type is known (a field, parameter or explicitly typed local)                                    | declared | declared   | declared   | no       | no   |

`resolution: declared` is enough for the structure claims: a base type, a member's type or an implemented interface declared in scope resolves, and one from a library stays as written. A rule comparing a library type compares its name as written.

### Units

The parts of a partial type are one `type` node, with a `unit` naming its files. The files of one component in one folder whose names are the same up to the first dot (`Checkout.razor` and `Checkout.razor.cs`, `customer-list.component.ts` and `customer-list.component.html`) are one `unit`, as Common.Drift reads them.

## Framework readers

A framework reader reads a committed artifact and produces framework nodes located in it ([R39](requirements.md)). It never builds the code to ask the framework.

| Artifact                         | Nodes                                                                                                                          |
| -------------------------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| EF Core `*ModelSnapshot.cs`      | `ef.entity` (table, schema, view), `ef.property` (column, type, max length, required), `ef.key`, `ef.index`, `ef.relationship` |
| `openapi.json`, `swagger.json`   | `openapi.operation` (verb, route, operation id, parameters, responses)                                                         |
| SQL project `.sql` table scripts | `sql.table`, `sql.column`, `sql.index`                                                                                         |

A snapshot says what the last migration made, which may lag the code. A pack chooses, per claim kind, whether its rule reads the code or the artifact, and a house pack can change that choice.

## Failures

- **A file the reader can't read at all** (not text, or a language reader's fatal error) gives one `engine/readerFailed` finding for the file, and `readerFailed` for each claim whose rule needed it ([R40](requirements.md)).
- **A file with syntax errors** is still read. Nodes inside an error's span are `incomplete`, and so is a node containing a missing token the parser inserted (a zero-width error). A reading taken from an incomplete node gives `readerFailed` with the reader's diagnostic, rather than a value that may be wrong. The file gets no finding of its own: what matters is the claims it fails, and those are reported.
- **A project file that won't parse** is a file its reader fails on: one `engine/readerFailed` finding for it, and `readerFailed` for each claim that needed its settings, such as a `member.nullable` claim on a file it governs ([R40](requirements.md)).

A file that isn't text at all can't be written as a case's fixture, so the suite shows reader failures with code that doesn't parse.

## Acceptance cases

The `readers` area of the suite has `csharp-text` (canonical text identical to Common.Drift's), `partial` (partial types and component units), `capabilities` (declared resolution and `unresolved`), `text` (`file:` and regular expressions on any file), `framework` (an EF Core snapshot), `markup` (component and `@inject` references), `typescript` and `protobuf`. Reader failures are `engine/readerFailed`'s. Reader cases assert on nodes as well as claims ([acceptance-suite.md](acceptance-suite.md#the-expect-block)).
