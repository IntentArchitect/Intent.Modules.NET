# page.title

A page's title: the Page stereotype's Title on a User Interface designer Component, against the literal text of its `<PageTitle>`. The kind is `stated: whenSet`, so it is claimed only where the model sets a Title, and compares with `contains`: eShopCustom's titles end with the site's name, which the model leaves out.

The kind is `later`, so these cases' house pack gives the kind, its vocabulary and its rule. The rule finds the page's `<PageTitle>` element with a `markup:` step and reads its text. A markup node's canonical text keeps one space between words and around punctuation (code-reader.md, Canonical text), so the title reads as written, `Shopping Bag | AdventureWorks`. A `<PageTitle>` computed from data can't be compared with a stated Title; eShopCustom reported that as Wrong, and here it is can't verify, `unreadable`, which is an error too.

## verified

The model's Title `Shopping Bag` is within the code's literal `<PageTitle>`, read as written.

```json case
{ "phase": "later", "kinds": ["page.title"], "traces": ["ESC-FE-13"], "requirements": ["R2", "R10", "R28"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "kinds": [
    { "kind": "page.title", "label": "page title", "valueType": "text", "comparer": "contains", "stated": "whenSet", "existence": false }
  ],
  "vocabulary": {
    "User Interface": {
      "entries": [
        {
          "kind": "page.title",
          "for": { "element": "Component", "where": "hasStereotype('Page')" },
          "value": "stereotype('Page').Title",
          "bindings": { "Component": "name" },
          "own": ["Component"],
          "source": "the Page stereotype's Title"
        }
      ]
    }
  },
  "rules": {
    "User Interface": {
      "files": ["src/WebApp/Components/**/*.razor"],
      "facts": {
        "page.title": {
          "find": "file:src/WebApp/Components/**/{Component}.razor > markup:PageTitle",
          "read": [
            { "match": "^<PageTitle>([^<@]*)</PageTitle>$" },
            { "match": "^<PageTitle>[^<]*@", "unreadable": true, "note": "A title computed from data" }
          ],
          "unstated": "no `<PageTitle>` in {Component}.razor"
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
{ "phase": "later", "base": "verified", "kinds": ["page.title"], "traces": ["ESC-FE-13"], "requirements": ["R2", "R14"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
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
{ "phase": "later", "base": "verified", "kinds": ["page.title"], "traces": ["ESC-FE-13"], "requirements": ["R4"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
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

The item page's title is computed from the item it shows, and the model states a fixed Title. The read case for a computed title matches, so the claim is can't verify, `unreadable`, naming the line: a stated Title against a computed one is never passed.

```json case
{ "phase": "later", "base": "verified", "without": ["src/WebApp/Components/Pages/Cart/CartPage.razor"], "kinds": ["page.title"], "cases": ["engine/unreadable"], "traces": ["ESC-FE-13"], "requirements": ["R5", "R11"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
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
    { "rule": "claim/page.title", "severity": "error", "subject": "item-page", "messageContains": "ItemPage.razor:4" }
  ]
}
```
