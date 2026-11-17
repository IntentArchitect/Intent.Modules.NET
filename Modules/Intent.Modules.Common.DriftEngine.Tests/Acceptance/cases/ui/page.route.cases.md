# page.route

A page's routes: the Page stereotype's Route in the User Interface designer, or the Route stereotype's Path in the Java spike's Front End designer. The kind is a set compared with `subset`, its items compared as routes (`itemComparer: route`): the model's route must be among the routes the code serves for the page, and further routes the code serves aren't drift (principle 1; claim-catalog.md, Notes on kinds). The `route` item comparer ignores a leading or trailing `/`, case in literal segments, and parameter constraints. The kind is `stated: whenSet`, so a modelled route the code doesn't state is contradicted, never taken from a convention (decision 4).

The Blazor cases use the `intent.ui` pack and the Blazor reader. A Blazor page states its routes with one `@page` directive each, and each is one item of its component's set. The Vue cases use the Java spike's own Front End designer, which no stock pack knows: its house pack extends `intent.ui`, claiming the stock kinds on its own elements with the stock bindings (where a page is, its route), and the stock Vue reader (`intent.vue`) reads the page's record in `router.ts`. A route written `/products/:id`, as vue-router and the Front End designer write it, is `/products/{id}`.

## verified-constraint

The model's route `/item/{itemId}` and the code's `@page "/item/{itemId:int}"` differ only by the leading `/` and the parameter's constraint, which the `route` item comparer ignores.

```json case
{ "phase": 6, "kinds": ["page.route"], "traces": ["ESC-FE-12"], "requirements": ["R2", "R10", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
                "stereotypes": { "Page": { "Route": "/item/{itemId}" } } }
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

    protected override async Task OnInitializedAsync()
    {
        item = await CatalogService.GetCatalogItem(ItemId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "page.route", "element": "item-page", "outcome": "verified",
      "expected": "item/{itemId}", "at": "src/WebApp/Components/Pages/Item/ItemPage.razor:1" }
  ]
}
```

## verified-extra-route

The Cart page answers on `/cart` and on the older `/basket`, one `@page` each. The model names only `/cart`, which is among the code's routes, so the claim is verified: the code's set is `basket, cart`, sorted, and the route the model doesn't mention isn't drift.

```json case
{ "phase": 6, "base": "verified-constraint", "without": ["src/WebApp/Components/Pages/Item/ItemPage.razor"], "kinds": ["page.route"], "cases": ["engine/comparers"], "traces": ["ESC-FE-12"], "requirements": ["R1", "R10"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
@page "/basket"
@inject BasketState Basket

<PageTitle>Shopping Bag | AdventureWorks</PageTitle>
```

```json expect
{
  "claims": [
    { "kind": "page.route", "element": "cart-page", "outcome": "verified", "expected": "cart", "actual": "basket, cart" }
  ],
  "findings": [
    { "rule": "claim/page.route", "subject": "cart-page", "count": 0 }
  ]
}
```

## contradicted

eShopCustom's `page.route`: the model says the Cart page is at `/bag`, and its `@page` directives say `/cart` and `/basket`. The model's route is in neither, so the claim is contradicted, and the finding names the routes the code does serve.

```json case
{ "phase": 6, "base": "verified-extra-route", "kinds": ["page.route"], "traces": ["ESC-FE-12"], "requirements": ["R2", "R11", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
                "stereotypes": { "Page": { "Route": "/bag" } } }
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
    { "kind": "page.route", "element": "cart-page", "outcome": "contradicted", "expected": "bag", "actual": "basket, cart" }
  ],
  "findings": [
    { "rule": "claim/page.route", "severity": "error", "subject": "cart-page", "messageContains": "cart" }
  ]
}
```

## contradicted-not-stated

The model gives the Cart page the route `/cart`, and its file has no `@page`. The route is `whenSet`, the page's component is in the code, and no `@page` row reaches its claim, so the claim is contradicted with the code side "not stated" (R4). Nothing derives a route from the file's name or folder, and the query has no `absent`, which a `whenSet` kind refuses.

```json case
{ "phase": 6, "base": "verified-extra-route", "kinds": ["page.route"], "traces": ["ESC-FE-12"], "requirements": ["R4"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@inject BasketState Basket

<PageTitle>Shopping Bag | AdventureWorks</PageTitle>
```

```json expect
{
  "claims": [
    { "kind": "page.route", "element": "cart-page", "outcome": "contradicted", "expected": "cart", "actual": null }
  ],
  "findings": [
    { "rule": "claim/page.route", "severity": "error", "subject": "cart-page", "messageContains": "doesn't state" }
  ]
}
```

## java-verified

The Java spike's Vue storefront registers its pages in `router.ts`, one `{ path, component }` record each. The stock Vue reader reads the `path` of the record whose component is the page its `type.exists` claim located, a set of one route; the vocabulary splits the model's Path into a set the same way.

```json case
{ "phase": 6, "kinds": ["page.route"], "traces": ["JAV-FE-04"], "requirements": ["R2", "R28", "R38"], "packs": ["intent.core", "intent.ui", "house.shop"], "language": "typescript" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.ui", "version": "^1.0.0" }],
  "languages": ["typescript"],
  "designers": ["Front End"],
  "vocabulary": {
    "Front End": {
      "entries": [
        {
          "kind": "type.exists",
          "for": { "element": "Page" },
          "value": "self",
          "bindings": { "Name": "name", "Component": "name", "Path": "stereotype('Implementation').Path" },
          "own": ["Name"],
          "source": "the page's Implementation Path"
        },
        {
          "kind": "page.route",
          "for": { "element": "Page" },
          "value": "split(stereotype('Route').Path, ',')",
          "bindings": { "Page": "name" },
          "own": ["Page"],
          "dependsOn": "type.exists",
          "source": "the Route stereotype's Path"
        }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Front End": {
      "packages": [{
        "id": "storefront", "name": "Storefront",
        "elements": [
          { "id": "product-details-page", "type": "Page", "name": "ProductDetailsPage",
            "stereotypes": {
              "Route": { "Path": "/products/:id" },
              "Implementation": { "Path": "src/pages/ProductDetailsPage.vue" }
            } }
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
import { createRouter, createWebHistory } from 'vue-router';
import CatalogPage from './pages/CatalogPage.vue';
import ProductDetailsPage from './pages/ProductDetailsPage.vue';

export const router = createRouter({
    history: createWebHistory(),
    routes: [
        { path: '/', component: CatalogPage },
        { path: '/products/:id', component: ProductDetailsPage },
    ],
});
```

```vue file src/pages/CatalogPage.vue
<template>
    <h1>Catalog</h1>
</template>
```

```vue file src/pages/ProductDetailsPage.vue
<script setup lang="ts">
import { useRoute } from 'vue-router';

const route = useRoute();
</script>

<template>
    <h1>{{ route.params.id }}</h1>
</template>
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "product-details-page", "outcome": "verified", "at": "src/pages/ProductDetailsPage.vue:2" },
    { "kind": "page.route", "element": "product-details-page", "outcome": "verified", "actual": "products/{id}", "at": "src/router.ts:9" }
  ]
}
```

## java-contradicted

The model's Route Path says `/product/:id`, and the router mounts the page at `/products/:id`.

```json case
{ "phase": 6, "base": "java-verified", "kinds": ["page.route"], "traces": ["JAV-FE-04"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui", "house.shop"], "language": "typescript" }
```


```json model
{
  "designers": {
    "Front End": {
      "packages": [{
        "id": "storefront", "name": "Storefront",
        "elements": [
          { "id": "product-details-page", "type": "Page", "name": "ProductDetailsPage",
            "stereotypes": {
              "Route": { "Path": "/product/:id" },
              "Implementation": { "Path": "src/pages/ProductDetailsPage.vue" }
            } }
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

```json expect
{
  "claims": [
    { "kind": "page.route", "element": "product-details-page", "outcome": "contradicted", "at": "src/router.ts:9" }
  ],
  "findings": [
    { "rule": "claim/page.route", "severity": "error", "subject": "product-details-page" }
  ]
}
```

## vue-standard-page

The standard User Interface designer (2.0.0): a Page element's Page stereotype gives its Route in the model's syntax, `/products/{id}`. The stock Vue reader reads the route record that mounts the page, `/products/:id`, as `products/{id}`, so the two match.

```json case
{ "phase": 6, "kinds": ["page.route"], "requirements": ["R2", "R10", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "pages", "children": [
            { "id": "product-details-page", "type": "Page", "name": "ProductDetailsPage", "stereotypes": { "Page": { "Route": "/products/{id}" } } },
            { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/basket" } } }
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
import ProductDetailsPage from './pages/ProductDetailsPage.vue';
import CartPage from './pages/CartPage.vue';

export const routes: RouteRecordRaw[] = [
    { path: '/products/:id', component: ProductDetailsPage },
    { path: '/cart', component: CartPage },
];
```

```vue file src/pages/ProductDetailsPage.vue
<script setup lang="ts">
const title = 'Product';
</script>
```

```vue file src/pages/CartPage.vue
<script setup lang="ts">
const title = 'Cart';
</script>
```

```json expect
{
  "claims": [
    { "kind": "page.route", "element": "product-details-page", "outcome": "verified", "expected": "products/{id}", "actual": "products/{id}", "at": "src/router.ts:6" },
    { "kind": "page.route", "element": "cart-page", "outcome": "contradicted", "expected": "basket", "actual": "cart", "at": "src/router.ts:7" }
  ],
  "findings": [
    { "rule": "claim/page.route", "severity": "error", "subject": "cart-page" }
  ]
}
```

## react-standard-page

The standard User Interface designer (2.0.0) read against react-router's route objects by the stock React reader. A child route's path is joined to its parent's, an index route has its parent's path, and a route mounts a component through its `element` or its `Component`. `/products/:id` reads as `products/{id}`; the cart page is at `/cart`, not the modelled `/basket`.

```json case
{ "phase": 6, "kinds": ["page.route"], "requirements": ["R2", "R10", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "pages", "children": [
            { "id": "catalog-page", "type": "Page", "name": "CatalogPage", "stereotypes": { "Page": { "Route": "/" } } },
            { "id": "product-details-page", "type": "Page", "name": "ProductDetailsPage", "stereotypes": { "Page": { "Route": "/products/{id}" } } },
            { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/basket" } } }
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

```tsx file src/router.tsx
import { createBrowserRouter } from 'react-router-dom';
import Shell from './Shell';
import CatalogPage from './pages/CatalogPage';
import ProductDetailsPage from './pages/ProductDetailsPage';
import CartPage from './pages/CartPage';

export const router = createBrowserRouter([
    {
        path: '/',
        element: <Shell />,
        children: [
            { index: true, element: <CatalogPage /> },
            { path: 'products/:id', element: <ProductDetailsPage /> },
            { path: 'cart', Component: CartPage },
        ],
    },
]);
```

```tsx file src/Shell.tsx
import { Outlet } from 'react-router-dom';

export default function Shell() {
    return <main><Outlet /></main>;
}
```

```tsx file src/pages/CatalogPage.tsx
export default function CatalogPage() {
    return <h1>Catalog</h1>;
}
```

```tsx file src/pages/ProductDetailsPage.tsx
export default function ProductDetailsPage() {
    return <h1>Product</h1>;
}
```

```tsx file src/pages/CartPage.tsx
export default function CartPage() {
    return <h1>Cart</h1>;
}
```

```json expect
{
  "claims": [
    { "kind": "page.route", "element": "catalog-page", "outcome": "verified", "at": "src/router.tsx:12" },
    { "kind": "page.route", "element": "product-details-page", "outcome": "verified", "expected": "products/{id}", "actual": "products/{id}", "at": "src/router.tsx:13" },
    { "kind": "page.route", "element": "cart-page", "outcome": "contradicted", "expected": "basket", "actual": "cart", "at": "src/router.tsx:14" }
  ],
  "findings": [
    { "rule": "claim/page.route", "severity": "error", "subject": "cart-page" }
  ]
}
```

## react-jsx-routes

The same routes written as `<Route>` elements in a JavaScript front end. A pathless layout route nests the others, a route's `element` can wrap its page in a guard (`<RequireAuth><OrderPage /></RequireAuth>` mounts `OrderPage`), and a page can be lazily imported with `lazy(() => import(...))`. The shell hosts an `<Outlet />`, so it is the layout.

```json case
{ "phase": 6, "kinds": ["page.route", "component.kind"], "requirements": ["R2", "R10", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "Shell" },
          { "id": "home-page", "type": "Page", "name": "HomePage", "stereotypes": { "Page": { "Route": "/" } } },
          { "id": "order-page", "type": "Page", "name": "OrderPage", "stereotypes": { "Page": { "Route": "/orders/{id}" } } },
          { "id": "reports-page", "type": "Page", "name": "ReportsPage", "stereotypes": { "Page": { "Route": "/admin/reports" } } }
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

```jsx file src/App.jsx
import { lazy } from 'react';
import { Route, Routes } from 'react-router-dom';
import Shell from './Shell';
import HomePage from './HomePage';
import OrderPage from './OrderPage';
import RequireAuth from './RequireAuth';

const Reports = lazy(() => import('./ReportsPage'));

export default function App() {
    return (
        <Routes>
            <Route element={<Shell />}>
                <Route index element={<HomePage />} />
                <Route path="orders/:id" element={<RequireAuth><OrderPage /></RequireAuth>} />
                <Route path="admin">
                    <Route path="reports" element={<Reports />} />
                </Route>
            </Route>
        </Routes>
    );
}
```

```jsx file src/Shell.jsx
import { Outlet } from 'react-router-dom';

export default function Shell() {
    return <div className="shell"><Outlet /></div>;
}
```

```jsx file src/HomePage.jsx
export default function HomePage() {
    return <h1>Home</h1>;
}
```

```jsx file src/OrderPage.jsx
export default function OrderPage() {
    return <h1>Order</h1>;
}
```

```jsx file src/ReportsPage.jsx
export default function ReportsPage() {
    return <h1>Reports</h1>;
}
```

```jsx file src/RequireAuth.jsx
import { Navigate } from 'react-router-dom';

export default function RequireAuth({ children }) {
    return localStorage.getItem('token') ? children : <Navigate to="/login" />;
}
```

```json expect
{
  "claims": [
    { "kind": "component.kind", "element": "shell", "outcome": "verified", "actual": "Layout", "at": "src/Shell.jsx:4" },
    { "kind": "component.kind", "element": "home-page", "outcome": "verified", "actual": "Page", "at": "src/App.jsx:14" },
    { "kind": "page.route", "element": "home-page", "outcome": "verified", "at": "src/App.jsx:14" },
    { "kind": "page.route", "element": "order-page", "outcome": "verified", "actual": "orders/{id}", "at": "src/App.jsx:15" },
    { "kind": "page.route", "element": "reports-page", "outcome": "verified", "actual": "admin/reports", "at": "src/App.jsx:17" }
  ]
}
```

## react-computed-route

A route's path read through a constant (`ROUTES.cart`) is the string the constant holds; one the code computes from what only runs in the browser can't be read, so that page's route can't be verified rather than contradicted as missing.

```json case
{ "phase": 6, "kinds": ["page.route"], "requirements": ["R2", "R10", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart" } } },
          { "id": "promo-page", "type": "Page", "name": "PromoPage", "stereotypes": { "Page": { "Route": "/promo" } } }
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

```tsx file src/router.tsx
import { createBrowserRouter } from 'react-router-dom';
import CartPage from './CartPage';
import PromoPage from './PromoPage';

const ROUTES = { cart: '/cart' };

export const router = createBrowserRouter([
    { path: ROUTES.cart, element: <CartPage /> },
    { path: window.location.hash.slice(1), element: <PromoPage /> },
]);
```

```tsx file src/CartPage.tsx
export default function CartPage() {
    return <h1>Cart</h1>;
}
```

```tsx file src/PromoPage.tsx
export default function PromoPage() {
    return <h1>Promo</h1>;
}
```

```json expect
{
  "claims": [
    { "kind": "page.route", "element": "cart-page", "outcome": "verified", "actual": "cart", "at": "src/router.tsx:8" },
    { "kind": "page.route", "element": "promo-page", "outcome": "cantVerify", "reason": "unreadable" }
  ],
  "findings": [
    { "rule": "claim/page.route", "subject": "cart-page", "count": 0 }
  ]
}
```

## react-file-route-no-rule

A Next.js page's route is its file's path (`app/products/[id]/page.tsx`), not code, and the React reader reads only react-router's routes. With no react-router route in the code, the page's kind and route have no rule, rather than being contradicted as a plain component with no route.

```json case
{ "phase": 6, "kinds": ["page.route", "component.kind"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "product-page", "type": "Page", "name": "ProductPage", "stereotypes": { "Page": { "Route": "/products/{id}" } } }
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

```tsx file src/app/products/[id]/page.tsx
export default function ProductPage({ params }: { params: { id: string } }) {
    return <h1>Product {params.id}</h1>;
}
```

```json expect
{
  "claims": [
    { "kind": "page.route", "element": "product-page", "outcome": "cantVerify", "reason": "noRule" },
    { "kind": "component.kind", "element": "product-page", "outcome": "cantVerify", "reason": "noRule" }
  ]
}
```
