# page.route

A page's routes: the Page stereotype's Route in the User Interface designer, or the Route stereotype's Path in the Java spike's Front End designer. The kind is a set compared with `subset`, its items compared as routes (`itemComparer: route`): the model's route must be among the routes the code serves for the page, and further routes the code serves aren't drift (principle 1; claim-catalog.md, Notes on kinds). The `route` item comparer ignores a leading or trailing `/`, case in literal segments, and parameter constraints. The kind is `stated: whenSet`, so a modelled route the code doesn't state is contradicted, never taken from a convention (decision 4).

The Blazor cases use the `intent.ui` pack's vocabulary and eShopCustom's house rule. A Blazor page states its routes with one `@page` directive each, so the rule is a query over the `directive` nodes: each `@page` row is one item of its component's set (rules.md, Queries). A recipe can't do it, since a find that ends at several directives is `ambiguous`. The Vue cases use the Java spike's house pack, which gives the kind and the Front End designer's vocabulary as well, since no stock pack knows that designer.

## verified-constraint

The model's route `/item/{itemId}` and the code's `@page "/item/{itemId:int}"` differ only by the leading `/` and the parameter's constraint, which the `route` item comparer ignores.

```json case
{ "phase": 6, "kinds": ["page.route"], "traces": ["ESC-FE-12"], "requirements": ["R2", "R10", "R38", "R47"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.ui", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "rules": {
    "User Interface": {
      "files": ["src/WebApp/Components/**/*.razor"],
      "facts": {
        "page.route": {
          "query": [
            { "from": "code:directive" },
            { "where": "directive == 'page'" },
            { "reading": { "claim": { "Component": "parent.name" }, "item": "replace(value, '\"', '')", "at": "span" } }
          ],
          "note": "A directive's parent is the component its .razor file declares; each @page is one route of that component"
        }
      }
    }
  }
}
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
{ "phase": 6, "base": "verified-constraint", "without": ["src/WebApp/Components/Pages/Item/ItemPage.razor"], "kinds": ["page.route"], "cases": ["engine/comparers"], "traces": ["ESC-FE-12"], "requirements": ["R1", "R10", "R47"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
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
{ "phase": 6, "base": "verified-extra-route", "kinds": ["page.route"], "traces": ["ESC-FE-12"], "requirements": ["R2", "R11", "R14"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
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
{ "phase": 6, "base": "verified-extra-route", "kinds": ["page.route"], "traces": ["ESC-FE-12"], "requirements": ["R4"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
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

The Java spike's Vue storefront registers its pages in `router.ts`, one `{ path, component }` record each. The house recipe reads the `path` of the page's record, a set of one route; the vocabulary splits the model's Path into a set the same way. The file is read as text, whose canonical form drops the whitespace around punctuation (code-reader.md, Canonical text).

```json case
{ "phase": 6, "kinds": ["page.route"], "traces": ["JAV-FE-04"], "requirements": ["R2", "R28", "R38"], "packs": ["intent.core", "intent.java", "house.shop"], "language": "typescript" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.java", "version": "^1.0.0" }],
  "languages": ["typescript"],
  "designers": ["Front End"],
  "kinds": [
    { "kind": "page.route", "label": "page route", "valueType": "set", "comparer": "subset", "itemComparer": "route", "stated": "whenSet", "existence": false }
  ],
  "vocabulary": {
    "Front End": {
      "entries": [
        {
          "kind": "page.route",
          "for": { "element": "Page" },
          "value": "split(stereotype('Route').Path, ',')",
          "bindings": { "Page": "name" },
          "own": ["Page"],
          "source": "the Route stereotype's Path"
        }
      ]
    }
  },
  "rules": {
    "Front End": {
      "files": ["src/**/*.ts", "src/**/*.vue"],
      "facts": {
        "page.route": {
          "find": "file:src/router.ts",
          "read": [{ "match": "\\{path:'([^']*)',component:{Page}\\}" }],
          "unstated": "no `{ path, component: {Page} }` record in `src/router.ts`"
        }
      }
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

```json expect
{
  "claims": [
    { "kind": "page.route", "element": "product-details-page", "outcome": "verified", "at": "src/router.ts:9" }
  ]
}
```

## java-contradicted

The model's Route Path says `/product/:id`, and the router mounts the page at `/products/:id`.

```json case
{ "phase": 6, "base": "java-verified", "kinds": ["page.route"], "traces": ["JAV-FE-04"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.java", "house.shop"], "language": "typescript" }
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
