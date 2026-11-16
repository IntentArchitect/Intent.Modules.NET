# Conversion from Common.Drift

Phase 6 maps every Common.Drift recipe file onto the engine ([R55](requirements.md)), and runs both engines side by side ([R57](requirements.md)). The engine has no recipes (user decision, 2026-11-16: a JSON way of reading code beside C# claim readers was a second system to learn and choose between). So a recipe isn't copied: the fact it reads is the stock reader's, where one reads its kind, and otherwise a claim reader in the house's module. This document fixes what each construct becomes, and which verdicts change and why. The acceptance suite's conversion test keeps it complete: it reads the real recipe files and fails on any construct this map doesn't cover.

## What converts into what

| Common.Drift                                      | DriftEngine                                                                                                  |
| ------------------------------------------------- | ------------------------------------------------------------------------------------------------------------ |
| `guardrails/<designer>.drift.json`                | Nothing of its own: its facts, `own` entries and exceptions each go as below                                 |
| Top-level `files`                                 | A proposed scope `In` row per pattern                                                                        |
| `facts.<kind>`                                    | The kind's stock reader, where one reads it; otherwise a claim reader the house's module provides, listed   |
| `own` entries                                     | Conventions for the `unmodelled.type` opt-in rule, and a proposed `Opt-in Rules` row turning it on           |
| A file-pattern exception                          | A proposed scope `Out` row, with the exception's reason                                                      |
| A type, `Type.Member` or ownership-name exception | A proposed `Exceptions` row for the matching opt-in rule, with its reason                                    |
| `note`, `//` comments                             | Kept, in the proposal                                                                                        |

The converter writes nothing. Scope rows, opt-in rows and exceptions are model changes, so it proposes them, and the adoption flow applies them with the developer ([R56](requirements.md)). Conventions go in the house pack, and the facts no stock reader reads are listed for the house's module.

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

Every construct the five codebases use, and what it becomes. A recipe's find and read constructs are dropped: the kind's reader reads the fact. The same find steps, placeholder filters and regular expressions in an `own` entry carry over into the opt-in conventions' find paths, which read the same canonical text ([code-reader.md](code-reader.md#canonical-text)).

| Construct                                                                           | Uses            | Converts to                                                                                                                                          |
| ----------------------------------------------------------------------------------- | --------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| `find`, as one path or a list                                                       | 27 / 99         | Dropped in a recipe; in an `own` entry, the convention's find path                                                                                   |
| `class:Name` (an alias of `type:`), `method:Name`, `property:Name`, `parameter:Name` | 139 / 123 / 36 / 3 | Dropped in a recipe; in an `own` entry, the same find step                                                                                        |
| `statement:text`, `file:pattern`                                                    | 60 / 9          | Dropped                                                                                                                                              |
| `class with`, `method with`, `statement with`, `chain with`                         | 339             | Dropped in a recipe; in an `own` entry, the same find step                                                                                           |
| `read` cases, with `value`, `part`, `in`, `case` and `unreadable`                   | 493             | Dropped: the kind's reader reads the value, and says `unreadable` itself                                                                             |
| `default`, `absent`, `unstated`                                                     | 116 / 38 / 121  | Dropped: the kind's reader reads silence; a `whenSet` kind never takes a value from it ([below](#semantic-changes))                                  |
| `inherit`, as one value or a list                                                   | 1 / 63          | Dropped                                                                                                                                              |
| `replace`, 1 to 3 rows                                                              | 27              | Dropped                                                                                                                                              |
| Recipe `files`, one pattern / a list                                                | 3 / 23          | Dropped: scope decides what is read                                                                                                                  |
| Top-level `files`, one pattern / a list                                             | 2 / 8           | Proposed scope `In` rows                                                                                                                             |
| Placeholder filters `camel`, `trimEnd`, `trimStart`, `replace`, chained 2 to 4 deep | 51 / 8          | Kept in conventions' find paths and names                                                                                                            |
| An empty placeholder as a switch; brace text that isn't a placeholder               |                 | Kept in conventions: an empty binding gives an empty placeholder, and `{version}` is kept as written                                                 |
| .NET regular expressions: lookbehind, back-references, `(?i)`, scoped `(?i:…)`      | 14 / 4 / 2 / 26 | Kept in conventions' find paths: the engine runs .NET's regular expressions for every language                                                      |
| `own` with `class with`, `method with`, `statement with`, `chain with`, `file with` | 53 entries      | `unmodelled.type` conventions (`file with` and member-level finds as `unmodelled.member` or `unmodelled.interaction` conventions, by what they find) |
| `own` `name` with `$2` or higher                                                    | 2               | Same naming in the opt-in rule's findings                                                                                                            |
| `own` `files`                                                                       | 35              | The convention's `files`                                                                                                                             |
| `note` (recipe, case, `own`, file)                                                  | 87              | Kept                                                                                                                                                 |

Constructs the format has but none of the five uses, which the converter still handles: the `method:Name(types)` overload form, `chain:text`, a bare `<kind>` step, `in: property`, `in: parameter`, `in: file`, `case: lower`, and the `pascal`, `kebab`, `lower` and `upper` filters.

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
| 8  | Unmodelled public members are reported only with the `unmodelled.member` opt-in rule on.                                                                                                                                    | 1        | none: proposed as an opt-in row where the codebase relied on it                                                                                                                                                                 |
| 9  | `own` conventions report only once their opt-in row is on. The converter proposes the row, so accepting it keeps the verdicts.                                                                                              | 1        | all 53 `own` entries                                                                                                                                                                                                            |
| 10 | A find ending at several places is `ambiguous` and names every candidate. The verdict, can't verify, is the same.                                                                                                           | R43      | none known                                                                                                                                                                                                                      |
| 11 | `column.required` is `member.nullable`: Is Nullable is the only requiredness the model states. A required column on a nullable property (`bool?` with `.IsRequired()`), which the model can't express, is no longer drift. | 16       | `column.required` (4 files)                                                                                                                                                                                                     |
| 12 | `relationship.multiplicity` is the end no navigation states, its value that end's multiplicity (`1`), not `<source>-><target>`: the navigable end is `member.type`'s and `member.nullable`'s. | 16       | `relationship.multiplicity` (2 files)                                                                                                                                                                                           |
| 13 | A fact is read by its kind's reader, not by its recipe. Where the stock reader reads more or less than the recipe did (following a raise into a collaborator, say), the verdict follows the reader. A fact of a kind no stock reader reads is `noRule` until the house's module reads it. | User, 2026-11-16 | every recipe                                                                                                                                                                                                                    |

## What the converter can't decide

These are listed in the converter's report for the adoption review:

- **Recipes that re-derive a convention.** basislink5's `table.name` read the `DbSet` property's name when there is no `ToTable`. The stock reader reads only what the code states (decision 4), so a model that names the table where the code doesn't is contradicted, and the adopter clears that model value instead. The report lists every recipe for a `whenSet` kind with more than one read case.
- **`files` that include junk.** A top-level `files` of `**/*.cs` proposes everything as in scope; the adopter narrows it.
- **Exceptions that hide claims.** None of the 25 do; Common.Drift never let them. A converted exception that matches no opt-in finding becomes an `optin/staleException` warning on the first run.

## The converter's output

1. The house pack's conventions, from the `own` entries.
2. A proposal: scope rows, opt-in rows and exceptions, each with its source line in the recipe file.
3. A list of the facts no stock reader reads, each with its kind and recipe, for the house's module to read.
4. A report: per recipe, which semantic changes apply, so the side-by-side run's differences can be checked against it.

## The conversion test (phase 0)

The suite's conversion test ([acceptance-suite.md](acceptance-suite.md#what-runs-now)) reads the five codebases' recipe files from their local paths, skipping any that isn't present, and checks that every construct each uses appears in `Acceptance/conversion-map.json` with one of the dispositions above. A construct the map lacks fails the test, so a new recipe feature can't arrive without this document saying what it becomes. CrmBus's recipe files are read in place and never copied into the repository.
