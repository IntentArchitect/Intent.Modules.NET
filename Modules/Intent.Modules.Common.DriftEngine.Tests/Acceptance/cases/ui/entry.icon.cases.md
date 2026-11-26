# entry.icon

The icon of the tab, activity bar item, side-panel switcher or menu entry presenting a placed element: the `Entry` stereotype's Icon on a Composition's target end (see [entry.label](entry.label.cases.md)), a `text` kind, `stated: whenSet`, waiting on the Composition's `intent.uses`.

.NET MAUI (`intent.maui`) reads it from the `ShellContent` a Shell Region's Composition shows, else from the `Tab` or item holding it alone: a `{StaticResource X}` or `{DynamicResource X}` icon is its resource's key `X`, a file name is itself.

## maui-static-resource

The catalog tab's icon is the `CatalogIcon` resource, as the model's Entry Icon says. The profile tab has an icon too, and its Composition no Entry: it states nothing.

```json case
{ "phase": 6, "kinds": ["entry.icon"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
        { "id": "main-catalog", "type": "Composition", "source": { "element": "main" }, "target": { "element": "catalog", "stereotypes": { "Entry": { "Icon": "CatalogIcon" } } } },
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
        <ShellContent Icon="{StaticResource CatalogIcon}" ContentTemplate="{DataTemplate views:CatalogView}" />
        <ShellContent Icon="profile.png" ContentTemplate="{DataTemplate views:ProfileView}" />
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
    { "kind": "entry.icon", "element": "main-catalog", "outcome": "verified", "expected": "CatalogIcon", "actual": "CatalogIcon", "at": "src/Shop/AppShell.xaml:6" },
    { "kind": "entry.icon", "element": "main-profile", "absent": true }
  ],
  "others": "verified",
  "noOtherFindings": true
}
```

## maui-icon-changed

A planted drift: the catalog tab shows another resource. One finding, the Entry Icon's.

```json case
{ "phase": 6, "base": "maui-static-resource", "kinds": ["entry.icon"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```xml file src/Shop/AppShell.xaml
<Shell x:Class="Shop.AppShell"
       xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:Shop.Views">
    <TabBar Route="Main">
        <ShellContent Icon="{StaticResource ShopIcon}" ContentTemplate="{DataTemplate views:CatalogView}" />
        <ShellContent Icon="profile.png" ContentTemplate="{DataTemplate views:ProfileView}" />
    </TabBar>
</Shell>
```

```json expect
{
  "claims": [
    { "kind": "entry.icon", "element": "main-catalog", "outcome": "contradicted", "expected": "CatalogIcon", "actual": "ShopIcon" }
  ],
  "findings": [
    { "rule": "claim/entry.icon", "severity": "error", "subject": "main-catalog", "messageContains": "`Main`'s entry for `CatalogView` Icon: model `CatalogIcon`, code `ShopIcon`." }
  ],
  "noOtherFindings": true
}
```

## maui-icon-removed

The catalog tab has no icon in the code: one finding, its value none.

```json case
{ "phase": 6, "base": "maui-static-resource", "kinds": ["entry.icon"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```xml file src/Shop/AppShell.xaml
<Shell x:Class="Shop.AppShell"
       xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:Shop.Views">
    <TabBar Route="Main">
        <ShellContent ContentTemplate="{DataTemplate views:CatalogView}" />
        <ShellContent Icon="profile.png" ContentTemplate="{DataTemplate views:ProfileView}" />
    </TabBar>
</Shell>
```

```json expect
{
  "claims": [
    { "kind": "entry.icon", "element": "main-catalog", "outcome": "contradicted", "expected": "CatalogIcon", "actual": "none" }
  ],
  "findings": [
    { "rule": "claim/entry.icon", "severity": "error", "subject": "main-catalog", "messageContains": "The `ShellContent` showing it has no `Icon`." }
  ],
  "noOtherFindings": true
}
```
