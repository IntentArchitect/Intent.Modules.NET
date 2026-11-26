# optin/unmodelled.region

The opt-in rule that reports a named placement point of a component the model describes that no Region of its element names: the code-to-model side of [region.outlet](../ui/region.outlet.cases.md). The Blazor stack reports a `<SectionOutlet SectionName="...">` whose name no `region.outlet` claim on a child of the component's element states. A component no element describes is [unmodelled.type](unmodelled.type.cases.md)'s, and a layout's `@Body`, which the code doesn't name, is never asked for: the designer adds a Region for a named placement point. By principle 1 it is off until the Root Folder's `Drift Check` lists it, it reads only in-scope code, and each finding can be excepted with a reason (R49).

## on

The model gives eShop's header bar a Region for the page title, not for the subtitle outlet beside it: the subtitle is reported, at its outlet, and the title isn't.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.region"], "kinds": ["region.outlet"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "header-bar", "type": "Component", "name": "HeaderBar", "children": [
            { "id": "header-title", "type": "Region", "name": "page-header-title" }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.region" } ] }
        } }]
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
    { "kind": "region.outlet", "element": "header-title", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.region", "severity": "error", "subject": "src/WebApp/Components/Layout/HeaderBar.razor:3", "messageContains": "`<SectionOutlet SectionName=\"page-header-subtitle\"/>` is a named placement point of `HeaderBar`, which the User Interface Component `HeaderBar` describes, but no Region of `HeaderBar` names it." },
    { "rule": "optin/unmodelled.region", "subject": "src/WebApp/Components/Layout/HeaderBar.razor:2", "count": 0, "note": "Modelled as a Region" }
  ],
  "noOtherFindings": true
}
```

## off-by-default

The same model and code with no `Opt-in Rules` row: nothing is reported.

```json case
{ "phase": 6, "base": "on", "cases": ["optin/unmodelled.region"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "header-bar", "type": "Component", "name": "HeaderBar", "children": [
            { "id": "header-title", "type": "Region", "name": "page-header-title" }
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
  "findings": [
    { "rule": "optin/unmodelled.region", "count": 0 }
  ]
}
```

## content-and-body-are-no-outlets

What isn't a named placement point states nothing: a page filling the header's sections with `<SectionContent>`, and a layout's `@Body`. The model has no Region on either, and nothing is reported.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.region"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "main-layout", "type": "Layout", "name": "MainLayout" },
          { "id": "catalog-page", "type": "Page", "name": "Catalog", "stereotypes": { "Page": { "Route": "/" } } }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.region" } ] }
        } }]
    }
  }
}
```

```razor file src/WebApp/Components/Layout/MainLayout.razor
@inherits LayoutComponentBase

<main class="eshop-main">
    @Body
</main>
```

```razor file src/WebApp/Components/Pages/Catalog.razor
@page "/"

<SectionContent SectionName="page-header-title">Ready for a new adventure?</SectionContent>
<SectionContent SectionName="page-header-subtitle">Start the season with the latest in clothing and equipment.</SectionContent>
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.region", "count": 0 }
  ]
}
```

## render-fragment-not-modelled

A component's `RenderFragment` parameters are named placement points too. The card has two, and the model gives it a Region for the header only, so the body is reported as a Region to add, and neither is asked for as a Property by `unmodelled.member`, whose opt-in row is on too: a Region states a placement point, not a Property.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.region", "optin/unmodelled.member"], "kinds": ["region.outlet"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "card", "type": "Component", "name": "Card", "children": [
            { "id": "card-header", "type": "Region", "name": "Header" }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.region" }, { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```razor file src/WebApp/Components/Card.razor
<div class="card">
    <div class="card-header">@Header</div>
    <div class="card-body">@Body</div>
</div>

@code {
    [Parameter] public RenderFragment? Header { get; set; }

    [Parameter] public RenderFragment? Body { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "card-header", "outcome": "verified", "at": "src/WebApp/Components/Card.razor:7" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.region", "severity": "error", "subject": "src/WebApp/Components/Card.razor:9", "messageContains": "`[Parameter] RenderFragment Body` is a named placement point of `Card`" },
    { "rule": "optin/unmodelled.member", "count": 0 }
  ],
  "noOtherFindings": true
}
```

## one-placement-point-isnt-asked-for

The designer adds Regions "only where an element has more than one placement point". A panel whose only placement point is its `ChildContent` needs no Region, so nothing is reported, by either rule.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.region", "optin/unmodelled.member"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "panel", "type": "Component", "name": "Panel" }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.region" }, { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```razor file src/WebApp/Components/Panel.razor
<section class="panel">@ChildContent</section>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "panel", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.region", "count": 0 },
    { "rule": "optin/unmodelled.member", "count": 0 }
  ],
  "noOtherFindings": true
}
```

## mvc-section-not-modelled

An MVC layout's named placement points are the sections it renders (`RenderSection`, `RenderSectionAsync`), which pages fill with `@section`, and they join this rule, read by the MVC reader (`intent.mvc`). The model gives Identity's layout a Region for its body only: the `scripts` section is reported, at the call that renders it, and the body, which the code doesn't name, isn't.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.region"], "kinds": ["region.outlet"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Identity.API", "type": "User Interface Package",
        "elements": [
          { "id": "layout", "type": "Layout", "name": "_Layout", "children": [
            { "id": "main", "type": "Region", "name": "Main" }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.region" } ] }
        } }]
    }
  }
}
```

```cshtml file src/Identity.API/Views/_ViewStart.cshtml
@{
    Layout = "_Layout";
}
```

```cshtml file src/Identity.API/Views/Shared/_Layout.cshtml
<!DOCTYPE html>
<html lang="en">
<body>
    <div class="container body-container">@RenderBody()</div>
    @await RenderSectionAsync("scripts", required: false)
</body>
</html>
```

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "main", "outcome": "verified", "at": "src/Identity.API/Views/Shared/_Layout.cshtml:4" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.region", "severity": "error", "subject": "src/Identity.API/Views/Shared/_Layout.cshtml:5", "messageContains": "`RenderSection(\"scripts\")` is a named placement point of `_Layout`, which the User Interface Layout `_Layout` describes, but no Region of `_Layout` names it." }
  ],
  "noOtherFindings": true
}
```

## maui-shell-item-not-modelled

A .NET MAUI Shell's named items join this rule (`intent.maui`): the flyout item a login content's Route names is a placement point of the Shell no Region of its Layout names. The tab bar's Region is modelled.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.region"], "kinds": ["region.outlet"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "AppShell", "children": [ { "id": "main-region", "type": "Region", "name": "Main" } ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.region" } ] }
        } }]
    }
  }
}
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

```json expect
{
  "claims": [
    { "kind": "region.outlet", "element": "main-region", "outcome": "verified", "at": "src/Shop/AppShell.xaml:8" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.region", "severity": "error", "subject": "src/Shop/AppShell.xaml:5", "messageContains": "The Shell item `<FlyoutItem>` showing `<ShellContent Route=\"Login\">` is a named placement point of `AppShell`, which the User Interface Layout `AppShell` describes, but no Region of `AppShell` names it." }
  ],
  "noOtherFindings": true
}
```
