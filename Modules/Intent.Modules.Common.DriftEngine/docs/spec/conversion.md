# Conversion from Common.Drift

Phase 6 converts every Common.Drift recipe file into a house pack with no hand edits ([R55](requirements.md)), and runs both engines side by side ([R57](requirements.md)). This document fixes what each construct becomes, and which verdicts change and why. The acceptance suite's conversion test keeps it complete: it reads the real recipe files and fails on any construct this map doesn't cover.

## What converts into what

| Common.Drift                                      | DriftEngine                                                                                         |
| ------------------------------------------------- | --------------------------------------------------------------------------------------------------- |
| `guardrails/<designer>.drift.json`                | The `rules` section for that designer in the house pack `drift/house.<application>.pack.json`       |
| Top-level `files`                                 | The rules section's `files`, which narrows within scope, plus a proposed scope `In` row per pattern |
| `facts.<kind>`                                    | `facts.<kind>`, as written, except silence on `whenSet` kinds ([below](#semantic-changes))          |
| `own` entries                                     | Conventions for the `unmodelled.type` opt-in rule, and a proposed `Opt-in Rules` row turning it on  |
| A file-pattern exception                          | A proposed scope `Out` row, with the exception's reason                                             |
| A type, `Type.Member` or ownership-name exception | A proposed `Exceptions` row for the matching opt-in rule, with its reason                           |
| `note`, `//` comments                             | Kept                                                                                                |

The converter writes only the house pack. Scope rows, opt-in rows and exceptions are model changes, so it proposes them, and the adoption flow applies them with the developer ([R56](requirements.md)).

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

Every construct the five codebases use, and what it becomes. "Same" means the recipe is copied and reads the same canonical text ([code-reader.md](code-reader.md#canonical-text)).

| Construct                                                                           | Uses            | Converts to                                                                                                                                          |
| ----------------------------------------------------------------------------------- | --------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| `find` as one path                                                                  | 27              | Same                                                                                                                                                 |
| `find` as a list, tried in order                                                    | 99              | Same                                                                                                                                                 |
| `class:Name`                                                                        | 139             | Same (an alias of `type:`)                                                                                                                           |
| `method:Name`                                                                       | 123             | Same, overloads chosen by parameter names                                                                                                            |
| `property:Name`                                                                     | 36              | Same                                                                                                                                                 |
| `parameter:Name`                                                                    | 3               | Same                                                                                                                                                 |
| `statement:text`                                                                    | 60              | Same                                                                                                                                                 |
| `file:pattern`                                                                      | 9               | Same; component files read as one unit                                                                                                               |
| `class with`, `method with`, `statement with`, `chain with`                         | 339             | Same                                                                                                                                                 |
| `read` cases                                                                        | 493             | Same                                                                                                                                                 |
| `value` with `$1`, `${name}` and placeholders                                       | 228             | Same                                                                                                                                                 |
| `part: declaration` / `part: body`                                                  | 119 / 82        | Same                                                                                                                                                 |
| `in: class` / `method` / `statement`                                                | 5 / 3 / 8       | Same                                                                                                                                                 |
| `case: upper` on a read case                                                        | 12              | Same                                                                                                                                                 |
| `unreadable: true`                                                                  | 17              | Same: can't verify, `unreadable`                                                                                                                     |
| `default`                                                                           | 116             | Same for `always` kinds; dropped or flagged for `whenSet` kinds ([below](#semantic-changes))                                                         |
| `absent`                                                                            | 38              | As `default`                                                                                                                                         |
| `unstated`                                                                          | 121             | Same: quoted when a reading is silence                                                                                                               |
| `inherit` as one value / as a list                                                  | 1 / 63          | Same. The list form is undocumented in Common.Drift; the engine documents it.                                                                        |
| `replace`, 1 to 3 rows                                                              | 27              | Same. `default` isn't passed through `replace`, as in Common.Drift.                                                                                  |
| Recipe `files`, one pattern / a list                                                | 3 / 23          | Same, narrowing within scope                                                                                                                         |
| Top-level `files`, one pattern / a list                                             | 2 / 8           | The rules section's `files`, and proposed scope `In` rows                                                                                            |
| Placeholder filters `camel`, `trimEnd`, `trimStart`, `replace`                      | 51              | Same                                                                                                                                                 |
| Filters chained 2, 3 and 4 deep                                                     | 8               | Same                                                                                                                                                 |
| An empty placeholder as a switch                                                    |                 | Same: a binding that evaluates to nothing gives an empty placeholder                                                                                 |
| Brace text that isn't a placeholder (`{version}`, `{controller=Home}`)              |                 | Same: kept as written, never an error                                                                                                                |
| .NET regular expressions: lookbehind, back-references, `(?i)`, scoped `(?i:…)`      | 14 / 4 / 2 / 26 | Same: the engine runs .NET's regular expressions for every language                                                                                  |
| `own` with `class with`, `method with`, `statement with`, `chain with`, `file with` | 53 entries      | `unmodelled.type` conventions (`file with` and member-level finds as `unmodelled.member` or `unmodelled.interaction` conventions, by what they find) |
| `own` `name` with `$2` or higher                                                    | 2               | Same naming in the opt-in rule's findings                                                                                                            |
| `own` `files`                                                                       | 35              | The convention's `files`                                                                                                                             |
| `note` (recipe, case, `own`, file)                                                  | 87              | Kept                                                                                                                                                 |

Constructs the format has but none of the five uses, which the converter still handles: the `method:Name(types)` overload form, `chain:text`, a bare `<kind>` step, `in: property`, `in: parameter`, `in: file`, `case: lower`, and the `pascal`, `kebab`, `lower` and `upper` filters.

### Kinds

All Common.Drift fact kinds keep their names and placeholders, so recipes need no renaming, except `column.required`, which becomes `member.nullable` (change 11):

- **Domain**: `table.name`, `table.schema`, `table.kind`, `key`, `column.name`, `column.type`, `column.textType`, `column.maxLength`, `column.precision`, `column.scale`, `column.default`, `relationship.multiplicity`, `relationship.foreignKey`, `index.columns`, `index.unique`, `intent.raises`.
- **Services**: `service.route`, `operation.verb`, `operation.route`, `security`, `security.roles`, `security.policy`, `parameter.source`, `intent.creates`, `intent.updates`, `intent.deletes`, `intent.queries`, `intent.invokes`, `intent.publishes`, `intent.sends`.
- **User Interface** (Intent.UI.Guardrails.Contracts, not in the configure-drift-check skill): `component.kind`, `page.route`, `intent.navigates`, `intent.opens`, `intent.uses`, and `intent.invokes`, `security`, `security.roles`, `security.policy`, with placeholders `{Folder}`, `{Component}`, `{Invoked}`, `{Route}`, `{RouteStart}`, `{Page}`, `{Dialog}` and `{Child}`. The `intent.ui` pack (phase 6) gives them vocabulary.

## Semantic changes

The engine's decisions change some verdicts on the same model and code. Phase 6's side-by-side run must explain every difference by one of these (open question 1).

| #  | Change                                                                                                                                                                                                                      | Decision | Affected recipes in the five codebases                                                                                                                                                                                          |
| -- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1  | A `whenSet` kind takes no value from silence. A modelled value the code doesn't state is contradicted.                                                                                                                      | 4        | `table.name` with `default: "{Entity}"` (4 files); one `operation.route` and one `service.route` recipe with a literal `default`. The converter drops these and lists them.                                                     |
| 2  | An empty `default` or `absent` on a `whenSet` kind is dropped, with no verdict change: the model's empty value is no longer a claim, and a set value the code doesn't state was contradicted before too.                    | 4, 10    | 69 uses across `column.name`, `column.type`, `column.maxLength`, `column.precision`, `column.scale`, `column.default`, `table.schema`, `service.route`, `operation.route`, `page.route`, `security.roles` and `security.policy` |
| 3  | An empty model value is no longer a claim, so code stating a value the model leaves empty (a `HasMaxLength(50)` with no modelled MaxLength) is no longer contradicted. The opt-in `unmodelled.value` rule reports it again. | 6, 10    | every recipe for the kinds in row 2                                                                                                                                                                                             |
| 4  | A modelled item the code lacks is a finding, never a pending Software Factory change. There is no baseline.                                                                                                                 | 7        | none: a behaviour of the check, not of recipes                                                                                                                                                                                  |
| 5  | Unnamed indexes are claims.                                                                                                                                                                                                 | 9        | `index.columns` and `index.unique` (4 files each)                                                                                                                                                                               |
| 6  | A claim kind with no rule gives one can't-verify error per claim, not one warning per kind.                                                                                                                                 | 8        | CrmBus Domain, whose `facts` is empty                                                                                                                                                                                           |
| 7  | Structure is compared on the located code, describe-only, for every element, not on what Guardrails contract templates generate.                                                                                            | Block 3  | none: structure had no recipes                                                                                                                                                                                                  |
| 8  | Unmodelled public members are reported only with the `unmodelled.member` opt-in rule on.                                                                                                                                    | 1        | none: proposed as an opt-in row where the codebase relied on it                                                                                                                                                                 |
| 9  | `own` conventions report only once their opt-in row is on. The converter proposes the row, so accepting it keeps the verdicts.                                                                                              | 1        | all 53 `own` entries                                                                                                                                                                                                            |
| 10 | A find ending at several places is `ambiguous` and names every candidate. The verdict, can't verify, is the same.                                                                                                           | R43      | none known                                                                                                                                                                                                                      |
| 11 | `column.required` is `member.nullable`: Is Nullable is the only requiredness the model states. A required column on a nullable property (`bool?` with `.IsRequired()`), which the model can't express, is no longer drift. | 16       | `column.required` (4 files)                                                                                                                                                                                                     |
| 12 | `relationship.multiplicity` is the end no navigation states, its value that end's multiplicity (`1`), not `<source>-><target>`: the navigable end is `member.type`'s and `member.nullable`'s. A recipe's read cases that give both ends convert to the unnavigated end's half. | 16       | `relationship.multiplicity` (2 files)                                                                                                                                                                                           |

## What the converter can't decide

These convert as written and are listed in the converter's report for the adoption review:

- **Read cases that re-derive a convention.** basislink5's `table.name` reads the `DbSet` property's name when there is no `ToTable`. It reads code, not silence, so rule 1 doesn't catch it. Decision 4 makes it unnecessary: a model that names the table where the code doesn't would now be contradicted, and the adopter should clear that model value instead. The report lists every recipe for a `whenSet` kind with more than one read case.
- **`files` that include junk.** A top-level `files` of `**/*.cs` proposes everything as in scope; the adopter narrows it.
- **Exceptions that hide claims.** None of the 25 do; Common.Drift never let them. A converted exception that matches no opt-in finding becomes an `optin/staleException` warning on the first run.

## The converter's output

1. The house pack, `drift/house.<application>.pack.json`.
2. A proposal: scope rows, opt-in rows and exceptions, each with its source line in the recipe file.
3. A report: per recipe, which semantic changes apply, so the side-by-side run's differences can be checked against it.

## The conversion test (phase 0)

The suite's conversion test ([acceptance-suite.md](acceptance-suite.md#what-runs-now)) reads the five codebases' recipe files from their local paths, skipping any that isn't present, and checks that every construct each uses appears in `Acceptance/conversion-map.json` with one of the dispositions above. A construct the map lacks fails the test, so a new recipe feature can't arrive without this document saying what it becomes. CrmBus's recipe files are read in place and never copied into the repository.
