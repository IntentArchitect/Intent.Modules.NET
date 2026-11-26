# entry.label

How a placed element is presented where it is placed: the User Interface designer's `Entry` stereotype on a Composition's target end (Intent.Modelers.UI 2.0.0) gives the label, icon and keyboard shortcut of the tab, activity bar item, side-panel switcher or menu entry presenting it, and the key the code opens or selects it by. The designer applies Entry "when the placed element is presented as a tab ... whose label differs from its name, or that has an icon, shortcut or key; set only the fields that apply". `intent.ui` claims each field it sets as a `text` kind, `stated: whenSet`, waiting on the Composition's `intent.uses`: `entry.label`, [entry.icon](entry.icon.cases.md), [entry.key](entry.key.cases.md) and [entry.shortcut](entry.shortcut.cases.md).

The kinds name no framework. .NET MAUI (`intent.maui`) reads them from a Shell: a Region of a Shell Layout is the Shell item it names, a Region's Composition of a page is a `ShellContent` of that item showing it, and its entry is the tab or flyout item presenting it: Label is the content's `Title`, else that of the `Tab` or item holding it alone.

## maui-shell-tab

eShop's ClientApp shell: a tab bar whose catalog tab is titled `CATALOG`, which the model states in its Composition's Entry Label. The profile tab's Composition has no Entry, so it claims no entry kind.

```json case
{ "phase": 6, "kinds": ["entry.label"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "AppShell", "children": [ { "id": "main", "type": "Region", "name": "Main" } ] },
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { } } },
          { "id": "profile", "type": "Page", "name": "ProfileView", "stereotypes": { "Page": { } } }
        ]
      }],
      "associations": [
        { "id": "main-catalog", "type": "Composition", "source": { "element": "main" }, "target": { "element": "catalog", "stereotypes": { "Entry": { "Label": "CATALOG" } } } },
        { "id": "main-profile", "type": "Composition", "source": { "element": "main" }, "target": { "element": "profile" } }
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
        <ShellContent Title="CATALOG" ContentTemplate="{DataTemplate views:CatalogView}" Route="Catalog" />
        <ShellContent Title="PROFILE" ContentTemplate="{DataTemplate views:ProfileView}" Route="Profile" />
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

```xml file src/Shop/Views/ProfileView.xaml
<ContentPage x:Class="Shop.Views.ProfileView" xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <Label Text="Profile" />
</ContentPage>
```

```csharp file src/Shop/Views/ProfileView.xaml.cs
namespace Shop.Views;

public partial class ProfileView : ContentPage
{
    public ProfileView() => InitializeComponent();
}
```

```json expect
{
  "claims": [
    { "kind": "entry.label", "element": "main-catalog", "outcome": "verified", "expected": "CATALOG", "actual": "CATALOG", "at": "src/Shop/AppShell.xaml:6" },
    { "kind": "intent.uses", "element": "main-catalog", "outcome": "verified", "at": "src/Shop/AppShell.xaml:6" },
    { "kind": "entry.label", "element": "main-profile", "absent": true }
  ],
  "others": "verified",
  "noOtherFindings": true
}
```

## maui-tab-title-changed

A planted drift: the catalog tab is retitled in the code. One finding, the Entry Label's, naming the `ShellContent`'s Title.

```json case
{ "phase": 6, "base": "maui-shell-tab", "kinds": ["entry.label"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```xml file src/Shop/AppShell.xaml
<Shell x:Class="Shop.AppShell"
       xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:Shop.Views">
    <TabBar Route="Main">
        <ShellContent Title="SHOP" ContentTemplate="{DataTemplate views:CatalogView}" Route="Catalog" />
        <ShellContent Title="PROFILE" ContentTemplate="{DataTemplate views:ProfileView}" Route="Profile" />
    </TabBar>
</Shell>
```

```json expect
{
  "claims": [
    { "kind": "entry.label", "element": "main-catalog", "outcome": "contradicted", "expected": "CATALOG", "actual": "SHOP" }
  ],
  "findings": [
    { "rule": "claim/entry.label", "severity": "error", "subject": "main-catalog", "messageContains": "`Main`'s entry for `CatalogView` Label: model `CATALOG`, code `SHOP`. The `ShellContent` showing it has `Title=\"SHOP\"`." }
  ],
  "noOtherFindings": true
}
```

## maui-title-from-its-tab

A `Tab` holding only the catalog's content presents it, so the tab's Title is its label.

```json case
{ "phase": 6, "base": "maui-shell-tab", "kinds": ["entry.label"], "requirements": ["R2"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```xml file src/Shop/AppShell.xaml
<Shell x:Class="Shop.AppShell"
       xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:Shop.Views">
    <TabBar Route="Main">
        <Tab Title="CATALOG">
            <ShellContent ContentTemplate="{DataTemplate views:CatalogView}" Route="Catalog" />
        </Tab>
        <ShellContent Title="PROFILE" ContentTemplate="{DataTemplate views:ProfileView}" Route="Profile" />
    </TabBar>
</Shell>
```

```json expect
{
  "claims": [
    { "kind": "entry.label", "element": "main-catalog", "outcome": "verified", "expected": "CATALOG", "actual": "CATALOG", "at": "src/Shop/AppShell.xaml:7" }
  ],
  "noOtherFindings": true
}
```

## maui-label-the-model-leaves-unset

With the opt-in rule for unmodelled values on, the profile tab's title and route, which differ from its name, are what an Entry would state, and the model's Composition has none: two findings. A tab titled with its page's own name needs no Entry Label, so the catalog tab's `CatalogView` title is none. The pages' Shell routes are modelled, so nothing else is asked for.

```json case
{ "phase": 6, "base": "maui-shell-tab", "kinds": ["entry.label"], "cases": ["optin/unmodelled.value"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "AppShell", "children": [ { "id": "main", "type": "Region", "name": "Main" } ] },
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { "Route": "/Main" } } },
          { "id": "profile", "type": "Page", "name": "ProfileView", "stereotypes": { "Page": { "Route": "/Main/Profile" } } }
        ]
      }],
      "associations": [
        { "id": "main-catalog", "type": "Composition", "source": { "element": "main" }, "target": { "element": "catalog" } },
        { "id": "main-profile", "type": "Composition", "source": { "element": "main" }, "target": { "element": "profile" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }
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
    <TabBar Route="Main">
        <ShellContent Title="CatalogView" ContentTemplate="{DataTemplate views:CatalogView}" />
        <ShellContent Title="PROFILE" ContentTemplate="{DataTemplate views:ProfileView}" Route="Profile" />
    </TabBar>
</Shell>
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.value", "severity": "error", "messageContains": "entry label: the code states `PROFILE`" },
    { "rule": "optin/unmodelled.value", "severity": "error", "messageContains": "entry key: the code states `Profile`" }
  ],
  "noOtherFindings": true
}
```
