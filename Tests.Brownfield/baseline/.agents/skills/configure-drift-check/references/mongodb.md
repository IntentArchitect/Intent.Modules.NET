---
contentHash: 2FDDFF067138409C34A017EEA8175B321E61ADD8035AB9B6229D8C5F52EFB158
---
# MongoDB: the Domain recipe file

Start `guardrails/domain.drift.json` from this guide when the code stores its documents with the MongoDB C# driver. The recipe file below is tested against code in each style it covers. Copy it, make the decisions below, set `files` to the projects that hold the document classes and their class maps, then adapt it to this codebase's own conventions. Don't write the recipes again from scratch.

## Recognise it

A `MongoDB.Driver` package, with `IMongoCollection<T>`, `BsonClassMap.RegisterClassMap<T>` or `[BsonId]` in the code. A document class is mapped in up to two styles, often mixed:

- **Attributes** on the class: `[BsonId]`, `[BsonElement("name")]`, `[BsonRequired]`, `[BsonDefaultValue(…)]`.
- **Class maps:** `BsonClassMap.RegisterClassMap<Order>(map => { map.AutoMap(); map.MapIdMember(o => o.Id); map.MapMember(o => o.Name).SetElementName("name"); })`.

The recipes read a member's class map first, then its attributes, then the driver's conventions.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| Are nullable reference types on for the document classes? | `<Nullable>enable</Nullable>` in their csproj, or `#nullable enable` in their files | On: nothing. Off: set `column.required`'s `default` to `false`, and its `unstated` to say a string is nullable |
| A convention pack that names elements (`CamelCaseElementNameConvention`) | `ConventionRegistry.Register` | Nothing: the names the code states are read, and the rest stay empty in the model |

## The recipe file

A document database has no SQL types, lengths or defaults of its own, so the recipes for those facts read only what the code states, and otherwise the value the model gives a document's attribute (`DEFAULT` or empty).

```json
{
  "files": "src/**/*.cs",
  "facts": {
    "key": {
      "find": ["statement:RegisterClassMap<{Entity}>", "class:{Entity}"],
      "read": [
        { "match": "\\.Map(?:IdMember|IdProperty|IdField)\\(\\w+=>\\w+\\.(\\w+)\\)" },
        { "match": "\\[BsonId\\b[^\\]]*\\][^{;]*?(\\w+)(?:\\{|=>)" },
        { "match": "\\b((?i:id|_id))(?:\\{|=>)" }
      ],
      "inherit": ["class:{Entity}"],
      "default": "",
      "unstated": "no `[BsonId]`, `MapIdMember` or `Id` property on `{Entity}` itself"
    },
    "column.name": {
      "find": [
        "statement:RegisterClassMap<{Entity}> > chain with Map(?:Member|Property|Field)\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class:{Entity} > property:{Attribute}"
      ],
      "read": [
        { "match": "\\.SetElementName\\(\"([^\"]+)\"\\)" },
        { "part": "declaration", "match": "\\[BsonElement\\(\"([^\"]+)\"\\)" }
      ],
      "inherit": ["class:{Entity} > property:{Attribute}"],
      "default": "",
      "absent": "",
      "unstated": "no `[BsonElement]` or `SetElementName` for `{Attribute}`, so the element is named after the property, or by a convention pack"
    },
    "column.type": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "MongoDB has no column types"
    },
    "column.textType": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "DEFAULT",
      "absent": "DEFAULT",
      "unstated": "MongoDB stores a string as a BSON string, with no SQL text type"
    },
    "column.maxLength": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "MongoDB has no maximum length"
    },
    "column.precision": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "MongoDB has no decimal precision"
    },
    "column.scale": {
      "find": "class:{Entity} > property:{Attribute}",
      "default": "",
      "absent": "",
      "unstated": "MongoDB has no decimal scale"
    },
    "column.required": {
      "find": "class:{Entity} > property:{Attribute}",
      "read": [
        { "part": "declaration", "match": "\\[BsonRequired\\]", "value": "true" },
        { "part": "declaration", "match": "\\?{Attribute}\\b", "value": "false" }
      ],
      "default": "true",
      "unstated": "`{Attribute}` isn't nullable, and nullable reference types are on"
    },
    "column.default": {
      "find": [
        "statement:RegisterClassMap<{Entity}> > chain with Map(?:Member|Property|Field)\\(\\w+=>\\w+\\.{Attribute}\\)",
        "class:{Entity} > property:{Attribute}"
      ],
      "read": [
        { "match": "\\.SetDefaultValue\\(((?:[^()]|\\([^()]*\\))*)\\)" },
        { "part": "declaration", "match": "\\[BsonDefaultValue\\(((?:[^()]|\\([^()]*\\))*)\\)\\]" }
      ],
      "inherit": ["class:{Entity} > property:{Attribute}"],
      "default": "",
      "absent": "",
      "unstated": "no `[BsonDefaultValue]` or `SetDefaultValue` for `{Attribute}`"
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
    },
    "index.columns": {
      "find": "statement with Name=\"{Index}\"",
      "read": [{ "match": "IndexKeys\\.((?:(?:Ascending|Descending)\\(\\w+=>\\w+\\.\\w+\\)\\.)*(?:Ascending|Descending)\\(\\w+=>\\w+\\.\\w+)\\)" }],
      "replace": [
        { "match": "(?:Ascending|Descending)\\(\\w+=>\\w+\\.(\\w+)\\)?\\.?", "with": "$1," },
        { "match": ",$", "with": "" }
      ],
      "absent": "",
      "unstated": "no index named `{Index}` is created"
    },
    "index.unique": {
      "find": "statement with Name=\"{Index}\"",
      "read": [{ "match": "\\bUnique=true\\b", "value": "true" }],
      "default": "false",
      "absent": "",
      "unstated": "the index named `{Index}` isn't created with `Unique = true`"
    }
  },
  "own": [
    { "kind": "class map", "find": "statement with RegisterClassMap<(\\w+)>", "name": "$1" },
    { "kind": "index", "find": "statement with IndexKeys[^;]*\\bName=\"(\\w+)\"", "name": "$1" }
  ]
}
```

## How each form is modelled

Model each form as this table says. The last column is where validate-drift-recipes should show each kind of fact's values coming from: one that comes from `default` or `absent` where the table says code means the recipe doesn't read this codebase's form yet. Fix the recipe, not the model.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `key` | `[BsonId]`, `MapIdMember`, or an `Id` property: `Primary Key` on that attribute | Code. `default` only where the id is on a base class |
| `column.name` | `[BsonElement("…")]` or `SetElementName("…")`: `Field Setting` → `Name` | Code where the element is renamed, else `default` |
| `column.required` | `?` on the property: nullable. `[BsonRequired]`: not nullable | Code for every nullable one, `default` for the rest |
| `column.default` | `[BsonDefaultValue(…)]` or `SetDefaultValue(…)`: `Default Constraint` → `Value`, as the code writes it | Code where stated, else `default` |
| `column.type`, `column.textType`, `column.maxLength`, `column.precision`, `column.scale` | None: the model leaves them unset | `default` |
| `relationship.multiplicity` | An embedded document (`Address Address`, `List<OrderLine> Lines`): a composition, `1->1` or `1->*`. A reference to another document (`string CustomerId`): an aggregation, `*->1` (`*->0..1` when nullable) | Code |
| `relationship.foreignKey` | The reference property (`CustomerId`): `Foreign Key` on that attribute | Code for a reference, `absent` for an embedded document |
| `index.columns`, `index.unique` | An index created with a name (`new CreateIndexOptions { Name = "IX_…", Unique = true }`): `Index` with a `UniqueKey`, or an `Index` element | Code |

## Adapting to house style

| You see | Change |
| --- | --- |
| A reference collection (`List<string> TagIds`) | A read case before the others for `relationship.multiplicity`: `\\bList<\\w+>{TargetEnd|trimEnd:s}Ids\\b`, value `*->*`, and a find path `class:{Source} > property:{TargetEnd|trimEnd:s}Ids` |
| Class maps registered in a helper (`RegisterMap<Order>(map => …)`) | Replace `RegisterClassMap` in the finds with the helper's name |
| An index created with `Builders<T>.IndexKeys.Combine(…)` | A second read case for `index.columns` that reads each key in the call |

## Ownership

The `own` entries report every class map and every named index the model doesn't describe. A document class mapped by attributes alone has no class map to find: add a convention for the document classes (configure-drift-check, "New types of each element type"), such as every class in the documents' folder.

## What it can't read

- An element name or index built at run time (from a constant or a loop): give the fact an `unreadable` case for it and tell the developer.
