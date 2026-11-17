# page.title

A page's title: the Page stereotype's Title on a User Interface designer Component, against the literal text of its `<PageTitle>`. The kind is `stated: whenSet`, so it is claimed only where the model sets a Title, and compares with `contains`: eShopCustom's titles end with the site's name, which the model leaves out.

The stock User Interface pack gives the kind and its vocabulary, and the Blazor reader reads the page's `<PageTitle>` element's text. A markup node's canonical text keeps one space between words and around punctuation (code-reader.md, Canonical text), so the title reads as written, `Shopping Bag | AdventureWorks`. A `<PageTitle>` computed from data can't be compared with a stated Title; eShopCustom reported that as Wrong, and here it is can't verify, `unreadable`, which is an error too.

## verified

The model's Title `Shopping Bag` is within the code's literal `<PageTitle>`, read as written.

```json case
{ "phase": "later", "kinds": ["page.title"], "traces": ["ESC-FE-13"], "requirements": ["R2", "R10", "R28"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
                "stereotypes": { "Page": { "Route": "/cart", "Title": "Shopping Bag" } } }
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
    { "kind": "page.title", "element": "cart-page", "outcome": "verified",
      "expected": "Shopping Bag", "actual": "Shopping Bag | AdventureWorks", "at": "src/WebApp/Components/Pages/Cart/CartPage.razor:4" }
  ]
}
```

## contradicted

eShopCustom's `page.title`: the model's Title is `Basket`, which the code's `<PageTitle>` doesn't contain.

```json case
{ "phase": "later", "base": "verified", "kinds": ["page.title"], "traces": ["ESC-FE-13"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
                "stereotypes": { "Page": { "Route": "/cart", "Title": "Basket" } } }
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
    { "kind": "page.title", "element": "cart-page", "outcome": "contradicted",
      "expected": "Basket", "at": "src/WebApp/Components/Pages/Cart/CartPage.razor:4" }
  ],
  "findings": [
    { "rule": "claim/page.title", "severity": "error", "subject": "cart-page" }
  ]
}
```

## contradicted-not-stated

The model sets a Title and the page has no `<PageTitle>`. The kind is `whenSet`, so the code's silence contradicts it (R4).

```json case
{ "phase": "later", "base": "verified", "kinds": ["page.title"], "traces": ["ESC-FE-13"], "requirements": ["R4"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@page "/cart"
@inject BasketState Basket

<div class="cart">
    <a href="checkout" class="button button-primary">Check out</a>
</div>
```

```json expect
{
  "claims": [
    { "kind": "page.title", "element": "cart-page", "outcome": "contradicted", "expected": "Shopping Bag", "actual": null }
  ]
}
```

## cant-verify-computed

The item page's title is computed from the item it shows, and the model states a fixed Title, so the claim is can't verify, `unreadable`, at the title's line: a stated Title against a computed one is never passed.

```json case
{ "phase": "later", "base": "verified", "without": ["src/WebApp/Components/Pages/Cart/CartPage.razor"], "kinds": ["page.title"], "cases": ["engine/unreadable"], "traces": ["ESC-FE-13"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
                "stereotypes": { "Page": { "Route": "/item/{itemId}", "Title": "Item" } } }
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
    { "kind": "page.title", "element": "item-page", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/WebApp/Components/Pages/Item/ItemPage.razor:4" }
  ],
  "findings": [
    { "rule": "claim/page.title", "severity": "error", "subject": "item-page", "messageContains": "computed" }
  ]
}
```

## react-title

The standard User Interface designer (2.0.0) read against React by the stock React reader. A React page states its title by rendering a `<title>` (React 19 hoists it into the document's head; react-helmet's `<Helmet>` holds one too), or by setting `document.title` in its code, an effect included. The cart page renders its title, and the orders page sets one other than the model's.

```json case
{ "phase": "later", "kinds": ["page.title"], "requirements": ["R2", "R14", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart", "Title": "Shopping Bag" } } },
          { "id": "orders-page", "type": "Page", "name": "OrdersPage", "stereotypes": { "Page": { "Route": "/orders", "Title": "My Orders" } } }
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

```tsx file src/CartPage.tsx
import { Helmet } from 'react-helmet-async';

export default function CartPage() {
    return (
        <>
            <Helmet><title>Shopping Bag</title></Helmet>
            <h1>Cart</h1>
        </>
    );
}
```

```tsx file src/OrdersPage.tsx
import { useEffect } from 'react';

const TITLE = 'Orders';

export default function OrdersPage() {
    useEffect(() => {
        document.title = TITLE;
    }, []);
    return <h1>Orders</h1>;
}
```

```json expect
{
  "claims": [
    { "kind": "page.title", "element": "cart-page", "outcome": "verified", "actual": "Shopping Bag", "at": "src/CartPage.tsx:6" },
    { "kind": "page.title", "element": "orders-page", "outcome": "contradicted", "expected": "My Orders", "actual": "Orders", "at": "src/OrdersPage.tsx:7" }
  ],
  "findings": [
    { "rule": "claim/page.title", "severity": "error", "subject": "orders-page" }
  ]
}
```
