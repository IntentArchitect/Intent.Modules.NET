# Acceptance suite

The suite every later phase must pass. It is the test project `Modules/Intent.Modules.Common.DriftEngine.Tests`, built from JSON Schemas and data fixtures (decision 5). Its integrity tests run now. Its engine tests are written now and skipped until phase 1 gives them an engine.

Phases are verified with these unit tests, fixtures and builds only. The existing evals (Tests.Brownfield and every agent-driven or smoke suite) are not part of the plan: the suite never runs them and no phase gate uses them.

## Layout

```text
Modules/Intent.Modules.Common.DriftEngine.Tests/
  Acceptance/
    checks.json                 every check's dispositions (claim-catalog.md)
    catalog.json                every claim kind, scope case, engine case and opt-in rule
    conversion-map.json         every recipe construct and what it becomes (conversion.md)
    codebases.json              where the real codebases are on this machine
    cases/<area>/<name>.cases.md
  Harness/                      case parser, schema validation, the engine adapter
  Integrity/                    tests that run now
  Engine/                       tests that run cases through the engine (skipped until phase 1)
  Real/                         tests over the real codebases (skipped until phase 1, and where absent)
```

The schemas are not copied: the project reads them from `docs/spec/schemas`, so the spec and the suite can't drift apart. The data files have their own: [`checks.schema.json`](schemas/checks.schema.json), [`catalog.schema.json`](schemas/catalog.schema.json) and [`conversion-map.schema.json`](schemas/conversion-map.schema.json). `codebases.json` is machine-local and has none.

## Case files

A case file is Markdown, readable on its own. Each `## <case-id>` heading starts a case: prose saying what it shows and why, then fenced blocks. A block's info string is `<language> <role> [<argument>]`; the language is only for highlighting.

| Role     | Holds                                                                 | Schema                                                           |
| -------- | --------------------------------------------------------------------- | ---------------------------------------------------------------- |
| `case`   | The case's metadata                                                   | [`case.schema.json`](schemas/case.schema.json)                   |
| `model`  | The model, as data                                                    | [`fixture-model.schema.json`](schemas/fixture-model.schema.json) |
| `file`   | One code file; the argument is its path relative to the output folder | none: any text                                                   |
| `pack`   | A single-file pack the case uses, such as a house pack                | [`pack.schema.json`](schemas/pack.schema.json)                   |
| `expect` | What the run must produce                                             | [`expect.schema.json`](schemas/expect.schema.json)               |

Every case has one `case`, one `model` and one `expect` block, and any number of `file` and `pack` blocks. Case ids are kebab case and unique within their file; a case's full id is `<area>/<name>#<case-id>`.

A case may reuse another's model and files with `"base": "<case-id>"` in its `case` block; its own blocks then replace the base's blocks of the same role and path, and `"without": ["<path>"]` drops a base's file. Most cases are a small change to a base, as eShopCustom's tests were a golden fixture plus one edit each.

### Example

`cases/persistence/table.name.cases.md`:

````markdown
## contradicted-not-stated

The model names the table `Orders`, and the code has no `ToTable`. EF Core's convention may well give `Orders`, but the model states the name only where the code does (decision 4), so the claim is contradicted. Traces ESC-DOM-52.

```json case
{ "phase": 4, "kinds": ["table.name"], "traces": ["ESC-DOM-52"], "requirements": ["R4"], "packs": ["intent.domain", "intent.efcore"] }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": { "Name": "Orders" } },
            "children": [ { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" },
                            "stereotypes": { "Primary Key": {} } } ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Infrastructure/OrderConfiguration.cs
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "outcome": "contradicted",
      "expected": "Orders", "actual": null, "at": "src/Infrastructure/OrderConfiguration.cs:1" }
  ],
  "others": "ignore"
}
```
````

### The model block

The model is plain data that the harness turns into Intent's model objects (decision 2). Each designer has packages; a package has elements, nested through `children`; associations sit beside the packages.

| Field                | Meaning                                                                                                |
| -------------------- | ------------------------------------------------------------------------------------------------------ |
| `id`                 | Case-local; `expect` refers to elements by it                                                          |
| `type`               | The element's specialization: `Class`, `Attribute`, `Operation`, `Enum`, `Service`, `Folder`, `Layer`… |
| `name`               | Its name                                                                                               |
| `stereotypes`        | `{ "<name>": { "<property>": <value> } }`; an item list is an array of objects; a property holding an element is `{ "ref": "<id>" }`, the element in any designer |
| `typeRef`            | `{ "type": "<primitive>" }` or `{ "element": "<id>" }`, with `nullable` and `collection`               |
| `abstract`, `static` | Booleans                                                                                               |
| `generalization`     | The id of the element it generalizes                                                                   |
| `genericTypes`       | Its generic type parameters' names, in order                                                           |
| `mappedTo`           | The id of the element it is mapped to, in any designer                                                 |
| `value`              | An enum literal's value, or a parameter's default                                                      |
| `children`           | Nested elements                                                                                        |
| associations         | `{ "id", "type", "source": { "element", "name", "multiplicity", "navigable", "stereotypes" }, "target": {…} }`. An end's `element` may be another association's id: that association's target end |
| package `application`, `referenced` | The application a package belongs to, and `true` for a package referenced from another application |

Primitive type names are Intent's: `string`, `int`, `long`, `decimal`, `bool`, `datetime`, `date`, `guid`, `binary`, and so on.

### The expect block

| Field             | Meaning                                                                                                                                                                                                                                                                |
| ----------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `claims`          | Claims the run must produce, each by `kind`, `element` and optional `key` (its discriminator), with `outcome`, and optionally `reason`, `expected`, `actual` (canonical text, `null` for no value), `at` (`path:line`), `fromSilence`, `scopeRow`, `blockedBy`. Or `"absent": true`: no such claim may exist |
| `others`          | What every claim not listed must be: `ignore` (the default), `verified`, or `absent` (no other claims)                                                                                                                                                                 |
| `findings`        | Findings the run must report, each by `rule`, with optional `severity`, `subject` (an element id or `path:line`), `messageContains`                                                                                                                                    |
| `noOtherFindings` | `true`: no finding beyond those listed                                                                                                                                                                                                                                 |
| `filesRead`       | `{ "include": [...], "exclude": [...] }`: paths that must, and must not, be read                                                                                                                                                                                       |
| `nodes`           | For reader cases: graph nodes by `kind` and `name`, with `at`, `text`, `declaration` or other fields                                                                                                                                                                   |
| `config`          | For configuration cases: `config/*` findings expected instead of a run                                                                                                                                                                                                 |

## The harness

- **Case parser.** Reads case files, applies `base`, and validates each block against its schema.
- **Model builder.** Turns a `model` block into Intent's model objects (`IPackage`, `IElement`, `IAssociation`, `IStereotype`, `ITypeReference`), the same interfaces the engine reads in the Software Factory. Phase 1 builds it, beside the engine.
- **Code files.** A case's `file` blocks are written to a temporary folder that stands for the output folder, so the engine reads real files.
- **Engine adapter.** One interface the phase plugs its engine into: given the model, the output folder and the packs, return the [run result](claims-and-findings.md#the-run-result) as data. Until phase 1 sets it, every engine test calls `Assert.Skip` with "The engine arrives in phase 1".
- **Packs.** The packs a case's `case` block names become its Root Folder's `Drift Check` `Packs` rows, as a real model lists them, unless the model's `Drift Check` lists `Packs` itself; configuration cases do, to show a missing or unused pack. A case that only turns on opt-in rules or adds exceptions sets those lists and leaves `Packs` to the harness.
- **Comparison.** Checks a run result against `expect`, reporting every difference, not only the first.

The engine adapter is the only thing the suite adds around the engine: the engine itself reads the model and the files exactly as it does in a Software Factory run.

## What runs now

The integrity tests run in phase 0 and must pass before sign-off:

1. Every schema is a valid JSON Schema (draft 2020-12).
2. `checks.json` has exactly the 321 ids in [check-classification.md](../check-classification.md), parsed from its tables, with the per-source counts in its totals table.
3. Every disposition names a kind, scope case, engine case or opt-in rule in `catalog.json`.
4. Every case file parses, every block validates, every `element` and association in `expect` exists in the case's model, every `at` names a case file and a line within it, every pack is inline or a stock pack the catalog plans, and every `traces` id is in `checks.json`.
5. Every check whose disposition is a claim, scope, engine or opt-in case is traced by at least one case, and every scope case, engine case and opt-in rule in `catalog.json` is shown by one.
6. Every claim kind with phase 1 to 4 has at least one case with each outcome: verified, contradicted and can't verify.
7. Every requirement a case names exists in [requirements.md](requirements.md), and every case a requirement names exists: a case file, an area with at least one file (`readers/*`), or a scope, engine or opt-in case some case shows.
8. Every construct the local Common.Drift recipe files use is in `conversion-map.json` ([conversion.md](conversion.md#the-conversion-test-phase-0)). Each codebase whose files aren't on this machine is skipped.

The harness has its own unit tests: the case parser, the case validator against deliberately broken cases, the comparison against synthetic run results, and the construct keys. A case is a phase gate only once the harness is trusted to read it.

While cases are being written, `DRIFTENGINE_ACCEPTANCE` points the suite at the project's own `Acceptance` folder, so the built test executable validates them without a rebuild.

## Real codebases

`Acceptance/codebases.json` says where each real codebase is on this machine, and an environment variable `DRIFTENGINE_CODEBASE_<NAME>` overrides it. A test whose codebase isn't present is skipped.

| Name                      | Used for                                                                                 |
| ------------------------- | ---------------------------------------------------------------------------------------- |
| basislink5                | Phase 1's gate; the conversion test                                                      |
| CrmBus                    | Phase 1's gate; the conversion test. Client code: read in place, never copied or quoted. |
| eShop10, eShop13, eShop14 | The conversion test; phase 6                                                             |
| eShopCustom               | Phases 2 and 3 (its Front End, Architecture and planted import break)                    |
| Java spike                | Phase 4                                                                                  |

The real-codebase tests run the engine on each application's own model and code. They assert the phase's gate and that no file in the codebase changed: the test records `git status --porcelain` before and after, and fails on any difference ([R17](requirements.md)).

## What each phase must pass

| Phase                 | Must pass                                                                                                                                      |
| --------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| 1. Foundations        | Every case with `phase` 1. basislink5's and CrmBus's structure claims verified, no file changed, and their excluded files not read and listed. |
| 2. Open vocabulary    | Every case with `phase` 2 or lower, including the Front End State Store and Architecture Layer cases, with no change to the engine's own code  |
| 3. Rules beyond regex | Every case with `phase` 3 or lower, including eShopCustom's planted import break and the Java spike's                                          |
| 4. Java and packs     | Every case with `phase` 4 or lower, and each stock pack's own cases                                                                            |
| 5. Adoption           | Not in the suite: the plan's two-hour adoption, with no code changes                                                                           |
| 6. Migration          | Every case. Both engines on basislink5 and CrmBus, with every difference explained by [conversion.md](conversion.md#semantic-changes).         |

## Writing cases

- One behaviour per case, with the smallest model and code that shows it, and prose saying what it shows and which check it traces.
- Line numbers in `at` are exact; the integrity tests check them.
- Prefer a `base` and one change over a new fixture.
- Never copy CrmBus code into a case. Small adaptations of basislink5, eShop and Java spike code are fine.
