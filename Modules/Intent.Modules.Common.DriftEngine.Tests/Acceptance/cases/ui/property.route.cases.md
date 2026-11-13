# property.route

Whether a component's Property is bound from its route: the Route Parameter stereotype on a User Interface designer Property, against a `{name}` parameter in the component's `@page` route. Route parameters bind to properties ignoring case, so the read is case-insensitive. The kind is `stated: always`: a route with no such parameter states that the Property isn't one, and the check runs both ways.

The kind is `later`, so these cases' house pack gives the kind, its vocabulary and its rule.

## verified

The item page's route `/item/{itemId:int}` binds its `ItemId` property, and the model's `ItemId` Property has the Route Parameter stereotype. The parameter's lower-case first letter and its constraint don't matter.

```json case
{ "phase": "later", "kinds": ["property.route"], "traces": ["ESC-FE-19"], "requirements": ["R2", "R28", "R38"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "kinds": [
    { "kind": "property.route", "label": "route parameter", "valueType": "bool", "comparer": "exact", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "User Interface": {
      "entries": [
        {
          "kind": "property.route",
          "for": { "element": "Property", "where": "parent.specialization == 'Component'" },
          "value": "hasStereotype('Route Parameter')",
          "bindings": { "Component": "parent.name", "Property": "name" },
          "own": ["Component", "Property"],
          "source": "the Route Parameter stereotype on the Property"
        }
      ]
    }
  },
  "rules": {
    "User Interface": {
      "files": ["src/WebApp/Components/**/*.razor"],
      "facts": {
        "property.route": {
          "find": "file:src/WebApp/Components/**/{Component}.razor",
          "read": [{ "match": "(?i)@page\"[^\"]*\\{{Property}(?::[^}]*)?\\}", "value": "true" }],
          "default": "false",
          "unstated": "no `{{Property}}` parameter in {Component}'s `@page` routes"
        }
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
        "id": "webapp", "name": "WebApp",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "Pages",
            "children": [
              { "id": "item-page", "type": "Component", "name": "ItemPage",
                "stereotypes": { "Page": { "Route": "/item/{itemId}" } },
                "children": [
                  { "id": "item-page-item-id", "type": "Property", "name": "ItemId",
                    "typeRef": { "type": "int" },
                    "stereotypes": { "Route Parameter": {} } }
                ] }
            ] }
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

```razor file src/WebApp/Components/Pages/Item/ItemPage.razor
@page "/item/{itemId:int}"
@inject CatalogService CatalogService

<PageTitle>@item?.Name | AdventureWorks</PageTitle>

@code {
    private CatalogItem? item;

    [Parameter]
    public int ItemId { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "property.route", "element": "item-page-item-id", "outcome": "verified",
      "expected": "true", "actual": "true", "at": "src/WebApp/Components/Pages/Item/ItemPage.razor:1" }
  ]
}
```

## contradicted-not-modelled

The route binds `itemId`, and the model's `ItemId` Property has no Route Parameter stereotype.

```json case
{ "phase": "later", "base": "verified", "kinds": ["property.route"], "traces": ["ESC-FE-19"], "requirements": ["R2", "R14"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "Pages",
            "children": [
              { "id": "item-page", "type": "Component", "name": "ItemPage",
                "stereotypes": { "Page": { "Route": "/item/{itemId}" } },
                "children": [
                  { "id": "item-page-item-id", "type": "Property", "name": "ItemId",
                    "typeRef": { "type": "int" } }
                ] }
            ] }
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

```json expect
{
  "claims": [
    { "kind": "property.route", "element": "item-page-item-id", "outcome": "contradicted",
      "expected": "false", "actual": "true", "at": "src/WebApp/Components/Pages/Item/ItemPage.razor:1" }
  ],
  "findings": [
    { "rule": "claim/property.route", "severity": "error", "subject": "item-page-item-id" }
  ]
}
```

## contradicted-not-in-route

The model's `ItemId` is a Route Parameter, and the page's route has lost its parameter: the item now comes from somewhere else. From silence, `ItemId` isn't a route parameter.

```json case
{ "phase": "later", "base": "verified", "kinds": ["property.route"], "cases": ["engine/silence"], "traces": ["ESC-FE-19"], "requirements": ["R3"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Item/ItemPage.razor
@page "/item"
@inject CatalogService CatalogService

<PageTitle>@item?.Name | AdventureWorks</PageTitle>

@code {
    private CatalogItem? item;

    [Parameter]
    public int ItemId { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "property.route", "element": "item-page-item-id", "outcome": "contradicted",
      "expected": "true", "actual": "false", "fromSilence": true }
  ]
}
```
