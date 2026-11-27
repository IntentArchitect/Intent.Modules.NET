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

## react-route-parameter-type

A Property the page reads from `useParams()` has no declaration of its own to type it: the router gives every route value as text, so it is a `string`. The order page's `Id` is modelled as a `string`, and its `Line` as an `int`, which the code reads as given.

```json case
{ "phase": "later", "kinds": ["member.exists", "member.type", "property.route"], "requirements": ["R2", "R3", "R28", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "order-page", "type": "Page", "name": "OrderPage", "stereotypes": { "Page": { "Route": "/orders/{id}/lines/{line}" } },
            "children": [
              { "id": "order-page-id", "type": "Property", "name": "Id", "typeRef": { "type": "string" }, "stereotypes": { "Route Parameter": {} } },
              { "id": "order-page-line", "type": "Property", "name": "Line", "typeRef": { "type": "int" }, "stereotypes": { "Route Parameter": {} } }
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

export default function OrderPage() {
    const { id, line } = useParams();
    return <h1>Order {id}, line {line}</h1>;
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-page-id", "outcome": "verified", "at": "src/OrderPage.tsx:4" },
    { "kind": "member.type", "element": "order-page-id", "outcome": "verified", "actual": "string", "at": "src/OrderPage.tsx:4" },
    { "kind": "member.type", "element": "order-page-line", "outcome": "contradicted", "actual": "string" },
    { "kind": "property.route", "element": "order-page-line", "outcome": "verified", "actual": "true" }
  ],
  "findings": [
    { "rule": "claim/member.type", "severity": "error", "subject": "order-page-line" }
  ]
}
```

## angular-route-parameter-nullability

A value a page reads from its route may be missing wherever the router can show the page without it. `UserForm` is shown at `users/create` and at `users/:id/edit`, so the `id` it reads with `paramMap.get('id')` is missing at `users/create`, and the model's non-nullable `id` is contradicted. `UserDetail` is shown only at `users/:id`, so its `id` is always there. A query value can always be left out of the URL, so `mode` is nullable.

```json case
{ "phase": "later", "kinds": ["member.nullable"], "requirements": ["R2", "R11", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "app", "name": "App", "type": "User Interface Package",
        "elements": [
          { "id": "user-detail", "type": "Page", "name": "UserDetail", "stereotypes": { "Page": { "Route": "/users/{id}" } }, "children": [
            { "id": "user-detail-id", "type": "Property", "name": "id", "typeRef": { "type": "string" }, "stereotypes": { "Route Parameter": {} } }
          ] },
          { "id": "user-form", "type": "Page", "name": "UserForm", "stereotypes": { "Page": { "Route": "/users/create, /users/{id}/edit" } }, "children": [
            { "id": "user-form-id", "type": "Property", "name": "id", "typeRef": { "type": "string" }, "stereotypes": { "Route Parameter": {} } },
            { "id": "user-form-mode", "type": "Property", "name": "mode", "typeRef": { "type": "string", "nullable": true }, "stereotypes": { "Query Parameter": {} } }
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

```typescript file src/main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { Component } from '@angular/core';
import { provideRouter, RouterOutlet } from '@angular/router';
import { UserDetail } from './user-detail';
import { UserForm } from './user-form';

@Component({ selector: 'app-root', imports: [RouterOutlet], template: `<router-outlet />` })
export class App {}

bootstrapApplication(App, {
    providers: [provideRouter([
        { path: 'users/create', component: UserForm },
        { path: 'users/:id', component: UserDetail },
        { path: 'users/:id/edit', component: UserForm },
    ])],
});
```

```typescript file src/user-detail.ts
import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({ selector: 'app-user-detail', template: `<h1>User</h1>` })
export class UserDetail {
    private route = inject(ActivatedRoute);

    load(): void {
        console.log(this.route.snapshot.paramMap.get('id'));
    }
}
```

```typescript file src/user-form.ts
import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({ selector: 'app-user-form', template: `<form></form>` })
export class UserForm {
    private route = inject(ActivatedRoute);

    load(): void {
        console.log(this.route.snapshot.paramMap.get('id'), this.route.snapshot.queryParamMap.get('mode'));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.nullable", "element": "user-detail-id", "outcome": "verified", "actual": "false" },
    { "kind": "member.nullable", "element": "user-form-id", "outcome": "contradicted", "expected": "false", "actual": "true" },
    { "kind": "member.nullable", "element": "user-form-mode", "outcome": "verified", "actual": "true" }
  ],
  "findings": [
    { "rule": "claim/member.nullable", "severity": "error", "subject": "user-form-id", "messageContains": "the page is also shown at `/users/create`, which has no `:id`" }
  ]
}
```

## angular-route-parameters-read-from-the-activated-route

An Angular page that reads its route's parameters from the injected `ActivatedRoute` rather than binding them to inputs: `UserDetail` keeps `paramMap.get('id')` in a signal of another name, `userId`, and `UserForm` reads it into a local. So the model's `id` is where the page reads it: it exists there, it is a route parameter, and it is the `string` the router gives. `UserForm`'s `id` is modelled as an `int`, which the code reads as given, and its `mode`, a query value, is read from `queryParamMap`. `UserDetail`'s `tab` is read nowhere.

```json case
{ "phase": "later", "kinds": ["member.exists", "member.type", "property.route", "property.query"], "requirements": ["R2", "R3", "R28", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "app", "name": "App", "type": "User Interface Package",
        "elements": [
          { "id": "user-detail", "type": "Page", "name": "UserDetail", "stereotypes": { "Page": { "Route": "/users/{id}" } }, "children": [
            { "id": "user-detail-id", "type": "Property", "name": "id", "typeRef": { "type": "string" }, "stereotypes": { "Route Parameter": {} } },
            { "id": "user-detail-tab", "type": "Property", "name": "tab", "typeRef": { "type": "string" }, "stereotypes": { "Query Parameter": {} } }
          ] },
          { "id": "user-form", "type": "Page", "name": "UserForm", "stereotypes": { "Page": { "Route": "/users/{id}/edit" } }, "children": [
            { "id": "user-form-id", "type": "Property", "name": "id", "typeRef": { "type": "int" }, "stereotypes": { "Route Parameter": {} } },
            { "id": "user-form-mode", "type": "Property", "name": "mode", "typeRef": { "type": "string" }, "stereotypes": { "Query Parameter": {} } }
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

```typescript file src/main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { Component } from '@angular/core';
import { provideRouter, RouterOutlet } from '@angular/router';
import { UserDetail } from './user-detail';
import { UserForm } from './user-form';

@Component({ selector: 'app-root', imports: [RouterOutlet], template: `<router-outlet />` })
export class App {}

bootstrapApplication(App, {
    providers: [provideRouter([
        { path: 'users/:id', component: UserDetail },
        { path: 'users/:id/edit', component: UserForm },
    ])],
});
```

```typescript file src/user-detail.ts
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({ selector: 'app-user-detail', template: `<h1>{{ userId() }}</h1>` })
export class UserDetail {
    private route = inject(ActivatedRoute);

    userId = signal(this.route.snapshot.paramMap.get('id'));
}
```

```typescript file src/user-form.ts
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({ selector: 'app-user-form', template: `<form></form>` })
export class UserForm implements OnInit {
    private route = inject(ActivatedRoute);

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('id');
        const mode = this.route.snapshot.queryParamMap.get('mode');
        console.log(id, mode);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "user-detail-id", "outcome": "verified", "at": "src/user-detail.ts:8" },
    { "kind": "member.type", "element": "user-detail-id", "outcome": "verified", "actual": "string" },
    { "kind": "property.route", "element": "user-detail-id", "outcome": "verified", "actual": "true", "at": "src/user-detail.ts:8" },
    { "kind": "member.exists", "element": "user-detail-tab", "outcome": "contradicted" },
    { "kind": "member.exists", "element": "user-form-id", "outcome": "verified", "at": "src/user-form.ts:9" },
    { "kind": "member.type", "element": "user-form-id", "outcome": "contradicted", "actual": "string", "at": "src/user-form.ts:9" },
    { "kind": "property.route", "element": "user-form-id", "outcome": "verified", "actual": "true" },
    { "kind": "member.exists", "element": "user-form-mode", "outcome": "verified", "at": "src/user-form.ts:10" },
    { "kind": "property.query", "element": "user-form-mode", "outcome": "verified", "actual": "true" },
    { "kind": "property.route", "element": "user-form-mode", "outcome": "verified", "expected": "false" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "user-detail-tab" },
    { "rule": "claim/member.type", "severity": "error", "subject": "user-form-id" }
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
    { "rule": "claim/property.route", "severity": "error", "subject": "order-number", "messageContains": "MAUI Shell has no route parameters: a page receives navigation values as query parameters. Remove [Route Parameter]." },
    { "rule": "engine/notStated", "severity": "info", "messageContains": "Not checked: 1 nullability claims (User Interface 1)", "note": "The project doesn't turn nullable reference types on" }
  ],
  "noOtherFindings": true
}
```
