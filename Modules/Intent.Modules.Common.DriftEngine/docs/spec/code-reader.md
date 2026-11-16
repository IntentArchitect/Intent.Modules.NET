# Code readers (block 2)

A code reader turns the in-scope files of one language into the **code graph**: one shape for every language, so structure comparison, claim readers and find paths work the same over C#, Java or anything a module adds ([R33](requirements.md)). Its schema is [`schemas/code-graph.schema.json`](schemas/code-graph.schema.json).

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
| `file`          | A source file                                                                                                      | `language`, `raw` (the file as written), `topLevel` (its top-level statements, a `function`), `importsFrom` (the files whose imports it has too: a Razor component's `_Imports.razor` files) |
| `unit`          | The files read as one place: a partial type's parts, a component's markup and code-behind ([R37](requirements.md)) | `files`                                                                                              |
| `namespace`     | A C# namespace, Java package or TypeScript module                                                                  | `qualifiedName`                                                                                      |
| `import`        | A `using`, `import` or `@using`                                                                                    | `target` (without a wildcard), `alias`, `static`, `global`, `wildcard` (it imports everything its target holds: a C# namespace `using`, Java's `.*`, TypeScript's `* as api`); TypeScript's also `module` (as written), `file` (the in-scope file it resolves to) and `typeOnly`; it resolves to the in-scope type it names |
| `type`          | A class, interface, record, struct, enum, annotation type or component; TypeScript's type alias, object and module | `typeKind`, `modifiers`, `typeParameters`, `supertypes` (type refs), `qualifiedName`; a type alias's `aliases` (the type it names) |
| `field`         | A field                                                                                                            | `modifiers`, `type`                                                                                  |
| `property`      | A property, or a JavaBeans property (a field with its accessors)                                                   | `modifiers`, `type`, `accessors`                                                                     |
| `method`        | A method                                                                                                           | `modifiers`, `returnType`, `typeParameters`                                                          |
| `constructor`   | A constructor, including a primary constructor                                                                     | `modifiers`, `primary`                                                                               |
| `event`         | An event; a Vue component's emit                                                                                   | `modifiers`, `type`; an emit's `parameters`                                                          |
| `enumMember`    | An enum literal                                                                                                    | `value` (as written, when written)                                                                   |
| `parameter`     | A parameter                                                                                                        | `type`, `default`, `optional` (TypeScript's `id?: string` and `...rest`), `modifiers`, `position`    |
| `typeParameter` | A generic type parameter                                                                                           | `constraints`                                                                                        |
| `annotation`    | A C# attribute, Java annotation or TypeScript decorator                                                            | `arguments`: each with `name` (if named), `text` (canonical) and `literal` (when it is one)          |
| `function`      | A unit of code: a method's or constructor's body, a lambda, or a file's top-level statements (Program.cs)          | `parameters`, `calls`, `creations`, `assignments`, `returns`, `async`; everything written inside it, its lambdas' code included |
| `statement`     | A statement                                                                                                        |                                                                                                      |
| `chain`         | A whole fluent call chain                                                                                          | `calls` in order                                                                                     |
| `call`          | A method or function call                                                                                          | `callee` (the name called), `receiver` (canonical text), `receiverType` (when resolved), `typeArguments` (type refs, as written), `arguments`: each with `text` (canonical), `types` and `literal` (when it is one) |
| `creation`      | An object creation (`new X(…)`)                                                                                    | `type`, `arguments`, `initializers` (each a member, or an indexer as written, and its value), `items` (a collection initializer's) |
| `markup`        | An element in markup: a Razor component's, a Vue template's ([Markup](#markup))                                    | `tag`, `component` (a type ref, when the tag names a component), `attributes`, `children`, `content` |

Framework readers add their own kinds, namespaced by framework: `ef.entity`, `openapi.operation`.

### Type references

A type as written: `text` (canonical), `name` (without namespace or generics), `qualifiedName` (when resolved), `arguments` (type refs), `array`, `nullable` (`true`, `false` or `unknown`) and `resolved` (the `type` node it resolves to, when the reader resolves names). A TypeScript union has its members as `union` (type refs, in order), and a literal type (`'Draft'`, `4`) its `literal`. The language profile's `nullable.unionMembers` (`null`, `undefined`) only make a union nullable, so `string | null` compares as `string`; `readonly string[]` is `string[]`.

Nullability is `unknown` when the language or context can't say. A C# reference type outside a nullable context is `unknown`; inside one, `string` is `false` and `string?` is `true`. A Java reference type is `unknown` unless annotated (`@Nullable`, `@NonNull`), or unless its package declares a non-null default the Java pack's profile names (Spring's `@NonNullApi`); a primitive is `false` and a boxed type `true`. A TypeScript type under `strict` or `strictNullChecks` is `true` when optional (`?`) or a union with `null` or `undefined`, else `false`; without either setting it is `unknown`. Nullability is the type's: a framework's binding rules, such as a required `@RequestParam`, are other kinds' business.

### Literals

A literal value, the same whatever the language's quoting: `kind` (`string`, `char`, `number`, `boolean`, `null`, `interpolated` or `type`) and `value`. A string's or char's value is its content with its escapes resolved (a verbatim or raw string's, a Java text block's with its indentation stripped); a number's is as written with its sign (`-1`, `18.4m`, `42L`); a boolean's is `true` or `false`. An interpolated string (C#'s `$"orders/{id}"`, a TypeScript template literal) has `parts` in order instead, its text as `string` literals and each hole a value. A type literal (`typeof(Order)`, `Order.class`) has its `type`. C#'s `nameof(Order.Total)` and `string.Empty` are strings, `Total` and empty. A call's arguments, an annotation's arguments and a member's initializer each have the literal they are; a name, a call or a concatenation is none.

### String values

What string a value is, as far as the code says without running it, is the same evaluation for every language (`CodeGraph.StringOf`), so a rule reads a proxy's request URI or a page's navigation as the code builds it. Each language gives its values the facts it needs:

| Fact        | What the value is                                                                                                   |
| ----------- | ------------------------------------------------------------------------------------------------------------------- |
| `joined`    | A `+` chain's operands, flattened; C#'s `string.Concat` and TypeScript's `concat` give theirs                      |
| `given`     | What it stands for, one per way the code gives it: a local's initializer and plain assignments before the use, a conditional's branches, a C# switch expression's arms (an arm that throws gives nothing), both sides of `??` (and TypeScript's `\|\|`), what `x.ToString()`, `String(x)`, `new Uri(x)` or `new URL(x)` is made from. Any other change to the local before the use (`+=`, `++`, C#'s `ref` or `out`) is a way whose text isn't known |
| `parameter` | The parameter of its unit it names                                                                                  |
| `field`     | The field or property it reads: its type's own, or another's through the type's, object's or module's name (`Routes.Catalog`, a module's `const`) |
| `call`      | The call it is the result of, with the method it reaches; for C#'s `await x.MakeAsync()`, the call it awaits       |
| `trimmed`   | A trim, and the characters it takes: C#'s `TrimEnd('&', '?')`, TypeScript's `trim()`                                |

The result is a `string` literal when the code gives all of it, otherwise `interpolated`: its text, and as holes the values whose text it doesn't give, each placed where it is written. `GetFromJsonAsync(uri)` after `var uri = $"{remoteServiceBaseUrl}items/{id}";` is `api/catalog/items/` and the hole `id`. The evaluation follows:

- a literal, and an interpolation's holes; a number, boolean or type is a hole, since its text is as a language prints it, and so is a hole written with a format or an alignment (`{total:F2}`);
- a field's or property's initializer, or the one value its type's code assigns it, after every type's initializers are read with what their names refer to; none when its type's code changes it any other way (`+=`);
- what a method of the code returns, with its parameters bound to the call's arguments, so `PageUri(remoteServiceBaseUrl, page)` is what `PageUri` builds from them; a library's method is a hole;
- a value given several ways, or a method with several returns, only when they all give the same string.

A trim takes characters from the text at an end only when the text there is known; a hole there may hold them. A loop that builds a string is a hole, as the value isn't one the code states. Markup's values (a Razor `href="@ItemHelper.Url(item)"`, a Vue `:to`) aren't evaluated yet, and Java gives its values none of these facts yet: no rule needs them.

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
- **Markup** (a `markup` node's `text`, the elements inside it included): each run of whitespace reduced to one space, and none beside `<` or `>` or before `/>`, so text content is trimmed and `<PageTitle> Shopping Bag | AdventureWorks </PageTitle>` reads `<PageTitle>Shopping Bag | AdventureWorks</PageTitle>`. Character references stay as written.
- **TypeScript**: the C# rule over TypeScript's tokens; template literals as written.
- **Protobuf**: the same rule over `.proto` tokens.
- **Any other file**, a `file` node's `text` whatever its language, and a Razor component's `type` (its `.razor` file; an `@inject`'s property is its directive): as Common.Drift reads text, with each run of whitespace removed, or reduced to one space between two words. Converted regular expressions over `.razor`, `.ts`, `.html` or JSON files then read what they read before. `raw` keeps the file as written.

## Readers

| Reader             | Phase | Reads                                                                        | Resolution |
| ------------------ | ----- | ---------------------------------------------------------------------------- | ---------- |
| C#                 | 1     | `.cs`, `.razor` (its directives, `@code` and markup), `.csproj`, `Directory.Build.props` | declared   |
| Text               | 1     | any file, as `file` with `raw`                                               | none       |
| Java               | 4     | `.java`, `pom.xml`, `build.gradle`                                           | declared   |
| EF Core migrations | 4     | committed `*ModelSnapshot.cs`                                                | n/a        |
| TypeScript         | later | `.ts`, `.tsx`, `.mts`, `.cts`, `.vue` (its `<script>` blocks, and its `<template>` as markup), `tsconfig.json` | declared |
| Protobuf           | later | `.proto` (proto2, proto3 and editions)                                       | declared   |

A reader is a plug-in like any other ([rules.md](rules.md#plug-ins)); the C# and text readers ship in the engine's own pack, the others in their stack packs ([packs.md](packs.md#stock-packs)).

### Razor components

The C# reader reads a Razor component (`.razor`) as the C# compiler does: as the C# the Razor compiler generates for it, with the `_Imports.razor` files of its folder and the folders above it, up to its project's. Every node is placed where it is written, in the component or the `_Imports.razor` it came from.

| Razor                                                        | Node                                                                                                     |
| ------------------------------------------------------------ | -------------------------------------------------------------------------------------------------------- |
| The component                                                | `type` with `typeKind` `component`, named after the file, in its `@namespace` or its project's root namespace and folders. Its code-behind (`X.razor.cs`, `partial class X` in that namespace) is a part of it. It is placed at the whole file |
| `@page`, `@layout`, `@attribute`                             | `annotation`s on the type: `Route` with the route, `Layout` with the layout's type, the attribute as written |
| `@inherits`, `@implements`                                   | Its supertypes. Without `@inherits`, `Microsoft.AspNetCore.Components.ComponentBase`                     |
| `@typeparam`                                                 | Its type parameters                                                                                      |
| `@inject T Name`                                             | A private `property` of type `T`, with an `Inject` annotation, placed at the directive and with it as its text |
| `@code`                                                      | Its members, as written in C#, with their bodies, calls and call targets                                 |
| `@using`                                                     | `import`s of the file, written `@using X`. An `_Imports.razor`'s are its own file's, and every component it applies to has them too (`importsFrom`); it declares no type |
| Markup                                                       | The file's `markup` nodes ([Markup](#markup)), as Razor parses them before it compiles them             |

A directive is the code it compiles to, so there's no node for the directive itself; `@rendermode` isn't read, as the Razor compiler the reader uses predates it. The markup isn't read from the generated code: without the components the referenced assemblies declare, the compiler can't tell a component's parameters, and the code in them, from HTML. A Razor diagnostic doesn't fail the file, as a C# syntax error doesn't. MVC views and Razor Pages (`.cshtml`) aren't read.

### The TypeScript reader

The Java spike's front end is Vue and TypeScript, and the Services designer's contracts end up as TypeScript there. The reader gives them the same graph as any language:

| TypeScript                                                                                          | Node                                                                                     |
| --------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| A module (a file)                                                                                   | `namespace`, named by its path without extension (`src/api/client`; `.d.ts` is one extension) |
| What a module declares outside its types: its functions and variables                               | `type` with `typeKind` `module`, named for the file with its extension (`client.ts`); its members are static, public when exported and private otherwise |
| `import`                                                                                            | One `import` per binding: `import { A, B as C } from './m'` is two. `target` is the name in the module's namespace when the module is in scope (`src/api/m.A`), else in the module as written; a default import names what the module's default export is; `* as m` is a `wildcard` with its alias |
| `class`, `interface`, `type X = …`, `enum`                                                          | `type`, with `typeKind` `class`, `interface`, `typeAlias` or `enum`. A type alias's object type gives it members, and the named types an intersection joins (`Base & { … }`) are its supertypes |
| A `const` holding an object literal (`export const api = { … }`)                                    | `type` with `typeKind` `object`, with its methods and properties, besides the module's `field` |
| A Vue single-file component                                                                         | `type` with `typeKind` `component`, named after the file; it is its file's default export |
| A property of a class, interface or object type; a constructor parameter with an access modifier or `readonly`; a `get`/`set` pair | `property`, with `type`; optional (`name?: string`) makes it nullable under `strict` |
| A component's props (`defineProps`, by type or at run time, with `withDefaults`' values) and `defineModel` | `property`; `defineModel`'s has the `model` modifier                               |
| A component's emits (`defineEmits`, by call signatures or named tuples)                             | `event`, with its `parameter` nodes                                                      |
| A method; a `function`; a `const` holding an arrow function or function                             | `method`, with `parameter` nodes and `returnType`; a module-level one is the module's, a `<script setup>`'s the component's |
| A module-level variable; a `<script setup>`'s variable                                              | `field`, with its initializer as `value`                                                 |
| A decorator                                                                                         | `annotation`; an object literal argument's properties are named arguments (`@Component({ selector: 'app-cart' })`) |
| Calls, `new`, assignments, `return`s; arrow functions and function expressions                      | As for C#. The statements a module runs when it loads (a `<script setup>`'s when its component is set up) are the file's `topLevel` |

A call reaches a function as the code says: a bare call the module's own function or one it imports, unless a local or parameter has the name; `this.find()` the class's or object's own; `api.getProducts()` what `api` is, whether a typed field or parameter, an object, or a namespace import's module. `await` gives what a `Promise<T>` resolves to.

Resolution is declared: relative imports, and the nearest `tsconfig.json`'s `baseUrl` and `paths` (following `extends`, and a solution-style config's `references` to the one that includes the file), resolve to in-scope files, with or without their extension, an ESM import's `.js` written for its `.ts`, or a folder's `index`; a package import stays as written. The same config's `strict` or `strictNullChecks` says whether types state nullability.

The reader parses with TypeScript's own parser (`typescript.js`, the last JavaScript release, 6.0), syntax only, on Node.js: Intent Architect's own in a Software Factory run, so the machine needs none, and the machine's `node` in tests. Every in-scope file is parsed in one Node.js process per run. A `.vue` file's `<script>` and `<script setup>` blocks are parsed as one script whose positions are the file's, and its `<template>` is its file's markup ([Markup](#markup)). JavaScript files (`.js`, `.jsx`) aren't read; a component whose script is JavaScript is parsed as JavaScript.

### The Protobuf reader

eShopCustom's gRPC services are described by `.proto` files beside the C# that implements them:

| Protobuf          | Node                                                                                                           |
| ----------------- | -------------------------------------------------------------------------------------------------------------- |
| `package`         | `namespace`, of its file and every type in it, nested ones included                                            |
| `import`          | `import` with `module` as written, the in-scope `file` it resolves to, and that file's package as its `target`; a `wildcard`, since it makes all of the file visible |
| `message`, `enum` | `type`, `typeKind` `message` or `enum`, named as written; a nested one is named alone, as a nested C# or Java type is |
| A message field   | `property`, with its label as a modifier and its number as `value`. `repeated` makes its type a collection (`repeated Line`), and `map<K, V>` is `map` with its key and value types |
| An enum value     | `enumMember`, with its number as `value`, sign included                                                        |
| `service`         | `type`, `typeKind` `service`                                                                                   |
| `rpc`             | `method`, with one `parameter` named `request` of the request type, and the response type as its type. A streamed side is `stream` of its message, as a generic type is of its argument: `stream OrderReply`, named `stream`, with `OrderReply` its argument |
| `option`, `[...]` | `annotation`s of what they are written on, named as written (`deprecated`, `(google.api.http)`), the value as the argument; a message value (`{ get: "/v1/orders" }`) has its fields as named items |
| proto2's `group`  | A nested `message`, and a field of it named after it in lower case, as protoc names it                         |

A field's type is nullable when the field has presence, so it can be left unset: `optional`, in a `oneof`, a message (a well-known wrapper such as `google.protobuf.Int32Value` included), proto2's every field but `required`, and an edition's fields by their `features.field_presence`. A proto3 scalar or enum without `optional` isn't, nor is a `repeated` field, a map, or an rpc's request or response. Whether a name is a message or an enum, the reader resolves as protoc does, innermost scope first, through the in-scope files, preferring the field's own file and then those it imports; a type in no in-scope file is `unknown`, apart from the well-known types.

An `import` is resolved as protoc finds it on an import path: from the importing file's folder, then each folder above it, so a path relative to the file, its project or the output folder is found. A file's syntax and its file options (`csharp_namespace`) are kept beside the graph (`ProtobufSyntax`), for a gRPC stack. `extend`, `extensions`, `reserved` and a `oneof`'s own options aren't read.

Names are kept as the `.proto` writes them. protoc's renaming for C# (`order_id` to `OrderId`, `BasketClient` for a service's client) is the gRPC pack's conventions' business, not the reader's.

The reader parses the language's grammar itself, proto2, proto3 and editions, with every position. On 72 real files (Google's `descriptor.proto`, `google/api`, `google/type` and `google/rpc`, eShopCustom's) it reads the same messages, fields, numbers, labels, enums and rpcs, and the same presence, as protobuf-net's parser. A syntax error is the file's reader failure, and the file is read on after the statement it is in.

### Markup

A Razor component's markup and a Vue component's template are read into one shape, so a rule about what a page renders, links to or titles itself reads either. A file's `markup` is the elements at its top level, each with the elements inside it; what an attribute means to a framework (a component's parameter, a link's route) is a stack's to say.

| Field        | What it is                                                                                                                                                       |
| ------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `tag`        | As written: `div`, `CatalogListItem`, `Ui.Grid`, `stock-pill`                                                                                                     |
| `component`  | The component the tag names, when the language reads it as one ([below](#markup-references))                                                                     |
| `attributes` | In order, each with `name` as written; `directive`, `argument` and `modifiers` when it is a directive; `value` as written; and a plain one's `literal`           |
| `children`   | The elements directly inside it, those inside code included: an element in a Razor `@foreach` is inside the element holding the `@foreach`                      |
| `content`    | When it holds only text and values: a `string` literal for text alone, else `interpolated`, each value's code a hole. Text is trimmed, each run of whitespace reduced to one space, character references resolved. None when it holds an element or other code |

| Written                                       | `directive`, `argument`, `modifiers`                    |
| --------------------------------------------- | ------------------------------------------------------- |
| Razor's `@onclick`, `@key`                    | `onclick`; `key`                                        |
| Razor's `@bind-Value:event`                   | `bind`, `Value`, `event`                                |
| Vue's `v-if`, `v-model:title.trim`            | `if`; `model`, `title`, `trim`                          |
| Vue's `:to` (`v-bind:to`), `@click.prevent`   | `bind`, `to`; `on`, `click`, `prevent`                  |
| Vue's `#item` (`v-slot:item`), `.value`       | `slot`, `item`; `bind`, `value`, `prop`                 |

A directive's value is code, so it has no `literal`. A plain attribute's is a `string`, or in Razor `interpolated` when values are written in it: `href="/orders/@id"` is `/orders/` and `id`, and `Item="@item"` is `item` alone.

Razor's markup is read as the Razor compiler parses it, before the compiler's own passes fold and encode it. A Vue template is read as Vue's compiler parses a single-file component: the top-level `<template>` is HTML and every other block raw text; an element closes at its end tag, at `/>` or as a void element, an end tag closes the elements left open inside its element, and one that closes nothing is ignored; `<textarea>` and `<title>` hold text and `{{ }}` values, `<script>` and `<style>` text alone. A template in another language (`lang="pug"`) or file (`src`) isn't read.

### Markup references

A component used in markup is a type ref like any other, so the `type` comparer can say which component the code really means, and an `@inject` is a property whose type is one.

- In a `.razor` file, an element whose tag starts with a capital letter, or is qualified, names that tag. The C# reader resolves it as Razor does: a qualified name as written; otherwise through the `@using` directives of the file and of every `_Imports.razor` in its folder and the folders above it, then the file's own namespace (its `@namespace`, or the project's root namespace plus the folder path) and the namespaces it is in.
- In a `.vue` file, a tag that isn't an HTML, SVG or MathML element, or that starts with a capital letter, names a component by its PascalCase name (`stock-pill` names `StockPill`), and a dotted tag (`ui.Badge`) names a member of what the script imports. It resolves through the script's imports. `<component :is>` names none, and `<slot>` and `<template>` aren't components.

A library's component, such as Blazor's `PageTitle` or Vue Router's `RouterLink`, isn't in the graph, so its name stays as written.

### Capabilities

Each reader declares what it can do ([R35](requirements.md)). A rule that needs a capability the reader lacks gives can't verify, `unresolved`.

| Capability     | Meaning                                                                                                                                                    | C#       | Java       | TypeScript | Protobuf | Text |
| -------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- | -------- | ---------- | ---------- | -------- | ---- |
| `declarations` | Types and members, with modifiers and type refs                                                                                                            | yes      | yes        | yes        | yes      | no   |
| `bodies`       | Statements, chains, calls and creations                                                                                                                    | yes      | yes        | yes        | no       | no   |
| `resolution`   | `declared`: names resolve to in-scope declarations through imports, namespaces and the project's global usings. Never to library types: there is no build. | declared | declared   | declared   | declared | none |
| `nullable`     | Knows the nullable context                                                                                                                                 | yes      | by profile | by `tsconfig.json` | by presence | no |
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

What each reader puts in the graph is tested by its fact tests (`Stacks/*FactsTests.cs`), node by node, and through the claims every kind's cases read with it: partial types and component units in `engine/ambiguous` and `ui/component.kind`, unresolved nullability in `engine/unresolved`, markup in the `ui` area. The C# fact tests hold canonical text Common.Drift captured, and a probe of both over the five copied codebases found no member or statement whose text differs. Reader failures are `engine/readerFailed`'s. No framework reader of a committed artifact is built yet.
