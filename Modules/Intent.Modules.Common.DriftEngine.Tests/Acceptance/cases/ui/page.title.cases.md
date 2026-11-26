# page.title

A page's title: the Page stereotype's Title on a User Interface designer Component, against the literal text of its `<PageTitle>`. The kind is `stated: whenSet`, so it is claimed only where the model sets a Title, and compares with `contains`: eShopCustom's titles end with the site's name, which the model leaves out.

The stock User Interface pack gives the kind and its vocabulary, and the Blazor reader reads the page's `<PageTitle>` element's text. A markup node's canonical text keeps one space between words and around punctuation (code-reader.md, Canonical text), so the title reads as written, `Shopping Bag | AdventureWorks`. A `<PageTitle>` computed from data can't be compared with a stated Title; eShopCustom reported that as Wrong, and here it is can't verify, `unreadable`, which is an error too.

## verified

The model's Title `Shopping Bag` is within the code's literal `<PageTitle>`, read as written.

```json case
{ "phase": "later", "kinds": ["page.title"], "traces": ["ESC-FE-13"], "requirements": ["R2", "R10", "R28"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
                "stereotypes": { "Page": { "Route": "/cart", "Title": "Shopping Bag" } } }
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
    { "kind": "page.title", "element": "cart-page", "outcome": "verified",
      "expected": "Shopping Bag", "actual": "Shopping Bag | AdventureWorks", "at": "src/WebApp/Components/Pages/Cart/CartPage.razor:4" }
  ]
}
```

## contradicted

eShopCustom's `page.title`: the model's Title is `Basket`, which the code's `<PageTitle>` doesn't contain.

```json case
{ "phase": "later", "base": "verified", "kinds": ["page.title"], "traces": ["ESC-FE-13"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
                "stereotypes": { "Page": { "Route": "/cart", "Title": "Basket" } } }
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
    { "kind": "page.title", "element": "cart-page", "outcome": "contradicted",
      "expected": "Basket", "at": "src/WebApp/Components/Pages/Cart/CartPage.razor:4" }
  ],
  "findings": [
    { "rule": "claim/page.title", "severity": "error", "subject": "cart-page" }
  ]
}
```

## contradicted-not-stated

The model sets a Title and the page has no `<PageTitle>`. The kind is `whenSet`, so the code's silence contradicts it (R4).

```json case
{ "phase": "later", "base": "verified", "kinds": ["page.title"], "traces": ["ESC-FE-13"], "requirements": ["R4"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@page "/cart"
@inject BasketState Basket

<div class="cart">
    <a href="checkout" class="button button-primary">Check out</a>
</div>
```

```json expect
{
  "claims": [
    { "kind": "page.title", "element": "cart-page", "outcome": "contradicted", "expected": "Shopping Bag", "actual": null }
  ]
}
```

## cant-verify-computed

The item page's title is computed from the item it shows, and the model states a fixed Title, so the claim is can't verify, `unreadable`, at the title's line: a stated Title against a computed one is never passed.

```json case
{ "phase": "later", "base": "verified", "without": ["src/WebApp/Components/Pages/Cart/CartPage.razor"], "kinds": ["page.title"], "cases": ["engine/unreadable"], "traces": ["ESC-FE-13"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
                "stereotypes": { "Page": { "Route": "/item/{itemId}", "Title": "Item" } } }
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
    { "kind": "page.title", "element": "item-page", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/WebApp/Components/Pages/Item/ItemPage.razor:4" }
  ],
  "findings": [
    { "rule": "claim/page.title", "severity": "error", "subject": "item-page", "messageContains": "computed" }
  ]
}
```

## react-title

The standard User Interface designer (2.0.0) read against React by the stock React reader. A React page states its title by rendering a `<title>` (React 19 hoists it into the document's head; react-helmet's `<Helmet>` holds one too), or by setting `document.title` in its code, an effect included. The cart page renders its title, and the orders page sets one other than the model's.

```json case
{ "phase": "later", "kinds": ["page.title"], "requirements": ["R2", "R14", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart", "Title": "Shopping Bag" } } },
          { "id": "orders-page", "type": "Page", "name": "OrdersPage", "stereotypes": { "Page": { "Route": "/orders", "Title": "My Orders" } } }
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

```tsx file src/CartPage.tsx
import { Helmet } from 'react-helmet-async';

export default function CartPage() {
    return (
        <>
            <Helmet><title>Shopping Bag</title></Helmet>
            <h1>Cart</h1>
        </>
    );
}
```

```tsx file src/OrdersPage.tsx
import { useEffect } from 'react';

const TITLE = 'Orders';

export default function OrdersPage() {
    useEffect(() => {
        document.title = TITLE;
    }, []);
    return <h1>Orders</h1>;
}
```

```json expect
{
  "claims": [
    { "kind": "page.title", "element": "cart-page", "outcome": "verified", "actual": "Shopping Bag", "at": "src/CartPage.tsx:6" },
    { "kind": "page.title", "element": "orders-page", "outcome": "contradicted", "expected": "My Orders", "actual": "Orders", "at": "src/OrdersPage.tsx:7" }
  ],
  "findings": [
    { "rule": "claim/page.title", "severity": "error", "subject": "orders-page" }
  ]
}
```

## mvc-view-data-title

An MVC view states its title in its code, for its layout's `<title>` to show: `ViewData["Title"]` or `ViewBag.Title`. The MVC reader (`intent.mvc`) reads it from the view; the layout's `<title>@ViewData["Title"]</title>`, which reads it, states none.

```json case
{ "phase": "later", "kinds": ["page.title"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Identity.API", "type": "User Interface Package",
        "elements": [
          { "id": "logged-out", "type": "Page", "name": "LoggedOut", "stereotypes": { "Page": { "Title": "Signed out" } } },
          { "id": "grants", "type": "Page", "name": "Grants", "stereotypes": { "Page": { "Title": "Client Application Permissions" } } }
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

```csharp file src/Identity.API/Quickstart/Controllers.cs
namespace Identity.API.Quickstart;

public class AccountController : Controller
{
    [HttpPost]
    public IActionResult Logout() => View("LoggedOut");
}

public class GrantsController : Controller
{
    public IActionResult Index() => View("Grants");
}
```

```cshtml file src/Identity.API/Views/Account/LoggedOut.cshtml
@{
    ViewData["Title"] = "Signed out";
}
<h1>Logout <small>You are now logged out</small></h1>
```

```cshtml file src/Identity.API/Views/Grants/Grants.cshtml
@{ ViewBag.Title = "Client Application Permissions"; }
<h1>Client Application Permissions</h1>
```

```cshtml file src/Identity.API/Views/Shared/_Layout.cshtml
<html><head><title>@ViewData["Title"] - Identity</title></head><body>@RenderBody()</body></html>
```

```json expect
{
  "claims": [
    { "kind": "page.title", "element": "logged-out", "outcome": "verified", "actual": "Signed out", "at": "src/Identity.API/Views/Account/LoggedOut.cshtml:2" },
    { "kind": "page.title", "element": "grants", "outcome": "verified", "actual": "Client Application Permissions", "at": "src/Identity.API/Views/Grants/Grants.cshtml:1" }
  ]
}
```

## mvc-title-changed

A planted drift: the view's title now reads `Logged out`. One finding.

```json case
{ "phase": "later", "base": "mvc-view-data-title", "kinds": ["page.title"], "requirements": ["R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```cshtml file src/Identity.API/Views/Account/LoggedOut.cshtml
@{
    ViewData["Title"] = "Logged out";
}
<h1>Logout <small>You are now logged out</small></h1>
```

```json expect
{
  "claims": [
    { "kind": "page.title", "element": "logged-out", "outcome": "contradicted", "expected": "Signed out", "actual": "Logged out", "at": "src/Identity.API/Views/Account/LoggedOut.cshtml:2" }
  ],
  "findings": [
    { "rule": "claim/page.title", "severity": "error", "subject": "logged-out" }
  ],
  "noOtherFindings": true
}
```

## mvc-title-computed

A title the view computes (`ViewData["Title"] = Model.ClientName`) can't be verified, and the layout's own `ViewData["Title"]`, which it only reads, says nothing about the page.

```json case
{ "phase": "later", "base": "mvc-view-data-title", "kinds": ["page.title"], "requirements": ["R2"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```cshtml file src/Identity.API/Views/Account/LoggedOut.cshtml
@{
    ViewData["Title"] = Model.ClientName;
}
<h1>Logout <small>You are now logged out</small></h1>
```

```json expect
{
  "claims": [
    { "kind": "page.title", "element": "logged-out", "outcome": "cantVerify", "reason": "unreadable", "at": "src/Identity.API/Views/Account/LoggedOut.cshtml:2" }
  ]
}
```

## maui-root-title

A .NET MAUI page's title (`intent.maui`) is its XAML root's `Title`, else the `Title` its constructor sets. The basket's XAML titles it, the catalog page, written in C# alone, sets its own, and the profile page states none, as its model's Page Title doesn't.

```json case
{ "phase": "later", "kinds": ["page.title"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "basket", "type": "Page", "name": "BasketView", "stereotypes": { "Page": { "Title": "Basket" } } },
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { "Title": "Catalog" } } },
          { "id": "profile", "type": "Page", "name": "ProfileView", "stereotypes": { "Page": { } } }
        ]
      }]
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

```xml file src/Shop/Views/BasketView.xaml
<ContentPage x:Class="Shop.Views.BasketView"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             Title="Basket">
    <Label Text="{Binding Total}" />
</ContentPage>
```

```csharp file src/Shop/Views/BasketView.xaml.cs
namespace Shop.Views;

public partial class BasketView : ContentPage
{
}
```

```csharp file src/Shop/Views/Pages.cs
namespace Shop.Views;

public class CatalogView : ContentPage
{
    public CatalogView()
    {
        Title = "Catalog";
    }
}

public class ProfileView : ContentPage
{
}
```

```json expect
{
  "claims": [
    { "kind": "page.title", "element": "basket", "outcome": "verified", "actual": "Basket", "at": "src/Shop/Views/BasketView.xaml:4" },
    { "kind": "page.title", "element": "catalog", "outcome": "verified", "actual": "Catalog", "at": "src/Shop/Views/Pages.cs:7" },
    { "kind": "page.title", "element": "profile", "absent": true }
  ],
  "others": "verified",
  "noOtherFindings": true
}
```

## maui-title-changed

A planted drift: the basket's XAML titles it `Cart`. One finding, its title's.

```json case
{ "phase": "later", "base": "maui-root-title", "kinds": ["page.title"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```xml file src/Shop/Views/BasketView.xaml
<ContentPage x:Class="Shop.Views.BasketView"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             Title="Cart">
    <Label Text="{Binding Total}" />
</ContentPage>
```

```json expect
{
  "claims": [
    { "kind": "page.title", "element": "basket", "outcome": "contradicted", "expected": "Basket", "actual": "Cart" }
  ],
  "findings": [
    { "rule": "claim/page.title", "severity": "error", "subject": "basket" }
  ],
  "noOtherFindings": true
}
```

## maui-bound-title

The basket's title is bound to its view model: what it says isn't the code's to state, so it can't be verified.

```json case
{ "phase": "later", "base": "maui-root-title", "kinds": ["page.title"], "cases": ["engine/unreadable"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```xml file src/Shop/Views/BasketView.xaml
<ContentPage x:Class="Shop.Views.BasketView"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             Title="{Binding Title}">
    <Label Text="{Binding Total}" />
</ContentPage>
```

```json expect
{
  "claims": [
    { "kind": "page.title", "element": "basket", "outcome": "cantVerify", "reason": "unreadable", "at": "src/Shop/Views/BasketView.xaml:4" }
  ]
}
```
