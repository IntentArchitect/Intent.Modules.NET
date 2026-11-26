# entry.shortcut

The keyboard shortcut of the tab, activity bar item, side-panel switcher or menu entry presenting a placed element: the `Entry` stereotype's Shortcut on a Composition's target end (see [entry.label](entry.label.cases.md)), a `text` kind, `stated: whenSet`, waiting on the Composition's `intent.uses`.

.NET MAUI's Shell has no keyboard shortcut for a tab or flyout item, so `intent.maui` reads none: a Shortcut the model sets is contradicted, and an Entry without one claims nothing.

## maui-shell-has-no-shortcut

The model gives the catalog tab a shortcut, which no MAUI Shell can state: one finding. Its label beside it verifies.

```json case
{ "phase": 6, "kinds": ["entry.shortcut"], "requirements": ["R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "AppShell", "children": [ { "id": "main", "type": "Region", "name": "Main" } ] },
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { } } }
        ]
      }],
      "associations": [
        { "id": "main-catalog", "type": "Composition", "source": { "element": "main" }, "target": { "element": "catalog", "stereotypes": { "Entry": { "Label": "CATALOG", "Shortcut": "Ctrl+1" } } } }
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
    <TabBar Route="Main">
        <ShellContent Title="CATALOG" ContentTemplate="{DataTemplate views:CatalogView}" />
    </TabBar>
</Shell>
```

```csharp file src/Shop/AppShell.xaml.cs
namespace Shop;

public partial class AppShell : Shell
{
    public AppShell() => InitializeComponent();
}
```

```xml file src/Shop/Views/CatalogView.xaml
<ContentPage x:Class="Shop.Views.CatalogView" xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <Label Text="Catalog" />
</ContentPage>
```

```csharp file src/Shop/Views/CatalogView.xaml.cs
namespace Shop.Views;

public partial class CatalogView : ContentPage
{
    public CatalogView() => InitializeComponent();
}
```

```json expect
{
  "claims": [
    { "kind": "entry.shortcut", "element": "main-catalog", "outcome": "contradicted", "expected": "Ctrl+1", "actual": "none" },
    { "kind": "entry.label", "element": "main-catalog", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/entry.shortcut", "severity": "error", "subject": "main-catalog", "messageContains": "MAUI Shell has no keyboard shortcut for a tab or flyout item." }
  ],
  "noOtherFindings": true
}
```

## maui-no-shortcut-states-nothing

The same Entry without a Shortcut claims none, and the Shell's code states none: clean.

```json case
{ "phase": 6, "base": "maui-shell-has-no-shortcut", "kinds": ["entry.shortcut"], "requirements": ["R4"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "AppShell", "children": [ { "id": "main", "type": "Region", "name": "Main" } ] },
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { } } }
        ]
      }],
      "associations": [
        { "id": "main-catalog", "type": "Composition", "source": { "element": "main" }, "target": { "element": "catalog", "stereotypes": { "Entry": { "Label": "CATALOG" } } } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "entry.shortcut", "element": "main-catalog", "absent": true },
    { "kind": "entry.label", "element": "main-catalog", "outcome": "verified" }
  ],
  "noOtherFindings": true
}
```
