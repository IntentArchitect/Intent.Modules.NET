---
description: Write or extend the Guardrails drift check's recipe files (guardrails/<designer>.drift.json), which tell the Software Factory where each fact the model states is in the code and how to read it, for whatever technology the code uses. Use while reverse-engineering a Guardrails application (straight after its extraction, and again at its end), when a drift report says the check is off, facts can't be verified or the recipe file is invalid, when validate-drift-recipes shows a recipe misreading the code, or when the code starts using a technology no recipe covers.
short-description: Write the drift check's recipes for this codebase's technology.
contentHash: C5D70C525241E550E459C736302BA3D4DE9E05262A1A3AB7229521C0281237A3
---
# Configure the drift check

Once a designer has a recipe file, every Software Factory run compares its model with the code; without one, the check is off. The model side is fixed: the facts listed below, read from the model's own stereotypes. The code side is yours to describe, once, in a **recipe file** per designer: for each kind of fact, where it is in the code and how to read its value in the model's vocabulary. The module knows no technology, and your recipes do, so they work for controllers, minimal APIs, FastEndpoints, EF Core, Mongo, Cosmos or anything else the code uses.

You are done when **validate-drift-recipes** shows every recipe reading the code, a Software Factory run reports no drift and no warnings, and the developer has approved the model changes the setup made (step 9). A report (`intent/.cache/guardrails/eShop/DomainDriftReport.mdx` and `intent/.cache/guardrails/eShop/ServicesDriftReport.mdx`, in the `.cache` folder beside the solution's `.isln` file) is only written while there is something to act on: drift, a kind of fact that isn't checked, an exception that matches nothing, or the check being off.

**Never change the code while setting up the check.** The recipes and the model are made to reflect the code as it is. A fact no recipe can read, or code that looks wrong, is reported to the developer, not rewritten.

Load **generating-and-applying-code** before running the Software Factory, and **sync-model** for the drift your recipes find.

## Start from a technology guide

For each technology below, a guide beside this skill has a recipe file tested against code in every style it covers, the decisions that change it, how each form of the code is modelled, and how to adapt the recipes to a codebase's own conventions. Find every technology each designer's code uses (a codebase often uses several, such as EF Core and MongoDB, or controllers and minimal APIs), read each one's guide, and start the recipe file from them.

| Designer | Technology | Recognise it by | Guide |
| --- | --- | --- | --- |
| Domain | EF Core | A `Microsoft.EntityFrameworkCore` package and a class deriving from `DbContext` | [references/ef-core.md](references/ef-core.md) |
| Domain | MongoDB | A `MongoDB.Driver` package: `IMongoCollection<T>`, `BsonClassMap`, `[BsonId]` | [references/mongodb.md](references/mongodb.md) |
| Domain | Azure Cosmos DB | A `Microsoft.Azure.Cosmos` package (`CosmosClient`, `Container`), or EF Core's `UseCosmos` | [references/cosmos-db.md](references/cosmos-db.md) |
| Domain | Domain events | A `Domain Event Origin` in the model, from a constructor or operation to the Domain Event it raises | [references/domain-events.md](references/domain-events.md) |
| Services | ASP.NET Core controllers | Classes deriving from `ControllerBase` or `Controller`, with `[ApiController]` or `[HttpGet]` | [references/aspnetcore-controllers.md](references/aspnetcore-controllers.md) |
| Services | Minimal APIs | `app.MapGet(…)`, `MapPost(…)` or `MapGroup(…)` | [references/minimal-apis.md](references/minimal-apis.md) |
| Services | FastEndpoints | A `FastEndpoints` package: classes deriving from `Endpoint<TRequest>` | [references/fastendpoints.md](references/fastendpoints.md) |
| Services | Handler intent | Entity actions, invocations, published integration events or sent integration commands in the model, on MediatR handlers, service classes or event handlers | [references/handler-intent.md](references/handler-intent.md) |

Where one designer's code uses two technologies, combine their recipe files fact by fact: list both guides' find paths and read cases, and keep their `default`, `absent` and `unstated` where they agree. Where they differ, keep one, and turn the other into a read case on the code its own find paths reach. For a technology no guide covers, write the recipes from the format below, with the closest guide as a model of a complete recipe file.

## Recipe files

`guardrails/domain.drift.json` and `guardrails/services.drift.json`, in the folder that holds the application's `.application.config` (not the code's folder). Commit them with the code. Paths inside a recipe file are relative to the application's output location (the codebase root), with forward slashes.

```json
{
  "files": "src/**/*.cs",
  "facts": {
    "<fact>": { "find": "...", "read": [{ "match": "..." }], "default": "..." }
  },
  "own": [{ "kind": "...", "find": "...", "name": "..." }],
  "exceptions": ["..."]
}
```

- `files`: the files searched, as a pattern or a list. Default `**/*.cs`; narrow it to where the code lives. `*` matches within one folder, `**` across folders and `?` one character. `bin`, `obj` and hidden folders are never read.
- `facts`: one recipe per kind of fact. Every kind the model states needs one; a kind with no recipe isn't checked, and the report lists it in its **Warnings** table (a warning, not a failure).
- `own`: the kinds of code the model must describe.
- `exceptions`: code that may stay unmodelled.
- `note`: free text, allowed on every object. `//` comments are allowed too.

The recipe file covers facts, ownership and exceptions only. The structure rows (a generated type's kind, modifiers, parameters, base types and members) come from comparing what the templates generate with the code, so no recipe adds or removes them. They only cover what each element can state, so nothing the designer can't express ever needs configuring.

## How code is read

C# is read without comments, layout or disabled code: its tokens are joined with no whitespace, except one space between two words. This code:

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> Get(int id, [FromQuery] string expand = null)
```

reads `[HttpGet("{id}")]public async Task<IActionResult>Get(int id,[FromQuery]string expand=null)`. String contents are kept as written. Any other file (JSON, YAML) is read with each run of whitespace removed, or reduced to one space between two words.

Regular expressions are .NET's, case-sensitive unless they start with `(?i)`, and are matched against that text. In JSON every backslash is doubled: `\\[HttpGet\\(`.

## A recipe

```json
"operation.verb": {
  "find": "class:{Service} > method:{Operation}",
  "read": [{ "part": "declaration", "match": "\\[Http(Get|Post|Put|Delete|Patch)\\b", "case": "upper" }]
}
```

| Property | Meaning |
| --- | --- |
| `find` | Where the fact is: a find path (below), or a list of paths tried in order until one finds code |
| `read` | Cases tried in order: the first `match` that matches the found code gives the value |
| `default` | The value when the code is found but no case matches. Without it, that's reported as can't be verified. Give a recipe with a `default` an `unreadable` case for the other forms of the same declaration, so that a form no case reads isn't taken as the default |
| `absent` | The value when `find` finds no code at all, for facts the code states by saying nothing (a column with no configuration). Without it, that's reported as can't be verified |
| `unstated` | What the code saying nothing means, in this codebase's own terms, such as `no IsRequired() for {Attribute} in EF Core, which maps a string as nullable`. A drift row whose value came from `default` or `absent` quotes it, so the reader sees why the code reads as it does. Without it, the row only says that the code doesn't state the fact. Placeholders work as in `find`. Only with `default` or `absent` |
| `inherit` | Where to try the same cases next, before `default`: `class` (the enclosing type), `base` (its base types, nearest first), `method`, `property`, `statement`, `file`, or a find path. For example `"inherit": "class"` for security an action takes from its controller |
| `replace` | `[{ "match": "...", "with": "..." }]`, applied in order to a value a case read, e.g. to turn `[controller]` into the controller's name |
| `case` | `upper` or `lower`, for every value the recipe reads |
| `files` | Replaces the top-level `files` for this recipe |

Each read case:

| Property | Meaning |
| --- | --- |
| `match` | The regular expression |
| `value` | The value, with `$1` or `${name}` for the match's groups and `{Placeholder}`s. Without it, group 1, or the whole match |
| `part` | `declaration`: only what comes before the body (attributes, modifiers, signature, base types). `body`: only the body (a type's members, a method's block, a property's accessors and initializer). Without it, the whole code |
| `in` | Match the nearest enclosing `class`, `method`, `property`, `parameter`, `statement` or `file` instead, e.g. a parameter's route segment in its method's attribute |
| `case` | `upper` or `lower`, for this case |
| `unreadable` | `true`: the code states the fact here, but in a form no recipe can read (a route from a constant, a value built by a helper). The fact is reported as can't be verified, quoting what `match` matched, instead of taking `default`; so make `match` cover the whole declaration. Not with `value` |

Values are compared exactly, after trimming. `default` and `absent` are used as written.

## Find paths

Steps are separated by ` > ` (a space on each side). Each step searches inside what the step before it found; the first searches `files`.

| Step | Finds |
| --- | --- |
| `class:Name` | Types (classes, records, structs, interfaces and enums) named `Name`. The parts of a partial type are read as one |
| `method:Name` | Methods, constructors and local functions named `Name`. `method:Get(int, string)` picks an overload by its parameter types |
| `property:Name` | Properties, fields, events and indexers (`this`) named `Name` |
| `parameter:Name` | Parameters named `Name` |
| `statement:text` | The smallest statement containing the text, whatever its layout, e.g. `statement:Entity<{Entity}>` |
| `chain:text` | The whole fluent call chain around the call containing the text: `chain:Property(e => e.{Attribute})` gives `entity.Property(e => e.Name).IsRequired().HasMaxLength(50)` |
| `file:pattern` | The files matching the pattern, of any type. First step only |
| `<kind> with <regex>` | The smallest code of that kind around each match, e.g. `class with \\[ApiController\\]`, `method with \\[Http\\w+`, `chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)` |
| `<kind>` | All code of that kind inside the step before it |

A fact's `find` must end at exactly one place, or at the parts of one partial type; several places are reported as can't be verified. When `method:Name` finds overloads of a Service operation, the one whose parameter names are the operation's is used.

### Placeholders

`{Name}` in a find path, a match, a value, `default`, `absent` or a replacement is the element's own value; each fact below lists its placeholders. Inside a regular expression it is escaped. Filters change it, and chain: `{Service|trimEnd:Controller}`, `{Entity|lower}`, `{Attribute|camel}`, `{Operation|pascal}`, `{Name|trimStart:I}`, `{Name|upper}`. Write a literal brace in a regular expression as `\\{`.

## Facts the model states

### Domain: `guardrails/domain.drift.json`

| Fact | Reported as | Placeholders | Model values | What the model states |
| --- | --- | --- | --- | --- |
| `table.name` | table name | `{Entity}` | the name exactly as modelled, or empty | `Table` → `Name`, or `View` → `Name`, for every class with one of them. |
| `table.schema` | schema | `{Entity}` | the schema name, or empty | `Table` or `View` → `Schema`, else the nearest folder's or the package's `Schema` → `Name`, else empty. For every class with `Table` or `View`. |
| `table.kind` | table or view | `{Entity}` | `Table` or `View` | Which of the two stereotypes the class has. |
| `key` | primary key | `{Entity}` | the primary key attributes' names separated by commas, in model order; empty when there are none | The attributes with `Primary Key`, for every persisted class. A persisted class has `Table` or `View`, or a database stereotype on itself or an attribute. |
| `column.name` | column name | `{Entity}`, `{Attribute}` | the name exactly as modelled, or empty | `Column` → `Name`, or Document DB `Field Setting` → `Name`, for every attribute of a persisted class. |
| `column.type` | column type | `{Entity}`, `{Attribute}` | the type exactly as modelled, or empty | `Column` → `Type`, for every attribute of a persisted class. |
| `column.textType` | text type | `{Entity}`, `{Attribute}` | `VARCHAR`, `NVARCHAR`, `TEXT`, `NTEXT` or `DEFAULT` | `Text Constraints` → `SQL Data Type`, `DEFAULT` when not modelled, for every `string` attribute of a persisted class. |
| `column.maxLength` | max length | `{Entity}`, `{Attribute}` | a number, or empty | `Text Constraints` → `MaxLength`, for every `string` attribute of a persisted class. |
| `column.required` | required | `{Entity}`, `{Attribute}` | `true` or `false` | `true` when the attribute isn't nullable, for every `string` and `binary` attribute of a persisted class that isn't a primary key. |
| `column.precision` | precision | `{Entity}`, `{Attribute}` | a number, or empty | `Decimal Constraints` → `Precision`, for every `decimal` attribute of a persisted class. |
| `column.scale` | scale | `{Entity}`, `{Attribute}` | a number, or empty | `Decimal Constraints` → `Scale`, for every `decimal` attribute of a persisted class. |
| `column.default` | default | `{Entity}`, `{Attribute}` | the value exactly as modelled, or empty | `Default Constraint` → `Value`, for every attribute of a persisted class. |
| `relationship.multiplicity` | multiplicity | `{Source}`, `{Target}`, `{SourceEnd}`, `{TargetEnd}` | `<source end>-><target end>`, each `1`, `0..1` or `*`, e.g. `*->1` for many orders to one customer | Each end of an association between two classes, at least one of them persisted. `*` is a collection end, `0..1` a nullable one and `1` any other. |
| `relationship.foreignKey` | foreign key | `{Source}`, `{Target}`, `{SourceEnd}`, `{TargetEnd}` | the foreign key attributes' names separated by commas, in model order; empty when there are none | The attributes whose `Foreign Key` → `Association` is this association. |
| `index.columns` | columns | `{Entity}`, `{Index}` | the key attributes' names separated by commas, in the index's order | Every named index of a persisted class: `Index` on its attributes, one index per `UniqueKey`, the attributes ordered by `Order`; or an `Index` element in the class with `Use Default Name` off, and its `Key` columns. An index with no name (`Index` without a `UniqueKey`, or `Use Default Name` on) isn't stated. |
| `index.unique` | unique | `{Entity}`, `{Index}` | `true` or `false` | `Index` → `IsUnique` on the index's first attribute, or the `Index` element's `Unique`, for every named index. |
| `intent.raises` | raises | `{Entity}`, `{Member}`, `{DomainEvent}` | the domain event's name | Each `Domain Event Origin` from a class's constructor or operation to a Domain Event: one fact per link. An operation's parameter names choose between overloads. |

Placeholders:

- `{Entity}`: the class's name: an entity action's entity, or the class whose constructor or operation raises a domain event.
- `{Attribute}`: the attribute's name.
- `{Source}`: the source class's name.
- `{Target}`: the target class's name.
- `{SourceEnd}`: the source end's name: the navigation on the target class (may be empty).
- `{TargetEnd}`: the target end's name: the navigation on the source class (may be empty).
- `{Index}`: the index's name: its attributes' `UniqueKey`, or the `Index` element's name.
- `{Member}`: the constructor's or operation's name, the class's name for a constructor; for an entity action, the one it targets, else empty.
- `{DomainEvent}`: the domain event's name.

### Services: `guardrails/services.drift.json`

| Fact | Reported as | Placeholders | Model values | What the model states |
| --- | --- | --- | --- | --- |
| `service.route` | route | `{Service}` | the route exactly as modelled, or empty | A Service's `Http Service Settings` → `Route`, for every Service with that stereotype. |
| `operation.verb` | verb | `{Service}`, `{Operation}`, `{Folder}` | `GET`, `POST`, `PUT`, `PATCH` or `DELETE` | An endpoint's `Http Settings` → `Verb`. An endpoint is a Service operation, Command or Query with `Http Settings`. |
| `operation.route` | route | `{Service}`, `{Operation}`, `{Folder}` | the route exactly as modelled, or empty | An endpoint's `Http Settings` → `Route`. A Service operation's route is relative to its Service's route, unless it starts with `~/` or `/`. |
| `security` | security | `{Service}`, `{Operation}`, `{Folder}` | `Secured`, `Unsecured` or `None` | An endpoint's effective security: `Unsecured` or `Secured` on the element itself, else on its Service, its folders or its package, else `None`. Stated for every endpoint once the designer uses `Secured` or `Unsecured` anywhere. |
| `security.roles` | roles | `{Service}`, `{Operation}`, `{Folder}` | role names separated by commas, with no spaces, in model order | `Secured` → `Roles` and `Security Roles`, from the stereotype that makes the endpoint secured, where any are modelled. |
| `security.policy` | policy | `{Service}`, `{Operation}`, `{Folder}` | policy names separated by commas, with no spaces, in model order | `Secured` → `Policy` and `Security Policies`, from the stereotype that makes the endpoint secured, where any are modelled. |
| `parameter.source` | binding | `{Service}`, `{Operation}`, `{Folder}`, `{Parameter}` | `Default`, `From Body`, `From Form`, `From Header`, `From Query` or `From Route` | `Parameter Settings` → `Source` on an endpoint's parameter, or on a field of a Command or Query endpoint. |
| `intent.creates` | creates | `{Service}`, `{Operation}`, `{Folder}`, `{Entity}`, `{Member}` | the entity's class name | Each `Create Entity Action` from a Command, Query or Service operation, or an event handler's subscription to an integration event, integration command or domain event: one fact per action. `{Member}` is the constructor or factory operation it targets, if any. |
| `intent.updates` | updates | `{Service}`, `{Operation}`, `{Folder}`, `{Entity}`, `{Member}` | the entity's class name | Each `Update Entity Action` from a Command, Query or Service operation, or an event handler's subscription to an integration event, integration command or domain event: one fact per action. `{Member}` is the operation it targets, if any. |
| `intent.deletes` | deletes | `{Service}`, `{Operation}`, `{Folder}`, `{Entity}` | the entity's class name | Each `Delete Entity Action` from a Command, Query or Service operation, or an event handler's subscription to an integration event, integration command or domain event: one fact per action. |
| `intent.queries` | queries | `{Service}`, `{Operation}`, `{Folder}`, `{Entity}` | the entity's class name | Each `Query Entity Action` from a Command, Query or Service operation, or an event handler's subscription to an integration event, integration command or domain event: one fact per action. Its multiplicity isn't stated. |
| `intent.invokes` | invokes | `{Service}`, `{Operation}`, `{Folder}`, `{Invoked}`, `{InvokedOwner}` | the invoked element's name | Each `Perform Invocation` (once called `Call Service Operation`) from a Command, Query or Service operation, or an event handler's subscription to an integration event, integration command or domain event, to a Service, Domain Service, Repository or Service Proxy operation, a Command, a Query or a stored procedure: one fact per invocation. |
| `intent.publishes` | publishes | `{Service}`, `{Operation}`, `{Folder}`, `{Message}` | the integration event's name | Each `Publish Integration Event` from a Command, Query or Service operation, or an event handler's subscription to an integration event, integration command or domain event: one fact per event. |
| `intent.sends` | sends | `{Service}`, `{Operation}`, `{Folder}`, `{Message}` | the integration command's name | Each `Send Integration Command` from a Command, Query or Service operation, or an event handler's subscription to an integration event, integration command or domain event: one fact per command. |

Placeholders:

- `{Service}`: the Service's name (empty for a Command or Query), or an event handler's name for its subscription.
- `{Operation}`: the operation's name, or the Command's or Query's, or the message or domain event an event handler's subscription handles.
- `{Folder}`: the name of the folder the endpoint, Service or handler is in (empty at the package root).
- `{Parameter}`: the parameter's or field's name.
- `{Entity}`: the class's name: an entity action's entity, or the class whose constructor or operation raises a domain event.
- `{Member}`: the constructor's or operation's name, the class's name for a constructor; for an entity action, the one it targets, else empty.
- `{Invoked}`: the invoked operation's, Command's, Query's or stored procedure's name.
- `{InvokedOwner}`: the name of the Service, Domain Service, Repository or Service Proxy the invoked operation belongs to (empty for a Command, Query or stored procedure).
- `{Message}`: the integration event's or integration command's name.

## Code the model must describe

`own` lists the kinds of code that must all be modelled. Code that a fact's `find` located, or that contains such code, is modelled. Anything else is reported as drift: it isn't modelled.

```json
"own": [
  { "kind": "endpoint", "find": "class with \\[ApiController\\] > method with \\[Http(Get|Post|Put|Delete|Patch)\\b", "name": "{Class}.{Method}" },
  { "kind": "entity configuration", "find": "statement with modelBuilder\\.Entity<(\\w+)>", "name": "$1" }
]
```

- `kind`: how the report names that kind of code.
- `find`: as for a fact, except that every place counts, and a list of paths adds them all.
- `name`: how the report names each one: `{Class}`, `{Method}`, `{Member}`, `{Name}`, `{File}`, and `$1` from the last regular expression or text step.

Members of the types the Guardrails templates generate are checked without a recipe.

### New types of each element type

New code that should have been modelled is found by **convention**, one `own` entry per element type the model describes (DTO, service, command, entity, enum, repository, …). For each, look at how its modelled types are recognisable in the code. Do they all share something a new one would share too?

- a folder and a name pattern: every `*DTO` class in the DTO project;
- a base class: every class deriving from `BaseBusinessObject`;
- an interface: every `IEntityTypeConfiguration<T>`;
- an attribute: every class with `[ApiController]`;
- a member: every class with a `Configure()` method.

Where they do, write one entry that finds every type following the convention, with `kind` naming the element type and its own `files` where the top-level `files` doesn't cover those types. Where they share nothing, write none: a new type that follows no convention can't be told from a helper, so it isn't detected, and a guess would report helpers as unmodelled. Where a project mixes helpers with the element type, leave the helpers out: `class with \\bpublic (?:sealed |partial )*class \\w+` finds every class but a static one, or give the entry `files` that leave out the helpers' folder. Types the templates generate, and types in any file a template writes, count as modelled, so an entry reports only the unmodelled ones:

```json
"own": [
  { "kind": "DTO", "find": "class with \\bclass \\w+DTO\\b", "files": "src/Contracts/**/*.cs" },
  { "kind": "Service", "find": "class with :BaseBusinessObject\\b", "files": "src/Business/**/*.cs" },
  { "kind": "Entity", "find": "class with :Entity\\b", "files": "src/Domain/**/*.cs" }
]
```

## Exceptions

`exceptions` lists code that may stay unmodelled: a file pattern (it has a `/`, `*` or `?`, or ends in `.cs`), a type, a `Type.Member`, or an ownership `name`. Write `{ "match": "...", "reason": "..." }` to record why. Add clear helpers and framework plumbing yourself; ask the developer about anything else. Exceptions never cover a contradicted or unverifiable fact. Only code the model could describe is ever reported, so an exception is for code the developer chooses not to model, never for something the designer can't express.

A type or `Type.Member` exception covers that declaration, and a type its members' declarations, but not code an `own` entry finds inside a member: each `Map…` call in a method that maps endpoints, each configuration call in `OnModelCreating`. Those are what the model describes, so never exempt the method that holds them to hide them; exempt one only by the name the report gives it, or by a file pattern.

## Checking the recipes

A recipe that misreads the code fails loudly (a contradiction the code doesn't bear out) or silently (it agrees with the model by accident). The drift report can't tell which, so check the recipes with **validate-drift-recipes** whenever you write or change them. It reads every fact through its recipe and shows, per kind of fact, where the values came from, with samples to open, and what each convention finds. Its **probe** changes, in memory, the code each read case matched, and warns when the case still reads the same value: that case isn't reading the code it seems to. It also warns about a read case that reads some facts from code naming another element and not their own, such as another attribute's configuration.

## Steps

**While reverse-engineering**, the reverse-engineer skill sends you here straight after its extraction, before anything is modelled: do steps 2 and 3 from the extraction's reports, without re-reading the code, and go back. Model each form of the code as its guide's **How each form is modelled** says, so that the model and the recipes use the same terms. The reverse-engineer skill's last step brings you back for steps 4 to 9.

1. **List the facts the model states.** Each designer's are listed above. To see which this model uses, write the recipe file as `{}` and run the Software Factory: the report lists each kind of fact that has no recipe, with its count, in its **Warnings** table. That run turns the check on, so the structure and ownership checks can fail the run from then on; a run with errors applies nothing by itself, so apply the changes you expect with `apply_staged_file_changes`.
2. **Start from the technology guides** (above). For each technology the code uses, make its guide's decisions from the code, copy its recipe file, and keep the find paths for the styles this codebase uses. For a technology no guide covers, read a few examples of each kind of fact, such as two controllers and two entity configurations, look for every style the code uses (some entities configured in `OnModelCreating`, others in `IEntityTypeConfiguration` classes), and give a recipe one `find` path per style. Also find each element type's convention, if it has one (above).
3. **Write the recipes.** From a guide, apply its **Adapting to house style** rows to whatever this codebase does its own way (a base class, a helper method, its own attribute). Add an `own` entry for each kind of code the model must describe that no guide's entries cover: every endpoint, every entity configuration, every class map, every index, and one per element type that follows a convention. Give every recipe with a `default` an `unreadable` case for the other forms of the same declaration, such as a route attribute whose template isn't a string literal, and every recipe with a `default` or `absent` an `unstated` that names what's missing and what the technology then does.
4. **Run the Software Factory** and read the reports, then load **validate-drift-recipes** and run it.
5. **Fix the recipes until no fact is reported as can't be verified, no kind of fact is warned about as having no recipe, and validate-drift-recipes has no probe or wrong-element warnings and no sample the code doesn't bear out.** After writing or changing a recipe, look at its fact with validate-drift-recipes §4: every element it covers should read its own code. Compare each kind of fact's counts with its guide's **Values come from** column: values from `default` or `absent` where the guide says code mean the recipe doesn't read this codebase's form yet. A fact that can't be verified is a recipe that doesn't read this code yet: widen its `find`, add a case, or add `default` or `absent` where saying nothing states a value (no `ToTable` means the `DbSet`'s name; no `[Authorize]` means `None`). If no recipe can read a fact (a value built in a helper method, or a form an `unreadable` case reports), list it for the developer; don't change the code or the model.
6. **Check each contradiction against the code before you touch the model.** Open the code at the row's location and confirm that it states what the row says. Several contradictions of one kind with the same code value (every one `code (none)`, say) mean the recipe is misreading the code: fix the recipe, not the model. Then resolve the rest by fixing the model to match the code, never the code. A recipe that reads the code correctly and disagrees with the model has found a modelling slip. Where the code itself looks wrong, list it for the developer; don't change it.
7. **Resolve the code the model doesn't describe:** model it, or add an exception, asking the developer unless it is clearly a helper. If a convention entry reports many helpers, the convention is wrong: narrow it, or remove it.
8. **Delete any `guardrails/*.sync.json`.** They belong to the old drift check, and nothing reads them.
9. **Have the developer approve the model changes.** In one `ask_user_question`, list every model change the setup made, for the developer to approve. Skip this step, and the approval in the exit condition, when an orchestrating skill's **User decision** says so.

**Exit condition:** validate-drift-recipes has no probe or wrong-element warnings and no sample the code doesn't bear out; a Software Factory run reports no drift and no warnings, so it writes no drift report; and the developer has approved the model changes the setup made.
