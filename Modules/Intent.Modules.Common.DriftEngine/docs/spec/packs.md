# Packs

A pack bundles the data the engine needs for one technology, or for one codebase's house style: claim kinds, vocabulary, conventions and language profiles ([R51](requirements.md)). It never says how to read code: that is a claim reader's, in C#, in a module's extension ([rules.md](rules.md#plug-ins)). New languages, stacks and vocabulary arrive as packs and extensions, never as engine changes (principle 5).

- **Stack packs** ship in modules: one for EF Core, one for ASP.NET Core, one for Java, and so on.
- **A house pack** belongs to one codebase and holds what is particular to it: conventions and vocabulary for its own designers and stereotypes. What its own patterns need reading is its module's claim readers.

## Layout

A pack is a folder:

```text
<pack>/
  pack.json                       the manifest
  kinds.json                      claim kinds it defines
  vocabulary/<designer>.json      vocabulary entries and non-claims, per designer
  conventions/<designer>.json     where each element type's code is, and opt-in rule conventions
  profiles/<language>.json        language profiles
  tests/*.cases.md                its own acceptance cases
```

A small pack can be one file instead, `<id>.pack.json`, with each folder's content as a section of the same name. House packs usually are.

The schemas are [`schemas/pack.schema.json`](schemas/pack.schema.json) (the manifest and the single-file form), [`schemas/claim-kinds.schema.json`](schemas/claim-kinds.schema.json), [`schemas/vocabulary.schema.json`](schemas/vocabulary.schema.json), [`schemas/conventions.schema.json`](schemas/conventions.schema.json) and [`schemas/language-profile.schema.json`](schemas/language-profile.schema.json).

## The manifest

| Field       | Meaning                                                                                          |
| ----------- | ------------------------------------------------------------------------------------------------ |
| `id`        | Lower case, dotted: `intent.efcore`, `house.basislink`                                           |
| `version`   | Semantic version. Any change to what the pack checks changes it.                                 |
| `engine`    | The engine versions it works with, as a range                                                    |
| `requires`  | Other packs it builds on, with version ranges: the EF Core pack requires the Domain and C# packs |
| `languages` | The languages its conventions and profiles apply to                                              |
| `designers` | The designers it has vocabulary or conventions for                                               |
| `plugins`   | Each plug-in file, with its SHA-256                                                              |

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
| House | `drift/` in the folder that holds the application's `.application.config`, committed with the code (open question 3)      |

Module upgrades never change a house pack ([R53](requirements.md)); it's in the repository, not the module.

## Stock packs

| Pack                 | Provides                                                                                        | Phase |
| -------------------- | ----------------------------------------------------------------------------------------------- | ----- |
| `intent.core`        | Structure kinds, comparers, the C# reader, the C# language profile                              | 1     |
| `intent.domain`      | Domain designer vocabulary: structure, persistence, `intent.raises`                             | 1     |
| `intent.services`    | Services designer vocabulary: structure, HTTP, security, `intent.*`                             | 1     |
| `intent.ui`          | User Interface designer vocabulary, read by `intent.blazor`                                     | 6     |
| `intent.rdbms`       | Relational vocabulary, framework-neutral: tables, keys, columns, indexes, foreign keys, from the RDBMS stereotypes. Reads no code | 1     |
| `intent.http`        | HTTP vocabulary, framework-neutral: routes, verbs, parameter sources, security. Reads no code   | 1     |
| `intent.efcore`      | Reads the relational claims from EF Core's fluent API and attributes, and the migration snapshot | 4     |
| `intent.aspnetcore`  | Reads the HTTP claims from controllers and minimal APIs: routes, verbs, binding, security, versioning; and reports an Operation whose empty Route puts it where Web API 2 doesn't serve it ([rules.md](rules.md#routes-web-api-2-serves-conventionally)) | 4     |
| `intent.nservicebus` | Message handlers, sends and publishes                                                           | 4     |
| `intent.java`        | The Java reader and language profile                                                            | 4     |
| `intent.springmvc`   | Routes, verbs, binding, status and security over Spring MVC                                     | 4     |
| `intent.jpa`         | Persistence over JPA annotations                                                                | 4     |
| `intent.mediatr`     | Commands, queries, handlers and validators                                                      | later |
| `intent.blazor`      | Razor component rules for the User Interface kinds: routes, titles, parameters, injection, security | later |
| `intent.typescript`  | The TypeScript reader (with Vue single-file components) and the TypeScript language profile     | later |
| `intent.protobuf`    | The Protobuf reader (proto2, proto3 and editions) and the Protobuf language profile             | later |
| `intent.grpc`        | gRPC service conventions: a Proto Message DTO is the `.proto` message of its name. The Protobuf profile names fields as protoc does (`ProductId` is `product_id`), and the C# profile has `ServerCallContext` as an injected parameter | 1 |
| `intent.messaging`   | Domain events raised, integration events published and commands sent, whatever does it: the `intent.raises`, `intent.publishes` and `intent.sends` links, and every message type and raise the diagram could show and doesn't, on by default ([rules.md](rules.md#messages-the-diagram-could-show)) | 1 |

Phase 2's demonstrations (eShopCustom's Front End State Store and Architecture Layer) are house packs, which is the point: no stock pack and no engine change.

A `later` pack has acceptance cases but no phase gate yet. Its cases fix what it must do; its rules are written when it is built, and must pass them.

## Who ships what

Vocabulary belongs with the stereotype or element type it reads; claim readers belong with the technology they read.

- **A module's own stereotypes** (eShopCustom's On Delete, Backing Field and Enum Storage; the Java spike's Repository and Version) get their vocabulary from a pack the module ships, or, until it does, from the codebase's house pack. No stock pack knows them.
- **Readers are keyed by kind**, so a stack extension's reader reads a kind whichever pack's vocabulary stated the claim. The EF Core extension reads `relationship.onDelete` from `OnDelete(…)` for any vocabulary that states it.
- **A kind no stack extension reads** gets its reader from the module that ships its vocabulary.
- **Conventions** for an element type come from the pack that knows the element type, and a pack that requires it may override them ([rules.md](rules.md#which-conventions-apply)).

The suite's cases bring module vocabulary as inline packs, as a house pack would, and a house module's readers as the test extension `house.readers`.

## A house pack

eShopCustom's, in single-file form: the vocabulary for its own Backing Field stereotype. Its module's claim reader reads the field; the pack only says what the model states.

```json
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        {
          "kind": "member.backingField",
          "for": { "element": "Attribute", "where": "hasStereotype('Backing Field')" },
          "value": "stereotype('Backing Field').Field",
          "bindings": { "Entity": "parent.name", "BackingField": "stereotype('Backing Field').Field" },
          "dependsOn": "parent:type.exists",
          "source": "the Backing Field stereotype's Field"
        }
      ]
    }
  }
}
```

## A pack's own cases

A pack carries acceptance cases in the suite's case format ([R54](requirements.md), [acceptance-suite.md](acceptance-suite.md#case-files)). The suite runs every installed stock pack's cases, so a pack proves its vocabulary and rules on its own fixtures before any codebase uses it.
