# component.kind

What a User Interface designer Component or Layout is: `Page` for a Component with the Page stereotype, `Dialog`, `Component`, or `Layout` for a Layout. The kind is `stated: always`: a component file with no `@page` and no `LayoutComponentBase` states that it is a plain component.

The `intent.ui` pack (phase 6) gives the vocabulary, and the Blazor reader reads the component: an `@page` directive makes it a page, and a `LayoutComponentBase` base a layout. The reader takes a component's files as one unit: `CartPage.razor` and `CartPage.razor.cs` are one place (R37).

The `mvc-*` cases read ASP.NET Core MVC views (`.cshtml`), which the C# language reads as components named after their files, with the MVC reader (`intent.mvc`): a view a `_ViewStart.cshtml` or another view names as its `Layout` is a layout, one an action renders (`View(...)`) or a Razor Page (`@page`) is a page, and any other is a plain component, a partial.

## verified-page

The Cart page has the Page stereotype, and its `.razor` file has an `@page` directive.

```json case
{ "phase": 6, "kinds": ["component.kind"], "traces": ["ESC-FE-08", "ESC-FE-11"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
{ "phase": 6, "base": "verified-page", "kinds": ["component.kind"], "traces": ["ESC-FE-11"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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

The model has `CartPage` as a page, and its file has lost its `@page` directive (`page.missing`). With no `@page` and no `LayoutComponentBase`, the reader says, from silence, that it is a plain component.

```json case
{ "phase": 6, "base": "verified-page", "kinds": ["component.kind"], "cases": ["engine/silence"], "traces": ["ESC-FE-11"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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

eShopCustom's `frontend.layout`: a Layout derives `LayoutComponentBase`, here in its code-behind rather than with `@inherits`. The `.razor` file and its `.razor.cs` declare one class in one namespace, so the reader takes them as one component unit (R37): one place, the `.razor` file, rather than ambiguous. It finds the base class in the `.razor.cs` part.

```json case
{ "phase": 6, "base": "verified-page", "without": ["src/WebApp/Components/Pages/Cart/CartPage.razor"], "kinds": ["component.kind"], "traces": ["ESC-FE-10"], "requirements": ["R2", "R37"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
@namespace eShop.WebApp.Components.Layout
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
      "expected": "Layout", "actual": "Layout", "at": "src/WebApp/Components/Layout/MainLayout.razor:1" }
  ]
}
```

## contradicted-layout-not-derived

The model has `MainLayout` as a Layout, and neither of its files derives `LayoutComponentBase`: the code-behind derives `ComponentBase`. From silence, it is a plain component.

```json case
{ "phase": 6, "base": "verified-layout-code-behind", "kinds": ["component.kind"], "traces": ["ESC-FE-10"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
{ "phase": 6, "base": "verified-layout-code-behind", "kinds": ["component.kind"], "traces": ["ESC-FE-10"], "requirements": ["R2"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
      "expected": "Component", "actual": "Layout", "at": "src/WebApp/Components/Layout/MainLayout.razor:1" }
  ]
}
```

## cant-verify-file-missing

eShopCustom's `component.missing`: the model has a Checkout page with no `Checkout.razor` in scope. The `intent.ui` pack claims each component's type, and its kind waits on it, so the missing component is one finding, its `type.exists`, and the kind it blocks has none (one mismatch, one finding).

```json case
{ "phase": 6, "base": "verified-page", "kinds": ["component.kind"], "cases": ["engine/blocked"], "traces": ["ESC-FE-08"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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

## vue-standard-designer

The standard User Interface designer (2.0.0) has Page, Layout and Component element types, each its own kind. In a Vue front end the stock Vue reader (`intent.vue`) reads what the code makes each one: a page is a component a vue-router route record mounts, a layout one whose template hosts a `<RouterView>`, and anything else is a plain component, which it states by saying nothing.

```json case
{ "phase": 6, "kinds": ["component.kind"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "app", "type": "Layout", "name": "App" },
          { "id": "pages", "type": "Folder", "name": "pages", "children": [
            { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart" } } }
          ] },
          { "id": "components", "type": "Folder", "name": "components", "children": [
            { "id": "stock-pill", "type": "Component", "name": "StockPill" }
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

```typescript file src/router.ts
import type { RouteRecordRaw } from 'vue-router';
import CartPage from './pages/CartPage.vue';

export const routes: RouteRecordRaw[] = [
    { path: '/cart', component: CartPage },
];
```

```vue file src/App.vue
<script setup lang="ts">
import { RouterView } from 'vue-router';
</script>

<template>
    <header>eShop</header>
    <RouterView />
</template>
```

```vue file src/pages/CartPage.vue
<script setup lang="ts">
const title = 'Cart';
</script>
```

```vue file src/components/StockPill.vue
<script setup lang="ts">
defineProps<{ inStock: boolean }>();
</script>
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "app", "outcome": "verified", "expected": "Layout", "actual": "Layout", "at": "src/App.vue:7" },
    { "kind": "component.kind", "element": "cart-page", "outcome": "verified", "expected": "Page", "actual": "Page", "at": "src/router.ts:5" },
    { "kind": "component.kind", "element": "stock-pill", "outcome": "verified", "expected": "Component", "actual": "Component" }
  ]
}
```

## vue-layout-without-router-view

The layout's template no longer hosts a `<RouterView>`: nothing in the code makes `App` a layout, so the model's Layout is contradicted, the code stating a plain component by saying nothing.

```json case
{ "phase": 6, "base": "vue-standard-designer", "kinds": ["component.kind"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```vue file src/App.vue
<script setup lang="ts">
const brand = 'eShop';
</script>

<template>
    <header>{{ brand }}</header>
    <slot />
</template>
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "app", "outcome": "contradicted", "expected": "Layout", "actual": "Component" }
  ],
  "findings": [
    { "rule": "claim/component.kind", "severity": "error", "subject": "app" }
  ]
}
```

## react-standard-designer

The standard User Interface designer (2.0.0) read against a React front end by the stock React reader (`intent.react`). A page is a component a react-router route mounts: here a route object's `element` in `createBrowserRouter`. A layout is one that renders an `<Outlet />`, even when a route with child routes mounts it, since that route is its children's. Anything else is a plain component, which it states by saying nothing. A Dialog can't be told from a component's own code, so that kind has no rule.

```json case
{ "phase": 6, "kinds": ["component.kind"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "app", "type": "Layout", "name": "App" },
          { "id": "pages", "type": "Folder", "name": "pages", "children": [
            { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart" } } }
          ] },
          { "id": "components", "type": "Folder", "name": "components", "children": [
            { "id": "stock-pill", "type": "Component", "name": "StockPill" },
            { "id": "confirm-dialog", "type": "Dialog", "name": "ConfirmDialog" }
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

```tsx file src/main.tsx
import { createRoot } from 'react-dom/client';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import App from './App';
import CartPage from './pages/CartPage';

const router = createBrowserRouter([
    { path: '/', element: <App />, children: [
        { path: 'cart', element: <CartPage /> },
    ] },
]);

createRoot(document.getElementById('root')!).render(<RouterProvider router={router} />);
```

```tsx file src/App.tsx
import { Outlet } from 'react-router-dom';

export default function App() {
    return (
        <>
            <header>eShop</header>
            <Outlet />
        </>
    );
}
```

```tsx file src/pages/CartPage.tsx
export default function CartPage() {
    return <h1>Cart</h1>;
}
```

```tsx file src/components/StockPill.tsx
export const StockPill = ({ inStock }: { inStock: boolean }) => <span>{inStock ? 'In stock' : 'Sold out'}</span>;
```

```tsx file src/components/ConfirmDialog.tsx
export function ConfirmDialog({ onClose }: { onClose(): void }) {
    return <dialog open><button onClick={onClose}>OK</button></dialog>;
}
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "app", "outcome": "verified", "expected": "Layout", "actual": "Layout", "at": "src/App.tsx:7" },
    { "kind": "component.kind", "element": "cart-page", "outcome": "verified", "expected": "Page", "actual": "Page", "at": "src/main.tsx:8" },
    { "kind": "component.kind", "element": "stock-pill", "outcome": "verified", "expected": "Component", "actual": "Component" },
    { "kind": "component.kind", "element": "confirm-dialog", "outcome": "cantVerify", "reason": "noRule" }
  ]
}
```

## react-layout-without-outlet

The layout no longer renders an `<Outlet />`, so the route at `/` that mounts it shows it and never its child routes: the code makes `App` a page, and the model's Layout is contradicted.

```json case
{ "phase": 6, "base": "react-standard-designer", "kinds": ["component.kind"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```tsx file src/App.tsx
export default function App() {
    return <header>eShop</header>;
}
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "app", "outcome": "contradicted", "expected": "Layout", "actual": "Page", "at": "src/main.tsx:7" }
  ],
  "findings": [
    { "rule": "claim/component.kind", "severity": "error", "subject": "app" }
  ]
}
```

## react-page-not-routed

The cart page lost its route, and it is no layout, so the code states, by saying nothing, that it is a plain component.

```json case
{ "phase": 6, "base": "react-standard-designer", "kinds": ["component.kind"], "cases": ["engine/silence"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```tsx file src/main.tsx
import { createRoot } from 'react-dom/client';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import App from './App';

const router = createBrowserRouter([
    { path: '/', element: <App />, children: [] },
]);

createRoot(document.getElementById('root')!).render(<RouterProvider router={router} />);
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "cart-page", "outcome": "contradicted", "expected": "Page", "actual": "Component", "fromSilence": true }
  ]
}
```

## react-component-forms

Every way React declares a component is one: a class extending `React.Component`, a function wrapped in `forwardRef`, and an anonymous default export, named for its file. A data router's route mounts a page through its `Component`, or lazily through `lazy: () => import(...)`, whose file's component is the page.

```json case
{ "phase": 6, "kinds": ["component.kind"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "orders-page", "type": "Page", "name": "OrdersPage", "stereotypes": { "Page": { "Route": "/orders" } } },
          { "id": "about-page", "type": "Page", "name": "AboutPage", "stereotypes": { "Page": { "Route": "/about" } } },
          { "id": "search-box", "type": "Component", "name": "SearchBox" }
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

```jsx file src/router.jsx
import { createBrowserRouter } from 'react-router-dom';
import OrdersPage from './OrdersPage';

export const router = createBrowserRouter([
    { path: '/orders', Component: OrdersPage },
    { path: '/about', lazy: () => import('./AboutPage') },
]);
```

```jsx file src/OrdersPage.jsx
import React from 'react';
import { SearchBox } from './SearchBox';

export default class OrdersPage extends React.Component {
    render() {
        return <section><SearchBox /></section>;
    }
}
```

```jsx file src/AboutPage.jsx
export default function () {
    return <h1>About</h1>;
}
```

```tsx file src/SearchBox.tsx
import { forwardRef } from 'react';

export const SearchBox = forwardRef<HTMLInputElement>(function SearchBox(_, ref) {
    return <input ref={ref} type="search" />;
});
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "about-page", "outcome": "verified", "at": "src/AboutPage.jsx:1" },
    { "kind": "component.kind", "element": "orders-page", "outcome": "verified", "actual": "Page", "at": "src/router.jsx:5" },
    { "kind": "component.kind", "element": "about-page", "outcome": "verified", "actual": "Page", "at": "src/router.jsx:6" },
    { "kind": "component.kind", "element": "search-box", "outcome": "verified", "actual": "Component" }
  ]
}
```

## react-and-vue-side-by-side

One front end with a Vue component and a React component, and a C# Razor one beside them. Each stock reader answers only for its own code: the Vue reader's `<RouterView>` makes the Vue shell a layout, the React reader's route makes the React page a page, and Blazor's `@page` the Razor one. No reader claims another's component.

```json case
{ "phase": 6, "kinds": ["component.kind"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "vue-shell", "type": "Layout", "name": "VueShell" },
          { "id": "react-page", "type": "Page", "name": "ReactPage", "stereotypes": { "Page": { "Route": "/react" } } },
          { "id": "razor-page", "type": "Page", "name": "RazorPage", "stereotypes": { "Page": { "Route": "/razor" } } }
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

```vue file src/VueShell.vue
<template>
    <RouterView />
</template>
```

```tsx file src/routes.tsx
import { Route, Routes } from 'react-router-dom';
import ReactPage from './ReactPage';

export function AppRoutes() {
    return (
        <Routes>
            <Route path="/react" element={<ReactPage />} />
        </Routes>
    );
}
```

```tsx file src/ReactPage.tsx
export default function ReactPage() {
    return <p>React</p>;
}
```

```razor file src/RazorPage.razor
@page "/razor"
<p>Razor</p>
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "vue-shell", "outcome": "verified", "actual": "Layout", "at": "src/VueShell.vue:2" },
    { "kind": "component.kind", "element": "react-page", "outcome": "verified", "actual": "Page", "at": "src/routes.tsx:7" },
    { "kind": "component.kind", "element": "razor-page", "outcome": "verified", "actual": "Page" }
  ]
}
```

## mvc-views

eShop's Identity site: `_ViewStart.cshtml` names `_Layout` as every view's layout, `HomeController.Index` renders its own view (`View()`), `AccountController.Login` renders `Redirect` through a controller extension method of the code's own (`this.LoadingPage("Redirect", url)`, whose body is `controller.View(viewName, ...)`), `_Nav` is a partial the layout renders, and `Privacy` is a Razor Page. A dialog can't be told from a view's own code. `_ViewStart.cshtml` and `_ViewImports.cshtml` hold no view, so they are no type.

```json case
{ "phase": 6, "kinds": ["component.kind"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Identity.API", "type": "User Interface Package",
        "elements": [
          { "id": "views", "type": "Folder", "name": "Views", "children": [
            { "id": "home", "type": "Folder", "name": "Home", "children": [
              { "id": "home-index", "type": "Page", "name": "Index", "stereotypes": { "Page": { } } }
            ] },
            { "id": "shared", "type": "Folder", "name": "Shared", "children": [
              { "id": "layout", "type": "Layout", "name": "_Layout" },
              { "id": "nav", "type": "Component", "name": "_Nav" },
              { "id": "redirect", "type": "Page", "name": "Redirect", "stereotypes": { "Page": { } } },
              { "id": "confirm", "type": "Dialog", "name": "_Confirm" }
            ] }
          ] },
          { "id": "pages", "type": "Folder", "name": "Pages", "children": [
            { "id": "privacy", "type": "Page", "name": "Privacy", "stereotypes": { "Page": { } } }
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

```xml file src/Identity.API/Identity.API.csproj
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
```

```csharp file src/Identity.API/Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
var app = builder.Build();
app.MapDefaultControllerRoute();
app.MapRazorPages();
app.Run();
```

```csharp file src/Identity.API/Quickstart/HomeController.cs
namespace Identity.API.Quickstart;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
```

```csharp file src/Identity.API/Quickstart/AccountController.cs
namespace Identity.API.Quickstart;

public class AccountController : Controller
{
    [HttpPost]
    public IActionResult Login(string returnUrl) => this.LoadingPage("Redirect", returnUrl);
}

public static class Extensions
{
    public static IActionResult LoadingPage(this Controller controller, string viewName, string redirectUri) =>
        controller.View(viewName, new RedirectViewModel { RedirectUrl = redirectUri });
}

public class RedirectViewModel
{
    public string RedirectUrl { get; set; }
}
```

```cshtml file src/Identity.API/Views/_ViewStart.cshtml
@{
    Layout = "_Layout";
}
```

```cshtml file src/Identity.API/Views/_ViewImports.cshtml
@using Identity.API.Quickstart
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

```cshtml file src/Identity.API/Views/Home/Index.cshtml
<h1>Welcome to Identity</h1>
```

```cshtml file src/Identity.API/Views/Shared/_Layout.cshtml
<!DOCTYPE html>
<html lang="en">
<body>
    <partial name="_Nav" />
    <div class="container body-container">@RenderBody()</div>
</body>
</html>
```

```cshtml file src/Identity.API/Views/Shared/_Nav.cshtml
<nav class="navbar"><a href="~/">Identity</a></nav>
```

```cshtml file src/Identity.API/Views/Shared/Redirect.cshtml
@model RedirectViewModel
<meta http-equiv="refresh" content="0;url=@Model.RedirectUrl" data-url="@Model.RedirectUrl">
```

```cshtml file src/Identity.API/Views/Shared/_Confirm.cshtml
<div class="modal"><button>OK</button></div>
```

```cshtml file src/Identity.API/Pages/Privacy.cshtml
@page
<h1>Privacy</h1>
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "layout", "outcome": "verified", "expected": "Layout", "actual": "Layout", "at": "src/Identity.API/Views/_ViewStart.cshtml:2" },
    { "kind": "component.kind", "element": "home-index", "outcome": "verified", "expected": "Page", "actual": "Page", "at": "src/Identity.API/Quickstart/HomeController.cs:5" },
    { "kind": "component.kind", "element": "redirect", "outcome": "verified", "expected": "Page", "actual": "Page", "at": "src/Identity.API/Quickstart/AccountController.cs:6" },
    { "kind": "component.kind", "element": "nav", "outcome": "verified", "expected": "Component", "actual": "Component", "at": "src/Identity.API/Views/Shared/_Layout.cshtml:4" },
    { "kind": "component.kind", "element": "privacy", "outcome": "verified", "expected": "Page", "actual": "Page", "at": "src/Identity.API/Pages/Privacy.cshtml:1" },
    { "kind": "component.kind", "element": "confirm", "outcome": "cantVerify", "reason": "noRule" }
  ]
}
```

## mvc-view-no-longer-rendered

A planted drift: `HomeController.Index` now redirects instead of rendering its view. Nothing renders `Views/Home/Index.cshtml` or names it as a layout, so it states, by saying nothing, that it is a plain component: one finding. The model leaves the dialog out, so its can't-verify row doesn't stand beside it.

```json case
{ "phase": 6, "base": "mvc-views", "kinds": ["component.kind"], "cases": ["engine/silence"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Identity.API", "type": "User Interface Package",
        "elements": [
          { "id": "views", "type": "Folder", "name": "Views", "children": [
            { "id": "home", "type": "Folder", "name": "Home", "children": [
              { "id": "home-index", "type": "Page", "name": "Index", "stereotypes": { "Page": { } } }
            ] },
            { "id": "shared", "type": "Folder", "name": "Shared", "children": [
              { "id": "layout", "type": "Layout", "name": "_Layout" },
              { "id": "nav", "type": "Component", "name": "_Nav" },
              { "id": "redirect", "type": "Page", "name": "Redirect", "stereotypes": { "Page": { } } }
            ] }
          ] },
          { "id": "pages", "type": "Folder", "name": "Pages", "children": [
            { "id": "privacy", "type": "Page", "name": "Privacy", "stereotypes": { "Page": { } } }
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

```csharp file src/Identity.API/Quickstart/HomeController.cs
namespace Identity.API.Quickstart;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Login", "Account");
}
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "home-index", "outcome": "contradicted", "expected": "Page", "actual": "Component", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/component.kind", "severity": "error", "subject": "home-index" },
    { "rule": "scope/coverage", "severity": "info" }
  ],
  "noOtherFindings": true
}
```

## mvc-view-configuration-states-nothing

What isn't a view states nothing: with the opt-in rules for unmodelled types and files on, a Razor Pages site whose every page, layout and partial is modelled reports nothing for its `_ViewStart.cshtml` and `_ViewImports.cshtml`, which configure the views of their folder and hold no view of their own.

```json case
{ "phase": 6, "kinds": ["component.kind"], "cases": ["optin/unmodelled.type", "optin/unmodelled.file"], "requirements": ["R2", "R38", "R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Web", "type": "User Interface Package",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "Pages", "children": [
            { "id": "index", "type": "Page", "name": "Index", "stereotypes": { "Page": { "Route": "/" } } },
            { "id": "shared", "type": "Folder", "name": "Shared", "children": [
              { "id": "layout", "type": "Layout", "name": "_Layout" },
              { "id": "row", "type": "Component", "name": "_Row" }
            ] }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Web", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" }, { "Rule": "optin/unmodelled.file" } ] }
        } }]
    }
  }
}
```

```xml file src/Web/Web.csproj
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
```

```cshtml file src/Web/Pages/_ViewImports.cshtml
@namespace Web.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

```cshtml file src/Web/Pages/_ViewStart.cshtml
@{
    Layout = "_Layout";
}
```

```cshtml file src/Web/Pages/Index.cshtml
@page
<h1>Orders</h1>
<partial name="_Row" />
```

```cshtml file src/Web/Pages/Shared/_Layout.cshtml
<main>@RenderBody()</main>
```

```cshtml file src/Web/Pages/Shared/_Row.cshtml
<tr><td>Order</td></tr>
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "index", "outcome": "verified", "actual": "Page", "at": "src/Web/Pages/Index.cshtml:1" },
    { "kind": "component.kind", "element": "layout", "outcome": "verified", "actual": "Layout", "at": "src/Web/Pages/_ViewStart.cshtml:2" },
    { "kind": "component.kind", "element": "row", "outcome": "verified", "actual": "Component", "at": "src/Web/Pages/Index.cshtml:3" }
  ],
  "findings": [
    { "rule": "scope/coverage", "severity": "info" }
  ],
  "noOtherFindings": true
}
```

## maui-views

A .NET MAUI app's views (`intent.maui`), each what the first framework base on its base chain makes it: a `Shell` or a page host (`NavigationPage`, `TabbedPage`, `FlyoutPage`) is a Layout, a `ContentPage` a Page, CommunityToolkit.Maui's `Popup` a Dialog, a view (`ContentView`, `Grid`, ...) a Component. `CatalogView`'s code-behind is written with no base: its XAML's root, the app's own `PageBase`, is its base, as the XAML compiler gives it. MAUI is in the code through the views' XAML namespace.

```json case
{ "phase": 6, "kinds": ["component.kind"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "AppShell" },
          { "id": "host", "type": "Layout", "name": "DetailHost" },
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { } } },
          { "id": "confirm", "type": "Dialog", "name": "ConfirmPopup" },
          { "id": "badge", "type": "Component", "name": "BadgeView" }
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

```xml file src/Shop/AppShell.xaml
<Shell x:Class="Shop.AppShell" xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
</Shell>
```

```csharp file src/Shop/AppShell.xaml.cs
namespace Shop;

public partial class AppShell : Shell
{
}
```

```csharp file src/Shop/Views/Views.cs
namespace Shop.Views;

public abstract class PageBase : ContentPage
{
}

public class DetailHost : NavigationPage
{
}

public class ConfirmPopup : Popup
{
}

public class BadgeView : Grid
{
}
```

```xml file src/Shop/Views/CatalogView.xaml
<views:PageBase x:Class="Shop.Views.CatalogView"
                xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                xmlns:views="clr-namespace:Shop.Views">
    <views:BadgeView />
</views:PageBase>
```

```csharp file src/Shop/Views/CatalogView.xaml.cs
namespace Shop.Views;

public partial class CatalogView
{
}
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "shell", "outcome": "verified", "actual": "Layout", "at": "src/Shop/AppShell.xaml:1" },
    { "kind": "component.kind", "element": "host", "outcome": "verified", "actual": "Layout", "at": "src/Shop/Views/Views.cs:7" },
    { "kind": "component.kind", "element": "catalog", "outcome": "verified", "actual": "Page", "at": "src/Shop/Views/CatalogView.xaml:1" },
    { "kind": "component.kind", "element": "confirm", "outcome": "verified", "actual": "Dialog", "at": "src/Shop/Views/Views.cs:11" },
    { "kind": "component.kind", "element": "badge", "outcome": "verified", "actual": "Component", "at": "src/Shop/Views/Views.cs:15" }
  ],
  "others": "verified",
  "noOtherFindings": true
}
```

## maui-view-modelled-as-a-page

A planted drift: the badge, a `Grid`, is modelled as a Page. One finding, its kind's, naming the base that makes it a Component.

```json case
{ "phase": 6, "base": "maui-views", "kinds": ["component.kind"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "AppShell" },
          { "id": "host", "type": "Layout", "name": "DetailHost" },
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { } } },
          { "id": "confirm", "type": "Dialog", "name": "ConfirmPopup" },
          { "id": "badge", "type": "Page", "name": "BadgeView", "stereotypes": { "Page": { } } }
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

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "badge", "outcome": "contradicted", "expected": "Page", "actual": "Component" }
  ],
  "findings": [
    { "rule": "claim/component.kind", "severity": "error", "subject": "badge", "messageContains": "`BadgeView` component kind: model `Page`, code `Component`. `BadgeView` derives from `Grid`. Model it as a Component." }
  ],
  "noOtherFindings": true
}
```

## wpf-page-isnt-mauis

What another XAML framework writes states nothing to MAUI's reader: a WPF page (XAML 2006, no MAUI in the code) is read as the markup half of its class, but its kind is left to a WPF reader the engine doesn't have.

```json case
{ "phase": 6, "kinds": ["component.kind"], "cases": ["engine/noRule"], "requirements": ["R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Desk", "type": "User Interface Package",
        "elements": [ { "id": "orders", "type": "Page", "name": "OrdersPage", "stereotypes": { "Page": { } } } ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Desk", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```xml file src/Desk/OrdersPage.xaml
<Page x:Class="Desk.OrdersPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Grid />
</Page>
```

```csharp file src/Desk/OrdersPage.xaml.cs
namespace Desk;

public partial class OrdersPage
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "orders", "outcome": "verified" },
    { "kind": "component.kind", "element": "orders", "outcome": "cantVerify", "reason": "noRule" }
  ]
}
```

## angular-standard-designer

The standard User Interface designer (2.0.0) read against an Angular front end by the stock Angular reader (`intent.angular`). A component is a class its `@Component` decorator declares; its template is the `.html` file its `templateUrl` names, which Angular claims file by file (code-reader.md, Claimed files), or its inline `template`. A tag a component's `selector` matches renders it. The router's tables are read from `provideRouter`, following `loadChildren` into the file it loads, so `OrderPage` is at `/orders/{id}`, and a static redirect sends the footer's `/` link on to `/cart`. `App` is bootstrapped and hosts a `<router-outlet>`: a Layout. A component a dialog service opens (`inject(Dialog).open(ConfirmDialog)`) is a Dialog. With `withComponentInputBinding()`, an input named like a `:param` is a route parameter and any other input of a routed component a query parameter; `model()` is two-way. A field `inject(X)` initializes is an `X`, so `this.orderApi.getOrdersResource()` in a field initializer calls the proxy. A proxy's `httpResource(() => '/api/orders')` is a GET, an `Observable<T>` return is `T`, a method's `<T = Order>` with no parameter to fix it is `Order`, and the absolute URL its requests start with is its Base URL. Security has no standard Angular reading: `None` holds from silence where no guard is on the route.

```json case
{ "phase": "later", "kinds": ["component.kind", "page.route", "page.title", "intent.uses", "intent.target", "intent.navigates", "intent.invokes", "intent.opens", "property.bindable", "property.route", "property.query", "proxy.verb", "proxy.route", "proxy.baseUrl", "operation.returns", "security"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "shop", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "app", "type": "Layout", "name": "App" },
          { "id": "footer", "type": "Component", "name": "Footer" },
          { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart", "Title": "Cart" } } },
          { "id": "cart-line", "type": "Component", "name": "CartLine", "children": [
            { "id": "cart-line-label", "type": "Property", "name": "label", "typeRef": { "type": "string" } },
            { "id": "cart-line-quantity", "type": "Property", "name": "quantity", "typeRef": { "type": "int" }, "stereotypes": { "Bindable": {} } }
          ] },
          { "id": "order-page", "type": "Page", "name": "OrderPage", "stereotypes": { "Page": { "Route": "/orders/{id}", "Title": "Order" } }, "children": [
            { "id": "order-page-id", "type": "Property", "name": "id", "typeRef": { "type": "string" }, "stereotypes": { "Route Parameter": {} } },
            { "id": "order-page-tab", "type": "Property", "name": "tab", "typeRef": { "type": "string" }, "stereotypes": { "Query Parameter": {} } }
          ] },
          { "id": "confirm-dialog", "type": "Dialog", "name": "ConfirmDialog" },
          { "id": "cart-store", "type": "Store", "name": "CartStore", "stereotypes": { "Implementation": { "Path": "app/cart.store.ts" } } },
          { "id": "order-api", "type": "Proxy", "name": "OrderApi", "stereotypes": { "Implementation": { "Path": "app/order-api.ts" } }, "children": [
            { "id": "get-orders-resource", "type": "Proxy Operation", "name": "getOrdersResource", "stereotypes": { "HTTP Request": { "Verb": "GET", "Route": "api/orders" } } },
            { "id": "get-orders", "type": "Proxy Operation", "name": "getOrders", "typeRef": { "element": "order", "collection": true }, "stereotypes": { "HTTP Request": { "Verb": "GET", "Route": "api/orders/all" } } },
            { "id": "get-order", "type": "Proxy Operation", "name": "getOrder", "typeRef": { "element": "order" }, "stereotypes": { "HTTP Request": { "Verb": "GET", "Route": "api/orders/{id}" } }, "children": [
              { "id": "get-order-id", "type": "Parameter", "name": "id", "typeRef": { "type": "string" } }
            ] }
          ] },
          { "id": "photon-api", "type": "Proxy", "name": "PhotonApi", "stereotypes": { "Implementation": { "Path": "app/photon-api.ts" }, "Base URL": { "Url": "https://photon.example.com" } }, "children": [
            { "id": "photon-search", "type": "Proxy Operation", "name": "search", "stereotypes": { "HTTP Request": { "Verb": "GET", "Route": "api" } }, "children": [
              { "id": "photon-search-q", "type": "Parameter", "name": "q", "typeRef": { "type": "string" } }
            ] }
          ] },
          { "id": "order", "type": "Contract", "name": "Order", "children": [
            { "id": "order-id", "type": "Property", "name": "id", "typeRef": { "type": "string" } },
            { "id": "order-status", "type": "Property", "name": "status", "typeRef": { "element": "order-status-enum", "nullable": true } },
            { "id": "order-lang", "type": "Property", "name": "lang", "typeRef": { "type": "string" } }
          ] },
          { "id": "coupon-check", "type": "Contract", "name": "CouponCheck", "children": [
            { "id": "coupon-check-valid", "type": "Property", "name": "valid", "typeRef": { "type": "bool" } },
            { "id": "coupon-check-message", "type": "Property", "name": "message", "typeRef": { "type": "string", "nullable": true } }
          ] },
          { "id": "order-status-enum", "type": "Enum", "name": "OrderStatus", "children": [
            { "id": "order-status-open", "type": "Enum-Literal", "name": "open" },
            { "id": "order-status-shipped", "type": "Enum-Literal", "name": "shipped" }
          ] },
          { "id": "role", "type": "Enum", "name": "Role", "children": [
            { "id": "role-customer", "type": "Enum-Literal", "name": "CUSTOMER" },
            { "id": "role-admin", "type": "Enum-Literal", "name": "ADMIN" }
          ] }
        ]
      }],
      "associations": [
        { "id": "app-footer", "type": "Composition", "source": { "element": "app" }, "target": { "element": "footer" } },
        { "id": "footer-home", "type": "Navigation", "source": { "element": "footer" }, "target": { "element": "cart-page" } },
        { "id": "cart-lines", "type": "Composition", "source": { "element": "cart-page" }, "target": { "element": "cart-line" } },
        { "id": "cart-orders", "type": "Navigation", "source": { "element": "cart-page" }, "target": { "element": "order-page" } },
        { "id": "cart-confirm", "type": "Show Dialog", "source": { "element": "cart-page" }, "target": { "element": "confirm-dialog" } },
        { "id": "cart-store-use", "type": "Uses", "source": { "element": "cart-page" }, "target": { "element": "cart-store" } },
        { "id": "cart-calls-orders", "type": "Call Service Operation Action", "source": { "element": "cart-page" }, "target": { "element": "get-orders-resource" } },
        { "id": "order-calls-order", "type": "Call Service Operation Action", "source": { "element": "order-page" }, "target": { "element": "get-order" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```typescript file src/main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';
import { appConfig } from './app/app.config';

bootstrapApplication(App, appConfig);
```

```typescript file src/app/app.config.ts
import { ApplicationConfig } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
    providers: [provideRouter(routes, withComponentInputBinding())],
};
```

```typescript file src/app/app.routes.ts
import { Routes } from '@angular/router';
import { CartPage } from './cart/cart-page';

export const routes: Routes = [
    { path: '', pathMatch: 'full', redirectTo: 'cart' },
    { path: 'cart', title: 'Cart', component: CartPage },
    { path: 'orders', loadChildren: () => import('./orders/orders.routes').then((m) => m.ORDER_ROUTES) },
];
```

```typescript file src/app/orders/orders.routes.ts
import { Routes } from '@angular/router';

export const ORDER_ROUTES: Routes = [
    { path: ':id', title: 'Order', loadComponent: () => import('./order-page').then((m) => m.OrderPage) },
];
```

```typescript file src/app/app.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Footer } from './footer';

@Component({ selector: 'app-root', imports: [RouterOutlet, Footer], templateUrl: './app.html' })
export class App {}
```

```html file src/app/app.html
<main>
  <router-outlet />
</main>
<app-footer />
```

```typescript file src/app/footer.ts
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({ selector: 'app-footer', imports: [RouterLink], template: `<footer><a routerLink="/">Home</a></footer>` })
export class Footer {}
```

```typescript file src/app/cart/cart-page.ts
import { Component, inject } from '@angular/core';
import { Dialog } from '@angular/cdk/dialog';
import { RouterLink } from '@angular/router';
import { CartLine } from './cart-line';
import { CartStore } from '../cart.store';
import { OrderApi } from '../order-api';
import { ConfirmDialog } from '../confirm-dialog';

@Component({ selector: 'app-cart-page', imports: [CartLine, RouterLink], templateUrl: './cart-page.html' })
export class CartPage {
    private readonly dialog = inject(Dialog);
    private readonly orderApi = inject(OrderApi);
    protected readonly store = inject(CartStore);
    protected readonly orders = this.orderApi.getOrdersResource();

    protected clear(): void {
        this.dialog.open(ConfirmDialog, { data: { message: 'Clear the cart?' } });
    }
}
```

```html file src/app/cart/cart-page.html
<h1>Cart</h1>
@for (line of store.lines(); track line.id) {
  <app-cart-line [label]="line.name" [(quantity)]="line.quantity" />
}
@if (orders.value(); as list) {
  @for (order of list; track order.id) {
    <a [routerLink]="['/orders', order.id]">{{ order.id }}</a>
  }
}
<button (click)="clear()">Clear</button>
```

```typescript file src/app/cart/cart-line.ts
import { Component, input, model } from '@angular/core';

@Component({ selector: 'app-cart-line', template: `<span>{{ label() }}</span><input type="number" [value]="quantity()" />` })
export class CartLine {
    readonly label = input.required<string>();
    readonly quantity = model(1);
}
```

```typescript file src/app/orders/order-page.ts
import { Component, inject, input } from '@angular/core';
import { OrderApi } from '../order-api';

@Component({ selector: 'app-order-page', templateUrl: './order-page.html' })
export class OrderPage {
    private readonly api = inject(OrderApi);
    readonly id = input.required<string>();
    readonly tab = input<string>('summary');

    protected load() {
        return this.api.getOrder(this.id());
    }
}
```

```html file src/app/orders/order-page.html
<h1>Order {{ id() }}</h1>
```

```typescript file src/app/confirm-dialog.ts
import { Component, inject } from '@angular/core';
import { DialogRef } from '@angular/cdk/dialog';

@Component({ selector: 'app-confirm-dialog', template: `<p>Sure?</p><button (click)="ref.close()">OK</button>` })
export class ConfirmDialog {
    protected readonly ref = inject(DialogRef);
}
```

```typescript file src/app/cart.store.ts
import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CartStore {
    readonly lines = signal<{ id: string; name: string; quantity: number }[]>([]);
}
```

```typescript file src/app/order-api.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, httpResource } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Order } from './order.models';

@Injectable({ providedIn: 'root' })
export class OrderApi {
    private readonly http = inject(HttpClient);

    getOrdersResource() {
        return httpResource<Order[]>(() => '/api/orders');
    }

    getOrders<T = Order>(): Observable<T[]> {
        return this.http.get<T[]>('/api/orders/all');
    }

    getOrder(id: string): Observable<Order> {
        return this.http.get<Order>(`/api/orders/${id}`);
    }
}
```

```typescript file src/app/photon-api.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PhotonApi {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = 'https://photon.example.com';

    search(q: string): Observable<unknown> {
        return this.http.get(this.baseUrl + '/api', { params: { q } });
    }
}
```

```typescript file src/app/order.models.ts
export type OrderStatus = 'open' | 'shipped' | null;

export const ROLES = { CUSTOMER: 'CUSTOMER', ADMIN: 'ADMIN' } as const;

export type Role = (typeof ROLES)[keyof typeof ROLES];

export interface Order {
    id: string;
    status: OrderStatus;
    lang: 'en';
}

export type CouponCheck = { valid: true; code: string } | { valid: false; message: string };
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "app", "outcome": "verified", "expected": "Layout", "actual": "Layout" },
    { "kind": "component.kind", "element": "cart-page", "outcome": "verified", "expected": "Page", "actual": "Page" },
    { "kind": "component.kind", "element": "order-page", "outcome": "verified", "expected": "Page", "actual": "Page" },
    { "kind": "component.kind", "element": "confirm-dialog", "outcome": "verified", "expected": "Dialog", "actual": "Dialog" },
    { "kind": "component.kind", "element": "cart-line", "outcome": "verified", "expected": "Component" },
    { "kind": "page.route", "element": "order-page", "outcome": "verified", "actual": "orders/{id}" },
    { "kind": "page.title", "element": "cart-page", "outcome": "verified", "actual": "Cart" },
    { "kind": "intent.uses", "element": "app-footer", "outcome": "verified", "at": "src/app/app.html:4" },
    { "kind": "intent.target", "element": "cart-lines", "outcome": "verified" },
    { "kind": "intent.navigates", "element": "footer-home", "outcome": "verified" },
    { "kind": "intent.navigates", "element": "cart-orders", "outcome": "verified", "at": "src/app/cart/cart-page.html:7" },
    { "kind": "intent.opens", "element": "cart-confirm", "outcome": "verified", "at": "src/app/cart/cart-page.ts:17" },
    { "kind": "intent.uses", "element": "cart-store-use", "outcome": "verified" },
    { "kind": "intent.invokes", "element": "cart-calls-orders", "outcome": "verified", "at": "src/app/cart/cart-page.ts:14" },
    { "kind": "intent.invokes", "element": "order-calls-order", "outcome": "verified" },
    { "kind": "property.bindable", "element": "cart-line-quantity", "outcome": "verified", "actual": "true" },
    { "kind": "property.bindable", "element": "cart-line-label", "outcome": "verified", "expected": "false" },
    { "kind": "property.route", "element": "order-page-id", "outcome": "verified", "actual": "true" },
    { "kind": "property.query", "element": "order-page-tab", "outcome": "verified", "actual": "true" },
    { "kind": "property.query", "element": "order-page-id", "outcome": "verified", "expected": "false" },
    { "kind": "proxy.verb", "element": "get-orders-resource", "outcome": "verified", "actual": "GET" },
    { "kind": "proxy.route", "element": "get-orders-resource", "outcome": "verified" },
    { "kind": "proxy.route", "element": "get-order", "outcome": "verified" },
    { "kind": "proxy.route", "element": "photon-search", "outcome": "verified" },
    { "kind": "proxy.baseUrl", "element": "photon-api", "outcome": "verified", "actual": "https://photon.example.com" },
    { "kind": "operation.returns", "element": "get-order", "outcome": "verified" },
    { "kind": "operation.returns", "element": "get-orders", "outcome": "verified" },
    { "kind": "enum.literals", "element": "order-status-enum", "outcome": "verified" },
    { "kind": "enum.literals", "element": "role", "outcome": "verified" },
    { "kind": "member.exists", "element": "coupon-check-message", "outcome": "verified" },
    { "kind": "member.type", "element": "coupon-check-message", "outcome": "verified" },
    { "kind": "member.type", "element": "order-lang", "outcome": "verified" },
    { "kind": "security", "element": "cart-page", "outcome": "verified", "expected": "None" }
  ],
  "findings": [
    { "rule": "claim/component.kind", "count": 0 },
    { "rule": "claim/intent.navigates", "count": 0 },
    { "rule": "claim/intent.invokes", "count": 0 },
    { "rule": "scope/unread", "count": 0 }
  ]
}
```

## angular-planted-breaks

The same app with a break in each reading: the footer's tag is gone from `app.html`, the cart links to `/order/...`, the dialog is no longer opened, `quantity` is a one-way `input()`, the proxy asks `/api/order`, and the Photon base URL is another host. Each is one finding of its kind.

```json case
{ "phase": "later", "base": "angular-standard-designer", "kinds": ["intent.uses", "intent.navigates", "intent.opens", "property.bindable", "proxy.route", "proxy.baseUrl"], "requirements": ["R1", "R14", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```html file src/app/app.html
<main>
  <router-outlet />
</main>
```

```html file src/app/cart/cart-page.html
<h1>Cart</h1>
@for (line of store.lines(); track line.id) {
  <app-cart-line [label]="line.name" [quantity]="line.quantity" />
}
@if (orders.value(); as list) {
  @for (order of list; track order.id) {
    <a [routerLink]="['/order', order.id]">{{ order.id }}</a>
  }
}
```

```typescript file src/app/cart/cart-page.ts
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CartLine } from './cart-line';
import { CartStore } from '../cart.store';
import { OrderApi } from '../order-api';

@Component({ selector: 'app-cart-page', imports: [CartLine, RouterLink], templateUrl: './cart-page.html' })
export class CartPage {
    private readonly orderApi = inject(OrderApi);
    protected readonly store = inject(CartStore);
    protected readonly orders = this.orderApi.getOrdersResource();
}
```

```typescript file src/app/cart/cart-line.ts
import { Component, input } from '@angular/core';

@Component({ selector: 'app-cart-line', template: `<span>{{ label() }}</span>` })
export class CartLine {
    readonly label = input.required<string>();
    readonly quantity = input(1);
}
```

```typescript file src/app/order-api.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, httpResource } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Order } from './order.models';

@Injectable({ providedIn: 'root' })
export class OrderApi {
    private readonly http = inject(HttpClient);

    getOrdersResource() {
        return httpResource<Order[]>(() => '/api/order');
    }

    getOrders<T = Order>(): Observable<T[]> {
        return this.http.get<T[]>('/api/orders/all');
    }

    getOrder(id: string): Observable<Order> {
        return this.http.get<Order>(`/api/orders/${id}`);
    }
}
```

```typescript file src/app/photon-api.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PhotonApi {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = 'https://maps.example.org';

    search(q: string): Observable<unknown> {
        return this.http.get(this.baseUrl + '/api', { params: { q } });
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "app-footer", "outcome": "contradicted" },
    { "kind": "intent.navigates", "element": "cart-orders", "outcome": "contradicted" },
    { "kind": "intent.opens", "element": "cart-confirm", "outcome": "contradicted" },
    { "kind": "property.bindable", "element": "cart-line-quantity", "outcome": "contradicted", "actual": "false" },
    { "kind": "proxy.route", "element": "get-orders-resource", "outcome": "contradicted" },
    { "kind": "proxy.baseUrl", "element": "photon-api", "outcome": "contradicted", "actual": "https://maps.example.org" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "app-footer" },
    { "rule": "claim/intent.navigates", "severity": "error", "subject": "cart-orders" },
    { "rule": "claim/intent.opens", "severity": "error", "subject": "cart-confirm" },
    { "rule": "claim/property.bindable", "severity": "error", "subject": "cart-line-quantity" },
    { "rule": "claim/proxy.route", "severity": "error", "subject": "get-orders-resource" },
    { "rule": "claim/proxy.baseUrl", "severity": "error", "subject": "photon-api" }
  ]
}
```
