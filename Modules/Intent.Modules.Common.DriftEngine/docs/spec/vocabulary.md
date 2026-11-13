# Vocabulary (block 1)

Vocabulary says what a designer's model states about the code. It is pack data: the stock designers' vocabularies ship in packs just as a module's or a house pack's do, so any designer, element type, association type or stereotype can carry claims without a change to the engine ([R28](requirements.md)).

Common.Drift fixed each designer's facts in C#, in the module contributing that designer, and allowed one contributor per designer. eShopCustom's Front End and Architecture modules defined their own designers, which no Common.Drift contribution could read; phase 2 proves the open vocabulary by checking their State Store and Layer elements with no core change.

## The model the vocabulary reads

The engine reads Intent's model directly (decision 2), through the Software Factory's metadata API: `IMetadataManager` for the application's designers, and `IPackage`, `IElement`, `IAssociation`, `IAssociationEnd`, `IStereotype` and `ITypeReference` beneath. Vocabulary sees, for each designer:

| Model feature   | What vocabulary can read                                                                                                |
| --------------- | ----------------------------------------------------------------------------------------------------------------------- |
| Package         | name, its stereotypes                                                                                                   |
| Folder          | name, its stereotypes, its path of folder names                                                                         |
| Element         | id, name, specialization, comment, parent, children by specialization, stereotypes and their properties, type reference |
| Type reference  | the referenced element (and its specialization), collection, nullable, generic arguments                                |
| Association     | specialization, its source and target ends                                                                              |
| Association end | name, the element at that end, multiplicity (`1`, `0..1`, `*`), navigable, the association it belongs to                |
| Stereotype      | name, each property's value; a property equal to its definition's default counts as unset                               |

The acceptance suite builds these objects from fixture data in its harness ([acceptance-suite.md](acceptance-suite.md#the-harness)); the engine never sees anything else.

## A vocabulary entry

Each entry produces claims of one kind from model elements of one shape. Its schema is [`schemas/vocabulary.schema.json`](schemas/vocabulary.schema.json).

| Field            | Meaning                                                                                                                                                                                  |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `kind`           | The claim kind, defined in the pack's or another active pack's `kinds` ([Claim kinds](#claim-kinds))                                                                                     |
| `for`            | Which model features state it: `{ "element": "<specialization>" }`, `{ "association": "<specialization>" }`, or `{ "package": "<specialization>" }`, with an optional `where` expression |
| `each`           | Optional. An expression giving a list; one claim per item, which expressions see as `item`. Without it, one claim per selected element.                                                  |
| `when`           | Optional. The claim exists only where this is true. A `whenSet` kind's claim exists only where its `value` isn't empty, without saying so.                                               |
| `value`          | The expected value, as an expression                                                                                                                                                     |
| `bindings`       | Placeholders for the kind's rules, each an expression                                                                                                                                    |
| `own`            | The placeholders that name the claim's own element, for the wrong-element check                                                                                                          |
| `discriminator`  | Optional. An expression telling apart several claims of this kind on one element                                                                                                         |
| `parameterNames` | Optional. For operations: an expression listing the parameter types, choosing between overloads                                                                                          |
| `dependsOn`      | Optional. The claim kind, on this element or `parent`, that must be verified first; usually `type.exists` or `member.exists`                                                             |
| `source`         | The model feature in words, for messages: "the Table stereotype's Name"                                                                                                                  |
| `fixHint`        | Optional. A sentence added to a contradicted finding, with placeholders                                                                                                                  |

The element that states a claim is the selected element; for an association, the association. Its `path` runs from the outermost owner (the package's top element) to it.

### Example

From the stock Domain pack. A table name is claimed only where the model sets it (decision 4); multiplicity is always stated:

```json
{
  "designer": "Domain",
  "entries": [
    {
      "kind": "table.name",
      "for": { "element": "Class", "where": "hasStereotype('Table') || hasStereotype('View')" },
      "value": "stereotype('Table').Name ?? stereotype('View').Name",
      "bindings": { "Entity": "name" },
      "own": ["Entity"],
      "dependsOn": "type.exists",
      "source": "the Table stereotype's Name",
      "fixHint": "Change the Table stereotype's Name, or the code's table name."
    },
    {
      "kind": "relationship.multiplicity",
      "for": { "association": "Association", "where": "source.element.hasStereotype('Table') || target.element.hasStereotype('Table')" },
      "value": "source.multiplicity + '->' + target.multiplicity",
      "bindings": {
        "Source": "source.element.name",
        "Target": "target.element.name",
        "SourceEnd": "source.name",
        "TargetEnd": "target.name"
      },
      "own": ["SourceEnd", "TargetEnd"],
      "source": "the association's ends"
    },
    {
      "kind": "index.columns",
      "for": { "element": "Class", "where": "hasStereotype('Table')" },
      "each": "plugin('intent.domain.indexes')",
      "value": "join(item.columns, ', ')",
      "discriminator": "item.name ?? join(item.columns, ',')",
      "bindings": { "Entity": "name", "Index": "item.name ?? ''" },
      "own": ["Entity"],
      "source": "the Index stereotype on the class's attributes"
    }
  ]
}
```

The `index.columns` entry shows a plug-in model function: grouping attributes by `UniqueKey` and ordering them by `Order` is easier in C# than in an expression. It also claims unnamed indexes (decision 9), whose `Index` placeholder is empty.

And from a phase 2 house pack for eShopCustom's own Architecture designer, which no stock pack knows:

```json
{
  "designer": "Architecture",
  "entries": [
    {
      "kind": "layer.populated",
      "for": { "element": "Layer" },
      "value": "true",
      "bindings": { "Layer": "name", "Path": "stereotype('Layer Settings').Path" },
      "own": ["Layer"],
      "source": "the Layer and its Layer Settings Path"
    }
  ]
}
```

## Placeholders

Bindings are what rules see. The stock packs keep Common.Drift's placeholder names so its recipes convert unchanged ([conversion.md](conversion.md)):

| Designer       | Placeholders                                                                                                              |
| -------------- | ------------------------------------------------------------------------------------------------------------------------- |
| Domain         | `{Entity}`, `{Attribute}`, `{Index}`, `{Source}`, `{Target}`, `{SourceEnd}`, `{TargetEnd}`, `{Member}`, `{DomainEvent}`   |
| Services       | `{Service}`, `{Operation}`, `{Folder}`, `{Parameter}`, `{Entity}`, `{Member}`, `{Invoked}`, `{InvokedOwner}`, `{Message}` |
| User Interface | `{Folder}`, `{Component}`, `{Invoked}`, `{Route}`, `{RouteStart}`, `{Page}`, `{Dialog}`, `{Child}`                        |

Structure kinds add the language-neutral `{Type}`, `{TypeKind}`, `{Namespace}` and `{Folder}` (the element's folder path in the designer). A binding whose expression is empty gives an empty placeholder, which recipes can use as a switch, as Common.Drift's do.

## Expressions

A small, side-effect-free language, evaluated against the selected element (or association, or package). It returns text, a number, a bool, a list, an object (from a plug-in) or nothing. Empty text is nothing, so an unset property gives no claim (decision 10).

| Form                                                                                                 | Meaning                                                                                                                                       |
| ---------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------- |
| `name`, `id`, `specialization`, `comment`                                                            | The element's own                                                                                                                             |
| `parent`, `package`, `folderPath`                                                                    | Its parent element, its package, and its designer folders joined by `/`                                                                       |
| `children('Attribute')`                                                                              | Its children of a specialization, in model order                                                                                              |
| `type.name`, `type.element`, `type.isCollection`, `type.isNullable`, `type.arguments`                | Its type reference                                                                                                                            |
| `associations('Association')`                                                                        | The association ends leading away from it, of a specialization                                                                                |
| `source`, `target`, `.multiplicity`, `.isNavigable`, `.element`                                      | An association's ends, and what they say                                                                                                      |
| `stereotype('Table').Name`, `hasStereotype('Table')`                                                 | A stereotype's property (a property name with spaces is quoted: `stereotype('Text Constraints')['SQL Data Type']`), and whether it is applied |
| `a ?? b`, `a + b`, `a == b`, `a != b`, `!a`, `a && b`, `a \|\| b`, `if(c, a, b)`, `exists(a)`        | Operators                                                                                                                                     |
| `join(list, sep)`, `map(list, expr)`, `where(list, expr)`, `count(list)`                             | Lists; inside `map` and `where`, `it` is the item                                                                                             |
| `lower`, `upper`, `camel`, `pascal`, `kebab`, `trimStart(x, s)`, `trimEnd(x, s)`, `replace(x, a, b)` | Text, worded as Common.Drift's placeholder filters                                                                                            |
| `plugin('id', args…)`                                                                                | A model function from a pack's plug-in ([rules.md](rules.md#plug-ins))                                                                        |
| `item`                                                                                               | The current item of `each`                                                                                                                    |

There are deliberately no functions that derive a framework's conventions, such as pluralising a class name into a table name. The model states what the code states explicitly (decision 4).

## Claim kinds

A kind is defined once, in a pack's `kinds`, and used by any number of entries. Its schema is in [`schemas/claim-kinds.schema.json`](schemas/claim-kinds.schema.json).

| Field       | Meaning                                                                                                 |
| ----------- | ------------------------------------------------------------------------------------------------------- |
| `kind`      | Its id: dotted, lower camel case, such as `column.maxLength`                                            |
| `label`     | How messages name it: "max length"                                                                      |
| `valueType` | `text`, `bool`, `number`, `list`, `set`, `type`, `route` or `presence`                                  |
| `comparer`  | One of [the comparers](claims-and-findings.md#comparers), or a plug-in's                                |
| `stated`    | `always` or `whenSet` ([claims-and-findings.md](claims-and-findings.md#stated-always-or-only-when-set)) |
| `existence` | `true` for kinds whose not-found is contradicted rather than can't verify                               |

The stock kinds and their acceptance cases are in [claim-catalog.md](claim-catalog.md) and the suite's `Acceptance/catalog.json`.

## Coverage of the model

Nothing modelled is skipped silently ([R6](requirements.md), [R7](requirements.md)). For each designer with content, the engine works out every model feature it holds (an element's specialization, each applied stereotype, each stereotype property set away from its default, each association's specialization, each type reference) and which features the active entries read. It finds that from the entries' `for` and from the stereotypes, properties and type references their expressions name; a plug-in model function declares what it reads.

A feature nothing reads is an `engine/noVocabulary` finding, once per element and feature:

> `Order`'s `Audited` stereotype: no pack says what it states in code. Add vocabulary for it, or declare it non-checkable with a reason.

A pack declares what has nothing to check in `nonClaims`, each with a reason ([R32](requirements.md)):

```json
"nonClaims": [
  { "designer": "Domain", "for": { "element": "Comment" }, "reason": "Documentation only" },
  { "designer": "Domain", "for": { "stereotype": "Diagram Settings" }, "reason": "Diagram styling" },
  { "designer": "Visual Studio", "reason": "Projects are compared by the Software Factory itself" }
]
```

The coverage listing shows every non-claim in use, with its pack and reason, so a reviewer sees what isn't checked and why.

## Conflicts

Several packs may give entries for one designer ([R29](requirements.md)). Two entries producing the same kind for the same element and discriminator are a `config/vocabularyConflict` error naming both packs; nothing is checked for that kind on that element until it's resolved. Two packs defining one kind differently (label, value type, comparer or `stated`) are a `config/kindConflict` error.
