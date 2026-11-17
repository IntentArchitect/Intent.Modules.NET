# component.kind

What a User Interface designer Component or Layout is: `Page` for a Component with the Page stereotype, `Dialog`, `Component`, or `Layout` for a Layout. The kind is `stated: always`: a component file with no `@page` and no `LayoutComponentBase` states that it is a plain component.

The `intent.ui` pack (phase 6) gives the vocabulary, and the Blazor reader reads the component: an `@page` directive makes it a page, and a `LayoutComponentBase` base a layout. The reader takes a component's files as one unit: `CartPage.razor` and `CartPage.razor.cs` are one place (R37).

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
