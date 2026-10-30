---
contentHash: E9D5F0115A994D1D0199E03E5A03462632849EF073A5A7E4EAA86BDBBFA14503
---
# Azure Cosmos DB: the Domain recipe file

Start `guardrails/domain.drift.json` from this guide when the code stores its documents in Azure Cosmos DB. The recipe file below is tested against code in each style it covers. Copy it, make the decisions below, set `files` to the projects that hold the document classes, then adapt it to this codebase's own conventions. Don't write the recipes again from scratch.

## Recognise it

Two kinds of code reach Cosmos DB:

- **The Cosmos DB SDK:** a `Microsoft.Azure.Cosmos` package, with `CosmosClient` and `Container`. Document classes are plain classes serialized to JSON: `[JsonProperty("id")]` (Newtonsoft.Json, the SDK's default) or `[JsonPropertyName("id")]` (System.Text.Json, with a custom serializer). The recipe file below is for this.
- **EF Core's Cosmos provider:** `UseCosmos`, with `ToContainer("Orders")` and `HasPartitionKey(…)` in the model configuration. Start from the EF Core guide (`ef-core.md`) instead, with the changes under "EF Core's Cosmos provider" below.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| Are nullable reference types on for the document classes? | `<Nullable>enable</Nullable>` in their csproj, or `#nullable enable` in their files | On: nothing. Off: set `column.required`'s `default` to `false`, and its `unstated` to say a string is nullable |
| A serializer that names properties (`CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase }`, or `JsonNamingPolicy.CamelCase`) | Where the `CosmosClient` is built | Nothing: the names the code states are read, and the rest stay empty in the model |

## The recipe file

A document database has no SQL types, lengths or defaults of its own, so the recipes for those facts read only what the code states, and otherwise the value the model gives a document's attribute (`DEFAULT` or empty).

```json
{
  "files": "src/**/*.cs",
  "facts": {
    "key": {
      "find": "class:{Entity}",
      "read": [
        { "match": "\\[Json(?:Property|PropertyName)\\((?:PropertyName=)?\"id\"[^\\]]*\\](?:\\[[^\\]]*\\])*[^{;\\[]*?(\\w+)(?:\\{|=>)" },
        { "match": "\\b((?i:id))(?:\\{|=>)" }
      ],
      "default": "",
      "unstated": "no property of `{Entity}` itself is serialized as `id`"
    },
    "column.name": {
      "find": "class:{Entity} > property:{Attribute}",
      "read": [
        { "part": "declaration", "match": "\\[JsonPropertyName\\(\"([^\"]+)\"\\)" },
        { "part": "declaration", "match": "\\[JsonProperty\\((?:PropertyName=)?\"([^\"]+)\"" }
      ],
      "default": "",
      "absent": "",
      "unstated": "no `[JsonProperty]` or `[JsonPropertyName]` name for `{Attribute}`, so it is named after the property, or by the serializer's naming policy"
    },
    "column.type": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "Cosmos DB has no column types"
    },
    "column.textType": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "DEFAULT",
      "absent": "DEFAULT",
      "unstated": "Cosmos DB stores a string as a JSON string, with no SQL text type"
    },
    "column.maxLength": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "Cosmos DB has no maximum length"
    },
    "column.precision": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "Cosmos DB has no decimal precision"
    },
    "column.scale": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "Cosmos DB has no decimal scale"
    },
    "column.required": {
      "find": "class:{Entity} > property:{Attribute}",
      "read": [
        { "part": "declaration", "match": "\\[JsonRequired\\]|Required=Required\\.(?:Always|DisallowNull)|\\brequired\\b", "value": "true" },
        { "part": "declaration", "match": "\\?{Attribute}\\b", "value": "false" }
      ],
      "default": "true",
      "unstated": "`{Attribute}` isn't nullable, and nullable reference types are on"
    },
    "column.default": {
      "find": "class:{Entity} > property:{Attribute}",
      "read": [{ "part": "declaration", "match": "\\[DefaultValue\\(((?:[^()]|\\([^()]*\\))*)\\)\\]" }],
      "default": "",
      "absent": "",
      "unstated": "no `[DefaultValue]` for `{Attribute}`"
    },
    "relationship.multiplicity": {
      "find": ["class:{Source} > property:{TargetEnd}", "class:{Source} > property:{TargetEnd}Id"],
      "read": [
        { "part": "declaration", "match": "<{Target}>{TargetEnd}\\b", "value": "1->*" },
        { "part": "declaration", "match": "\\b{Target}\\?{TargetEnd}\\b", "value": "1->0..1" },
        { "part": "declaration", "match": "\\b{Target} {TargetEnd}\\b", "value": "1->1" },
        { "part": "declaration", "match": "\\?{TargetEnd}(?<=\\w)Id\\b", "value": "*->0..1" },
        { "part": "declaration", "match": "\\b{TargetEnd}(?<=\\w)Id\\b", "value": "*->1" }
      ]
    },
    "relationship.foreignKey": {
      "find": "class:{Source} > property:{TargetEnd}Id",
      "read": [{ "part": "declaration", "match": "\\b({TargetEnd}(?<=\\w)Id)\\b" }],
      "absent": "",
      "unstated": "no `{TargetEnd}Id` property, so `{TargetEnd}` is embedded, with no foreign key"
    }
  },
  "own": []
}
```

Cosmos DB has no named indexes, only a container's indexing policy, so the recipe file has no `index.columns` or `index.unique`.

## How each form is modelled

Model each form as this table says. The last column is where validate-drift-recipes should show each kind of fact's values coming from: one that comes from `default` or `absent` where the table says code means the recipe doesn't read this codebase's form yet. Fix the recipe, not the model.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `key` | The property serialized as `id` (`[JsonProperty("id")]`, `[JsonPropertyName("id")]`, or one named `Id`): `Primary Key` on that attribute. The partition key isn't a fact | Code. `default` only where the id is on a base class |
| `column.name` | `[JsonProperty("…")]` or `[JsonPropertyName("…")]`: `Field Setting` → `Name` | Code where the property is renamed, the id included (`id`); else `default` |
| `column.required` | `?` on the property: nullable. `[JsonRequired]`, `Required = Required.Always` or `required`: not nullable | Code for every nullable one, `default` for the rest |
| `column.default` | `[DefaultValue(…)]`: `Default Constraint` → `Value`, as the code writes it | Code where stated, else `default` |
| `column.type`, `column.textType`, `column.maxLength`, `column.precision`, `column.scale` | None: the model leaves them unset | `default` |
| `relationship.multiplicity` | An embedded document (`Address Address`, `List<OrderLine> Lines`): a composition, `1->1` or `1->*`. A reference to another document (`string CustomerId`): an aggregation, `*->1` (`*->0..1` when nullable) | Code |
| `relationship.foreignKey` | The reference property (`CustomerId`): `Foreign Key` on that attribute | Code for a reference, `absent` for an embedded document |

## EF Core's Cosmos provider

Start from `ef-core.md`'s recipe file, and:

- `table.name`, `table.schema`, `table.kind`, `index.columns` and `index.unique`: delete them. A document model has no tables or named indexes; `ToContainer` names a container, which isn't a fact.
- `column.type`, `column.textType`, `column.maxLength`, `column.precision` and `column.scale`: keep them. EF Core reads `HasMaxLength` and the rest from the model even on Cosmos DB, and the model states what the code says.
- `column.name`: replace it with this one, which reads `ToJsonProperty`:

```json
"column.name": {
  "find": [
    "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
    "class with IEntityTypeConfiguration<{Entity}> > method:Configure > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
    "class:{Entity} > property:{Attribute}"
  ],
  "read": [
    { "match": "\\.ToJsonProperty\\(\"([^\"]+)\"\\)" },
    { "part": "declaration", "match": "\\[JsonPropertyName\\(\"([^\"]+)\"\\)" }
  ],
  "inherit": ["class:{Entity} > property:{Attribute}"],
  "default": "",
  "absent": "",
  "unstated": "no `ToJsonProperty` or `[JsonPropertyName]` for `{Attribute}`, so it is named after the property"
}
```

## Adapting to house style

| You see | Change |
| --- | --- |
| A base document class that declares the id (`class Document { [JsonProperty("id")] public string Id … }`) | Nothing: `key` reads `default`, since the model doesn't state a key on a base class |
| A reference collection (`List<string> TagIds`) | A read case before the others for `relationship.multiplicity`: `\\bList<\\w+>{TargetEnd|trimEnd:s}Ids\\b`, value `*->*`, and a find path `class:{Source} > property:{TargetEnd|trimEnd:s}Ids` |

## Ownership

The SDK has no registration per document type for an `own` entry to find. Add a convention for the document classes (configure-drift-check, "New types of each element type"), such as every class in the documents' folder.
