# type.generics

The element's generic type parameters (R41). Vocabulary reads them with the `genericTypes` expression, by name and in model order; the C# reader gives the type node's `typeParameters`, by name and in declaration order. The comparer is `list`: the same names in the same order. Constraints such as `where TEntity : class` are on the `typeParameter` nodes, not in their names, so they don't take part.

## dto-verified

eShopCustom's Services module checked that a generic DTO declares the generic parameters its model has. `PaginatedItems` is modelled with one, `TEntity`, and the class declares it.

```json case
{ "phase": 1, "kinds": ["type.generics"], "traces": ["ESC-SVC-45"], "requirements": ["R2", "R10", "R30", "R41"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Catalog.Services",
        "elements": [
          { "id": "paginated-items", "type": "DTO", "name": "PaginatedItems", "genericTypes": ["TEntity"],
            "children": [
              { "id": "paginated-items-page-index", "type": "DTO-Field", "name": "PageIndex", "typeRef": { "type": "int" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Catalog", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Catalog.API/Model/PaginatedItems.cs
namespace eShop.Catalog.API.Model;

public class PaginatedItems<TEntity> where TEntity : class
{
    public int PageIndex { get; init; }

    public IEnumerable<TEntity> Data { get; init; } = [];
}
```

```json expect
{
  "claims": [
    { "kind": "type.generics", "element": "paginated-items", "outcome": "verified", "expected": "TEntity", "actual": "TEntity",
      "at": "src/Catalog.API/Model/PaginatedItems.cs:3" }
  ]
}
```

## dto-parameter-renamed

The class names its parameter `T`. The parameters are compared by name, so the DTO is contradicted, naming both lists and the line (R11, R14).

```json case
{ "phase": 1, "base": "dto-verified", "kinds": ["type.generics"], "traces": ["ESC-SVC-45"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```csharp file src/Catalog.API/Model/PaginatedItems.cs
namespace eShop.Catalog.API.Model;

public class PaginatedItems<T> where T : class
{
    public int PageIndex { get; init; }

    public IEnumerable<T> Data { get; init; } = [];
}
```

```json expect
{
  "claims": [
    { "kind": "type.generics", "element": "paginated-items", "outcome": "contradicted", "expected": "TEntity", "actual": "T",
      "at": "src/Catalog.API/Model/PaginatedItems.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.generics", "severity": "error", "subject": "paginated-items", "messageContains": "TEntity" }
  ]
}
```

## dto-not-generic

The class isn't generic at all. The model's one parameter isn't among the code's none, so the claim is contradicted. The kind is `always`, so a type with no type parameters states that it has none.

```json case
{ "phase": 1, "base": "dto-verified", "kinds": ["type.generics"], "traces": ["ESC-SVC-45"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```csharp file src/Catalog.API/Model/PaginatedItems.cs
namespace eShop.Catalog.API.Model;

public class PaginatedItems
{
    public int PageIndex { get; init; }

    public IEnumerable<object> Data { get; init; } = [];
}
```

```json expect
{
  "claims": [
    { "kind": "type.generics", "element": "paginated-items", "outcome": "contradicted", "expected": "TEntity",
      "at": "src/Catalog.API/Model/PaginatedItems.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.generics", "severity": "error", "subject": "paginated-items", "messageContains": "PaginatedItems" }
  ]
}
```

## blocked

`PaginatedItems` isn't in the code. Its type parameters have nothing to be read from, so `type.generics` is can't verify, `blocked` behind the contradicted `type.exists`, and only the missing type is reported (R5).

```json case
{ "phase": 1, "base": "dto-verified", "without": ["src/Catalog.API/Model/PaginatedItems.cs"], "kinds": ["type.exists", "type.generics"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```csharp file src/Catalog.API/Model/CatalogItem.cs
namespace eShop.Catalog.API.Model;

public class CatalogItem
{
    public int Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "paginated-items", "outcome": "contradicted" },
    { "kind": "type.generics", "element": "paginated-items", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "paginated-items", "messageContains": "isn't in the code" },
    { "rule": "claim/type.generics", "count": 0, "note": "Counted in the missing type's finding" }
  ]
}
```

## model-definition-verified

eShopCustom's Front End module checked that a generic User Interface Model Definition declares the same type parameters, in the same order. No stock pack knows the User Interface designer before phase 6, so the house pack states the Model Definition's type and its type parameters, read with the same `genericTypes` expression (R28, R30, R53).

```json case
{ "phase": 2, "kinds": ["type.generics"], "traces": ["ESC-FE-41"], "requirements": ["R2", "R10", "R28", "R30", "R53"], "packs": ["intent.core", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "vocabulary": {
    "User Interface": {
      "entries": [
        {
          "kind": "type.exists",
          "for": { "element": "Model Definition" },
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "source": "the Model Definition"
        },
        {
          "kind": "type.generics",
          "for": { "element": "Model Definition" },
          "value": "genericTypes",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Model Definition's generic types",
          "fixHint": "Declare the same type parameters, in the same order, or change the Model Definition's generic types."
        }
      ]
    }
  },
  "conventions": {
    "User Interface": {
      "types": {
        "Model Definition": { "typeKinds": ["record", "class", "struct"] }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "web-app-components", "name": "WebAppComponents",
        "elements": [
          { "id": "lookup-item", "type": "Model Definition", "name": "LookupItem", "genericTypes": ["TKey", "TValue"] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/WebAppComponents/Catalog/LookupItem.cs
namespace eShop.WebAppComponents.Catalog;

public record LookupItem<TKey, TValue>(TKey Key, TValue Value);
```

```json expect
{
  "claims": [
    { "kind": "type.generics", "element": "lookup-item", "outcome": "verified", "expected": "TKey, TValue", "actual": "TKey, TValue",
      "at": "src/WebAppComponents/Catalog/LookupItem.cs:3" }
  ]
}
```

## model-definition-order

The record declares the same two parameters the other way round. The comparer is `list`, so order counts, and the claim is contradicted.

```json case
{ "phase": 2, "base": "model-definition-verified", "kinds": ["type.generics"], "traces": ["ESC-FE-41"], "requirements": ["R1", "R10", "R11", "R14"], "packs": ["intent.core", "house.eshop"], "language": "csharp" }
```

```csharp file src/WebAppComponents/Catalog/LookupItem.cs
namespace eShop.WebAppComponents.Catalog;

public record LookupItem<TValue, TKey>(TKey Key, TValue Value);
```

```json expect
{
  "claims": [
    { "kind": "type.generics", "element": "lookup-item", "outcome": "contradicted", "expected": "TKey, TValue", "actual": "TValue, TKey",
      "at": "src/WebAppComponents/Catalog/LookupItem.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.generics", "severity": "error", "subject": "lookup-item", "messageContains": "LookupItem" }
  ]
}
```
