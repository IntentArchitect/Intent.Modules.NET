# intent.target

The type a component's link resolves to in the code, against the element the link targets: a Razor tag's component, or the class an `@inject` names for a Call Service Operation. The tag or injected name is resolved as the compiler would: its qualifier, the file's `@namespace` and `@using`s, `_Imports.razor`'s, then the component's own namespace ([code-reader.md](../../../../Intent.Modules.Common.DriftEngine/docs/spec/code-reader.md#markup-references)). The kind is `stated: always` and compares with the `type` comparer: the resolved type must be the one the target element is located as. `intent.uses` says the link is in the code; this kind says it reaches the modelled element and not another of the same name.

The `intent.ui` pack gives the vocabulary; the kind is `later`.

## composition-target

eShopCustom's catalog page renders `<CatalogListItem>`. `_Imports.razor` brings in `eShop.WebAppComponents.Catalog`, so the tag is the component in WebAppComponents' Catalog folder, the one the Composition targets (`composition.package`).

```json case
{ "phase": "later", "kinds": ["intent.target"], "traces": ["ESC-FE-30"], "requirements": ["R2", "R37"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [
        {
          "id": "webapp", "name": "WebApp",
          "elements": [
            { "id": "pages", "type": "Folder", "name": "Pages",
              "children": [
                { "id": "catalog-page", "type": "Component", "name": "Catalog",
                  "stereotypes": { "Page": { "Route": "/" } } }
              ] }
          ]
        },
        {
          "id": "webappcomponents", "name": "WebAppComponents",
          "elements": [
            { "id": "catalog", "type": "Folder", "name": "Catalog",
              "children": [
                { "id": "catalog-list-item", "type": "Component", "name": "CatalogListItem" }
              ] }
          ]
        }
      ],
      "associations": [
        { "id": "catalog-renders-list-item", "type": "Composition",
          "source": { "element": "catalog-page" }, "target": { "element": "catalog-list-item" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebApp/Components/_Imports.razor
@using eShop.WebAppComponents.Catalog
@using eShop.WebAppComponents.Services
```

```razor file src/WebApp/Components/Pages/Catalog/Catalog.razor
@page "/"

<div class="catalog-items">
    @foreach (var item in items)
    {
        <CatalogListItem Item="@item" />
    }
</div>
```

```razor file src/WebAppComponents/Catalog/CatalogListItem.razor
@namespace eShop.WebAppComponents.Catalog

<div class="catalog-item">@Item.Name</div>

@code {
    [Parameter, EditorRequired]
    public required CatalogItem Item { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.target", "element": "catalog-renders-list-item", "outcome": "verified", "at": "src/WebApp/Components/Pages/Catalog/Catalog.razor:6" }
  ]
}
```

## react-composition-of-a-wrapped-default-export

A React tag names what the module it imports exports by default: `Footer.jsx` exports `withStyles(styles)(Footer)`, so `<Footer />` is the `Footer` component the wrapper is given. The file's module is named `Footer` too, as a module is named by its file, and the tag never means the module.

```json case
{ "phase": "later", "kinds": ["intent.target"], "requirements": ["R2", "R37"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "spa", "name": "Spa", "type": "User Interface Package",
        "elements": [
          { "id": "layout", "type": "Component", "name": "Layout" },
          { "id": "footer", "type": "Component", "name": "Footer" }
        ]
      }],
      "associations": [
        { "id": "layout-renders-footer", "type": "Composition", "source": { "element": "layout" }, "target": { "element": "footer" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "spa", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```jsx file src/components/Footer.jsx
import React from 'react';
import { withStyles } from '@material-ui/styles';

const styles = { footer: { padding: 8 } };

class Footer extends React.Component {
  render() {
    return <footer className={this.props.classes.footer}>Footer</footer>;
  }
}

export default withStyles(styles)(Footer);
```

```jsx file src/components/Layout.jsx
import React from 'react';
import Footer from './Footer';

export default class Layout extends React.Component {
  render() {
    return (
      <main>
        <Footer />
      </main>
    );
  }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "footer", "outcome": "verified", "at": "src/components/Footer.jsx:6" },
    { "kind": "intent.target", "element": "layout-renders-footer", "outcome": "verified", "at": "src/components/Layout.jsx:8" }
  ]
}
```

## composition-target-other-package

The WebApp gains its own `CatalogListItem`, and the page imports its namespace instead of WebAppComponents' Catalog, so the tag is the WebApp's component, not the one the Composition targets.

```json case
{ "phase": "later", "base": "composition-target", "kinds": ["intent.target"], "traces": ["ESC-FE-30"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/_Imports.razor
@using eShop.WebAppComponents.Services
```

```razor file src/WebApp/Components/Pages/Catalog/Catalog.razor
@page "/"
@using eShop.WebApp.Components.Catalog

<div class="catalog-items">
    @foreach (var item in items)
    {
        <CatalogListItem Item="@item" />
    }
</div>
```

```razor file src/WebApp/Components/Catalog/CatalogListItem.razor
@namespace eShop.WebApp.Components.Catalog

<div class="catalog-item">@Item.Name</div>

@code {
    [Parameter, EditorRequired]
    public required CatalogItem Item { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.target", "element": "catalog-renders-list-item", "outcome": "contradicted", "at": "src/WebApp/Components/Pages/Catalog/Catalog.razor:7" }
  ],
  "findings": [
    { "rule": "claim/intent.target", "severity": "error", "subject": "catalog-renders-list-item", "messageContains": "eShop.WebApp.Components.Catalog.CatalogListItem" }
  ]
}
```

## call-target

eShopCustom's catalog page calls the catalog proxy's `GetCatalogItems`, and the model has a Call Service Operation from the page to that Service Proxy Operation. `@inject CatalogService` resolves, through `_Imports.razor`, to the proxy class in WebAppComponents' Services namespace, the one the Service Proxy is located as (`call.package`).

```json case
{ "phase": "later", "kinds": ["intent.target"], "traces": ["ESC-FE-38"], "requirements": ["R2", "R37"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [
        {
          "id": "webapp", "name": "WebApp",
          "elements": [
            { "id": "pages", "type": "Folder", "name": "Pages",
              "children": [
                { "id": "catalog-page", "type": "Component", "name": "Catalog",
                  "stereotypes": { "Page": { "Route": "/" } } }
              ] }
          ]
        },
        {
          "id": "webappcomponents", "name": "WebAppComponents",
          "elements": [
            { "id": "services", "type": "Folder", "name": "Services",
              "children": [
                { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService",
                  "children": [
                    { "id": "catalog-service-get-catalog-items", "type": "Operation", "name": "GetCatalogItems" }
                  ] }
              ] }
          ]
        }
      ],
      "associations": [
        { "id": "catalog-calls-get-catalog-items", "type": "Call Service Operation",
          "source": { "element": "catalog-page" }, "target": { "element": "catalog-service-get-catalog-items" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebApp/Components/_Imports.razor
@using eShop.WebAppComponents.Catalog
@using eShop.WebAppComponents.Services
```

```razor file src/WebApp/Components/Pages/Catalog/Catalog.razor
@page "/"
@inject CatalogService CatalogService

<div class="catalog-items">
    @foreach (var item in items)
    {
        <CatalogListItem Item="@item" />
    }
</div>

@code {
    private List<CatalogItem> items = [];

    protected override async Task OnInitializedAsync()
    {
        items = await CatalogService.GetCatalogItems();
    }
}
```

```csharp file src/WebAppComponents/Services/CatalogService.cs
using System.Net.Http.Json;
using eShop.WebAppComponents.Catalog;

namespace eShop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient)
{
    public async Task<List<CatalogItem>> GetCatalogItems()
    {
        return await httpClient.GetFromJsonAsync<List<CatalogItem>>("api/catalog/items") ?? [];
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.target", "element": "catalog-calls-get-catalog-items", "outcome": "verified", "at": "src/WebApp/Components/Pages/Catalog/Catalog.razor:2" }
  ]
}
```
