# Conversion from Common.Drift

Phase 6 maps every Common.Drift recipe file onto the engine ([R55](requirements.md)), and runs both engines side by side ([R57](requirements.md)). The engine has no recipes (user decision, 2026-11-16: a JSON way of reading code beside C# claim readers was a second system to learn and choose between). So a recipe isn't copied: the fact it reads is the stock reader's, where one reads its kind, and otherwise a claim reader in the house's module. This document fixes what each construct becomes, and which verdicts change and why. The acceptance suite's conversion test keeps it complete: it reads the real recipe files and fails on any construct this map doesn't cover.

The conversion is Intent.Common.Drift 2.0's migration (user decision, 2026-11-23: updating Common.Drift is the whole move). It writes the rows itself, on the first Codebase Structure package's Root Folder, unless the application already has a Drift Scope or Drift Check, and logs each one ([R56](requirements.md)). Its tests read the same five codebases' recipe files.

## What converts into what

| Common.Drift                          | DriftEngine                                                                                                                                                                               |
| ------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `guardrails/<designer>.drift.json`    | Nothing of its own: its facts, `own` entries and exceptions each go as below. The migration deletes it                                                                                    |
| The code v1 checked                   | One scope `In` row for `**`: v1 checked code wherever its recipes' finds reached, so top-level `files` don't bound it (user decision, 2026-11-23)                                         |
| `facts.<kind>`                        | Nothing: the kind's stock reader reads it by name, or, where none does, it is `noRule` until the house's module reads it                                                                  |
| `own` entries                         | An `optin/unmodelled.type` Opt-in Rules row each, over the entry's `files` (else its recipe file's, else `**/*.cs`), with its `kind` as the Description. Rows with the same globs merge |
| v1's always-on unmodelled members     | One `optin/unmodelled.member` row over `**/*.cs`, where there is a Domain or Services recipe                                                                                               |
| A file-pattern exception              | A scope `Out` row, with the exception's reason                                                                                                                                            |
| A type or `Type.Member` exception     | An `Exceptions` row for `optin/unmodelled.type` or `optin/unmodelled.member`, one row per reason                                                                                          |
| An exception naming an `own` finding  | Nothing: that `own` entry gets no row, and the migration says so                                                                                                                          |
| `note`, `//` comments                 | Dropped; they stay in git history                                                                                                                                                         |

Conventions are globs only (user decision, 2026-11-16), so an `own` entry's find, placeholder filters and regular expressions don't carry over: the row reports every unmodelled type in its globs.

## The five codebases' recipe files

As of 2026-11-12, read from the local copies the suite's conversion test points at ([acceptance-suite.md](acceptance-suite.md#real-codebases)):

| Codebase   | Domain recipes  | Services recipes | UI recipes | `own` entries | Exceptions |
| ---------- | --------------- | ---------------- | ---------- | ------------- | ---------- |
| basislink5 | 16              | 12               |            | 8             | 0          |
| CrmBus     | 0 (`facts: {}`) | 10               |            | 11            | 12         |
| eShop10    | 17              | 14               | 9          | 14            | 9          |
| eShop13    | 17              |                  |            | 7             | 1          |
| eShop14    | 17              | 14               |            | 13            | 3          |
| **Total**  | **67**          | **50**           | **9**      | **53**        | **25**     |

126 recipes in all. Every exception is `{ "match", "reason" }`: 1 file pattern, 14 `Type.Member`, and 10 type or ownership names. Two files have no `exceptions` key, which means none.

## Constructs

Every construct the five codebases use, and what it becomes. A recipe's find and read constructs are dropped: the kind's reader reads the fact. An `own` entry's find steps, placeholder filters and regular expressions are dropped too: its row is a glob.

| Construct                                                                           | Uses            | Converts to                                                                                                                                          |
| ----------------------------------------------------------------------------------- | --------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| `find`, as one path or a list                                                       | 27 / 99         | Dropped                                                                                                                                              |
| `class:Name` (an alias of `type:`), `method:Name`, `property:Name`, `parameter:Name` | 139 / 123 / 36 / 3 | Dropped                                                                                                                                           |
| `statement:text`, `file:pattern`                                                    | 60 / 9          | Dropped                                                                                                                                              |
| `class with`, `method with`, `statement with`, `chain with`                         | 339             | Dropped                                                                                                                                              |
| `read` cases, with `value`, `part`, `in`, `case` and `unreadable`                   | 493             | Dropped: the kind's reader reads the value, and says `unreadable` itself                                                                             |
| `default`, `absent`, `unstated`                                                     | 116 / 38 / 121  | Dropped: the kind's reader reads silence; a `whenSet` kind never takes a value from it ([below](#semantic-changes))                                  |
| `inherit`, as one value or a list                                                   | 1 / 63          | Dropped                                                                                                                                              |
| `replace`, 1 to 3 rows                                                              | 27              | Dropped                                                                                                                                              |
| Recipe `files`, one pattern / a list                                                | 3 / 23          | The globs of an `own` entry without `files` of its own; otherwise dropped: scope decides what is read                                               |
| Top-level `files`, one pattern / a list                                             | 2 / 8           | Dropped: the `**` In row covers it                                                                                                                   |
| Placeholder filters `camel`, `trimEnd`, `trimStart`, `replace`, chained 2 to 4 deep | 51 / 8          | Dropped                                                                                                                                              |
| An empty placeholder as a switch; brace text that isn't a placeholder               |                 | Dropped                                                                                                                                              |
| .NET regular expressions: lookbehind, back-references, `(?i)`, scoped `(?i:…)`      | 14 / 4 / 2 / 26 | Dropped                                                                                                                                              |
| `own` with `class with`, `method with`, `statement with`, `chain with`, `file with` | 53 entries      | An `optin/unmodelled.type` row each, merged where the globs are the same                                                                             |
| `own` `name` with `$2` or higher                                                    | 2               | Dropped: the engine names a finding by its type. An exception naming such a finding drops its entry's row                                           |
| `own` `files`                                                                       | 35              | The row's globs, braces expanded to one line per alternative (the engine's matcher takes none)                                                       |
| `note` (recipe, case, `own`, file)                                                  | 87              | Dropped                                                                                                                                              |

Constructs the format has but none of the five uses are dropped as their kind above is: the `method:Name(types)` overload form, `chain:text`, a bare `<kind>` step, `in: property`, `in: parameter`, `in: file`, `case: lower`, and the `pascal`, `kebab`, `lower` and `upper` filters.

### Kinds

All Common.Drift fact kinds keep their names and placeholders, so a fact maps onto the engine's kind by name, except `column.required`, which becomes `member.nullable` (change 11):

- **Domain**: `table.name`, `table.schema`, `table.kind`, `key`, `column.name`, `column.type`, `column.textType`, `column.maxLength`, `column.precision`, `column.scale`, `column.default`, `relationship.multiplicity`, `relationship.foreignKey`, `index.columns`, `index.unique`, `intent.raises`.
- **Services**: `service.route`, `operation.verb`, `operation.route`, `security`, `security.roles`, `security.policy`, `parameter.source`, `intent.creates`, `intent.updates`, `intent.deletes`, `intent.queries`, `intent.invokes`, `intent.publishes`, `intent.sends`.
- **User Interface** (Intent.UI.Guardrails.Contracts, not in the configure-drift-check skill): `component.kind`, `page.route`, `intent.navigates`, `intent.opens`, `intent.uses`, and `intent.invokes`, `security`, `security.roles`, `security.policy`, with placeholders `{Folder}`, `{Component}`, `{Invoked}`, `{Route}`, `{RouteStart}`, `{Page}`, `{Dialog}` and `{Child}`. The `intent.ui` pack (phase 6) gives them vocabulary, and the Blazor reader reads them.

## Semantic changes

The engine's decisions change some verdicts on the same model and code. Phase 6's side-by-side run must explain every difference by one of these (open question 1).

| #  | Change                                                                                                                                                                                                                      | Decision | Affected recipes in the five codebases                                                                                                                                                                                          |
| -- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1  | A `whenSet` kind takes no value from silence. A modelled value the code doesn't state is contradicted.                                                                                                                      | 4        | `table.name` with `default: "{Entity}"` (4 files); one `operation.route` and one `service.route` recipe with a literal `default`.                                                                                              |
| 2  | An empty `default` or `absent` on a `whenSet` kind had no verdict of its own: the model's empty value is no longer a claim, and a set value the code doesn't state was contradicted before too.                            | 4, 10    | 69 uses across `column.name`, `column.type`, `column.maxLength`, `column.precision`, `column.scale`, `column.default`, `table.schema`, `service.route`, `operation.route`, `page.route`, `security.roles` and `security.policy` |
| 3  | An empty model value is no longer a claim, so code stating a value the model leaves empty (a `HasMaxLength(50)` with no modelled MaxLength) is no longer contradicted. The opt-in `unmodelled.value` rule reports it again. | 6, 10    | every recipe for the kinds in row 2                                                                                                                                                                                             |
| 4  | A modelled item the code lacks is a finding, never a pending Software Factory change. There is no baseline.                                                                                                                 | 7        | none: a behaviour of the check, not of recipes                                                                                                                                                                                  |
| 5  | Unnamed indexes are claims.                                                                                                                                                                                                 | 9        | `index.columns` and `index.unique` (4 files each)                                                                                                                                                                               |
| 6  | A claim kind with no rule gives one can't-verify error per claim, not one warning per kind.                                                                                                                                 | 8        | CrmBus Domain, whose `facts` is empty                                                                                                                                                                                           |
| 7  | Structure is compared on the located code, describe-only, for every element, not on what Guardrails contract templates generate.                                                                                            | Block 3  | none: structure had no recipes                                                                                                                                                                                                  |
| 8  | Unmodelled public members are reported only with the `unmodelled.member` opt-in rule on.                                                                                                                                    | 1        | none: the migration writes the row over `**/*.cs` where there is a Domain or Services recipe                                                                                                                                    |
| 9  | `own` conventions report only once their opt-in row is on, and a row is a glob. The migration writes the rows, but each reports every unmodelled type in its globs, not only those the entry's find matched: a helper class beside the DTOs is now reported. | 1, user 2026-11-16 | all 53 `own` entries                                                                                                                                                                                                            |
| 10 | A find ending at several places is `ambiguous` and names every candidate. The verdict, can't verify, is the same.                                                                                                           | R43      | none known                                                                                                                                                                                                                      |
| 11 | `column.required` is `member.nullable`: Is Nullable is the only requiredness the model states. A required column on a nullable property (`bool?` with `.IsRequired()`), which the model can't express, is no longer drift. | 16       | `column.required` (4 files)                                                                                                                                                                                                     |
| 12 | `relationship.multiplicity` is the end no navigation states, its value that end's multiplicity (`1`), not `<source>-><target>`: the navigable end is `member.type`'s and `member.nullable`'s. | 16       | `relationship.multiplicity` (2 files)                                                                                                                                                                                           |
| 13 | A fact is read by its kind's reader, not by its recipe. Where the stock reader reads more or less than the recipe did (following a raise into a collaborator, say), the verdict follows the reader. A fact of a kind no stock reader reads is `noRule` until the house's module reads it. | User, 2026-11-16 | every recipe                                                                                                                                                                                                                    |

## What the migration can't decide

The first Software Factory run after the migration shows these as findings, for the developer to resolve:

- **Recipes that re-derive a convention.** basislink5's `table.name` read the `DbSet` property's name when there is no `ToTable`. The stock reader reads only what the code states (decision 4), so a model that names the table where the code doesn't is contradicted, and the developer clears that model value instead.
- **Code v1 never read.** The `**` In row brings in everything under the output folder that isn't always out; the developer narrows it with Out rows, or models what it finds.
- **Globs the engine's matcher can't take.** Braces are expanded; anything else is written as it is, so the run reports it as `config/glob` rather than the migration changing its meaning.
- **Exceptions that hide claims.** None of the 25 do; Common.Drift never let them. A converted exception that matches no opt-in finding is a stale-exception warning on the first run.

## The migration's output

1. Rows on the first Codebase Structure package's Root Folder: a Drift Scope with the `**` In row and the Out rows, and a Drift Check with the opt-in rows and exceptions.
2. A line in the install's output for each row, each note (an `own` entry dropped for an exception naming its finding, a recipe file that won't parse), and each contract file it released.

The facts no stock reader reads aren't listed: they are `noRule` findings on the first run.

## The conversion test (phase 0)

The suite's conversion test ([acceptance-suite.md](acceptance-suite.md#what-runs-now)) reads the five codebases' recipe files from their local paths, skipping any that isn't present, and checks that every construct each uses appears in `Acceptance/conversion-map.json` with one of the dispositions above. A construct the map lacks fails the test, so a new recipe feature can't arrive without this document saying what it becomes. Common.Drift's `RecipeConversionTests` convert the same files and check the rows each gives. CrmBus's recipe files are read in place and never copied into the repository.
