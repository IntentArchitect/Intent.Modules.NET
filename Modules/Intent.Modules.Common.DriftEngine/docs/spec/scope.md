# Scope

Scope says which code the engine reads (block 5). It is part of the model, on the Codebase Structure designer (decision 1), so it is reviewed, versioned and changed like the rest of the model.

Nothing is in scope until the model says so. Real codebases carry code nobody wants checked: test projects, scaffolds, generated clients, committed binaries, scripts, leftovers. basislink5 has a test project, test code in production folders, an EF Core database-first scaffold and a `Libs/` folder of binaries; listing what is in, with a reason for every exception, keeps that out without touching it.

## Why the Codebase Structure designer

- It already describes the application's folders and projects, so a scope row sits on the folder it is about.
- File Classification already puts glob rows on the same elements, resolved the same way. Scope reuses that matcher and its resolution, so a developer learns one set of rules.
- eShopCustom's Architecture module put its scope on the Codebase Structure Root Folder too. The Domain, Services and Front End modules each put theirs on their own package, so one codebase had four scopes to keep in step.

## The `Drift Scope` stereotype

The module adds a `Drift Scope` stereotype that applies to the Root Folder and to every Folder of the Codebase Structure designer. A row on either covers any project inside it by glob, so project elements don't carry it ([decision 27](README.md)). It has one property, `Entries`, an item list. Each row has:

| Property | Type                | Meaning                                                                                                |
| -------- | ------------------- | ------------------------------------------------------------------------------------------------------ |
| `Glob`   | text, one line each | The files the row matches, relative to the element's folder. Same rules as File Classification, below. |
| `Types`  | text, one line each | Optional. Type names in the matched files that the row excludes. A row with `Types` must be `Out`.     |
| `Scope`  | `In` or `Out`       | Whether the matched files (or types) are read                                                          |
| `Reason` | text                | Why. Required on every `Out` row; optional on `In` rows.                                               |

### Glob rules

The rules are File Classification's, so the same matcher serves both:

- Every pattern is anchored to the element's folder. `*.cs` matches the top level only; `**/*.cs` matches at every depth; `src/**` matches everything under `src`. A bare folder name (`src`) doesn't match its contents.
- `*` and `?` stay within one folder. Matching ignores case.
- `!` excludes, `#` starts a comment, and within one row the last matching line wins. A row with no lines matches everything.
- A leading `/` matches nothing, and braces `{a,b}` and classes `[ab]` aren't supported. Both are `config/glob` errors.

### How a file's scope resolves

As File Classification resolves a file's row ([R27](requirements.md)):

1. Take the deepest Folder, project or Root Folder whose folder contains the file and that has a `Drift Scope` row matching it. Rows with `Types` don't count here.
2. Within that element, the last matching row decides `In` or `Out`.
3. An element none of whose rows match hands the file up to the next element above.
4. A file no row matches is out. It isn't listed one by one: the coverage listing counts it as "not in any row".

Built-in exclusions come first and no row can bring them back in: `.git`, `bin`, `obj` and `node_modules` folders at any depth, hidden folders (a name starting with `.`), and the Intent solution's own folders (any folder holding an `.isln`, `.application.config` or `Intent.Metadata`).

An example, with the Root Folder and a `src/Api` project element:

| Element     | Row | Glob                | Scope | Reason                                  |
| ----------- | --- | ------------------- | ----- | --------------------------------------- |
| Root Folder | 1   | `src/**`            | In    |                                         |
| Root Folder | 2   | `src/**/*.Tests/**` | Out   | Test projects                           |
| Root Folder | 3   | `src/Legacy/**`     | Out   | Being replaced by `src/Domain`          |
| `src/Api`   | 1   | `Generated/**`      | Out   | NSwag client, regenerated from the spec |

| File                                      | Decided by                                                        | Scope |
| ----------------------------------------- | ----------------------------------------------------------------- | ----- |
| `src/Api/Controllers/OrdersController.cs` | Root Folder row 1 (`src/Api` has no matching row, so it hands up) | In    |
| `src/Api/Generated/OrdersClient.cs`       | `src/Api` row 1                                                   | Out   |
| `src/Shop.Tests/OrderTests.cs`            | Root Folder row 2                                                 | Out   |
| `src/Legacy/Order.cs`                     | Root Folder row 3                                                 | Out   |
| `build/pipeline.yml`                      | no row                                                            | Out   |
| `src/Api/obj/Debug/Api.AssemblyInfo.cs`   | built-in (`obj`)                                                  | Out   |

### Excluding types

A row with `Types` excludes types inside files that are otherwise in scope ([R26](requirements.md)). Each line is a type's simple or qualified name, and `*` matches within a name. The row applies to the files its `Glob` matches; it must be `Out` (a `Types` row marked `In` is a `config/typesIn` error). An excluded type's declaration, members and bodies aren't read, and nothing in it locates a claim.

basislink5 needs this: test helpers live in production files beside the code they test.

## What scope decides

- **Readers read only in-scope files** ([R22](requirements.md)). An out-of-scope file is never opened, and the run result's `files` proves it.
- **Readings come only from in-scope code**, for every rule kind: recipes, queries and plug-ins.
- **Opt-in rules report only in-scope code** ([R49](requirements.md)).
- **Packs and the house pack aren't code.** They are read wherever they are.

## Shadow lookup

When a claim's code isn't found in scope, the engine checks whether it would have been found in out-of-scope code, so the finding says "out of scope" rather than "missing" ([R25](requirements.md)). Out-of-scope code is a file an Out row excludes, or one no row includes (nothing is in scope until a row says so). Built-in exclusions (`bin`, `obj`, ...) are junk by definition and aren't looked at. A file no row includes has no row to name, so the message says no Drift Scope row includes it.

The lookup uses **file paths only and never opens an excluded file**. It repeats the find's named steps against out-of-scope paths: a `class:Order` step looks for a file whose name, up to its first dot, is `Order`; a `file:` step matches as written. A hit makes the claim can't verify, `outOfScope`, naming the file and the row that excludes it:

> Class `Order` isn't in scope. `src/Legacy/Order.cs`, which the Root Folder's scope row `src/Legacy/**` excludes ("being replaced by `src/Domain`"), may hold it.

No hit leaves the outcome to the rest of the order in [claims-and-findings.md](claims-and-findings.md#outcomes): a missing type is contradicted.

## Findings about scope

| Rule             | When                                                                                            | Severity               |
| ---------------- | ----------------------------------------------------------------------------------------------- | ---------------------- |
| `scope/missing`  | No `Drift Scope` row anywhere. One finding, with the number of claims none can be verified for. | Error                  |
| `scope/reason`   | An `Out` row with no reason                                                                     | Error                  |
| `scope/stale`    | A row that decides no file and excludes no type                                                 | Warning                |
| `scope/coverage` | The listing below                                                                               | Info, logged only      |
| `config/glob`    | A pattern the matcher rejects                                                                   | Error                  |
| `config/typesIn` | A row with `Types` marked `In`                                                                  | Error                  |

With no scope, every claim is can't verify, `outOfScope` (or `blocked` behind an owner that is), and only `scope/missing` is reported, with the count:

> No Drift Scope is set, so no code is in scope and none of the 412 claims can be verified. Add a Drift Scope to the Codebase Structure designer's Root Folder.

## The coverage listing

The Software Factory log lists what is out of scope, so a person can see what a green run didn't look at ([R15](requirements.md)). The drift report never does: it holds only drift (decision 30), and what is out of scope isn't drift. The listing gives:

- per row: its element, glob, scope, reason, and how many files (or types) it decides;
- the built-in exclusions, with counts;
- files in no row, with a count per top-level folder.

## The `Drift Check` stereotype

The rest of the engine's model configuration sits on the Root Folder in a second stereotype, `Drift Check`, with four item lists.

| List                 | Row                          | Meaning                                                                                                                                                                                                                                  |
| -------------------- | ---------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Packs`              | `Pack`, `Version`, `Hash`    | The packs the codebase uses, pinned ([R52](requirements.md)). Stack packs come from installed modules; the house pack from the application's `drift/` folder. `Hash` pins a house pack's plug-ins ([packs.md](packs.md#choosing-packs)). |
| `Severity Overrides` | `Rule`, `Severity`, `Reason` | A severity change for findings whose rule id matches ([claims-and-findings.md](claims-and-findings.md#overrides))                                                                                                                        |
| `Opt-in Rules`       | `Rule`, `Glob`               | Turns an opt-in rule on, optionally narrowed by a glob relative to the output folder ([rules.md](rules.md#opt-in-must-be-modelled-rules))                                                                                                |
| `Exceptions`         | `Rule`, `Match`, `Reason`    | Code an opt-in rule must not report, and why                                                                                                                                                                                             |

Every `Rule` column holds a finding's rule id, as findings carry it: `optin/unmodelled.type` to turn that rule on or except its findings, `claim/column.*` to override every column claim's severity.

Configuration findings:

| Rule                     | When                                                                     | Severity |
| ------------------------ | ------------------------------------------------------------------------ | -------- |
| `config/packMissing`     | A listed pack isn't available at any version                             | Error    |
| `config/packVersion`     | A listed pack is available only at another version, or its row has no `Version` | Error    |
| `config/packUnused`      | A pack provides vocabulary for no designer with content and no rule used | Warning  |
| `config/overrideReason`  | A severity override with no reason                                       | Error    |
| `config/exceptionReason` | An exception with no reason                                              | Error    |
| `optin/staleException`   | An exception that matches nothing                                        | Warning  |
| `config/unknownRule`     | An override, opt-in row or exception naming a rule no pack provides      | Error    |

## Acceptance cases

The `scope` area of the suite ([acceptance-suite.md](acceptance-suite.md)) has a file per case kind: `missing`, `reason`, `stale`, `coverage`, `gate` (opt-in rules ignore out-of-scope code), `shadow`, `notRead`, `builtin`, `type` and `nested`. The catalog maps eShopCustom's adoption-scope checks onto them ([claim-catalog.md](claim-catalog.md)).
