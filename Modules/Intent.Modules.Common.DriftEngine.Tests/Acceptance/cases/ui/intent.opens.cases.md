# intent.opens

A component opens a dialog: a Show Dialog association in the User Interface designer, from the component (or one of its Component Operations) to a Component with the Dialog stereotype. It is a `presence` kind, an existence kind, `stated: always`, and the claim sits on the association. No eShopCustom or Java spike check states it: the kind comes from Common.Drift's User Interface facts, so these cases show the converted recipe, with the `intent.ui` pack's `{Component}` and `{Dialog}` placeholders.

## verified

The cart page asks before removing an item, opening the `RemoveItemDialog` through the dialog service, and the model has a Show Dialog from the page to that dialog.

```json case
{ "phase": 6, "kinds": ["intent.opens"], "requirements": ["R2", "R8", "R46"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
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
      "files": ["src/WebApp/Components/**/*.razor", "src/WebApp/Components/**/*.razor.cs"],
      "facts": {
        "intent.opens": {
          "find": "file:src/WebApp/Components/**/{Component}.razor*",
          "read": [{ "match": "\\b(?:Show|Open)(?:Async)?<{Dialog}>", "value": "present" }],
          "default": "absent",
          "unstated": "no `ShowAsync<{Dialog}>` in {Component}'s files"
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
                "stereotypes": { "Page": { "Route": "/cart" } } },
              { "id": "remove-item-dialog", "type": "Component", "name": "RemoveItemDialog",
                "stereotypes": { "Dialog": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "cart-opens-remove-item", "type": "Show Dialog",
          "source": { "element": "cart-page" }, "target": { "element": "remove-item-dialog" } }
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
@inject IDialogService DialogService

<PageTitle>Shopping Bag | AdventureWorks</PageTitle>

@code {
    private async Task RemoveAsync(BasketItem item)
    {
        var dialog = await DialogService.ShowAsync<RemoveItemDialog>("Remove item");
        if (!(await dialog.Result).Canceled)
        {
            await Basket.SetQuantityAsync(item.ProductId, 0);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.opens", "element": "cart-opens-remove-item", "outcome": "verified",
      "expected": "present", "actual": "present", "at": "src/WebApp/Components/Pages/Cart/CartPage.razor:10" }
  ]
}
```

## contradicted-not-opened

The cart page now removes items without asking, and the model's Show Dialog still says it opens the dialog. The page's file is found and no read case matches, so its silence says `absent`.

```json case
{ "phase": 6, "base": "verified", "kinds": ["intent.opens"], "cases": ["engine/silence"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui", "house.eshopcustom"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@page "/cart"
@inject BasketState Basket

<PageTitle>Shopping Bag | AdventureWorks</PageTitle>

@code {
    private async Task RemoveAsync(BasketItem item)
    {
        await Basket.SetQuantityAsync(item.ProductId, 0);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.opens", "element": "cart-opens-remove-item", "outcome": "contradicted",
      "expected": "present", "actual": "absent", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/intent.opens", "severity": "error", "subject": "cart-opens-remove-item", "messageContains": "RemoveItemDialog" }
  ]
}
```
