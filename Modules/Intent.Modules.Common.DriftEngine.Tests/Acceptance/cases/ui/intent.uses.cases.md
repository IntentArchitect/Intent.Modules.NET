# intent.uses

A component renders, or uses, another: a Composition, or a Display Component in a Layout's header, body, sider or footer, in the User Interface designer; a Rendered Component or a Uses association in the Java spike's Front End designer. It is a `presence` kind, an existence kind, `stated: always`: the code either renders the target or doesn't. The claim sits on the model feature that states it: the association for a Composition or Uses, the element for a Display Component or Rendered Component (vocabulary.md, A vocabulary entry).

The Blazor cases use the `intent.ui` pack's vocabulary, with Common.Drift's placeholders (`{Component}` for the component that renders, `{Child}` for the one rendered), and eShopCustom's house recipe. The recipe's `default` makes a found component file that doesn't render the target say `absent` from silence. The Vue cases use the Java spike's house pack, which gives the Front End designer's vocabulary too.

## verified-composition

eShopCustom's catalog page renders a `CatalogListItem` for each item, and the model has a Composition from the page to that component, which lives in another package.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "traces": ["ESC-FE-27"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
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
        "intent.uses": {
          "find": "file:src/WebApp/Components/**/{Component}.razor",
          "read": [{ "match": "<(?:[\\w.]+\\.)?{Child}\\b", "value": "present" }],
          "default": "absent",
          "unstated": "no `<{Child}>` in {Component}'s markup"
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

eShopCustom's `composition.missing`: the catalog page now renders its items inline, and the model's Composition still says it renders `CatalogListItem`. The page's file is found and no read case matches, so its silence says `absent`.

```json case
{ "phase": 6, "base": "verified-composition", "kinds": ["intent.uses"], "cases": ["engine/silence"], "traces": ["ESC-FE-27"], "requirements": ["R3", "R8", "R14"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
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
      "expected": "present", "actual": "absent", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "catalog-renders-list-item", "messageContains": "CatalogListItem" }
  ]
}
```

## layout-display-components

The designer has no Composition from a Layout, so what eShopCustom's main layout renders is modelled as Display Components in its Layout Header and Layout Footer; eShopCustom only warned about them. Here each is a claim on its Display Component: the header bar is rendered, and the footer bar, which the layout no longer renders, is contradicted.

```json case
{ "phase": 6, "base": "verified-composition", "without": ["src/WebApp/Components/Pages/Catalog/Catalog.razor"], "kinds": ["intent.uses"], "traces": ["ESC-FE-69"], "requirements": ["R2", "R3", "R8", "R30"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
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
      "expected": "present", "actual": "absent", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/intent.uses", "severity": "error", "subject": "main-layout-footer-bar", "messageContains": "FooterBar" }
  ]
}
```

## java-rendered-component

The Java spike's catalog page renders a product image and a stock pill, the second in kebab case as Vue templates allow. Each Rendered Component is a claim, found by its PascalCase or kebab-case tag inside the page's template. The page's file comes from its Implementation stereotype's Path. A component the template doesn't render is contradicted.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "traces": ["JAV-FE-05"], "requirements": ["R2", "R3", "R28", "R38"], "packs": ["intent.core", "intent.java", "house.shop"], "language": "typescript" }
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
    { "kind": "intent.uses", "label": "uses", "valueType": "presence", "comparer": "presence", "stated": "always", "existence": true }
  ],
  "vocabulary": {
    "Front End": {
      "entries": [
        {
          "kind": "intent.uses",
          "for": { "element": "Rendered Component" },
          "value": "'present'",
          "bindings": { "File": "parent.stereotype('Implementation').Path", "Child": "type.name" },
          "own": ["Child"],
          "source": "the Rendered Component"
        }
      ]
    }
  },
  "rules": {
    "Front End": {
      "files": ["src/**/*.vue"],
      "facts": {
        "intent.uses": {
          "find": "file:{File}",
          "read": [{ "match": "(?<=<template>.*)<(?:{Child}|{Child|kebab})\\b", "value": "present" }],
          "default": "absent",
          "unstated": "no `<{Child}>` or `<{Child|kebab}>` in the template of `{File}`"
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

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "catalog-page-product-image", "outcome": "verified", "at": "src/pages/CatalogPage.vue:13" },
    { "kind": "intent.uses", "element": "catalog-page-stock-pill", "outcome": "verified", "at": "src/pages/CatalogPage.vue:15" },
    { "kind": "intent.uses", "element": "catalog-page-price-tag", "outcome": "contradicted",
      "expected": "present", "actual": "absent", "fromSilence": true }
  ]
}
```

## java-uses-import

The Java spike's cart page uses the cart store: a Uses association, which the code states as a relative value import of the store's file. A type-only import (`import type`) uses no value at runtime, so the checkout page, which imports only the store's types, is contradicted.

```json case
{ "phase": 6, "kinds": ["intent.uses"], "traces": ["JAV-FE-07"], "requirements": ["R2", "R3", "R28", "R38"], "packs": ["intent.core", "intent.java", "house.shop"], "language": "typescript" }
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
    { "kind": "intent.uses", "label": "uses", "valueType": "presence", "comparer": "presence", "stated": "always", "existence": true }
  ],
  "vocabulary": {
    "Front End": {
      "entries": [
        {
          "kind": "intent.uses",
          "for": { "association": "Uses" },
          "value": "'present'",
          "bindings": {
            "File": "source.element.stereotype('Implementation').Path",
            "Used": "trimStart(trimEnd(trimEnd(target.element.stereotype('Implementation').Path, '.ts'), '.vue'), 'src/')"
          },
          "own": ["Used"],
          "source": "the Uses association"
        }
      ]
    }
  },
  "rules": {
    "Front End": {
      "files": ["src/**/*.vue", "src/**/*.ts"],
      "facts": {
        "intent.uses": {
          "find": "file:{File}",
          "read": [{ "match": "\\bimport(?! type\\b)[^;]*from'(?:\\.\\.?/)+{Used}(?:\\.vue|\\.ts)?'", "value": "present" }],
          "default": "absent",
          "unstated": "no value import of `{Used}` in `{File}`"
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

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "cart-page-uses-cart-store", "outcome": "verified", "at": "src/pages/CartPage.vue:3" },
    { "kind": "intent.uses", "element": "checkout-page-uses-cart-store", "outcome": "contradicted",
      "expected": "present", "actual": "absent", "fromSilence": true }
  ]
}
```
