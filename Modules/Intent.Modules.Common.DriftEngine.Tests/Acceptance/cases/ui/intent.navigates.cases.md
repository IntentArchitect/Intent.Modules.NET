# intent.navigates

A component navigates to a page: a Navigation in the User Interface designer, or a Navigates To association in the Java spike's Front End designer. It is a `presence` kind, an existence kind, `stated: always`, and the claim sits on the association that states it.

The Blazor cases use the `intent.ui` pack's vocabulary, and the Blazor reader reads the navigation: an `href`, a form `action` or a `NavigateTo` whose URI is the target's route. A navigation whose URI is computed is can't verify, `unreadable`. The Vue cases use the Java spike's house pack, which gives the Front End designer's vocabulary too.

## verified-href

The cart page links to checkout with an `href`, and the model has a Navigation from the cart page to the Checkout page.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "traces": ["ESC-FE-32"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
      }],
      "associations": [
        { "id": "cart-navigates-to-checkout", "type": "Navigation",
          "source": { "element": "cart-page" }, "target": { "element": "checkout-page", "navigable": true } }
      ]
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

<div class="cart-summary">
    <a href="checkout" class="button button-primary">Check out</a>
</div>
```

```razor file src/WebApp/Components/Pages/Checkout/Checkout.razor
@page "/checkout"
@inject BasketState Basket

<PageTitle>Checkout | AdventureWorks</PageTitle>
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "cart-navigates-to-checkout", "outcome": "verified",
      "expected": "present", "actual": "present", "at": "src/WebApp/Components/Pages/Cart/CartPage.razor:7" }
  ]
}
```

## verified-navigate-to

eShopCustom's checkout page sends the user to their orders after placing one. The model's Navigation starts at the page's `HandleSubmitAsync` Component Operation, and the code's `NavigateTo` is in that method.

```json case
{ "phase": 6, "base": "verified-href", "kinds": ["intent.navigates"], "traces": ["ESC-FE-32"], "requirements": ["R2", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
              { "id": "checkout-page", "type": "Component", "name": "Checkout",
                "stereotypes": { "Page": { "Route": "/checkout" } },
                "children": [
                  { "id": "checkout-handle-submit", "type": "Component Operation", "name": "HandleSubmitAsync" }
                ] },
              { "id": "orders-page", "type": "Component", "name": "Orders",
                "stereotypes": { "Page": { "Route": "/user/orders" } } }
            ] }
        ]
      }],
      "associations": [
        { "id": "checkout-navigates-to-orders", "type": "Navigation",
          "source": { "element": "checkout-handle-submit" }, "target": { "element": "orders-page", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebApp/Components/Pages/Checkout/Checkout.razor
@page "/checkout"
@inject NavigationManager Nav
@inject BasketState Basket

<PageTitle>Checkout | AdventureWorks</PageTitle>

<EditForm Model="@Info" FormName="checkout" OnSubmit="@HandleSubmitAsync">
    <button type="submit" class="button button-primary">Place order</button>
</EditForm>

@code {
    private async Task HandleSubmitAsync()
    {
        await Basket.CheckoutAsync(Info.CreateOrderRequest());
        Nav.NavigateTo("user/orders");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "checkout-navigates-to-orders", "outcome": "verified",
      "at": "src/WebApp/Components/Pages/Checkout/Checkout.razor:15" }
  ]
}
```

## contradicted-missing

eShopCustom's `navigation.missing`: the cart page has lost its link to checkout, and the model's Navigation still says it navigates there. The page is found and nothing in it navigates to the checkout route, so its silence says `absent`.

```json case
{ "phase": 6, "base": "verified-href", "kinds": ["intent.navigates"], "cases": ["engine/silence"], "traces": ["ESC-FE-32"], "requirements": ["R3", "R8", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@page "/cart"
@inject BasketState Basket

<PageTitle>Shopping Bag | AdventureWorks</PageTitle>

<div class="cart-summary">
    <a href="" class="cart-summary-link">Continue shopping</a>
</div>
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "cart-navigates-to-checkout", "outcome": "contradicted",
      "expected": "present", "actual": "none", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "severity": "error", "subject": "cart-navigates-to-checkout", "messageContains": "`Checkout` navigates: in the model, not in the code." }
  ]
}
```

## cant-verify-computed-uri

The cart page's link builds its URI in code, so the reader can't tell where it goes. The claim is can't verify, `unreadable`, at the link's line, rather than passing or failing on a guess.

```json case
{ "phase": 6, "base": "verified-href", "kinds": ["intent.navigates"], "cases": ["engine/unreadable"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@page "/cart"
@inject BasketState Basket

<PageTitle>Shopping Bag | AdventureWorks</PageTitle>

<div class="cart-summary">
    <a href="@CheckoutUri" class="button button-primary">Check out</a>
</div>

@code {
    private string CheckoutUri => Basket.IsEmpty ? "" : "checkout";
}
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "cart-navigates-to-checkout", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/WebApp/Components/Pages/Cart/CartPage.razor:7" }
  ]
}
```

## java-router-link

The Java spike's cart page links to checkout with a `RouterLink`, and the model has a Navigates To from the cart page to the checkout page. The checkout page has no link back to the cart, so its Navigates To is contradicted. No stock pack knows the Front End designer, so its house pack extends `intent.ui`: it claims where each page is (`type.exists` with its Implementation Path) and the stock `intent.navigates` on its own association, with the target's route as the `Route` binding, and the stock Vue reader (`intent.vue`) reads the page's `to` attributes and `router.push` calls.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "traces": ["JAV-FE-06"], "requirements": ["R2", "R3", "R28", "R38"], "packs": ["intent.core", "intent.ui", "house.shop"], "language": "typescript" }
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
          "kind": "intent.navigates",
          "for": { "association": "Navigates To" },
          "value": "'present'",
          "bindings": {
            "Page": "target.element.name",
            "Route": "target.element.stereotype('Route').Path"
          },
          "own": ["Page"],
          "source": "the Navigates To association"
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
        { "id": "cart-navigates-to-checkout", "type": "Navigates To",
          "source": { "element": "cart-page" }, "target": { "element": "checkout-page" } },
        { "id": "checkout-navigates-to-cart", "type": "Navigates To",
          "source": { "element": "checkout-page" }, "target": { "element": "cart-page" } }
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
import { useCartStore } from '../stores/cart';

const cart = useCartStore();
</script>

<template>
    <section class="cart">
        <RouterLink to="/" class="btn">Browse the catalog</RouterLink>
        <RouterLink to="/checkout" class="btn btn-block" data-testid="proceed-to-checkout">Proceed to checkout</RouterLink>
    </section>
</template>
```

```vue file src/pages/CheckoutPage.vue
<script setup lang="ts">
import { useRouter } from 'vue-router';

const router = useRouter();

async function placed(orderId: string) {
    await router.push(`/orders/${orderId}`);
}
</script>

<template>
    <form class="checkout" @submit.prevent="placed('')"></form>
</template>
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "cart-navigates-to-checkout", "outcome": "verified", "at": "src/pages/CartPage.vue:11" },
    { "kind": "intent.navigates", "element": "checkout-navigates-to-cart", "outcome": "contradicted", "expected": "present" }
  ]
}
```

## vue-standard-navigation

The standard User Interface designer (2.0.0): a Navigation from the layout to the catalog page, whose route is the root `/`, and from the checkout page to the order page at `/orders/{id}`. The stock Vue reader takes each target's routes from its `page.route` claim and reads the source's `<RouterLink to>` and `router.push(...)`: the layout's `to="/"` goes to the root, and the checkout's `` router.push(`/orders/${orderId}`) `` to the order page, its value standing for the `{id}` segment. The checkout page has no link back to the catalog, so that Navigation is contradicted.

```json case
{ "phase": 6, "kinds": ["intent.navigates"], "requirements": ["R2", "R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
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
            { "id": "catalog-page", "type": "Page", "name": "CatalogPage", "stereotypes": { "Page": { "Route": "/" } } },
            { "id": "checkout-page", "type": "Page", "name": "CheckoutPage", "stereotypes": { "Page": { "Route": "/checkout" } } },
            { "id": "order-page", "type": "Page", "name": "OrderPage", "stereotypes": { "Page": { "Route": "/orders/{id}" } } }
          ] }
        ]
      }],
      "associations": [
        { "id": "app-to-catalog", "type": "Navigation", "source": { "element": "app" }, "target": { "element": "catalog-page" } },
        { "id": "checkout-to-order", "type": "Navigation", "source": { "element": "checkout-page" }, "target": { "element": "order-page" } },
        { "id": "checkout-to-catalog", "type": "Navigation", "source": { "element": "checkout-page" }, "target": { "element": "catalog-page" } }
      ]
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
import CatalogPage from './pages/CatalogPage.vue';
import CheckoutPage from './pages/CheckoutPage.vue';
import OrderPage from './pages/OrderPage.vue';

export const routes: RouteRecordRaw[] = [
    { path: '/', component: CatalogPage },
    { path: '/checkout', component: CheckoutPage },
    { path: '/orders/:id', component: OrderPage },
];
```

```vue file src/App.vue
<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router';
</script>

<template>
    <RouterLink to="/" class="brand">eShop</RouterLink>
    <RouterView />
</template>
```

```vue file src/pages/CatalogPage.vue
<script setup lang="ts">
const title = 'Catalog';
</script>
```

```vue file src/pages/CheckoutPage.vue
<script setup lang="ts">
import { useRouter } from 'vue-router';

const router = useRouter();

async function placed(orderId: string) {
    await router.push(`/orders/${orderId}`);
}
</script>
```

```vue file src/pages/OrderPage.vue
<script setup lang="ts">
const title = 'Order';
</script>
```

```json expect
{
  "claims": [
    { "kind": "intent.navigates", "element": "app-to-catalog", "outcome": "verified", "at": "src/App.vue:6" },
    { "kind": "intent.navigates", "element": "checkout-to-order", "outcome": "verified", "at": "src/pages/CheckoutPage.vue:7" },
    { "kind": "intent.navigates", "element": "checkout-to-catalog", "outcome": "contradicted", "expected": "present" }
  ],
  "findings": [
    { "rule": "claim/intent.navigates", "severity": "error", "subject": "checkout-to-catalog" }
  ]
}
```
