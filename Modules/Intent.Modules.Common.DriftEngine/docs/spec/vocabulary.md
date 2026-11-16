# Vocabulary (block 1)

Vocabulary says what a designer's model states about the code. It is pack data: the stock designers' vocabularies ship in packs just as a module's or a house pack's do, so any designer, element type, association type or stereotype can carry claims without a change to the engine ([R28](requirements.md)).

Common.Drift fixed each designer's facts in C#, in the module contributing that designer, and allowed one contributor per designer. eShopCustom's Front End and Architecture modules defined their own designers, which no Common.Drift contribution could read; phase 2 proves the open vocabulary by checking their State Store and Layer elements with no core change.

## The model the vocabulary reads

The engine reads Intent's model directly (decision 2), through the Software Factory's metadata API: `IMetadataManager` for the application's designers, and `IPackage`, `IElement`, `IAssociation`, `IAssociationEnd`, `IStereotype` and `ITypeReference` beneath. Vocabulary sees, for each designer:

| Model feature   | What vocabulary can read                                                                                                                                                                      |
| --------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Package         | name, its stereotypes, the application it belongs to, and whether it is referenced from another application                                                                                   |
| Folder          | name, its stereotypes, its path of folder names                                                                                                                                               |
| Element         | id, name, specialization, comment, parent, children by specialization, stereotypes and their properties, type reference, generic type parameters, the element it is mapped to               |
| Type reference  | the referenced element (and its specialization), collection, nullable, generic arguments                                                                                                      |
| Association     | specialization, its source and target ends                                                                                                                                                    |
| Association end | name, the element at that end, multiplicity (`1`, `0..1`, `*`), navigable, its stereotypes, whether it is the source or the target, the association it belongs to                            |
| Stereotype      | name, each property's value. A property that holds an element (an element lookup) gives that element, which expressions can follow. A property equal to its definition's default counts as unset |

"Counts as unset" matters in two places only: a `whenSet` kind makes no claim from it (decision 10), and coverage doesn't count it as a feature ([Coverage of the model](#coverage-of-the-model)). A `stated: always` kind claims its value whatever it is, so an index's `IsUnique` left at `false` is still a claim that the index isn't unique.

An association end can itself be the source of another association: Intent draws a handler's interactions from the target end of the Subscribe association it handles. Such an association's source is that end, and `source.element` is the element at the end.

The acceptance suite builds these objects from fixture data in its harness ([acceptance-suite.md](acceptance-suite.md#the-harness)); the engine never sees anything else.

## A vocabulary entry

Each entry produces claims of one kind from model elements of one shape. Its schema is [`schemas/vocabulary.schema.json`](schemas/vocabulary.schema.json).

| Field            | Meaning                                                                                                                                                                                  |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `kind`           | The claim kind, defined in the pack's or another active pack's `kinds` ([Claim kinds](#claim-kinds))                                                                                     |
| `for`            | Which model features state it: `{ "element": "<specialization>" }`, `{ "association": "<specialization>" }`, or `{ "package": "<specialization>" }`, with an optional `where` expression, and an optional `applies` expression for where the kind applies at all, kept apart from `where`'s test that the value is set: `unmodelled.value`'s probe drops `where` and keeps `applies` (key generation applies to a Primary Key only) |
| `each`           | Optional. An expression giving a list; one claim per item, which expressions see as `item`. Without it, one claim per selected element.                                                  |
| `when`           | Optional. The claim exists only where this is true. A `whenSet` kind's claim exists only where its `value` isn't empty, without saying so.                                               |
| `value`          | The expected value, as an expression                                                                                                                                                     |
| `bindings`       | Placeholders for the kind's rules, each an expression                                                                                                                                    |
| `own`            | The placeholders that name the claim's own element, for the wrong-element check                                                                                                          |
| `discriminator`  | Optional. An expression telling apart several claims of this kind on one element                                                                                                         |
| `parameterNames` | Optional. For operations: an expression listing the parameter names in order, choosing between overloads ([rules.md](rules.md#locating-an-elements-code))                               |
| `dependsOn`      | Optional. The claim kind, on this element or `parent`, that must be verified first; usually `type.exists` or `member.exists`. A structure kind that reads what an existence claim located (`member.type`, `operation.returns`, `type.kind`…) depends on its element's (an enum literal's on its enum's) when none is given |
| `waitsOn`        | Optional. Further claim kinds, on this element or `parent`, that block it when the code contradicts them, where the model states them; they don't locate its code. A table's columns wait on `parent:entity.persisted`, so a table nothing maps is one finding |
| `source`         | The model feature in words, for messages: "the Table stereotype's Name"                                                                                                                  |
| `fixHint`        | Optional. A sentence added to a contradicted finding, with placeholders                                                                                                                  |

The element that states a claim is the selected element; for an association, the association. Its `path` runs from the outermost owner (the package's top element) to it.

### Extending another pack's entry

A pack can add placeholders to the claims another pack's entry produces, without redefining the entry. An extending entry has only `extends` (`<pack>/<kind>`), an optional `for` that narrows it, and `bindings`:

```json
{
  "extends": "intent.domain/column.name",
  "for": { "element": "Attribute", "where": "hasStereotype('Backing Field')" },
  "bindings": { "BackingField": "stereotype('Backing Field').Field" }
}
```

This is how a house or module pack gives its own rules what they need from its own stereotypes, here for an attribute EF Core persists from a private field. The extended entry still decides whether a claim exists, its value and its source. An extending entry that binds a placeholder the extended entry already binds, or that another extending entry binds for the same element, is a `config/vocabularyConflict` error.

### Example

From the stock Domain pack. A table name is claimed only where the model sets it (decision 4); multiplicity is always stated, and claimed only for the end no navigation states, as the navigable end is its property's `member.type` and `member.nullable` (the relational extension states it in code, `Multiplicities`, which this entry mirrors):

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
      "for": { "association": "Association", "where": "target.isNavigable && !source.isNavigable && (source.element.hasStereotype('Table') || source.element.hasStereotype('View')) || source.isNavigable && !target.isNavigable && (target.element.hasStereotype('Table') || target.element.hasStereotype('View'))" },
      "value": "if(source.isNavigable, target.multiplicity, source.multiplicity)",
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

And from a phase 2 house pack for the Layers eShopCustom's Architecture module adds to the Codebase Structure designer, which no stock pack knows:

```json
{
  "designer": "Codebase Structure",
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

Bindings are what claim readers see by name, and what a convention's find path and name fill in. The stock packs keep Common.Drift's placeholder names, so its checks map onto the engine's by name ([conversion.md](conversion.md)):

| Designer       | Placeholders                                                                                                              |
| -------------- | ------------------------------------------------------------------------------------------------------------------------- |
| Domain         | `{Entity}`, `{Attribute}`, `{Index}`, `{Source}`, `{Target}`, `{SourceEnd}`, `{TargetEnd}`, `{Member}`, `{DomainEvent}`   |
| Services       | `{Service}`, `{Operation}`, `{Folder}`, `{Parameter}`, `{Entity}`, `{Member}`, `{Invoked}`, `{InvokedOwner}`, `{Message}` |
| User Interface | `{Folder}`, `{Component}`, `{Invoked}`, `{Route}`, `{RouteStart}`, `{Page}`, `{Dialog}`, `{Child}`                        |

`{Folder}` keeps Common.Drift's meaning: the name of the element's nearest designer folder. Structure kinds add the language-neutral `{Type}`, `{TypeKind}`, `{Namespace}` and `{FolderPath}` (the element's designer folders joined by `/`). A binding whose expression is empty gives an empty placeholder, which a claim reader can use as a switch, as Common.Drift's recipes did.

## Expressions

A small, side-effect-free language, evaluated against the selected element (or association, or package). It returns text, a number, a bool, a list, an object (from a plug-in) or nothing. Empty text is nothing, so an unset property gives no claim (decision 10).

| Form                                                                                                 | Meaning                                                                                                                                       |
| ---------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------- |
| `self`, `name`, `id`, `specialization`, `comment`                                                    | The element itself, and its own fields. Where a kind's value type is `type`, an element gives a type value naming it (`self` for "this class")  |
| `parent`, `package`, `folderPath`                                                                    | Its parent element, its package, and its designer folders joined by `/`                                                                       |
| `package.application`, `package.isReferenced`                                                        | The name of the application the package belongs to, and whether it belongs to another application than the one being checked                 |
| `children('Attribute')`                                                                              | Its children of a specialization, in model order                                                                                              |
| `inherited('Attribute')`                                                                             | Its children of a specialization, then those of the classes it derives from through Generalization, nearest first                             |
| `genericTypes`                                                                                       | Its generic type parameters' names, in order                                                                                                  |
| `mappedElement`                                                                                      | The element it is mapped to, or nothing                                                                                                       |
| `type.name`, `type.element`, `type.isCollection`, `type.isNullable`, `type.arguments`                | Its type reference                                                                                                                            |
| `associations('Association')`                                                                        | The association ends leading away from it, of a specialization                                                                                |
| `source`, `target`, `.multiplicity`, `.isNavigable`, `.element`                                      | An association's ends, and what they say                                                                                                      |
| `.association`, `.isSource`, `.isTarget`, `.otherEnd`                                                | On an association end: its association, which end it is, and the end opposite                                                                 |
| `stereotype('Table').Name`, `hasStereotype('Table')`                                                 | A stereotype's property (a property name with spaces is quoted: `stereotype('Text Constraints')['SQL Data Type']`), and whether it is applied. Elements, association ends and packages all have them |
| `stereotype('Backend Operation').Operation.parent.name`                                              | A property holding an element gives the element, so every form above applies to it, in whatever designer it is                                |
| `a ?? b`, `a + b`, `a == b`, `a != b`, `!a`, `a && b`, `a \|\| b`, `if(c, a, b)`, `exists(a)`        | Operators                                                                                                                                     |
| `join(list, sep)`, `split(text, sep)`, `map(list, expr)`, `where(list, expr)`, `count(list)`         | Lists; `split` trims each item and drops empty ones; inside `map` and `where`, `it` is the item                                               |
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
| `itemComparer` | Optional. For list and set kinds: how two items compare (`exact` by default, or `ignoreCase`, `number`, `type`, `route`, `version`) |
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

A non-claim only explains what nothing reads. A pack or extension that reads the feature overrides it, whichever pack declared it: a house pack can check a stereotype a stock pack declares non-checkable, or part of a designer another pack declares non-checkable as a whole. Its claims are checked, the non-claim isn't listed, and what it leaves unread is reported as above.

## Conflicts

Several packs may give entries for one designer ([R29](requirements.md)). Two entries that both produce a claim of the same kind for the same element and discriminator are a `config/vocabularyConflict` error naming both packs; nothing is checked for that kind on that element until it's resolved. Entries that could overlap but don't, because a `where`, a `when` or an empty `whenSet` value leaves one of them without a claim on that element, don't conflict. Two packs defining one kind differently (label, value type, comparer or `stated`) are a `config/kindConflict` error.
