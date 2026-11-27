# region.outlet

A Region is a named placement point of a Layout, Page, Dialog or Component ("Main", "Side Panel", "Toolbar") where content others supply is composed (Intent.Modelers.UI 2.0.0). The claim sits on the Region, valued with its name, and waits on its parent's `type.exists`. It is a `text` kind, `stated: always`: the code names the placement point or doesn't.

The kind names no framework. Blazor (`intent.blazor`) reads it from the parent's Razor markup: a `<SectionOutlet SectionName="...">` of the Region's name, which pages fill with `<SectionContent>`. A layout's `@Body` is its one placement point the code doesn't name, so it is the layout's Region no outlet names when only one is, or only one of those places a Page (the designer places a Page in a Layout through a Region's Composition). A component that isn't a Razor component is left to its own framework's reader. The code-to-model side is the opt-in rule [unmodelled.region](../optin/unmodelled.region.cases.md).

## verified

eShop's header bar has two section outlets, which each page fills with its title and subtitle, and the model gives the `HeaderBar` component a Region for each.

```json case
{ "phase": 6, "kinds": ["region.outlet"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "header-bar", "type": "Component", "name": "HeaderBar", "children": [
            { "id": "header-title", "type": "Region", "name": "page-header-title" },
            { "id": "header-subtitle", "type": "Region", "name": "page-header-subtitle" }
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

```razor file src/WebApp/Components/Layout/HeaderBar.razor
<header class="eshop-topbar">
    <h1><SectionOutlet SectionName="page-header-title" /></h1>
    <p><SectionOutlet SectionName="page-header-subtitle" /></p>
</header>
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "header-title", "outcome": "verified", "expected": "page-header-title", "actual": "page-header-title", "at": "src/WebApp/Components/Layout/HeaderBar.razor:2" },
    { "kind": "region.outlet", "element": "header-subtitle", "outcome": "verified", "expected": "page-header-subtitle", "actual": "page-header-subtitle", "at": "src/WebApp/Components/Layout/HeaderBar.razor:3" }
  ],
  "noOtherFindings": true
}
```

## render-fragment-parameters

A component's `RenderFragment` parameters are its named placement points a parent fills with content, which the designer's Regions are ("a named placement point ... where content is composed"), so a card with a header and a body to fill is modelled with a Region for each. `RenderFragment<T>` counts too.

```json case
{ "phase": 6, "kinds": ["region.outlet"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "card", "type": "Component", "name": "Card", "children": [
            { "id": "card-header", "type": "Region", "name": "Header" },
            { "id": "card-body", "type": "Region", "name": "Body" }
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

```razor file src/WebApp/Components/Card.razor
<div class="card">
    <div class="card-header">@Header</div>
    <div class="card-body">@Body(Item)</div>
</div>

@code {
    [Parameter] public RenderFragment? Header { get; set; }

    [Parameter] public RenderFragment<string>? Body { get; set; }

    [Parameter] public string Item { get; set; } = "";
}
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "card-header", "outcome": "verified", "expected": "Header", "actual": "Header", "at": "src/WebApp/Components/Card.razor:7" },
    { "kind": "region.outlet", "element": "card-body", "outcome": "verified", "expected": "Body", "actual": "Body", "at": "src/WebApp/Components/Card.razor:9" }
  ],
  "noOtherFindings": true
}
```

## render-fragment-renamed

A planted drift: the header's `RenderFragment` is renamed `Title`. Its Region is one finding, naming the placement point it was renamed to; the body's Region still verifies.

```json case
{ "phase": 6, "base": "render-fragment-parameters", "kinds": ["region.outlet"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Card.razor
<div class="card">
    <div class="card-header">@Title</div>
    <div class="card-body">@Body(Item)</div>
</div>

@code {
    [Parameter] public RenderFragment? Title { get; set; }

    [Parameter] public RenderFragment<string>? Body { get; set; }

    [Parameter] public string Item { get; set; } = "";
}
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "card-header", "outcome": "contradicted", "expected": "Header", "actual": "Title", "at": "src/WebApp/Components/Card.razor:7" },
    { "kind": "region.outlet", "element": "card-body", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "severity": "error", "subject": "card-header", "messageContains": "`Card` has the `RenderFragment` parameters `Title`, `Body`" }
  ],
  "noOtherFindings": true
}
```

## contradicted-outlet-removed

A planted drift: the subtitle's outlet is gone from the header bar. Its Region is one finding, naming the outlets the component has; the title's Region still verifies.

```json case
{ "phase": 6, "base": "verified", "kinds": ["region.outlet"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Layout/HeaderBar.razor
<header class="eshop-topbar">
    <h1><SectionOutlet SectionName="page-header-title" /></h1>
    <p></p>
</header>
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "header-title", "outcome": "verified" },
    { "kind": "region.outlet", "element": "header-subtitle", "outcome": "contradicted", "expected": "page-header-subtitle", "actual": "none" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "severity": "error", "subject": "header-subtitle", "messageContains": "`HeaderBar` region `page-header-subtitle`: model `page-header-subtitle`, code none. `HeaderBar` has the outlets `page-header-title`" }
  ],
  "noOtherFindings": true
}
```

## contradicted-outlet-renamed

The subtitle's outlet is renamed in the code. The one outlet no Region names is what the Region's name was changed from, so the finding says the code's name.

```json case
{ "phase": 6, "base": "verified", "kinds": ["region.outlet"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Layout/HeaderBar.razor
<header class="eshop-topbar">
    <h1><SectionOutlet SectionName="page-header-title" /></h1>
    <p><SectionOutlet SectionName="page-header-tagline" /></p>
</header>
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "header-subtitle", "outcome": "contradicted", "expected": "page-header-subtitle", "actual": "page-header-tagline", "at": "src/WebApp/Components/Layout/HeaderBar.razor:3" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "severity": "error", "subject": "header-subtitle" }
  ],
  "noOtherFindings": true
}
```

## layout-body

A layout following the designer gives its body a Region too ("Main"), and places its pages there through the Region's Composition; a toolbar outlet beside it is a second Region. The layout's `@Body` names no placement point, so `Main` is the one Region no outlet names, read at the element holding `@Body`.

```json case
{ "phase": 6, "kinds": ["region.outlet"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "main-layout", "type": "Layout", "name": "MainLayout", "children": [
            { "id": "main-region", "type": "Region", "name": "Main" },
            { "id": "toolbar-region", "type": "Region", "name": "toolbar" }
          ] },
          { "id": "catalog-page", "type": "Page", "name": "Catalog", "stereotypes": { "Page": { "Route": "/" } } }
        ]
      }],
      "associations": [
        { "id": "main-shows-catalog", "type": "Composition", "source": { "element": "main-region" }, "target": { "element": "catalog-page" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebApp/Components/Layout/MainLayout.razor
@inherits LayoutComponentBase

<div class="toolbar"><SectionOutlet SectionName="toolbar" /></div>
<main class="eshop-main">
    @Body
</main>
```

```razor file src/WebApp/Components/Pages/Catalog.razor
@page "/"

<PageTitle>Catalog</PageTitle>
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "main-region", "outcome": "verified", "expected": "Main", "at": "src/WebApp/Components/Layout/MainLayout.razor:4" },
    { "kind": "region.outlet", "element": "toolbar-region", "outcome": "verified", "expected": "toolbar", "at": "src/WebApp/Components/Layout/MainLayout.razor:3" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "count": 0 }
  ]
}
```

## layout-body-outlet-removed

A planted drift: the toolbar's outlet is gone. Two Regions now name no outlet, and the layout's `@Body` is the one that places a page, so `Main` still verifies and the toolbar is one finding.

```json case
{ "phase": 6, "base": "layout-body", "kinds": ["region.outlet"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Layout/MainLayout.razor
@inherits LayoutComponentBase

<div class="toolbar"></div>
<main class="eshop-main">
    @Body
</main>
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "main-region", "outcome": "verified", "at": "src/WebApp/Components/Layout/MainLayout.razor:4" },
    { "kind": "region.outlet", "element": "toolbar-region", "outcome": "contradicted", "expected": "toolbar", "actual": "none" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "severity": "error", "subject": "toolbar-region" },
    { "rule": "claim/region.outlet", "subject": "main-region", "count": 0 }
  ]
}
```

## component-no-body

A component that isn't a layout has no `@Body`, so a Region its markup doesn't name is contradicted even when it is the component's only one.

```json case
{ "phase": 6, "base": "verified", "kinds": ["region.outlet"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "layout", "type": "Folder", "name": "Layout", "children": [
            { "id": "header-bar", "type": "Component", "name": "HeaderBar", "children": [
              { "id": "header-main", "type": "Region", "name": "Main" }
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

```razor file src/WebApp/Components/Layout/HeaderBar.razor
<header class="eshop-topbar">
    <h1>AdventureWorks</h1>
</header>
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "header-main", "outcome": "contradicted", "expected": "Main", "actual": "none" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "severity": "error", "subject": "header-main", "messageContains": "has no `<SectionOutlet>`" }
  ]
}
```

## not-a-razor-component

A Region of a Vue component: the Blazor reader answers only for a Razor component, so it leaves the claim to the Vue reader, which reads the component's `<slot name="actions">`.

```json case
{ "phase": 6, "kinds": ["region.outlet"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "components", "type": "Folder", "name": "components", "children": [
            { "id": "page-shell", "type": "Component", "name": "PageShell", "children": [
              { "id": "page-shell-actions", "type": "Region", "name": "actions" }
            ] }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```vue file src/components/PageShell.vue
<template>
  <section>
    <slot name="actions" />
  </section>
</template>
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "page-shell-actions", "outcome": "verified", "actual": "actions", "at": "src/components/PageShell.vue:3" }
  ]
}
```

## vue-slots

A Vue component's placement points are its `<slot>`s: a named one is the Region of its name, and the bare `<slot />`, its default, is the Region no slot names. `Card` has a default slot and a `header`. `Panel`'s only slot is `footer`, which the model's `Actions` Region doesn't name, and no other Region does, so it was renamed from `footer`.

```json case
{ "phase": "later", "kinds": ["region.outlet"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "card", "type": "Component", "name": "Card", "children": [
            { "id": "card-body", "type": "Region", "name": "Body" },
            { "id": "card-header", "type": "Region", "name": "Header" }
          ] },
          { "id": "panel", "type": "Component", "name": "Panel", "children": [
            { "id": "panel-actions", "type": "Region", "name": "Actions" }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```vue file src/Card.vue
<template>
  <article>
    <header><slot name="header" /></header>
    <slot />
  </article>
</template>
```

```vue file src/Panel.vue
<template>
  <aside>
    <footer><slot name="footer" /></footer>
  </aside>
</template>
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "card-body", "outcome": "verified", "at": "src/Card.vue:4" },
    { "kind": "region.outlet", "element": "card-header", "outcome": "verified", "at": "src/Card.vue:3" },
    { "kind": "region.outlet", "element": "panel-actions", "outcome": "contradicted", "actual": "footer", "at": "src/Panel.vue:3" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "severity": "error", "subject": "panel-actions" }
  ]
}
```

## angular-content-projection

An Angular component's placement points are its template's `<ng-content>`s, each named by what its `select` names (an attribute's value, the attribute, a class or a tag), and its `<router-outlet>`s, by their `name`. A bare `<ng-content />` is the Region no placement point names. The modal, an inline template, has a default slot and a `[slot=footer]` one; the card, a `templateUrl` template, a `.card-title` one and a bare one. The panel's only one is `[actions]`, which its `Buttons` Region doesn't name.

```json case
{ "phase": "later", "kinds": ["region.outlet"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "app", "name": "App", "type": "User Interface Package",
        "elements": [
          { "id": "modal", "type": "Dialog", "name": "Modal", "children": [
            { "id": "modal-default", "type": "Region", "name": "default" },
            { "id": "modal-footer", "type": "Region", "name": "footer" }
          ] },
          { "id": "card", "type": "Component", "name": "Card", "children": [
            { "id": "card-title", "type": "Region", "name": "CardTitle" },
            { "id": "card-content", "type": "Region", "name": "Content" }
          ] },
          { "id": "panel", "type": "Component", "name": "Panel", "children": [
            { "id": "panel-buttons", "type": "Region", "name": "Buttons" }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "app", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```typescript file src/modal.ts
import { Component, input } from '@angular/core';

@Component({
    selector: 'app-modal',
    template: `
        @if (isOpen()) {
            <div role="dialog">
                <ng-content />
                <footer><ng-content select="[slot=footer]" /></footer>
            </div>
        }
    `,
})
export class Modal {
    readonly isOpen = input.required<boolean>();
}
```

```typescript file src/card.ts
import { Component } from '@angular/core';

@Component({ selector: 'app-card', templateUrl: './card.html' })
export class Card {}
```

```html file src/card.html
<section class="card">
  <h2><ng-content select=".card-title" /></h2>
  <ng-content />
</section>
```

```typescript file src/panel.ts
import { Component } from '@angular/core';

@Component({ selector: 'app-panel', template: `<aside><ng-content select="[actions]" /></aside>` })
export class Panel {}
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "modal-default", "outcome": "verified", "at": "src/modal.ts:8" },
    { "kind": "region.outlet", "element": "modal-footer", "outcome": "verified", "at": "src/modal.ts:9" },
    { "kind": "region.outlet", "element": "card-title", "outcome": "verified", "at": "src/card.html:2" },
    { "kind": "region.outlet", "element": "card-content", "outcome": "verified", "at": "src/card.html:3" },
    { "kind": "region.outlet", "element": "panel-buttons", "outcome": "contradicted", "actual": "actions", "at": "src/panel.ts:3" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "severity": "error", "subject": "panel-buttons" }
  ]
}
```

## angular-content-projection-renamed

A planted drift: the modal's footer slot is renamed `[slot=actions]`. Now two Regions, `default` and `footer`, name no placement point, and nothing says which of them is the bare `<ng-content />`, so neither can be verified: each names the two placement points it could be, rather than one being read as renamed and the other as the bare one.

```json case
{ "phase": "later", "base": "angular-content-projection", "kinds": ["region.outlet"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```typescript file src/modal.ts
import { Component, input } from '@angular/core';

@Component({
    selector: 'app-modal',
    template: `
        @if (isOpen()) {
            <div role="dialog">
                <ng-content />
                <footer><ng-content select="[slot=actions]" /></footer>
            </div>
        }
    `,
})
export class Modal {
    readonly isOpen = input.required<boolean>();
}
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "modal-default", "outcome": "cantVerify", "reason": "ambiguous" },
    { "kind": "region.outlet", "element": "modal-footer", "outcome": "cantVerify", "reason": "ambiguous" },
    { "kind": "region.outlet", "element": "card-content", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "subject": "modal-default", "messageContains": "at src/modal.ts:8, src/modal.ts:9" },
    { "rule": "claim/region.outlet", "subject": "modal-footer", "messageContains": "at src/modal.ts:8, src/modal.ts:9" }
  ]
}
```

## mvc-layout-body-and-section

An MVC layout's placement points are its `RenderBody()`, where the page it lays out is shown, and the sections pages fill (`RenderSection("scripts")`, `RenderSectionAsync`), read by the MVC reader (`intent.mvc`). Following the designer, the layout has a Region for its body (`Main`), placing its pages through the Region's Composition, and one for the section the page fills (`@section scripts`): the section's is the one `RenderSection` names, and the body's the Region no section names. Here the page is a Razor Page, laid out by `_ViewStart.cshtml`'s layout, as an MVC view is.

```json case
{ "phase": 6, "kinds": ["region.outlet"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Identity.API", "type": "User Interface Package",
        "elements": [
          { "id": "layout", "type": "Layout", "name": "_Layout", "children": [
            { "id": "main", "type": "Region", "name": "Main" },
            { "id": "scripts", "type": "Region", "name": "scripts" }
          ] },
          { "id": "index", "type": "Page", "name": "Index", "stereotypes": { "Page": { "Route": "/" } } }
        ]
      }],
      "associations": [
        { "id": "main-shows-index", "type": "Composition", "source": { "element": "main" }, "target": { "element": "index" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```xml file src/Identity.API/Identity.API.csproj
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
```

```cshtml file src/Identity.API/Pages/_ViewStart.cshtml
@{
    Layout = "_Layout";
}
```

```cshtml file src/Identity.API/Pages/Shared/_Layout.cshtml
<!DOCTYPE html>
<html lang="en">
<body>
    <div class="container body-container">@RenderBody()</div>
    @await RenderSectionAsync("scripts", required: false)
</body>
</html>
```

```cshtml file src/Identity.API/Pages/Index.cshtml
@page
<h1>Welcome to Identity</h1>
@section scripts {
    <script src="~/js/welcome.js"></script>
}
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "main", "outcome": "verified", "expected": "Main", "at": "src/Identity.API/Pages/Shared/_Layout.cshtml:4" },
    { "kind": "region.outlet", "element": "scripts", "outcome": "verified", "expected": "scripts", "at": "src/Identity.API/Pages/Shared/_Layout.cshtml:5" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "count": 0 }
  ]
}
```

## mvc-section-renamed

A planted drift: the layout's section is now `footer`. The section's Region is reported once, with the section it was renamed to; the body's Region is still the one no section names, as the page fills a section named for the other.

```json case
{ "phase": 6, "base": "mvc-layout-body-and-section", "kinds": ["region.outlet"], "requirements": ["R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```cshtml file src/Identity.API/Pages/Shared/_Layout.cshtml
<!DOCTYPE html>
<html lang="en">
<body>
    <div class="container body-container">@RenderBody()</div>
    @await RenderSectionAsync("footer", required: false)
</body>
</html>
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "main", "outcome": "verified" },
    { "kind": "region.outlet", "element": "scripts", "outcome": "contradicted", "expected": "scripts", "actual": "footer", "at": "src/Identity.API/Pages/Shared/_Layout.cshtml:5" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "severity": "error", "subject": "scripts", "messageContains": "`_Layout` region `scripts`: model `scripts`, code `footer`." }
  ],
  "noOtherFindings": true
}
```

## mvc-section-of-another-view-states-nothing

What another view renders isn't this one's: the `scripts` section is rendered by an admin layout, not by `_Layout`, which renders only its body (and the page no longer fills it). `_Layout`'s `scripts` Region is contradicted, and its `Main`, the one Region placing a page, is still its body.

```json case
{ "phase": 6, "base": "mvc-layout-body-and-section", "kinds": ["region.outlet"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```cshtml file src/Identity.API/Pages/Shared/_Layout.cshtml
<!DOCTYPE html>
<html lang="en">
<body>
    <div class="container body-container">@RenderBody()</div>
</body>
</html>
```

```cshtml file src/Identity.API/Pages/Shared/_AdminLayout.cshtml
<main>@RenderBody()</main>
@await RenderSectionAsync("scripts", required: false)
```

```cshtml file src/Identity.API/Pages/Index.cshtml
@page
<h1>Welcome to Identity</h1>
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "main", "outcome": "verified", "at": "src/Identity.API/Pages/Shared/_Layout.cshtml:4" },
    { "kind": "region.outlet", "element": "scripts", "outcome": "contradicted", "expected": "scripts", "actual": "none" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "severity": "error", "subject": "scripts" }
  ],
  "noOtherFindings": true
}
```

## maui-shell-items

A .NET MAUI Shell's named placement points are its items (`intent.maui`): a Region of a Shell Layout is the item whose Route is its name, or, for an item with no Route, whose only content's Route is (eShop's ClientApp reaches its login through `//Login` that way). A Region's Composition of a page is a content of that item showing it.

```json case
{ "phase": 6, "kinds": ["region.outlet"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "AppShell", "children": [
            { "id": "login-region", "type": "Region", "name": "Login" },
            { "id": "main-region", "type": "Region", "name": "Main" }
          ] },
          { "id": "login", "type": "Page", "name": "LoginView", "stereotypes": { "Page": { } } },
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { } } }
        ]
      }],
      "associations": [
        { "id": "shows-login", "type": "Composition", "source": { "element": "login-region" }, "target": { "element": "login" } },
        { "id": "shows-catalog", "type": "Composition", "source": { "element": "main-region" }, "target": { "element": "catalog" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```xml file src/Shop/Shop.csproj
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <UseMaui>true</UseMaui>
  </PropertyGroup>
</Project>
```

```xml file src/Shop/AppShell.xaml
<Shell x:Class="Shop.AppShell"
       xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:Shop.Views">
    <FlyoutItem>
        <ShellContent ContentTemplate="{DataTemplate views:LoginView}" Route="Login" />
    </FlyoutItem>
    <TabBar Route="Main">
        <ShellContent ContentTemplate="{DataTemplate views:CatalogView}" />
    </TabBar>
</Shell>
```

```csharp file src/Shop/AppShell.xaml.cs
namespace Shop;

public partial class AppShell : Shell
{
}
```

```csharp file src/Shop/Views/Pages.cs
namespace Shop.Views;

public class LoginView : ContentPage
{
}

public class CatalogView : ContentPage
{
}
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "login-region", "outcome": "verified", "expected": "Login", "actual": "Login", "at": "src/Shop/AppShell.xaml:5" },
    { "kind": "region.outlet", "element": "main-region", "outcome": "verified", "expected": "Main", "actual": "Main", "at": "src/Shop/AppShell.xaml:8" },
    { "kind": "intent.uses", "element": "shows-login", "outcome": "verified", "at": "src/Shop/AppShell.xaml:6" },
    { "kind": "intent.uses", "element": "shows-catalog", "outcome": "verified", "at": "src/Shop/AppShell.xaml:9" }
  ],
  "others": "verified",
  "noOtherFindings": true
}
```

## maui-shell-item-renamed

A planted drift: the tab bar's Route is now `Home`. The one item no Region names is what `Main` was renamed from, so the finding says the code's name, and the Region's Composition is still read through it: one finding.

```json case
{ "phase": 6, "base": "maui-shell-items", "kinds": ["region.outlet"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```xml file src/Shop/AppShell.xaml
<Shell x:Class="Shop.AppShell"
       xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:Shop.Views">
    <FlyoutItem>
        <ShellContent ContentTemplate="{DataTemplate views:LoginView}" Route="Login" />
    </FlyoutItem>
    <TabBar Route="Home">
        <ShellContent ContentTemplate="{DataTemplate views:CatalogView}" />
    </TabBar>
</Shell>
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "main-region", "outcome": "contradicted", "expected": "Main", "actual": "Home", "at": "src/Shop/AppShell.xaml:8" },
    { "kind": "intent.uses", "element": "shows-catalog", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/region.outlet", "severity": "error", "subject": "main-region", "messageContains": "`AppShell` region `Main`: model `Main`, code `Home`. `AppShell` has the items `Login`, `Home`" }
  ],
  "noOtherFindings": true
}
```
