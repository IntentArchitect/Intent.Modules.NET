# intent.opens

A component opens a dialog: a Show Dialog association in the User Interface designer, from the component (or one of its Component Operations) to a Component with the Dialog stereotype. It is a `presence` kind, an existence kind, `stated: always`, and the claim sits on the association. No eShopCustom or Java spike check states it: the kind comes from Common.Drift's User Interface facts. The `intent.ui` pack gives the vocabulary, and the Blazor reader finds a `Show` or `Open` call, or its `Async` form, whose type argument is the dialog.

## verified

The cart page asks before removing an item, opening the `RemoveItemDialog` through the dialog service, and the model has a Show Dialog from the page to that dialog.

```json case
{ "phase": 6, "kinds": ["intent.opens"], "requirements": ["R2", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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

The cart page now removes items without asking, and the model's Show Dialog still says it opens the dialog. The page is found and nothing in it opens the dialog, so its silence says `absent`.

```json case
{ "phase": 6, "base": "verified", "kinds": ["intent.opens"], "cases": ["engine/silence"], "requirements": ["R3", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
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
      "expected": "present", "actual": "none", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/intent.opens", "severity": "error", "subject": "cart-opens-remove-item", "messageContains": "RemoveItemDialog" }
  ]
}
```
