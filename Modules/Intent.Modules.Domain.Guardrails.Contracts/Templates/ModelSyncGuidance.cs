using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The always-on model sync rule, and the sync-model, configure-drift-check and validate-drift-recipes skills, shared
    /// by both Guardrails modules. One file each covers every Guardrails designer installed in the application, so only one
    /// module writes it: the Domain module when it is installed, otherwise the Services module. Identical in both
    /// modules apart from the namespace; keep them in step. Edit the text with a plain-text editor: Intent's file
    /// tools re-indent raw strings.
    /// </summary>
    internal static class ModelSyncGuidance
    {
        public const string DomainModuleId = "Intent.Domain.Guardrails.Contracts";
        public const string ServicesModuleId = "Intent.Services.Guardrails.Contracts";

        /// <summary>Whether <paramref name="moduleId"/> is the module that writes the shared rule and skills.</summary>
        public static bool IsWriter(ISoftwareFactoryExecutionContext context, string moduleId)
        {
            return moduleId == DomainModuleId || !IsInstalled(context, DomainModuleId);
        }

        public static string Rule(ISoftwareFactoryExecutionContext context)
        {
            var designers = Designers(context);
            return """
---
description: Keep the Intent Architect model true to the hand-written code of this Guardrails application. Whenever you change code, update the model in the same piece of work; once the drift check is set up, every Software Factory run fails while the model and the code disagree.
alwaysApply: true
---

# Keep the model true to the code

In this application the code is hand-written and the Intent Architect model describes it. Whenever you change code here, update the model in the same piece of work.

| If you change… | Also update in the model |
| --- | --- |
""" + "\n" + string.Concat(designers.Select(x => x.RuleRows)) + """

## The Software Factory checks it

Every Software Factory run compares the model with the code on disk and writes the result to
""" + " " + Reports(context, designers) + """
, in the `.cache` folder beside the solution's `.isln` file, which git ignores. A report is only written when it needs attention, and the run then lists it in its changes as a new file; a run with no drift writes none and logs that it found none. A run with errors applies none of its changes, so its reports aren't on disk until they are applied. Its one error says how many drifts there are and names the report (it is on the first drifted model element, so Intent offers to fix it with AI): read the rows in the report, applying it with `apply_staged_file_changes` and its path. The check is on for a designer once its recipe file, `guardrails/<designer>.drift.json`, exists; until then its report says the check is off, and the **configure-drift-check** skill sets it up.

A report has up to two tables, and an empty one is left out:

- **Model and code drift**: one row per difference, with the model element (and the class or Service it belongs to), the code file (where the code states it, or where it would) and what the drift is. The model must never be wrong, so every row fails the run. A row says that the model and the code state different values, that a fact can't be verified (the check couldn't read it from the code), that code of a kind the model must describe isn't modelled, or that a modelled type or member is missing from the code.
- **Warnings**: a kind of fact with no recipe, an exception that matches nothing, or the check being off. They only warn: **configure-drift-check** adds and fixes recipes, and a stale exception is removed.

A recipe can misread the code. Before you change the model or the code for a contradiction, open the code at the row's location and check that it says what the row says it does; if it doesn't, the recipe is wrong: fix it with **configure-drift-check**. **validate-drift-recipes** shows what every recipe reads, on demand.

A type or member you change in the model is left to the Software Factory until its change is applied, as long as its code hasn't changed: the check remembers, on this machine, what the model and the code looked like when they last agreed. A difference the code made is reported, and so is one where both changed, because applying the model's change would overwrite the code's; that row ends by saying both have changed. A fact about a type or member you have just added (a column's configuration, a route), or about a method whose parameters you changed, is checked on the same run, against the code as it will be once the Software Factory's changes are applied: the member itself isn't a row, but anything the code still lacks then (no `IsRequired()`, no route) is, and fails the run. On a machine's first run, or on CI, there is nothing to remember, so a change you modelled first is reported there until it is applied. A run with errors doesn't apply its changes by itself (Intent leaves them unticked): apply the ones you expect with `apply_staged_file_changes` and their paths, then run the Software Factory again.

## Before you finish

1. Save every designer you changed (`saveOnSuccess: true` on the last script).
2. Run the Software Factory, apply the changes you expect, and run it again.
3. The last run must report no drift. Resolve every drift row with the **sync-model** skill.
4. If code you wrote states a fact in a way the recipes don't read yet (a new technology, or a new way of declaring it), the fact is reported as can't be verified. Load **configure-drift-check** and extend the recipes, then check them with **validate-drift-recipes**.

Never edit a drift report: it is rewritten on every run.

Any `guardrails/*.sync.json` files belong to the old drift check. Nothing reads them, and they can be deleted.
""";
        }

        public static string Skill(ISoftwareFactoryExecutionContext context)
        {
            var designers = Designers(context);
            return """
---
description: Fix drift between the Intent Architect model and the hand-written code of a Guardrails application. Resolves every row of the drift reports the Software Factory writes, by updating the model, the code, a recipe or the exceptions, until the run reports no drift. Use when the user asks to fix, resolve or sync drift, when a Software Factory run fails with drift, or to sync the model with the code.
short-description: Resolve every row of the drift reports.
argument-hint: "[designer]"
---

# Sync the model with the code

In a Guardrails application the code is hand-written and the model describes it. Every Software Factory run compares the two and writes
""" + " " + Reports(context, designers) + """
, in the `.cache` folder beside the solution's `.isln` file, only when there is something in them; the run lists each one in its changes as a new file. A report has a **Model and code drift** table (the model element, the code file and what the drift is; every row fails the run) and a **Warnings** table. A run with errors applies nothing, and its one error only says how many drifts there are and names the report: apply the report with `apply_staged_file_changes` and its path, then read it. You are done when a run finds no drift: it logs that, and writes no report. A report that says the check is off has no recipe file yet: load **configure-drift-check** to set it up.

Load **scripting-the-model** and **changing-the-model** before changing the model, and **generating-and-applying-code** before running the Software Factory.

## How the check works

- **Facts**
""" + " (" + string.Join("; ", designers.Select(x => $"{x.Name}: {x.FactsText}")) + ")" + """
 are read from the model's stereotypes, and compared with the values that the recipes in `guardrails/<designer>.drift.json`, beside the application's `.application.config`, read from the code. A recipe says where a kind of fact is in the code and how to read its value, for whatever technology the code uses. The **configure-drift-check** skill describes them.
- **Structure:** each type the Guardrails templates generate is compared with its file on disk, on what its element can state: kind, modifiers, generic parameters, parameters on the type (a positional record, a Class's Primary Constructor), the base types the model names, and the members its element can model, with their types and signatures. What the designer can't express is never compared: a Service's, handler's or repository's primary constructor holding its injected dependencies, a DTO's or command's methods, a Service's properties, a handler's members, any field, event or indexer. A member the model doesn't describe is reported only when it's public: whether a protected, internal or private one belongs in the model is a judgement call, and once modelled it's compared like any other. These rows come from what the templates generate, not from the recipe file: no recipe adds or removes them, and an exception only covers an "isn't modelled" row. Spellings that mean the same type agree (`Nullable<int>` and `int?`, `System.String` and `string`, a qualified name and its last part).
- **Ownership:** the recipes' `own` lists say which code the model must describe, such as every controller action, every `Entity<T>` configuration, and every type that follows the convention of a modelled element type (every `*DTO` class in the DTO project, every class deriving from the business base class). Types the templates generate count as modelled. Members of a generated type count too, at the access levels the model uses for that kind of member.
- **Exceptions:** the recipe file's `exceptions` list code that may stay unmodelled. A type or member exception covers that declaration, and a type its members, but not code a convention finds inside a member. They never cover a contradicted or unverifiable fact.

## 1. Read the drift

Run the Software Factory, or read the reports of the last run. A failing run's error doesn't list the rows: apply the report with `apply_staged_file_changes` and its path, then read it. Each drift row names the model element, the code's file and line, and what the drift is. Read only the code the rows name.

## 2. Resolve each drift row

| The Drift column says | What to do |
| --- | --- |
| Any row ending (the model and the code have both changed since they last agreed) | Each side changed it since they last agreed, so the Software Factory's change would overwrite the code's. Don't apply that change, or touch either side, until you know which is right: a change made in this piece of work, or one the developer asked for, is right; otherwise ask the developer in one `ask_user_question` covering every such row, showing both versions. Then resolve it as its row below says |
| `<element>` *fact*: model `…`, code `…` | First check the recipe read the code: open the code at the row's location, or look at the row's element and fact with **validate-drift-recipes** (its §4), which shows the text the value was read from. If it doesn't say what the row says (the value comes from another line, or from the recipe's default where the code does state the fact), the recipe is wrong: look at that fact for every element the same way, fix it with **configure-drift-check**, and touch neither side. Otherwise decide which side is right. A code value followed by a bracket comes from the code saying nothing: the bracket says what's missing and what the technology then does, and the row points at where the code would say it (the entity's configuration, the controller). A change made in this piece of work, or one the developer asked for, is right: update the other side. Otherwise ask the developer in one `ask_user_question` covering every such line, recommending the code where its change looks deliberate. Then update the model, or change the code to match it |
| `<element>` *fact* can't be verified: … | The recipe couldn't read the fact. Load **configure-drift-check** and fix the recipe until it reads the value. Never change the code so that a recipe can read it, and never make a recipe return a fixed value to get past it. If no recipe can read it (a value built in a helper method, or a line that says the code states it in a form the recipe can't read, such as a route from a constant), tell the developer which facts and why, and leave the model as it is |
| … isn't modelled | Decide what it is. A concept the model describes (an endpoint, a service operation, a DTO, a business class, an entity, a property of a request, DTO or entity, an entity configuration, an index): model it. Clear helpers and framework plumbing: add an exception. Anything else: ask the developer in one `ask_user_question` listing each item with what you think it is, and offer per item: model it, or add an exception |
| … is modelled, but not in the code | If you modelled it first (reported only on a machine's first run, or on CI), apply the Software Factory's output (a run with errors leaves it unticked) and run it again. Otherwise the code removed or renamed it. If that was deliberate, update the model (rename the element rather than deleting it and creating another); if not, restore the code. Ask when unsure |
| The recipe file is wrong: … | Load **configure-drift-check** and fix the recipe file |

Never change a recipe to make a contradiction pass: a recipe that reads the code correctly and disagrees with the model has found drift, and either the model or the code is wrong.

The **Warnings** table only warns, but resolve it too. **No recipe**: a kind of fact the recipe file doesn't cover, such as one a module upgrade added; load **configure-drift-check** and add its recipe. **Exception matches nothing**: a stale exception; remove it from the recipe file.

When several rows look wrong in the same way, or you changed a recipe, load **validate-drift-recipes**: it shows what each recipe reads and warns about read cases that don't read the code, or that read another element's.

## Exceptions

Add an exception to `exceptions` in the designer's recipe file: a file pattern (`src/Api/Health/*.cs`), a type (`HealthController`), a member (`OrdersController.Ping`) or the name the report gives, optionally with a reason. A type or member exception exempts that declaration, and a type its members, but not code a convention finds inside a member, such as each endpoint a mapping method registers or each entity a configuration method configures: exempt one of those by the name the report gives it, or by a file pattern.

```json
"exceptions": [
  { "match": "HealthController", "reason": "load-balancer probe" }
]
```

The report warns about any exception that matches nothing: remove it.

## 3. Confirm

Save the designers, run the Software Factory, apply its output, and run it again. The run must report no drift errors, and write no drift report.

## Gotchas

- The check reads the saved model and the code on disk: save the designers, and apply the Software Factory's output, before judging a report.
- A type or member changed in the model isn't reported while the Software Factory's change to it is waiting to be applied, on a machine where the check has run before (it remembers the model and the code in `.cache`), unless its code changed too. On a first run, or on CI, it is reported until applied. A run with errors applies nothing by itself: apply the changes you expect with `apply_staged_file_changes`.
- A fact about a type or member the Software Factory is adding, or about a method whose parameters it is changing, is read from the code as it will be once those changes are applied. Its row is a gap to fix now, alongside applying the changes, not something to wait for. While the code it read isn't on disk yet, its row points at the file without a line.
- When code has removed a modelled member, the Software Factory proposes adding it back as an empty stub. Don't apply that unless the member should come back; update the model instead.
- Every row is about something the model can state. If one asks for something the designer has no way to express, don't work around it with an exception, a recipe or a code change: tell the developer, because the check shouldn't report it.
- The drift reports are in the solution's `.cache` folder, which git ignores, and a run lists each one it writes as a new file. They aren't code: they need no review, and they reach the disk when the run's changes are applied. A run with no drift deletes the last one and writes none. Never edit them.
""";
        }

        public static string ConfigureDriftCheckSkill(ISoftwareFactoryExecutionContext context)
        {
            var designers = Designers(context);
            var files = string.Join(" and ", designers.Select(x => $"`guardrails/{x.Name.ToLowerInvariant()}.drift.json`"));
            return """
---
description: Write or extend the Guardrails drift check's recipe files (guardrails/<designer>.drift.json), which tell the Software Factory where each fact the model states is in the code and how to read it, for whatever technology the code uses. Use while reverse-engineering a Guardrails application (straight after its extraction, and again at its end), when a drift report says the check is off, facts can't be verified or the recipe file is invalid, when validate-drift-recipes shows a recipe misreading the code, or when the code starts using a technology no recipe covers.
short-description: Write the drift check's recipes for this codebase's technology.
---

# Configure the drift check

Once a designer has a recipe file, every Software Factory run compares its model with the code; without one, the check is off. The model side is fixed: the facts listed below, read from the model's own stereotypes. The code side is yours to describe, once, in a **recipe file** per designer: for each kind of fact, where it is in the code and how to read its value in the model's vocabulary. The module knows no technology, and your recipes do, so they work for controllers, minimal APIs, FastEndpoints, EF Core, Mongo, Cosmos or anything else the code uses.

You are done when **validate-drift-recipes** shows every recipe reading the code, a Software Factory run reports no drift and no warnings, and the developer has approved the model changes the setup made (step 9). A report (
""" + Reports(context, designers) + """
, in the `.cache` folder beside the solution's `.isln` file) is only written while there is something to act on: drift, a kind of fact that isn't checked, an exception that matches nothing, or the check being off.

**Never change the code while setting up the check.** The recipes and the model are made to reflect the code as it is. A fact no recipe can read, or code that looks wrong, is reported to the developer, not rewritten.

Load **generating-and-applying-code** before running the Software Factory, and **sync-model** for the drift your recipes find.

## Start from a technology guide

For each technology below, a guide beside this skill has a recipe file tested against code in every style it covers, the decisions that change it, how each form of the code is modelled, and how to adapt the recipes to a codebase's own conventions. Find every technology each designer's code uses (a codebase often uses several, such as EF Core and MongoDB, or controllers and minimal APIs), read each one's guide, and start the recipe file from them.

| Designer | Technology | Recognise it by | Guide |
| --- | --- | --- | --- |
""" + "\n" + string.Concat(designers.Select(x => x.Guides)) + """

Where one designer's code uses two technologies, combine their recipe files fact by fact: list both guides' find paths and read cases, and keep their `default`, `absent` and `unstated` where they agree. Where they differ, keep one, and turn the other into a read case on the code its own find paths reach. For a technology no guide covers, write the recipes from the format below, with the closest guide as a model of a complete recipe file.

## Recipe files
""" + "\n\n" + files + """
, in the folder that holds the application's `.application.config` (not the code's folder). Commit them with the code. Paths inside a recipe file are relative to the application's output location (the codebase root), with forward slashes.

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

""" + string.Concat(designers.Select(FactsSection)) + """
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
""";
        }

        public static string ValidateDriftRecipesSkill(ISoftwareFactoryExecutionContext context)
        {
            var designers = Designers(context);
            var tasks = string.Concat(designers.Select(x => $"| {x.Name} | `guardrails/{x.Name.ToLowerInvariant()}.drift.json` | `{x.ModuleId}.ValidateDriftRecipes` |\n"));
            return """
---
description: Check that the Guardrails drift check's recipe files read the right code. Runs each Guardrails designer's ValidateDriftRecipes module task, which reads every fact the model states through its recipe and returns what it read, then checks it against the code; or narrows it to one kind of fact, element or code file and lists every reading there. Use after writing or changing a recipe file, when drift rows look wrong in the same way (contradictions or can't-be-verified facts the code doesn't bear out), before changing the model for a contradiction, or when the developer questions what the drift check reports.
short-description: Check what each drift recipe reads from the code.
---

# Validate the drift recipes

A recipe file tells the drift check where each fact the model states is in the code, and how to read it. A recipe that misreads the code fails loudly (a contradiction the code doesn't bear out) or silently (it agrees with the model by accident), and the drift report can't tell which. Each Guardrails designer has a module task that reads every fact through its recipe, on demand, and returns what it read as JSON. It changes nothing.

## 1. Run the task

Load **scripting-the-model**. Module tasks run only in `run_designer_script`, not in `run_designer_query`. Run this against any of the application's designers, with the same application id, once per designer:

```js
return JSON.parse(await executeModuleTask("<task>", JSON.stringify({ applicationId: "<the application's id>" })));
```

| Designer | Recipe file | Task |
| --- | --- | --- |
""" + "\n" + tasks + """

It reads only the recipe file and the code on disk as they are now (save the recipe file first), and the model as saved. Nothing from a Software Factory run goes in, so there is no need to run one first.

## 2. Read what it returns

- `errorMessage`: the task couldn't run, such as for an unknown application id.
- `recipeFileExists`: `false` when the designer has no recipe file yet: load **configure-drift-check**.
- `problems`: what is wrong with the recipe file itself: invalid JSON, an unknown key, a bad find path or regular expression.
- `notChecked`: the kinds of fact the model states that have no recipe, with their counts.
- `facts`: per kind of fact, how many facts the model states, and how many were `readFromCode`, `fromDefault`, `fromAbsent`, `unverifiable`, and `contradicted` (the code's value differs from the model's, wherever it came from). Each has up to three `samples`: the `element`, the `model` and `code` values, and the `location` the value was read at, or the place `near` which the code would state it, or the `problem`. `readCasesMatchingNothing` lists the read cases no fact matched.
- `conventions`: per `own` entry, how much code it `found`, and the first few `samples`, named as the drift report would name them, with their locations.
- `probeWarnings`: read cases whose reading didn't change when the code they matched was changed in memory: the case isn't reading the code it seems to.
- `wrongElementWarnings`: read cases that read some facts from code that names another element of the same kind and not their own (another attribute's configuration, another action's attributes): the case takes whichever such code comes first.

## 3. Check the recipes against the code

Open the code at the samples' locations, and check that each states what its sample says. A recipe is wrong when:

- the probe warns about one of its read cases: make the case's `match` cover the code that states the fact;
- a wrong-element warning names one of its read cases: tie its `find` or `match` to the element, usually with the element's placeholder (`{Attribute}`, `{Operation}`), so that each fact reads its own code;
- a sample's value comes from another line, or from `default` or `absent` where the code does state the fact;
- most of a kind's facts come from `default` or `absent`, or many can't be verified;
- its `contradicted` facts share one code value (every one `(none)`, say): it misreads the code. A contradiction the code bears out is drift, for **sync-model**, not a recipe fault;
- a read case matches nothing, when the code does use that form;
- a convention finds code that isn't of its element type (helpers, base classes), or finds far more or fewer than the model has of that element type.

Fix the recipe file with **configure-drift-check**, then run the task again until none of these is left. The probe proves that a case reads some code, and the wrong-element warnings catch a case that reads a sibling's, but neither proves it reads the right code, and a value from `default` or `absent` has no code to change: the samples, and the readings in §4, are what show that a recipe reads the right code. Never change the code, or make a recipe return a fixed value, to make a finding go away.

## 4. Look at one recipe, element or file

To see everything one recipe reads, or how one element's facts or one file were read, add any of these to the argument:

```js
return JSON.parse(await executeModuleTask("<task>", JSON.stringify({ applicationId: "<the application's id>", fact: "column.maxLength", element: "Order" })));
```

- `fact`: a kind of fact, as the recipe file names it. One the designer doesn't state returns an `errorMessage` listing those it does.
- `element`: an element as the drift report names it. `Order` also selects `Order.Notes`.
- `file`: a code file's path relative to the codebase, or its end (`AppDbContext.cs`). It selects the facts read there, or that would be stated there.

It returns the same, for those facts only and without `conventions`, plus `readings`: every fact selected, not just samples. Each has its `kind`, `element`, `model` and `code` values, `from` (`code`, `default`, `absent` or `unverifiable`), whether it is `contradicted`, the `findPath` and `readCase` that produced it (counted from 1, as the recipe file lists them), for a value read from the code its `level` (0 for the code its `find` found, 1 or more for code inherited from further out) and the text it `matched`, and its `location`, `near` or `problem`. Use it:

- after adding or changing a recipe, with `fact`, to check that every element it covers reads its own code;
- before changing the model for a contradiction, with the row's `element` and `fact`, to see the code the value was read from;
- after a wrong-element warning, with its `fact`, to see which elements read whose code.

A kind of fact with many elements returns many readings, so narrow it with `element` or `file` where you can.
""";
        }

        private static string FactsSection(DesignerGuidance designer)
        {
            var kinds = FactKinds.For(designer.Name);
            var rows = string.Concat(kinds.Select(x =>
                $"| `{x.Key}` | {x.Label} | {string.Join(", ", x.Placeholders.Select(p => $"`{{{p}}}`"))} | {x.Values} | {x.Source} |\n"));
            var placeholders = string.Concat(kinds.SelectMany(x => x.Placeholders).Distinct().Select(x => $"- `{{{x}}}`: {FactKinds.Describe(x)}.\n"));
            return $"\n### {designer.Name}: `guardrails/{designer.Name.ToLowerInvariant()}.drift.json`\n\n" +
                   "| Fact | Reported as | Placeholders | Model values | What the model states |\n| --- | --- | --- | --- | --- |\n" +
                   rows + "\n" +
                   "Placeholders:\n\n" + placeholders + "\n";
        }

        /// <summary>
        /// A designer's drift report as the guidance names it, from the folder holding the solution's <c>.isln</c>, e.g.
        /// <c>intent/.cache/guardrails/BasislinkService/DomainDriftReport.mdx</c>. Not absolute: the guidance is committed,
        /// and an absolute path differs per machine.
        /// </summary>
        public static string ReportPath(ISoftwareFactoryExecutionContext context, string designer)
        {
            var solutionFolder = Path.GetFileName(context.GetSolutionConfig().SolutionRootLocation.TrimEnd('/', '\\'));
            return $"{solutionFolder}/.cache/guardrails/{context.GetApplicationConfig().Name}/{designer}DriftReport.mdx";
        }

        private static string Reports(ISoftwareFactoryExecutionContext context, IReadOnlyList<DesignerGuidance> designers)
        {
            return string.Join(" and ", designers.Select(x => $"`{ReportPath(context, x.Name)}`"));
        }

        private static IReadOnlyList<DesignerGuidance> Designers(ISoftwareFactoryExecutionContext context)
        {
            var result = new List<DesignerGuidance>();
            if (IsInstalled(context, DomainModuleId))
            {
                result.Add(DesignerGuidance.Domain);
            }

            if (IsInstalled(context, ServicesModuleId))
            {
                result.Add(DesignerGuidance.Services);
            }

            return result;
        }

        private static bool IsInstalled(ISoftwareFactoryExecutionContext context, string moduleId)
        {
            return context.InstalledModules.Any(x => x.ModuleId == moduleId);
        }

        /// <param name="Guides">
        /// The configure-drift-check skill's index rows for the designer's technology guides, which its own module writes
        /// beside the skill (<c>DriftRecipeGuides</c>).
        /// </param>
        private sealed record DesignerGuidance(string Name, string ModuleId, string FactsText, string RuleRows, string Guides)
        {
            public static readonly DesignerGuidance Domain = new(
                Name: FactKinds.Domain,
                ModuleId: DomainModuleId,
                FactsText: "database tables, keys, columns and relationships",
                RuleRows: """
| An entity, value object, data contract, enum, repository, domain service or domain event: the type, a property, a method or its signature | That element in the Domain designer |
| A database table, collection, column, key or relationship | The Domain designer's database stereotypes and associations |

""",
                Guides: """
| Domain | EF Core | A `Microsoft.EntityFrameworkCore` package and a class deriving from `DbContext` | [references/ef-core.md](references/ef-core.md) |
| Domain | MongoDB | A `MongoDB.Driver` package: `IMongoCollection<T>`, `BsonClassMap`, `[BsonId]` | [references/mongodb.md](references/mongodb.md) |
| Domain | Azure Cosmos DB | A `Microsoft.Azure.Cosmos` package (`CosmosClient`, `Container`), or EF Core's `UseCosmos` | [references/cosmos-db.md](references/cosmos-db.md) |

""");

            public static readonly DesignerGuidance Services = new(
                Name: FactKinds.Services,
                ModuleId: ServicesModuleId,
                FactsText: "HTTP routes, verbs and parameter binding, and security",
                RuleRows: """
| A command, query, DTO, service, enum, message, handler or API client: the type, a field, a method or its signature | That element in the Services designer |
| An endpoint's route, verb or parameter binding, or its authorization | Its `Http Settings` and `Parameter Settings`, and `Secured` or `Unsecured` |

""",
                Guides: """
| Services | ASP.NET Core controllers | Classes deriving from `ControllerBase` or `Controller`, with `[ApiController]` or `[HttpGet]` | [references/aspnetcore-controllers.md](references/aspnetcore-controllers.md) |
| Services | Minimal APIs | `app.MapGet(…)`, `MapPost(…)` or `MapGroup(…)` | [references/minimal-apis.md](references/minimal-apis.md) |
| Services | FastEndpoints | A `FastEndpoints` package: classes deriving from `Endpoint<TRequest>` | [references/fastendpoints.md](references/fastendpoints.md) |

""");
        }
    }
}
