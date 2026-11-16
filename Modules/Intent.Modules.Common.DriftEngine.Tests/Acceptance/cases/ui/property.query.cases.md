# property.query

Whether a component's Property is bound from the query string: the Query Parameter stereotype on a User Interface designer Property, against `[SupplyParameterFromQuery]` on the component's property. The kind is `stated: always`: a property with no such attribute states that it isn't a query parameter, so a modelled Property without the stereotype is verified from that silence, and the check runs both ways.

The stock User Interface pack gives the kind and its vocabulary, and the Blazor reader reads the property. A Razor component is a `type` node whose `.razor` and `.razor.cs` files are one unit (R37), so the reader finds the property on the component's type.

## verified

eShopCustom's catalog page takes its page number from the query string, and the model's `Page` property has the Query Parameter stereotype.

```json case
{ "phase": "later", "kinds": ["property.query"], "traces": ["ESC-FE-18"], "requirements": ["R2", "R28", "R37"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
              { "id": "catalog-page", "type": "Component", "name": "Catalog",
                "stereotypes": { "Page": { "Route": "/" } },
                "children": [
                  { "id": "catalog-page-page", "type": "Property", "name": "Page",
                    "typeRef": { "type": "int", "nullable": true },
                    "stereotypes": { "Query Parameter": {} } }
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

```razor file src/WebApp/Components/Pages/Catalog/Catalog.razor
@page "/"
@inject CatalogService CatalogService

<PageTitle>AdventureWorks</PageTitle>

<div class="catalog">
    <CatalogSearch BrandId="@BrandId" />
</div>

@code {
    [SupplyParameterFromQuery]
    public int? Page { get; set; }

    [SupplyParameterFromQuery(Name = "brand")]
    public int? BrandId { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "property.query", "element": "catalog-page-page", "outcome": "verified",
      "expected": "true", "actual": "true", "at": "src/WebApp/Components/Pages/Catalog/Catalog.razor:11" }
  ]
}
```

## verified-false-from-silence

A Property the model leaves without the stereotype, on a component parameter with no `[SupplyParameterFromQuery]`: the code's silence states `false`, which the model states too. `BrandId`, which the model doesn't have, is not a claim (P1).

```json case
{ "phase": "later", "base": "verified", "kinds": ["property.query"], "cases": ["engine/silence"], "traces": ["ESC-FE-18"], "requirements": ["R3", "R44"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
              { "id": "catalog-page", "type": "Component", "name": "Catalog",
                "stereotypes": { "Page": { "Route": "/" } },
                "children": [
                  { "id": "catalog-page-page", "type": "Property", "name": "Page",
                    "typeRef": { "type": "int", "nullable": true },
                    "stereotypes": { "Query Parameter": {} } },
                  { "id": "catalog-page-selected-item", "type": "Property", "name": "SelectedItem",
                    "typeRef": { "type": "int", "nullable": true } }
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

```razor file src/WebApp/Components/Pages/Catalog/Catalog.razor
@page "/"
@inject CatalogService CatalogService

<PageTitle>AdventureWorks</PageTitle>

<div class="catalog">
    <CatalogSearch BrandId="@BrandId" />
</div>

@code {
    [SupplyParameterFromQuery]
    public int? Page { get; set; }

    [SupplyParameterFromQuery(Name = "brand")]
    public int? BrandId { get; set; }

    [Parameter]
    public int? SelectedItem { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "property.query", "element": "catalog-page-selected-item", "outcome": "verified",
      "expected": "false", "actual": "false", "fromSilence": true },
    { "kind": "property.query", "element": "catalog-page-page", "outcome": "verified" }
  ]
}
```

## contradicted-not-modelled

The code binds `BrandId` from the query string (`?brand=`), and the model's `BrandId` Property has no Query Parameter stereotype.

```json case
{ "phase": "later", "base": "verified", "kinds": ["property.query"], "traces": ["ESC-FE-18"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
              { "id": "catalog-page", "type": "Component", "name": "Catalog",
                "stereotypes": { "Page": { "Route": "/" } },
                "children": [
                  { "id": "catalog-page-page", "type": "Property", "name": "Page",
                    "typeRef": { "type": "int", "nullable": true },
                    "stereotypes": { "Query Parameter": {} } },
                  { "id": "catalog-page-brand-id", "type": "Property", "name": "BrandId",
                    "typeRef": { "type": "int", "nullable": true } }
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
    { "kind": "property.query", "element": "catalog-page-brand-id", "outcome": "contradicted",
      "expected": "false", "actual": "true", "at": "src/WebApp/Components/Pages/Catalog/Catalog.razor:14" }
  ],
  "findings": [
    { "rule": "claim/property.query", "severity": "error", "subject": "catalog-page-brand-id" }
  ]
}
```

## contradicted-attribute-missing

The model's `Page` Property is a Query Parameter, and the code's `Page` has lost its `[SupplyParameterFromQuery]`, keeping only `[Parameter]`. From silence, it isn't one.

```json case
{ "phase": "later", "base": "verified", "kinds": ["property.query"], "traces": ["ESC-FE-18"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Catalog/Catalog.razor
@page "/"
@inject CatalogService CatalogService

<PageTitle>AdventureWorks</PageTitle>

<div class="catalog">
    <CatalogSearch BrandId="@BrandId" />
</div>

@code {
    [Parameter]
    public int? Page { get; set; }

    [SupplyParameterFromQuery(Name = "brand")]
    public int? BrandId { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "property.query", "element": "catalog-page-page", "outcome": "contradicted",
      "expected": "true", "actual": "false", "fromSilence": true }
  ]
}
```
