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

## blazor-behaviour-acts-on-markup

A Behaviour is reusable behaviour attached to an element, and the component that applies it has a Uses to it (Intent.Modelers.UI 2.0.0). eShop's chat dock is a script `App.razor` loads for the whole application, which finds the elements it acts on through the selectors it passes to the DOM; the chatbot button carries `data-chat-toggle`, so the button's component applies it. The Uses vocabulary binds `Behaviour` for a Behaviour target, so the Blazor reader reads the script's selectors: only one naming a class, an id or a `data-` attribute marks an element (`form` doesn't).

```json case
{ "phase": 6, "kinds": ["intent.uses"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "chat-dock", "type": "Behaviour", "name": "chat-dock" },
          { "id": "show-chatbot-button", "type": "Component", "name": "ShowChatbotButton" }
        ]
      }],
      "associations": [
        { "id": "button-uses-chat-dock", "type": "Uses", "source": { "element": "show-chatbot-button" }, "target": { "element": "chat-dock" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebApp/Components/App.razor
<!DOCTYPE html>
<html lang="en">
<body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
    <script src="js/chat-dock.js"></script>
</body>
</html>
```

```razor file src/WebApp/Components/Chatbot/ShowChatbotButton.razor
<div class="chatbot-launcher">
    <button type="button" class="show-chatbot" data-chat-toggle aria-label="Ask">Ask</button>
</div>
```

```js file src/WebApp/wwwroot/js/chat-dock.js
(function () {
    const dock = () => document.querySelector('[data-chat-dock]');
    document.addEventListener('click', (e) => {
        if (e.target.closest('[data-chat-toggle]')) { dock().hidden = !dock().hidden; }
        const form = e.target.closest('form');
    });
})();
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "chat-dock", "outcome": "verified" },
    { "kind": "intent.uses", "element": "button-uses-chat-dock", "outcome": "verified", "expected": "present", "actual": "present", "at": "src/WebApp/Components/Chatbot/ShowChatbotButton.razor:2" }
  ],
  "noOtherFindings": true
}
```

## blazor-behaviour-markup-not-marked

A planted drift: the button no longer carries `data-chat-toggle`. The script is still loaded, but no element of the component's markup matches a selector it acts on: one finding, naming the selectors.

```json case
{ "phase": 6, "base": "blazor-behaviour-acts-on-markup", "kinds": ["intent.uses"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Chatbot/ShowChatbotButton.razor
<div class="chatbot-launcher">
    <button type="button" class="show-chatbot" aria-label="Ask">Ask</button>
</div>
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "button-uses-chat-dock", "outcome": "contradicted", "expected": "present", "actual": "none" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "button-uses-chat-dock", "messageContains": "`src/WebApp/wwwroot/js/chat-dock.js`, loaded for the application by `<script src=\"js/chat-dock.js\">` in `App`, acts on `[data-chat-dock]`, `[data-chat-toggle]`, and no element of `ShowChatbotButton`'s markup matches." }
  ],
  "noOtherFindings": true
}
```

## blazor-behaviour-script-not-loaded

A planted drift: `App.razor` no longer loads the chat dock. Nothing imports it either, so the Uses is one finding.

```json case
{ "phase": 6, "base": "blazor-behaviour-acts-on-markup", "kinds": ["intent.uses"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/App.razor
<!DOCTYPE html>
<html lang="en">
<body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "button-uses-chat-dock", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "button-uses-chat-dock", "messageContains": "and no `<script src>` loads it for the application." }
  ],
  "noOtherFindings": true
}
```

## blazor-behaviour-tags-and-aria-mark-nothing

What isn't a mark states nothing: this script finds its elements only by a bare tag (`textarea`) and an ARIA attribute (`[role="option"]`), which match any markup of their kind, so a component's `<textarea>` and `role="option"` don't show it applies the behaviour.

```json case
{ "phase": 6, "base": "blazor-behaviour-acts-on-markup", "kinds": ["intent.uses"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Chatbot/ShowChatbotButton.razor
<div class="chatbot-launcher">
    <textarea></textarea>
    <span role="option">Ask</span>
</div>
```

```js file src/WebApp/wwwroot/js/chat-dock.js
(function () {
    document.addEventListener('keydown', (e) => {
        const box = document.querySelector('textarea');
        const option = e.target.closest('[role="option"]');
    });
})();
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "button-uses-chat-dock", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "button-uses-chat-dock", "messageContains": "acts on no element by a selector naming a class, an id or a data- attribute" }
  ]
}
```

## blazor-module-imported-through-js-interop

The chatbot imports its collocated module through JS interop, the path relative to the application's root. The Behaviour's Implementation Path names the module's file, so the import is the Uses.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "chatbot-module", "type": "Behaviour", "name": "Chatbot.razor", "stereotypes": { "Implementation": { "Path": "Components/Chatbot/Chatbot.razor.js" } } },
          { "id": "chatbot", "type": "Component", "name": "Chatbot" }
        ]
      }],
      "associations": [
        { "id": "chatbot-uses-module", "type": "Uses", "source": { "element": "chatbot" }, "target": { "element": "chatbot-module" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebApp/Components/Chatbot/Chatbot.razor
@inject IJSRuntime JS

<div class="chatbot"><textarea @ref="textbox"></textarea></div>

@code {
    private ElementReference textbox;
    private IJSObjectReference? jsModule;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Chatbot/Chatbot.razor.js");
        await jsModule.InvokeVoidAsync("submitOnEnter", textbox);
    }
}
```

```js file src/WebApp/Components/Chatbot/Chatbot.razor.js
export function submitOnEnter(element) {
    element.addEventListener('keydown', event => { });
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "chatbot-module", "outcome": "verified" },
    { "kind": "intent.uses", "element": "chatbot-uses-module", "outcome": "verified", "at": "src/WebApp/Components/Chatbot/Chatbot.razor:11" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "count": 0 }
  ]
}
```

## blazor-module-imported-elsewhere

A planted drift: the chatbot imports another module. The Behaviour's module isn't imported and no script tag loads it: one finding.

```json case
{ "phase": 6, "base": "blazor-module-imported-through-js-interop", "kinds": ["intent.uses"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Chatbot/Chatbot.razor
@inject IJSRuntime JS

<div class="chatbot"><textarea @ref="textbox"></textarea></div>

@code {
    private ElementReference textbox;
    private IJSObjectReference? jsModule;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Chatbot/Other.razor.js");
        await jsModule.InvokeVoidAsync("submitOnEnter", textbox);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "chatbot-uses-module", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "chatbot-uses-module", "messageContains": "doesn't import `src/WebApp/Components/Chatbot/Chatbot.razor.js` through JS interop" }
  ]
}
```

## view-model-uses-store

A View Model is a stateful class one element creates for its own use; it is located as the class of its name, and its Properties are its members. Its Uses of a Store is a dependency of the class on the Store's type: here a field and constructor parameter of `IBasketState`, the interface the located `BasketState` implements.

```json case
{ "phase": 6, "kinds": ["intent.uses", "type.exists", "member.exists"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "basket-state", "type": "Store", "name": "BasketState" },
          { "id": "chat-state", "type": "View Model", "name": "ChatState", "children": [
            { "id": "chat-state-messages", "type": "Property", "name": "Messages", "typeRef": { "element": "chat-message", "collection": true } }
          ] },
          { "id": "chat-message", "type": "Contract", "name": "ChatMessage", "children": [
            { "id": "chat-message-text", "type": "Property", "name": "Text", "typeRef": { "type": "string" } }
          ] }
        ]
      }],
      "associations": [
        { "id": "chat-state-uses-basket", "type": "Uses", "source": { "element": "chat-state" }, "target": { "element": "basket-state" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/WebApp/Services/BasketState.cs
namespace eShop.WebApp.Services;

public interface IBasketState
{
    Task AddAsync(int id);
}

public class BasketState : IBasketState, IDisposable
{
    Task IBasketState.AddAsync(int id) => Task.CompletedTask;

    public void Dispose() { }
}
```

```csharp file src/WebApp/Components/Chatbot/ChatState.cs
using eShop.WebApp.Services;

namespace eShop.WebApp.Components.Chatbot;

public record ChatMessage(string Text);

public class ChatState
{
    private readonly IBasketState _basketState;

    public ChatState(IBasketState basketState)
    {
        _basketState = basketState;
    }

    public IList<ChatMessage> Messages { get; } = new List<ChatMessage>();

    public Task AddUserMessageAsync(string text) => _basketState.AddAsync(1);
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "chat-state", "outcome": "verified", "at": "src/WebApp/Components/Chatbot/ChatState.cs:7" },
    { "kind": "member.exists", "element": "chat-state-messages", "outcome": "verified", "at": "src/WebApp/Components/Chatbot/ChatState.cs:16" },
    { "kind": "intent.uses", "element": "chat-state-uses-basket", "outcome": "verified", "expected": "present", "actual": "present", "at": "src/WebApp/Components/Chatbot/ChatState.cs:9" }
  ],
  "noOtherFindings": true
}
```

## view-model-holds-only-a-library-interface

A planted drift: the View Model no longer takes the basket, only an `IDisposable`. `BasketState` implements `IDisposable` too, but a library interface stands for any type, so it doesn't show the dependency: one finding.

```json case
{ "phase": 6, "base": "view-model-uses-store", "kinds": ["intent.uses"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/WebApp/Components/Chatbot/ChatState.cs
namespace eShop.WebApp.Components.Chatbot;

public record ChatMessage(string Text);

public class ChatState(IDisposable subscription)
{
    private readonly IDisposable _subscription = subscription;

    public IList<ChatMessage> Messages { get; } = new List<ChatMessage>();
}
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "chat-state-uses-basket", "outcome": "contradicted", "expected": "present", "actual": "none" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "chat-state-uses-basket", "messageContains": "`ChatState` uses `BasketState`: in the model, not in the code. No field, property or constructor parameter of `ChatState` is a `BasketState` or an interface of it, and it creates none." }
  ],
  "noOtherFindings": true
}
```

## view-model-creates-what-it-uses

The designer's Uses is also "an instance it creates": a View Model making the tree nodes it presents uses their View Model, though it holds none in a field.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "requirements": ["R2", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "catalog-tree", "type": "View Model", "name": "CatalogTreeViewModel" },
          { "id": "catalog-node", "type": "View Model", "name": "CatalogNodeViewModel" }
        ]
      }],
      "associations": [
        { "id": "tree-uses-node", "type": "Uses", "source": { "element": "catalog-tree" }, "target": { "element": "catalog-node" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/WebApp/Catalog/CatalogTreeViewModel.cs
namespace eShop.WebApp.Catalog;

public class CatalogNodeViewModel(string name)
{
    public string Name { get; } = name;
}

public class CatalogTreeViewModel
{
    public IEnumerable<object> Build(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            yield return new CatalogNodeViewModel(name);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "tree-uses-node", "outcome": "verified", "at": "src/WebApp/Catalog/CatalogTreeViewModel.cs:14" }
  ]
}
```

## mvc-composition

ASP.NET Core MVC views, read by the MVC reader (`intent.mvc`). A view composes what it renders: a partial (`<partial name="_Nav" />`, `Html.PartialAsync("_Promotions")`), found as MVC finds it, in the view's folder and then `Shared`, and a view component (`<vc:cart-summary />`, the `CartSummary` class). The layout places its pages through its body's Region, which `_ViewStart.cshtml` makes every view's layout. A view's Uses of a service is what it injects (`@inject`).

```json case
{ "phase": "later", "kinds": ["intent.uses", "intent.target"], "requirements": ["R2", "R28", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop.Web", "type": "User Interface Package",
        "elements": [
          { "id": "views", "type": "Folder", "name": "Views", "children": [
            { "id": "home", "type": "Folder", "name": "Home", "children": [
              { "id": "index", "type": "Page", "name": "Index", "stereotypes": { "Page": { "Route": "/" } } },
              { "id": "promotions", "type": "Component", "name": "_Promotions" }
            ] },
            { "id": "shared", "type": "Folder", "name": "Shared", "children": [
              { "id": "layout", "type": "Layout", "name": "_Layout", "children": [
                { "id": "main", "type": "Region", "name": "Main" }
              ] },
              { "id": "nav", "type": "Component", "name": "_Nav" }
            ] }
          ] },
          { "id": "cart-summary", "type": "Component", "name": "CartSummary" },
          { "id": "cart-service", "type": "UI Service", "name": "CartService" }
        ]
      }],
      "associations": [
        { "id": "main-shows-index", "type": "Composition", "source": { "element": "main" }, "target": { "element": "index" } },
        { "id": "layout-nav", "type": "Composition", "source": { "element": "layout" }, "target": { "element": "nav" } },
        { "id": "layout-cart", "type": "Composition", "source": { "element": "layout" }, "target": { "element": "cart-summary" } },
        { "id": "index-promotions", "type": "Composition", "source": { "element": "index" }, "target": { "element": "promotions" } },
        { "id": "index-uses-cart", "type": "Uses", "source": { "element": "index" }, "target": { "element": "cart-service" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```xml file src/Shop.Web/Shop.Web.csproj
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
```

```csharp file src/Shop.Web/Program.cs
var app = WebApplication.CreateBuilder(args).Build();
app.MapDefaultControllerRoute();
app.Run();
```

```csharp file src/Shop.Web/Controllers/HomeController.cs
namespace Shop.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
```

```csharp file src/Shop.Web/ViewComponents/CartSummary.cs
namespace Shop.Web.ViewComponents;

public class CartSummary : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
```

```csharp file src/Shop.Web/Services/CartService.cs
namespace Shop.Web.Services;

public class CartService
{
    public int Count() => 0;
}
```

```cshtml file src/Shop.Web/Views/_ViewStart.cshtml
@{
    Layout = "_Layout";
}
```

```cshtml file src/Shop.Web/Views/Shared/_Layout.cshtml
<!DOCTYPE html>
<html lang="en">
<body>
    <partial name="_Nav" />
    <vc:cart-summary />
    <main>@RenderBody()</main>
</body>
</html>
```

```cshtml file src/Shop.Web/Views/Shared/_Nav.cshtml
<nav><a href="~/">Shop</a></nav>
```

```cshtml file src/Shop.Web/Views/Home/Index.cshtml
@inject Shop.Web.Services.CartService Cart
<h1>Welcome</h1>
@await Html.PartialAsync("_Promotions")
```

```cshtml file src/Shop.Web/Views/Home/_Promotions.cshtml
<section class="promotions"></section>
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "main-shows-index", "outcome": "verified", "at": "src/Shop.Web/Views/_ViewStart.cshtml:2" },
    { "kind": "intent.uses", "element": "layout-nav", "outcome": "verified", "at": "src/Shop.Web/Views/Shared/_Layout.cshtml:4" },
    { "kind": "intent.uses", "element": "layout-cart", "outcome": "verified", "at": "src/Shop.Web/Views/Shared/_Layout.cshtml:5" },
    { "kind": "intent.uses", "element": "index-promotions", "outcome": "verified", "at": "src/Shop.Web/Views/Home/Index.cshtml:3" },
    { "kind": "intent.uses", "element": "index-uses-cart", "outcome": "verified", "at": "src/Shop.Web/Views/Home/Index.cshtml:1" },
    { "kind": "intent.target", "element": "layout-nav", "outcome": "verified", "actual": "Shop.Web.Views.Shared._Nav" },
    { "kind": "intent.target", "element": "index-promotions", "outcome": "verified", "actual": "Shop.Web.Views.Home._Promotions" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "count": 0 },
    { "rule": "claim/intent.target", "count": 0 }
  ]
}
```

## mvc-composition-partial-removed

A planted drift: the home page no longer renders `_Promotions`. One finding, on the Composition, naming what the view renders; its target waits on it.

```json case
{ "phase": "later", "base": "mvc-composition", "kinds": ["intent.uses"], "requirements": ["R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```cshtml file src/Shop.Web/Views/Home/Index.cshtml
@inject Shop.Web.Services.CartService Cart
<h1>Welcome</h1>
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "index-promotions", "outcome": "contradicted", "actual": "none" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "index-promotions", "messageContains": "`Index` renders `_Promotions`: in the model, not in the code. `src/Shop.Web/Views/Home/Index.cshtml` renders no partial or view component." },
    { "rule": "scope/coverage", "severity": "info" }
  ],
  "noOtherFindings": true
}
```

## mvc-partial-of-another-folder-states-nothing

A partial's name is found as MVC finds it, in the view's own folder first: the home page's `<partial name="_Nav" />` now renders the home folder's own `_Nav`, not the shared one the model composes, so the Composition is contradicted even though a view of that name is rendered.

```json case
{ "phase": "later", "base": "mvc-composition", "kinds": ["intent.uses"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop.Web", "type": "User Interface Package",
        "elements": [
          { "id": "views", "type": "Folder", "name": "Views", "children": [
            { "id": "home", "type": "Folder", "name": "Home", "children": [
              { "id": "index", "type": "Page", "name": "Index", "stereotypes": { "Page": { "Route": "/" } } }
            ] },
            { "id": "shared", "type": "Folder", "name": "Shared", "children": [
              { "id": "nav", "type": "Component", "name": "_Nav" }
            ] }
          ] }
        ]
      }],
      "associations": [
        { "id": "index-nav", "type": "Composition", "source": { "element": "index" }, "target": { "element": "nav" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```cshtml file src/Shop.Web/Views/Home/Index.cshtml
<h1>Welcome</h1>
<partial name="_Nav" />
```

```cshtml file src/Shop.Web/Views/Home/_Nav.cshtml
<nav class="home-nav"></nav>
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "index-nav", "outcome": "contradicted", "actual": "_Nav" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "index-nav" },
    { "rule": "scope/coverage", "severity": "info" }
  ],
  "noOtherFindings": true
}
```

## maui-views

A .NET MAUI view's links (`intent.maui`). A Composition is its XAML placing the component: an element of it (`<templates:ProductTemplate />`), a data template making one (`{DataTemplate templates:RowTemplate}`), or a type it names (`{x:Type local:Routes}`, the root component a Blazor hybrid page hosts). A Uses of a view model is the view's binding context: set in its code (`BindingContext = viewModel`) or in a `<ContentPage.BindingContext>` element of its XAML.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { } } },
          { "id": "login", "type": "Page", "name": "LoginView", "stereotypes": { "Page": { } } },
          { "id": "main", "type": "Page", "name": "MainPage", "stereotypes": { "Page": { } } },
          { "id": "product", "type": "Component", "name": "ProductTemplate" },
          { "id": "row", "type": "Component", "name": "RowTemplate" },
          { "id": "routes", "type": "Component", "name": "Routes" },
          { "id": "catalog-vm", "type": "View Model", "name": "CatalogViewModel" },
          { "id": "login-vm", "type": "View Model", "name": "LoginViewModel" }
        ]
      }],
      "associations": [
        { "id": "catalog-product", "type": "Composition", "source": { "element": "catalog" }, "target": { "element": "product" } },
        { "id": "catalog-row", "type": "Composition", "source": { "element": "catalog" }, "target": { "element": "row" } },
        { "id": "main-routes", "type": "Composition", "source": { "element": "main" }, "target": { "element": "routes" } },
        { "id": "catalog-uses-vm", "type": "Uses", "source": { "element": "catalog" }, "target": { "element": "catalog-vm" } },
        { "id": "login-uses-vm", "type": "Uses", "source": { "element": "login" }, "target": { "element": "login-vm" } }
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
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <UseMaui>true</UseMaui>
  </PropertyGroup>
</Project>
```

```xml file src/Shop/Views/CatalogView.xaml
<ContentPage x:Class="Shop.Views.CatalogView"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:templates="clr-namespace:Shop.Views.Templates">
    <VerticalStackLayout>
        <templates:ProductTemplate />
        <CollectionView ItemsSource="{Binding Items}" ItemTemplate="{DataTemplate templates:RowTemplate}" />
    </VerticalStackLayout>
</ContentPage>
```

```csharp file src/Shop/Views/CatalogView.xaml.cs
using Shop.ViewModels;

namespace Shop.Views;

public partial class CatalogView : ContentPage
{
    public CatalogView(CatalogViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}
```

```xml file src/Shop/Views/LoginView.xaml
<ContentPage x:Class="Shop.Views.LoginView"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:Shop.ViewModels">
    <ContentPage.BindingContext>
        <vm:LoginViewModel />
    </ContentPage.BindingContext>
    <Button Text="Sign in" Command="{Binding SignInCommand}" />
</ContentPage>
```

```csharp file src/Shop/Views/LoginView.xaml.cs
namespace Shop.Views;

public partial class LoginView : ContentPage
{
}
```

```csharp file src/Shop/Views/Templates/Templates.cs
namespace Shop.Views.Templates;

public class ProductTemplate : ContentView
{
}

public class RowTemplate : ContentView
{
}
```

```csharp file src/Shop/ViewModels/ViewModels.cs
namespace Shop.ViewModels;

public class CatalogViewModel
{
}

public class LoginViewModel
{
}
```

```xml file src/Shop/MainPage.xaml
<ContentPage x:Class="Shop.MainPage"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:Shop">
    <BlazorWebView HostPage="wwwroot/index.html">
        <BlazorWebView.RootComponents>
            <RootComponent Selector="#app" ComponentType="{x:Type local:Routes}" />
        </BlazorWebView.RootComponents>
    </BlazorWebView>
</ContentPage>
```

```csharp file src/Shop/MainPage.xaml.cs
namespace Shop;

public partial class MainPage : ContentPage
{
}
```

```razor file src/Shop/Routes.razor
<Router AppAssembly="typeof(MainPage).Assembly">
    <Found Context="routeData"><RouteView RouteData="routeData" /></Found>
</Router>
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "catalog-product", "outcome": "verified", "at": "src/Shop/Views/CatalogView.xaml:6" },
    { "kind": "intent.uses", "element": "catalog-row", "outcome": "verified", "at": "src/Shop/Views/CatalogView.xaml:7" },
    { "kind": "intent.uses", "element": "main-routes", "outcome": "verified", "at": "src/Shop/MainPage.xaml:7" },
    { "kind": "intent.uses", "element": "catalog-uses-vm", "outcome": "verified", "at": "src/Shop/Views/CatalogView.xaml.cs:9" },
    { "kind": "intent.uses", "element": "login-uses-vm", "outcome": "verified", "at": "src/Shop/Views/LoginView.xaml:6" }
  ],
  "others": "verified",
  "noOtherFindings": true
}
```

## maui-view-model-swapped

A planted drift: the catalog page's binding context is the login's view model. One finding, the Uses', naming the binding context the code sets.

```json case
{ "phase": 6, "base": "maui-views", "kinds": ["intent.uses"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/Shop/Views/CatalogView.xaml.cs
using Shop.ViewModels;

namespace Shop.Views;

public partial class CatalogView : ContentPage
{
    public CatalogView(LoginViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "catalog-uses-vm", "outcome": "contradicted", "actual": "LoginViewModel" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "catalog-uses-vm", "messageContains": "`CatalogView` uses `CatalogViewModel`: in the model, not in the code. `CatalogView`'s binding context is `LoginViewModel`" }
  ],
  "noOtherFindings": true
}
```

## maui-template-removed

A planted drift: the catalog page no longer places the product template. One finding, its Composition's, naming what the XAML does use.

```json case
{ "phase": 6, "base": "maui-views", "kinds": ["intent.uses"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```xml file src/Shop/Views/CatalogView.xaml
<ContentPage x:Class="Shop.Views.CatalogView"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:templates="clr-namespace:Shop.Views.Templates">
    <VerticalStackLayout>
        <Label Text="Products" />
        <CollectionView ItemsSource="{Binding Items}" ItemTemplate="{DataTemplate templates:RowTemplate}" />
    </VerticalStackLayout>
</ContentPage>
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "catalog-product", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "catalog-product", "messageContains": "`src/Shop/Views/CatalogView.xaml` uses `RowTemplate`, and no element, `DataTemplate` or `x:Type` of it is a `ProductTemplate`." }
  ],
  "noOtherFindings": true
}
```

## maui-region-shows-another-page

A Region of a .NET MAUI Shell shows its pages through the contents of the item it names. Here the tab bar shows the profile page where the model places the catalog: one finding, the Region's Composition's.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "AppShell", "children": [ { "id": "main-region", "type": "Region", "name": "Main" } ] },
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { } } }
        ]
      }],
      "associations": [
        { "id": "shows-catalog", "type": "Composition", "source": { "element": "main-region" }, "target": { "element": "catalog" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```xml file src/Shop/AppShell.xaml
<Shell x:Class="Shop.AppShell"
       xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:Shop.Views">
    <TabBar Route="Main">
        <ShellContent ContentTemplate="{DataTemplate views:ProfileView}" />
    </TabBar>
</Shell>
```

```csharp file src/Shop/AppShell.xaml.cs
namespace Shop;

public partial class AppShell : Shell
{
}
```

```csharp file src/Shop/Views/Pages.cs
namespace Shop.Views;

public class CatalogView : ContentPage
{
}

public class ProfileView : ContentPage
{
}
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "shows-catalog", "outcome": "contradicted", "actual": "ProfileView" }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "shows-catalog", "messageContains": "`Main` shows `CatalogView`: in the model, not in the code. The Shell item `Main` shows `ProfileView`." }
  ],
  "noOtherFindings": true
}
```
