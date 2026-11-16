# component.kind

What a User Interface designer Component or Layout is: `Page` for a Component with the Page stereotype, `Dialog`, `Component`, or `Layout` for a Layout. The kind is `stated: always`: a component file with no `@page` and no `LayoutComponentBase` states that it is a plain component.

The `intent.ui` pack (phase 6) gives the vocabulary, with Common.Drift's placeholders (`{Folder}`, `{Component}`); the rules are eShopCustom's house recipes, as phase 6's converter writes them from a Common.Drift recipe file. The find takes the component's files by name, which the reader takes as one unit: `CartPage.razor` and `CartPage.razor.cs` are one place (R37).

## verified-page

The Cart page has the Page stereotype, and its `.razor` file has an `@page` directive.

```json case
{ "phase": 6, "kinds": ["component.kind"], "traces": ["ESC-FE-08", "ESC-FE-11"], "requirements": ["R2", "R38", "R46"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.ui", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "rules": {
    "User Interface": {
      "files": ["src/WebApp/Components/**/*.razor", "src/WebApp/Components/**/*.razor.cs"],
      "facts": {
        "component.kind": {
          "find": "file:src/WebApp/Components/**/{Component}.razor*",
          "read": [
            { "match": "@inherits LayoutComponentBase\\b|:LayoutComponentBase\\b", "value": "Layout" },
            { "match": "@page\"", "value": "Page" }
          ],
          "default": "Component",
          "unstated": "no `@page` or `LayoutComponentBase` in {Component}'s files"
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
              { "id": "cart-page", "type": "Component", "name": "CartPage",
                "stereotypes": { "Page": { "Route": "/cart" } } }
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

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@page "/cart"
@inject BasketState Basket

<PageTitle>Shopping Bag | AdventureWorks</PageTitle>

<div class="cart">
    <a href="checkout" class="button button-primary">Check out</a>
</div>
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "cart-page", "outcome": "verified",
      "expected": "Page", "actual": "Page", "at": "src/WebApp/Components/Pages/Cart/CartPage.razor:1" }
  ]
}
```

## contradicted-page-not-modelled

The code's `@page` makes `CartPage` a page, and the model has it as a plain Component, with no Page stereotype (`page.unmodelled`).

```json case
{ "phase": 6, "base": "verified-page", "kinds": ["component.kind"], "traces": ["ESC-FE-11"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
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
              { "id": "cart-page", "type": "Component", "name": "CartPage" }
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
    { "kind": "component.kind", "element": "cart-page", "outcome": "contradicted",
      "expected": "Component", "actual": "Page", "at": "src/WebApp/Components/Pages/Cart/CartPage.razor:1" }
  ],
  "findings": [
    { "rule": "claim/component.kind", "severity": "error", "subject": "cart-page" }
  ]
}
```

## contradicted-page-without-directive

The model has `CartPage` as a page, and its file has lost its `@page` directive (`page.missing`). No read case matches, so the recipe's `default` says, from silence, that it is a plain component.

```json case
{ "phase": 6, "base": "verified-page", "kinds": ["component.kind"], "cases": ["engine/silence"], "traces": ["ESC-FE-11"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@inject BasketState Basket

<PageTitle>Shopping Bag | AdventureWorks</PageTitle>

<div class="cart">
    <a href="checkout" class="button button-primary">Check out</a>
</div>
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "cart-page", "outcome": "contradicted",
      "expected": "Page", "actual": "Component", "fromSilence": true }
  ]
}
```

## verified-layout-code-behind

eShopCustom's `frontend.layout`: a Layout derives `LayoutComponentBase`, here in its code-behind rather than with `@inherits`. The find's `MainLayout.razor*` matches both files; they are one component unit (R37), so the find is one place rather than ambiguous, and the read case finds the base class in the `.razor.cs` part.

```json case
{ "phase": 6, "base": "verified-page", "without": ["src/WebApp/Components/Pages/Cart/CartPage.razor"], "kinds": ["component.kind"], "traces": ["ESC-FE-10"], "requirements": ["R2", "R37"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp",
        "elements": [
          { "id": "layout", "type": "Folder", "name": "Layout",
            "children": [
              { "id": "main-layout", "type": "Layout", "name": "MainLayout" }
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

```razor file src/WebApp/Components/Layout/MainLayout.razor
<div class="page">
    <main>
        @Body
    </main>
</div>
```

```csharp file src/WebApp/Components/Layout/MainLayout.razor.cs
using Microsoft.AspNetCore.Components;

namespace eShop.WebApp.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
}
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "main-layout", "outcome": "verified",
      "expected": "Layout", "actual": "Layout", "at": "src/WebApp/Components/Layout/MainLayout.razor.cs:5" }
  ]
}
```

## contradicted-layout-not-derived

The model has `MainLayout` as a Layout, and neither of its files derives `LayoutComponentBase`: the code-behind derives `ComponentBase`. From silence, it is a plain component.

```json case
{ "phase": 6, "base": "verified-layout-code-behind", "kinds": ["component.kind"], "traces": ["ESC-FE-10"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/WebApp/Components/Layout/MainLayout.razor.cs
using Microsoft.AspNetCore.Components;

namespace eShop.WebApp.Components.Layout;

public partial class MainLayout : ComponentBase
{
}
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "main-layout", "outcome": "contradicted",
      "expected": "Layout", "actual": "Component", "fromSilence": true }
  ]
}
```

## contradicted-layout-not-modelled

The other direction of the same check: the code derives `LayoutComponentBase`, and the model has `MainLayout` as a Component, not a Layout.

```json case
{ "phase": 6, "base": "verified-layout-code-behind", "kinds": ["component.kind"], "traces": ["ESC-FE-10"], "requirements": ["R2"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp",
        "elements": [
          { "id": "layout", "type": "Folder", "name": "Layout",
            "children": [
              { "id": "main-layout", "type": "Component", "name": "MainLayout" }
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
    { "kind": "component.kind", "element": "main-layout", "outcome": "contradicted",
      "expected": "Component", "actual": "Layout", "at": "src/WebApp/Components/Layout/MainLayout.razor.cs:5" }
  ]
}
```

## cant-verify-file-missing

eShopCustom's `component.missing`: the model has a Checkout page with no `Checkout.razor` in scope. The `intent.ui` pack claims each component's type, and its kind waits on it, so the missing component is one finding, its `type.exists`, and the kind it blocks has none (one mismatch, one finding).

```json case
{ "phase": 6, "base": "verified-page", "kinds": ["component.kind"], "cases": ["engine/blocked"], "traces": ["ESC-FE-08"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
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
              { "id": "cart-page", "type": "Component", "name": "CartPage",
                "stereotypes": { "Page": { "Route": "/cart" } } },
              { "id": "checkout-page", "type": "Component", "name": "Checkout",
                "stereotypes": { "Page": { "Route": "/checkout" } } }
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
    { "kind": "component.kind", "element": "checkout-page", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" },
    { "kind": "component.kind", "element": "cart-page", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "checkout-page" },
    { "rule": "claim/component.kind", "subject": "checkout-page", "count": 0, "note": "A blocked claim has no finding of its own" }
  ]
}
```
