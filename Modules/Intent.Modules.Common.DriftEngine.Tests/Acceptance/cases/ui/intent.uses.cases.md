# intent.uses

A component renders, or uses, another: a Composition, or a Display Component in a Layout's header, body, sider or footer, in the User Interface designer; a Rendered Component or a Uses association in the Java spike's Front End designer. It is a `presence` kind, an existence kind, `stated: always`: the code either renders the target or doesn't. The claim sits on the model feature that states it: the association for a Composition or Uses, the element for a Display Component or Rendered Component (vocabulary.md, A vocabulary entry).

The Blazor cases use the `intent.ui` pack's vocabulary, and the Blazor reader finds the target's tag in the rendering component's markup. A found component that doesn't render the target says `absent` from silence. The Vue cases use the Java spike's house pack, which gives the Front End designer's vocabulary too.

## verified-composition

eShopCustom's catalog page renders a `CatalogListItem` for each item, and the model has a Composition from the page to that component, which lives in another package.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "traces": ["ESC-FE-27"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [
        {
          "id": "webapp", "name": "WebApp",
          "elements": [
            { "id": "pages", "type": "Folder", "name": "Pages",
              "children": [
                { "id": "catalog-page", "type": "Component", "name": "Catalog",
                  "stereotypes": { "Page": { "Route": "/" } } }
              ] }
          ]
        },
        {
          "id": "webappcomponents", "name": "WebAppComponents",
          "elements": [
            { "id": "catalog", "type": "Folder", "name": "Catalog",
              "children": [
                { "id": "catalog-list-item", "type": "Component", "name": "CatalogListItem" }
              ] }
          ]
        }
      ],
      "associations": [
        { "id": "catalog-renders-list-item", "type": "Composition",
          "source": { "element": "catalog-page" }, "target": { "element": "catalog-list-item" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebApp/Components/Pages/Catalog/Catalog.razor
@page "/"
@inject CatalogService CatalogService

<PageTitle>AdventureWorks</PageTitle>

<div class="catalog-items">
    @foreach (var item in catalogResult.Data)
    {
        <CatalogListItem Item="@item" />
    }
</div>
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "catalog-renders-list-item", "outcome": "verified",
      "expected": "present", "actual": "present", "at": "src/WebApp/Components/Pages/Catalog/Catalog.razor:9" }
  ]
}
```

## contradicted-composition-missing

eShopCustom's `composition.missing`: the catalog page now renders its items inline, and the model's Composition still says it renders `CatalogListItem`. The page is found and its markup has no `<CatalogListItem>`, so its silence says `absent`.

```json case
{ "phase": 6, "base": "verified-composition", "kinds": ["intent.uses"], "cases": ["engine/silence"], "traces": ["ESC-FE-27"], "requirements": ["R3", "R8", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Catalog/Catalog.razor
@page "/"
@inject CatalogService CatalogService

<PageTitle>AdventureWorks</PageTitle>

<div class="catalog-items">
    @foreach (var item in catalogResult.Data)
    {
        <div class="catalog-item">@item.Name</div>
    }
</div>
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "catalog-renders-list-item", "outcome": "contradicted",
      "expected": "present", "actual": "none", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "catalog-renders-list-item", "messageContains": "CatalogListItem" }
  ]
}
```

## layout-display-components

The designer has no Composition from a Layout, so what eShopCustom's main layout renders is modelled as Display Components in its Layout Header and Layout Footer; eShopCustom only warned about them. Here each is a claim on its Display Component: the header bar is rendered, and the footer bar, which the layout no longer renders, is contradicted.

```json case
{ "phase": 6, "base": "verified-composition", "without": ["src/WebApp/Components/Pages/Catalog/Catalog.razor"], "kinds": ["intent.uses"], "traces": ["ESC-FE-69"], "requirements": ["R2", "R3", "R8", "R30"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
              { "id": "header-bar", "type": "Component", "name": "HeaderBar" },
              { "id": "footer-bar", "type": "Component", "name": "FooterBar" },
              { "id": "main-layout", "type": "Layout", "name": "MainLayout",
                "children": [
                  { "id": "main-layout-header", "type": "Layout Header", "name": "Header",
                    "children": [
                      { "id": "main-layout-header-bar", "type": "Display Component", "name": "HeaderBar", "typeRef": { "element": "header-bar" } }
                    ] },
                  { "id": "main-layout-footer", "type": "Layout Footer", "name": "Footer",
                    "children": [
                      { "id": "main-layout-footer-bar", "type": "Display Component", "name": "FooterBar", "typeRef": { "element": "footer-bar" } }
                    ] }
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

```razor file src/WebApp/Components/Layout/MainLayout.razor
@inherits LayoutComponentBase

<HeaderBar />

<main>
    @Body
</main>
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "main-layout-header-bar", "outcome": "verified",
      "at": "src/WebApp/Components/Layout/MainLayout.razor:3" },
    { "kind": "intent.uses", "element": "main-layout-footer-bar", "outcome": "contradicted",
      "expected": "present", "actual": "none", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "main-layout-footer-bar", "messageContains": "FooterBar" }
  ]
}
```

## java-rendered-component

The Java spike's catalog page renders a product image and a stock pill, the second in kebab case as Vue templates allow. Each Rendered Component is a claim, found by its PascalCase or kebab-case tag inside the page's template. The Front End designer is the Java spike's own, which no stock pack knows, so its house pack extends `intent.ui`: it claims where its pages and components are (`type.exists`, from each one's Implementation Path) and the stock `intent.uses` on its Rendered Component, a child element typed by the component it renders. The stock Vue reader (`intent.vue`) reads the page's template. A component the template doesn't render is contradicted.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "traces": ["JAV-FE-05"], "requirements": ["R2", "R3", "R28", "R38"], "packs": ["intent.core", "intent.ui", "house.shop"], "language": "typescript" }
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
          "kind": "type.exists",
          "for": { "element": "Component" },
          "value": "self",
          "bindings": { "Name": "name", "Component": "name", "Path": "stereotype('Implementation').Path" },
          "own": ["Name"],
          "source": "the component's Implementation Path"
        },
        {
          "kind": "intent.uses",
          "for": { "element": "Rendered Component" },
          "value": "'present'",
          "bindings": { "Child": "type.name" },
          "own": ["Child"],
          "source": "the Rendered Component"
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
          { "id": "product-image", "type": "Component", "name": "ProductImage",
            "stereotypes": { "Implementation": { "Path": "src/components/ProductImage.vue" } } },
          { "id": "stock-pill", "type": "Component", "name": "StockPill",
            "stereotypes": { "Implementation": { "Path": "src/components/StockPill.vue" } } },
          { "id": "price-tag", "type": "Component", "name": "PriceTag",
            "stereotypes": { "Implementation": { "Path": "src/components/PriceTag.vue" } } },
          { "id": "catalog-page", "type": "Page", "name": "CatalogPage",
            "stereotypes": {
              "Route": { "Path": "/" },
              "Implementation": { "Path": "src/pages/CatalogPage.vue" }
            },
            "children": [
              { "id": "catalog-page-product-image", "type": "Rendered Component", "name": "ProductImage", "typeRef": { "element": "product-image" } },
              { "id": "catalog-page-stock-pill", "type": "Rendered Component", "name": "StockPill", "typeRef": { "element": "stock-pill" } },
              { "id": "catalog-page-price-tag", "type": "Rendered Component", "name": "PriceTag", "typeRef": { "element": "price-tag" } }
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

```vue file src/pages/CatalogPage.vue
<script setup lang="ts">
import { RouterLink } from 'vue-router';
import ProductImage from '../components/ProductImage.vue';
import StockPill from '../components/StockPill.vue';

defineProps<{ products: { Id: string; Name: string; InStock: boolean }[] }>();
</script>

<template>
    <ul class="catalog">
        <li v-for="product in products" :key="product.Id">
            <RouterLink :to="`/products/${product.Id}`">
                <ProductImage :product-id="product.Id" />
            </RouterLink>
            <stock-pill :in-stock="product.InStock" />
        </li>
    </ul>
</template>
```

```vue file src/components/ProductImage.vue
<script setup lang="ts">
defineProps<{ productId: string }>();
</script>
```

```vue file src/components/StockPill.vue
<script setup lang="ts">
defineProps<{ inStock: boolean }>();
</script>
```

```vue file src/components/PriceTag.vue
<script setup lang="ts">
defineProps<{ price: number }>();
</script>
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "catalog-page-product-image", "outcome": "verified", "at": "src/pages/CatalogPage.vue:13" },
    { "kind": "intent.uses", "element": "catalog-page-stock-pill", "outcome": "verified", "at": "src/pages/CatalogPage.vue:15" },
    { "kind": "intent.uses", "element": "catalog-page-price-tag", "outcome": "contradicted", "expected": "present" }
  ]
}
```

## java-uses-import

The Java spike's cart page uses the cart store: a Uses association, which the code states as a relative value import of the store's file. A type-only import (`import type`) uses no value at runtime, so the checkout page, which imports only the store's types, is contradicted. The house pack claims where the pages and the store are, and the stock Vue reader (`intent.vue`) reads the page's imports of the store's file.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "traces": ["JAV-FE-07"], "requirements": ["R2", "R3", "R28", "R38"], "packs": ["intent.core", "intent.ui", "house.shop"], "language": "typescript" }
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
          "kind": "type.exists",
          "for": { "element": "Store" },
          "value": "self",
          "bindings": { "Name": "name", "Store": "name", "Path": "stereotype('Implementation').Path" },
          "own": ["Name"],
          "source": "the store's Implementation Path"
        },
        {
          "kind": "intent.uses",
          "for": { "association": "Uses" },
          "value": "'present'",
          "bindings": { "Used": "target.element.name" },
          "own": ["Used"],
          "source": "the Uses association"
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
          { "id": "cart-store", "type": "Store", "name": "cart",
            "stereotypes": { "Implementation": { "Path": "src/stores/cart.ts" } } },
          { "id": "cart-page", "type": "Page", "name": "CartPage",
            "stereotypes": {
              "Route": { "Path": "/cart" },
              "Implementation": { "Path": "src/pages/CartPage.vue" }
            } },
          { "id": "checkout-page", "type": "Page", "name": "CheckoutPage",
            "stereotypes": {
              "Route": { "Path": "/checkout" },
              "Implementation": { "Path": "src/pages/CheckoutPage.vue" }
            } }
        ]
      }],
      "associations": [
        { "id": "cart-page-uses-cart-store", "type": "Uses",
          "source": { "element": "cart-page" }, "target": { "element": "cart-store" } },
        { "id": "checkout-page-uses-cart-store", "type": "Uses",
          "source": { "element": "checkout-page" }, "target": { "element": "cart-store" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```vue file src/pages/CartPage.vue
<script setup lang="ts">
import { RouterLink } from 'vue-router';
import { MAX_QUANTITY, clampQuantity, useCartStore } from '../stores/cart';

const cart = useCartStore();
</script>

<template>
    <section class="cart">
        <RouterLink to="/checkout" class="btn btn-block">Proceed to checkout</RouterLink>
    </section>
</template>
```

```vue file src/pages/CheckoutPage.vue
<script setup lang="ts">
import type { CartLine } from '../stores/cart';

defineProps<{ lines: CartLine[] }>();
</script>

<template>
    <section class="checkout">
        <RouterLink to="/cart" class="back-link">Back to cart</RouterLink>
    </section>
</template>
```

```typescript file src/stores/cart.ts
import { defineStore } from 'pinia';

export interface CartLine {
    ProductId: string;
    Quantity: number;
}

export const MAX_QUANTITY = 99;

export function clampQuantity(value: number): number {
    return Math.min(value, MAX_QUANTITY);
}

export const useCartStore = defineStore('cart', () => ({ lines: [] as CartLine[] }));
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "cart-store", "outcome": "verified", "at": "src/stores/cart.ts:14" },
    { "kind": "intent.uses", "element": "cart-page-uses-cart-store", "outcome": "verified", "at": "src/pages/CartPage.vue:3" },
    { "kind": "intent.uses", "element": "checkout-page-uses-cart-store", "outcome": "contradicted", "expected": "present" }
  ]
}
```

## vue-standard-uses-and-composition

The standard User Interface designer (2.0.0) draws a page placing a component as a Composition, and a page depending on a store or a utility as a Uses association. The stock Vue reader reads a Composition as the child's tag in the page's template, resolving to the component the child is located as, and a Uses as a value import of the file the store or utility is located in: the Pinia store by its `defineStore` id, the utility by its file's name in the folder its designer folders name. The catalog page imports the store but not the utility, so that Uses is contradicted.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "requirements": ["R2", "R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "pages", "children": [
            { "id": "catalog-page", "type": "Page", "name": "CatalogPage", "stereotypes": { "Page": { "Route": "/" } } }
          ] },
          { "id": "components", "type": "Folder", "name": "components", "children": [
            { "id": "stock-pill", "type": "Component", "name": "StockPill" }
          ] },
          { "id": "stores", "type": "Folder", "name": "stores", "children": [
            { "id": "cart", "type": "Store", "name": "Cart" }
          ] },
          { "id": "utils", "type": "Folder", "name": "utils", "children": [
            { "id": "format", "type": "Utility", "name": "Format" }
          ] }
        ]
      }],
      "associations": [
        { "id": "catalog-places-stock-pill", "type": "Composition", "source": { "element": "catalog-page" }, "target": { "element": "stock-pill" } },
        { "id": "catalog-uses-cart", "type": "Uses", "source": { "element": "catalog-page" }, "target": { "element": "cart" } },
        { "id": "catalog-uses-format", "type": "Uses", "source": { "element": "catalog-page" }, "target": { "element": "format" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```vue file src/pages/CatalogPage.vue
<script setup lang="ts">
import StockPill from '../components/StockPill.vue';
import { useCartStore } from '../stores/cart';

const cart = useCartStore();
</script>

<template>
    <StockPill :in-stock="cart.lines.length > 0" />
</template>
```

```vue file src/components/StockPill.vue
<script setup lang="ts">
defineProps<{ inStock: boolean }>();
</script>
```

```typescript file src/stores/cart.ts
import { defineStore } from 'pinia';

export const useCartStore = defineStore('cart', () => ({ lines: [] as string[] }));
```

```typescript file src/utils/format.ts
export function formatMoney(value: number): string {
    return `$${value.toFixed(2)}`;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "cart", "outcome": "verified", "at": "src/stores/cart.ts:3" },
    { "kind": "type.exists", "element": "format", "outcome": "verified", "at": "src/utils/format.ts:1" },
    { "kind": "intent.uses", "element": "catalog-places-stock-pill", "outcome": "verified", "at": "src/pages/CatalogPage.vue:9" },
    { "kind": "intent.uses", "element": "catalog-uses-cart", "outcome": "verified", "at": "src/pages/CatalogPage.vue:3" },
    { "kind": "intent.uses", "element": "catalog-uses-format", "outcome": "contradicted", "expected": "present" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "catalog-uses-format" },
    { "rule": "claim/intent.uses", "subject": "catalog-uses-cart", "count": 0 }
  ]
}
```

## react-standard-uses-and-composition

The same associations read against React by the stock React reader. A Composition is the child's tag in the page's JSX, here inside a `.map(...)` and resolving through an aliased import; a Uses is a value import of the file the store or utility is located in. The store is a Redux Toolkit slice, located by its `createSlice` name in `cartSlice.ts`; the page imports its selector. A Uses of a component the page doesn't render, but imports to hand on as a value, holds; a Composition needs the tag, so the page's Composition of the banner it only imports is contradicted, as is the Uses of the utility it doesn't import.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "requirements": ["R2", "R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "pages", "children": [
            { "id": "catalog-page", "type": "Page", "name": "CatalogPage", "stereotypes": { "Page": { "Route": "/" } } }
          ] },
          { "id": "components", "type": "Folder", "name": "components", "children": [
            { "id": "stock-pill", "type": "Component", "name": "StockPill" },
            { "id": "empty-state", "type": "Component", "name": "EmptyState" },
            { "id": "promo-banner", "type": "Component", "name": "PromoBanner" }
          ] },
          { "id": "features", "type": "Folder", "name": "features", "children": [
            { "id": "cart-folder", "type": "Folder", "name": "cart", "children": [
              { "id": "cart", "type": "Store", "name": "Cart" }
            ] }
          ] },
          { "id": "utils", "type": "Folder", "name": "utils", "children": [
            { "id": "format", "type": "Utility", "name": "Format" }
          ] }
        ]
      }],
      "associations": [
        { "id": "catalog-places-stock-pill", "type": "Composition", "source": { "element": "catalog-page" }, "target": { "element": "stock-pill" } },
        { "id": "catalog-places-promo-banner", "type": "Composition", "source": { "element": "catalog-page" }, "target": { "element": "promo-banner" } },
        { "id": "catalog-uses-empty-state", "type": "Uses", "source": { "element": "catalog-page" }, "target": { "element": "empty-state" } },
        { "id": "catalog-uses-cart", "type": "Uses", "source": { "element": "catalog-page" }, "target": { "element": "cart" } },
        { "id": "catalog-uses-format", "type": "Uses", "source": { "element": "catalog-page" }, "target": { "element": "format" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```tsx file src/pages/CatalogPage.tsx
import { useSelector } from 'react-redux';
import { StockPill as Pill } from '../components/StockPill';
import { EmptyState } from '../components/EmptyState';
import { PromoBanner } from '../components/PromoBanner';
import { selectLines } from '../features/cart/cartSlice';
import { List } from './List';

export default function CatalogPage({ products }: { products: { id: string; inStock: boolean }[] }) {
    const lines = useSelector(selectLines);
    return (
        <List empty={EmptyState} banner={lines.length > 0 ? PromoBanner : null}>
            {products.map((p) => <Pill key={p.id} inStock={p.inStock} />)}
        </List>
    );
}
```

```tsx file src/pages/List.tsx
import type { ComponentType, ReactNode } from 'react';

export function List({ empty: Empty, children }: { empty: ComponentType; banner: ComponentType | null; children: ReactNode }) {
    return children ? <ul>{children}</ul> : <Empty />;
}
```

```tsx file src/components/StockPill.tsx
export function StockPill({ inStock }: { inStock: boolean }) {
    return <span>{inStock ? 'In stock' : 'Sold out'}</span>;
}
```

```tsx file src/components/EmptyState.tsx
export function EmptyState() {
    return <p>Nothing here</p>;
}
```

```tsx file src/components/PromoBanner.tsx
export function PromoBanner() {
    return <aside>Sale</aside>;
}
```

```ts file src/features/cart/cartSlice.ts
import { createSlice } from '@reduxjs/toolkit';

const cartSlice = createSlice({
    name: 'cart',
    initialState: { lines: [] as string[] },
    reducers: {
        added(state, action) {
            state.lines.push(action.payload);
        },
    },
});

export const selectLines = (state: { cart: { lines: string[] } }) => state.cart.lines;
export default cartSlice.reducer;
```

```ts file src/utils/format.ts
export function formatMoney(value: number): string {
    return `$${value.toFixed(2)}`;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "cart", "outcome": "verified", "at": "src/features/cart/cartSlice.ts:3" },
    { "kind": "type.exists", "element": "format", "outcome": "verified", "at": "src/utils/format.ts:1" },
    { "kind": "intent.uses", "element": "catalog-places-stock-pill", "outcome": "verified", "at": "src/pages/CatalogPage.tsx:12" },
    { "kind": "intent.uses", "element": "catalog-uses-empty-state", "outcome": "verified", "at": "src/pages/CatalogPage.tsx:3" },
    { "kind": "intent.uses", "element": "catalog-uses-cart", "outcome": "verified", "at": "src/pages/CatalogPage.tsx:5" },
    { "kind": "intent.uses", "element": "catalog-places-promo-banner", "outcome": "contradicted", "expected": "present" },
    { "kind": "intent.uses", "element": "catalog-uses-format", "outcome": "contradicted", "expected": "present" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "catalog-places-promo-banner" },
    { "rule": "claim/intent.uses", "severity": "error", "subject": "catalog-uses-format" },
    { "rule": "claim/intent.uses", "subject": "catalog-uses-cart", "count": 0 }
  ]
}
```

## react-component-not-a-module

A plain TypeScript module beside the components isn't a component, so the React reader leaves it alone: a Uses from a utility is read from its code by the readers every TypeScript front end shares, as an import, whatever the framework. `checkout.ts` imports the cart store but not the formatting utility.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "requirements": ["R2", "R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "stores", "type": "Folder", "name": "stores", "children": [
            { "id": "cart", "type": "Store", "name": "Cart" }
          ] },
          { "id": "utils", "type": "Folder", "name": "utils", "children": [
            { "id": "checkout", "type": "Utility", "name": "Checkout" },
            { "id": "format", "type": "Utility", "name": "Format" }
          ] }
        ]
      }],
      "associations": [
        { "id": "checkout-uses-cart", "type": "Uses", "source": { "element": "checkout" }, "target": { "element": "cart" } },
        { "id": "checkout-uses-format", "type": "Uses", "source": { "element": "checkout" }, "target": { "element": "format" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```ts file src/stores/useCartStore.ts
import { create } from 'zustand';

export const useCartStore = create<{ lines: string[] }>()(() => ({ lines: [] }));
```

```ts file src/utils/checkout.ts
import { useCartStore } from '../stores/useCartStore';

export function checkout(): string[] {
    return useCartStore.getState().lines;
}
```

```ts file src/utils/format.ts
export const format = (value: number) => value.toFixed(2);
```

```tsx file src/CartBadge.tsx
import { useCartStore } from './stores/useCartStore';

export function CartBadge() {
    const lines = useCartStore((s) => s.lines);
    return <span>{lines.length}</span>;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "cart", "outcome": "verified", "at": "src/stores/useCartStore.ts:3" },
    { "kind": "intent.uses", "element": "checkout-uses-cart", "outcome": "verified", "at": "src/utils/checkout.ts:1" },
    { "kind": "intent.uses", "element": "checkout-uses-format", "outcome": "contradicted", "expected": "present" }
  ]
}
```
