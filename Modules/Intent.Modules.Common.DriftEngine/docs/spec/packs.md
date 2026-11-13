# Packs

A pack bundles what the engine needs for one technology, or for one codebase's house style: claim kinds, vocabulary, rules, conventions, language profiles and plug-ins ([R51](requirements.md)). New languages, stacks and vocabulary arrive as packs, never as engine changes (principle 5).

- **Stack packs** ship in modules: one for EF Core, one for ASP.NET Core, one for Java, and so on.
- **A house pack** belongs to one codebase. It lives with the code and holds what is particular to it: recipes for its own patterns, conventions, vocabulary for its own designers, and overrides of stack pack rules.

## Layout

A pack is a folder:

```text
<pack>/
  pack.json                       the manifest
  kinds.json                      claim kinds it defines
  vocabulary/<designer>.json      vocabulary entries and non-claims, per designer
  rules/<designer>.json           recipes and queries, per designer
  conventions/<designer>.json     where each element type's code is, and opt-in rule conventions
  profiles/<language>.json        language profiles
  plugins/*.dll                   plug-ins
  tests/*.cases.md                its own acceptance cases
```

A small pack can be one file instead, `<id>.pack.json`, with each folder's content as a section of the same name. House packs usually are.

The schemas are [`schemas/pack.schema.json`](schemas/pack.schema.json) (the manifest and the single-file form), [`schemas/claim-kinds.schema.json`](schemas/claim-kinds.schema.json), [`schemas/vocabulary.schema.json`](schemas/vocabulary.schema.json), [`schemas/rules.schema.json`](schemas/rules.schema.json), [`schemas/conventions.schema.json`](schemas/conventions.schema.json) and [`schemas/language-profile.schema.json`](schemas/language-profile.schema.json).

## The manifest

| Field       | Meaning                                                                                          |
| ----------- | ------------------------------------------------------------------------------------------------ |
| `id`        | Lower case, dotted: `intent.efcore`, `house.basislink`                                           |
| `version`   | Semantic version. Any change to what the pack checks changes it.                                 |
| `engine`    | The engine versions it works with, as a range                                                    |
| `requires`  | Other packs it builds on, with version ranges: the EF Core pack requires the Domain and C# packs |
| `languages` | The languages its readers and rules apply to                                                     |
| `designers` | The designers it has vocabulary or rules for                                                     |
| `plugins`   | Each plug-in file, with its SHA-256                                                              |
| `replaces`  | Rules of other packs it replaces, `<pack>/<kind>`, each with a reason                            |

## Choosing packs

The model lists the packs a codebase uses on the `Drift Check` stereotype's `Packs` ([scope.md](scope.md#the-drift-check-stereotype)), each with its version ([R52](requirements.md)). Only listed packs are used, plus the engine's own core pack.

- **A listed pack that isn't available** is a `config/packMissing` error. Nothing it would have provided runs, so its kinds' claims are `noRule` and its designers' features `noVocabulary`, all reported.
- **An available pack at a different version** is a `config/packVersion` error, and the pack doesn't run. Upgrading a module that ships a pack never silently changes what is checked: the team reviews the pack's changes and moves the pin, one row in the model.
- **A missing required pack** is a `config/packRequires` error.
- **A plug-in whose SHA-256 differs from the manifest's**, or, for a house pack, from the `Hash` pinned in the model's `Packs` row, isn't loaded (`config/pluginHash`). A house pack's plug-in is code from the repository run during the Software Factory; the pin makes changing it a reviewed model change.

## Where packs live

| Pack  | Where                                                                                                                     |
| ----- | ------------------------------------------------------------------------------------------------------------------------- |
| Core  | Inside Intent.Common.DriftEngine: the C# and text readers, the comparers, the structure kinds and the C# language profile |
| Stack | Inside the module that ships it, discovered from the application's installed modules, never copied into the application   |
| House | `drift/` in the folder that holds the application's `.application.config`, committed with the code (open question 4)      |

Module upgrades never change a house pack ([R53](requirements.md)); it's in the repository, not the module.

## Stock packs

| Pack                 | Provides                                                                                        | Phase |
| -------------------- | ----------------------------------------------------------------------------------------------- | ----- |
| `intent.core`        | Structure kinds, comparers, the C# and text readers, the C# language profile                    | 1     |
| `intent.domain`      | Domain designer vocabulary: structure, persistence, `intent.raises`                             | 1     |
| `intent.services`    | Services designer vocabulary: structure, HTTP, security, `intent.*`                             | 1     |
| `intent.ui`          | User Interface designer vocabulary, for converted recipes                                       | 6     |
| `intent.efcore`      | Persistence recipes over EF Core's fluent API and attributes, and the migration snapshot reader | 4     |
| `intent.aspnetcore`  | Routes, verbs, binding, security and versioning over controllers and minimal APIs               | 4     |
| `intent.nservicebus` | Message handlers, sends and publishes                                                           | 4     |
| `intent.java`        | The Java reader and language profile                                                            | 4     |
| `intent.springmvc`   | Routes, verbs, binding, status and security over Spring MVC                                     | 4     |
| `intent.jpa`         | Persistence over JPA annotations                                                                | 4     |
| `intent.mediatr`     | Commands, queries, handlers and validators                                                      | later |

Phase 2's demonstrations (eShopCustom's Front End State Store and Architecture Layer) are house packs, which is the point: no stock pack and no engine change.

## A house pack

basislink5's, in single-file form, as phase 6's converter would start it from its Common.Drift `domain.drift.json` (one recipe shown):

```json
{
  "id": "house.basislink",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }, { "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain", "Services"],
  "rules": {
    "Domain": {
      "files": "Basislink.Data/Model/**/*.cs",
      "facts": {
        "table.name": {
          "find": "statement:Entity<{Entity}>",
          "read": [
            { "match": "To(?:Table|View)\\(\"([^\"]+)\"" },
            { "match": "DbSet<{Entity}>(\\w+)\\{", "note": "No ToTable: EF names the table after the DbSet." }
          ],
          "inherit": "property with DbSet<{Entity}>"
        }
      }
    }
  }
}
```

Its second read case takes the table name from the `DbSet` property when there is no `ToTable`, which re-derives EF Core's convention. Decision 4 makes that unnecessary, but the converter can't tell a convention from any other read case, so it keeps it; [conversion.md](conversion.md#what-the-converter-cant-decide) lists it for the adoption review.

## A pack's own cases

A pack carries acceptance cases in the suite's case format ([R54](requirements.md), [acceptance-suite.md](acceptance-suite.md#case-files)). The suite runs every installed stock pack's cases, so a pack proves its vocabulary and rules on its own fixtures before any codebase uses it.
