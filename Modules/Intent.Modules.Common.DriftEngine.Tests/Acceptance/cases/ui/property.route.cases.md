# property.route

Whether a component's Property is bound from its route: the Route Parameter stereotype on a User Interface designer Property, against a `{name}` parameter in the component's `@page` route. Route parameters bind to properties ignoring case, so the read is case-insensitive. The kind is `stated: always`: a route with no such parameter states that the Property isn't one, and the check runs both ways.

The stock User Interface pack gives the kind and its vocabulary, and the Blazor reader reads the component's `@page` routes.

## verified

The item page's route `/item/{itemId:int}` binds its `ItemId` property, and the model's `ItemId` Property has the Route Parameter stereotype. The parameter's lower-case first letter and its constraint don't matter.

```json case
{ "phase": "later", "kinds": ["property.route"], "traces": ["ESC-FE-19"], "requirements": ["R2", "R28", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
                "stereotypes": { "Page": { "Route": "/item/{itemId}" } },
                "children": [
                  { "id": "item-page-item-id", "type": "Property", "name": "ItemId",
                    "typeRef": { "type": "int" },
                    "stereotypes": { "Route Parameter": {} } }
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
    { "kind": "property.route", "element": "item-page-item-id", "outcome": "verified",
      "expected": "true", "actual": "true", "at": "src/WebApp/Components/Pages/Item/ItemPage.razor:9" }
  ]
}
```

## contradicted-not-modelled

The route binds `itemId`, and the model's `ItemId` Property has no Route Parameter stereotype.

```json case
{ "phase": "later", "base": "verified", "kinds": ["property.route"], "traces": ["ESC-FE-19"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
                "stereotypes": { "Page": { "Route": "/item/{itemId}" } },
                "children": [
                  { "id": "item-page-item-id", "type": "Property", "name": "ItemId",
                    "typeRef": { "type": "int" } }
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
    { "kind": "property.route", "element": "item-page-item-id", "outcome": "contradicted",
      "expected": "false", "actual": "true", "at": "src/WebApp/Components/Pages/Item/ItemPage.razor:9" }
  ],
  "findings": [
    { "rule": "claim/property.route", "severity": "error", "subject": "item-page-item-id" }
  ]
}
```

## contradicted-not-in-route

The model's `ItemId` is a Route Parameter, and the page's route has lost its parameter: the item now comes from somewhere else. From silence, `ItemId` isn't a route parameter.

```json case
{ "phase": "later", "base": "verified", "kinds": ["property.route"], "cases": ["engine/silence"], "traces": ["ESC-FE-19"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Item/ItemPage.razor
@page "/item"
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
    { "kind": "property.route", "element": "item-page-item-id", "outcome": "contradicted",
      "expected": "true", "actual": "false", "fromSilence": true }
  ]
}
```

## react-route-parameters

A React page is given its route's parameters by react-router's `useParams()`, so the stock React reader reads a Property as a route parameter when the page reads it from there: destructured (`const { id } = useParams()`) or read from what it returns (`params.tab`). The order page reads `id` and `tab`; the model's `Tab` has no Route Parameter, so the code states otherwise, and `Note`, which nothing reads from the route, states `false` by saying nothing.

```json case
{ "phase": "later", "kinds": ["property.route"], "requirements": ["R2", "R3", "R28", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "order-page", "type": "Page", "name": "OrderPage", "stereotypes": { "Page": { "Route": "/orders/{id}/{tab}" } },
            "children": [
              { "id": "order-page-id", "type": "Property", "name": "Id", "typeRef": { "type": "string" }, "stereotypes": { "Route Parameter": {} } },
              { "id": "order-page-tab", "type": "Property", "name": "Tab", "typeRef": { "type": "string" } },
              { "id": "order-page-note", "type": "Property", "name": "Note", "typeRef": { "type": "string" } }
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

```tsx file src/OrderPage.tsx
import { useParams } from 'react-router-dom';

export default function OrderPage({ note }: { note?: string }) {
    const { id } = useParams<{ id: string }>();
    const params = useParams();
    return <h1>Order {id} ({params.tab}) {note}</h1>;
}
```

```json expect
{
  "claims": [
    { "kind": "property.route", "element": "order-page-id", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/OrderPage.tsx:4" },
    { "kind": "property.route", "element": "order-page-tab", "outcome": "contradicted", "expected": "false", "actual": "true" },
    { "kind": "property.route", "element": "order-page-note", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/property.route", "severity": "error", "subject": "order-page-tab" }
  ]
}
```

## react-router-5-match-params

react-router 5 gives the component a route mounts, or `withRouter` wraps, its `match`, so a class component reads a route parameter as `this.props.match.params.accountId`. The account page keeps it in its state; `Note`, a prop it is given, isn't from the route, which the code states by saying nothing.

```json case
{ "phase": "later", "kinds": ["property.route"], "requirements": ["R2", "R3", "R28", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "spa", "name": "Spa", "type": "User Interface Package",
        "elements": [
          { "id": "account-page", "type": "Page", "name": "AccountPage", "stereotypes": { "Page": { "Route": "/accounts/{accountId}" } },
            "children": [
              { "id": "account-page-account-id", "type": "Property", "name": "AccountId", "typeRef": { "type": "string" }, "stereotypes": { "Route Parameter": {} } },
              { "id": "account-page-note", "type": "Property", "name": "Note", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "spa", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```jsx file src/pages/AccountPage.jsx
import React from 'react';
import { withRouter } from 'react-router-dom';

class AccountPage extends React.Component {
  constructor(props) {
    super(props);
    this.state = { accountId: this.props.match.params.accountId, account: null };
  }

  render() {
    const { note } = this.props;
    return <h1>{this.state.accountId} {note}</h1>;
  }
}

export default withRouter(AccountPage);
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "account-page-account-id", "outcome": "verified", "at": "src/pages/AccountPage.jsx:7" },
    { "kind": "property.route", "element": "account-page-account-id", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/pages/AccountPage.jsx:5" },
    { "kind": "property.route", "element": "account-page-note", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true }
  ]
}
```

## maui-shell-has-no-route-parameters

.NET MAUI's Shell has no route parameters (`intent.maui`): a page receives its navigation's values as query parameters. So a MAUI page's Property the model marks a route parameter is one finding, and one it doesn't verifies.

```json case
{ "phase": "later", "kinds": ["property.route"], "requirements": ["R2", "R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "order", "type": "Page", "name": "OrderDetailView", "stereotypes": { "Page": { } }, "children": [
            { "id": "order-number", "type": "Property", "name": "OrderNumber", "typeRef": { "type": "int" }, "stereotypes": { "[Route Parameter]": { } } },
            { "id": "order-tab", "type": "Property", "name": "Tab", "typeRef": { "type": "string" }, "stereotypes": { "[Query Parameter]": { } } }
          ] }
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

```xml file src/Shop/Views/OrderDetailView.xaml
<ContentPage x:Class="Shop.Views.OrderDetailView" xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <Label Text="{Binding OrderNumber}" />
</ContentPage>
```

```csharp file src/Shop/Views/OrderDetailView.xaml.cs
namespace Shop.Views;

[QueryProperty(nameof(Tab), "tab")]
public partial class OrderDetailView : ContentPage
{
    public int OrderNumber { get; set; }

    public string Tab { get; set; } = "";
}
```

```json expect
{
  "claims": [
    { "kind": "property.route", "element": "order-number", "outcome": "contradicted", "expected": "true", "actual": "false" },
    { "kind": "property.route", "element": "order-tab", "outcome": "verified", "expected": "false", "actual": "false" },
    { "kind": "property.query", "element": "order-tab", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/property.route", "severity": "error", "subject": "order-number", "messageContains": "MAUI Shell has no route parameters: a page receives navigation values as query parameters. Remove [Route Parameter]." }
  ],
  "noOtherFindings": true
}
```
