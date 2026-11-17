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
