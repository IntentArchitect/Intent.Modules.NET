# entry.key

The key the code opens or selects a placed element by: the `Entry` stereotype's Key on a Composition's target end (see [entry.label](entry.label.cases.md)), a `text` kind, `stated: whenSet`, waiting on the Composition's `intent.uses`.

.NET MAUI (`intent.maui`) reads it as the Route of the `ShellContent` a Shell Region's Composition shows, else that of the `Tab` or item holding it alone: the name a Shell navigation selects the tab by.

## maui-content-route

The catalog's content is routed `Catalog`, and the model's Entry Key says so. The login page sits in a flyout item routed `Login` that holds only it, so that route is its key. The pages state no route of their own in the model, so only the keys are claimed.

```json case
{ "phase": 6, "kinds": ["entry.key"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
            { "id": "main", "type": "Region", "name": "Main" }
          ] },
          { "id": "login", "type": "Page", "name": "LoginView", "stereotypes": { "Page": { } } },
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { } } }
        ]
      }],
      "associations": [
        { "id": "shows-login", "type": "Composition", "source": { "element": "login-region" }, "target": { "element": "login", "stereotypes": { "Entry": { "Key": "Login" } } } },
        { "id": "main-catalog", "type": "Composition", "source": { "element": "main" }, "target": { "element": "catalog", "stereotypes": { "Entry": { "Key": "Catalog" } } } }
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
    <FlyoutItem Route="Login">
        <ShellContent ContentTemplate="{DataTemplate views:LoginView}" />
    </FlyoutItem>
    <TabBar Route="Main">
        <ShellContent ContentTemplate="{DataTemplate views:CatalogView}" Route="Catalog" />
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

```xml file src/Shop/Views/LoginView.xaml
<ContentPage x:Class="Shop.Views.LoginView" xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <Button Text="Sign in" />
</ContentPage>
```

```csharp file src/Shop/Views/LoginView.xaml.cs
namespace Shop.Views;

public partial class LoginView : ContentPage
{
    public LoginView() => InitializeComponent();
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
    { "kind": "entry.key", "element": "main-catalog", "outcome": "verified", "expected": "Catalog", "actual": "Catalog", "at": "src/Shop/AppShell.xaml:9" },
    { "kind": "entry.key", "element": "shows-login", "outcome": "verified", "expected": "Login", "actual": "Login", "at": "src/Shop/AppShell.xaml:6" }
  ],
  "others": "verified",
  "noOtherFindings": true
}
```

## maui-content-rerouted

A planted drift: the catalog's content is routed `Products`. One finding, the Entry Key's; no page states a route that it changes.

```json case
{ "phase": 6, "base": "maui-content-route", "kinds": ["entry.key"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```xml file src/Shop/AppShell.xaml
<Shell x:Class="Shop.AppShell"
       xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:Shop.Views">
    <FlyoutItem Route="Login">
        <ShellContent ContentTemplate="{DataTemplate views:LoginView}" />
    </FlyoutItem>
    <TabBar Route="Main">
        <ShellContent ContentTemplate="{DataTemplate views:CatalogView}" Route="Products" />
    </TabBar>
</Shell>
```

```json expect
{
  "claims": [
    { "kind": "entry.key", "element": "main-catalog", "outcome": "contradicted", "expected": "Catalog", "actual": "Products" }
  ],
  "findings": [
    { "rule": "claim/entry.key", "severity": "error", "subject": "main-catalog", "messageContains": "`Main`'s entry for `CatalogView` Key: model `Catalog`, code `Products`. The `ShellContent` showing it has `Route=\"Products\"`." }
  ],
  "noOtherFindings": true
}
```
