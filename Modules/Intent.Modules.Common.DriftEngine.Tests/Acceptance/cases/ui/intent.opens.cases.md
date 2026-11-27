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

## react-dialog-rendered

The standard User Interface designer (2.0.0) has a Dialog element type. A React component opens a dialog by rendering it, here only while its state says so: the stock React reader reads the dialog's tag in the component's JSX. The cart page renders no `ClearCartDialog`, so that Show Dialog is contradicted.

```json case
{ "phase": 6, "kinds": ["intent.opens"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart" } } },
          { "id": "remove-item-dialog", "type": "Dialog", "name": "RemoveItemDialog" },
          { "id": "clear-cart-dialog", "type": "Dialog", "name": "ClearCartDialog" }
        ]
      }],
      "associations": [
        { "id": "cart-opens-remove-item", "type": "Show Dialog", "source": { "element": "cart-page" }, "target": { "element": "remove-item-dialog" } },
        { "id": "cart-opens-clear-cart", "type": "Show Dialog", "source": { "element": "cart-page" }, "target": { "element": "clear-cart-dialog" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```tsx file src/CartPage.tsx
import { useState } from 'react';
import { RemoveItemDialog } from './RemoveItemDialog';
import { ClearCartDialog } from './ClearCartDialog';

export default function CartPage() {
    const [removing, setRemoving] = useState<string | null>(null);
    return (
        <section>
            <button onClick={() => setRemoving('sku-1')}>Remove</button>
            {removing && <RemoveItemDialog sku={removing} onClose={() => setRemoving(null)} />}
        </section>
    );
}
```

```tsx file src/RemoveItemDialog.tsx
export function RemoveItemDialog({ sku, onClose }: { sku: string; onClose(): void }) {
    return <dialog open><button onClick={onClose}>Remove {sku}</button></dialog>;
}
```

```tsx file src/ClearCartDialog.tsx
export function ClearCartDialog() {
    return <dialog open>Clear?</dialog>;
}
```

```json expect
{
  "claims": [
    { "kind": "intent.opens", "element": "cart-opens-remove-item", "outcome": "verified", "at": "src/CartPage.tsx:10" },
    { "kind": "intent.opens", "element": "cart-opens-clear-cart", "outcome": "contradicted", "expected": "present" }
  ],
  "findings": [
    { "rule": "claim/intent.opens", "severity": "error", "subject": "cart-opens-clear-cart" }
  ]
}
```

## angular-dialog-rendered-behind-a-flag

An Angular component that shows a dialog by rendering it behind a flag, with no dialog service: the user list renders `<app-modal [isOpen]="showDeleteModal()">`, which the stock Angular reader reads by the modal's `selector`, as it reads a React or Vue dialog's tag. The dashboard renders no modal, so its Show Dialog is contradicted. Nothing in the modal's own code says it is a dialog (only a dialog service's `open` does), so its kind can't be verified, as a React or Vue dialog's can't.

```json case
{ "phase": "later", "kinds": ["intent.opens", "component.kind"], "requirements": ["R2", "R8", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "app", "name": "App", "type": "User Interface Package",
        "elements": [
          { "id": "user-list", "type": "Component", "name": "UserList" },
          { "id": "dashboard", "type": "Component", "name": "Dashboard" },
          { "id": "modal", "type": "Dialog", "name": "Modal" }
        ]
      }],
      "associations": [
        { "id": "user-list-opens-modal", "type": "Show Dialog", "source": { "element": "user-list" }, "target": { "element": "modal" } },
        { "id": "dashboard-opens-modal", "type": "Show Dialog", "source": { "element": "dashboard" }, "target": { "element": "modal" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "app", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```typescript file src/modal.ts
import { Component, input, output } from '@angular/core';

@Component({
    selector: 'app-modal',
    template: `
        @if (isOpen()) {
            <div class="backdrop">
                <div role="dialog" aria-modal="true"><ng-content /></div>
            </div>
        }
    `,
})
export class Modal {
    readonly isOpen = input.required<boolean>();
    readonly closed = output<void>();
}
```

```typescript file src/user-list.ts
import { Component, signal } from '@angular/core';
import { Modal } from './modal';

@Component({
    selector: 'app-user-list',
    imports: [Modal],
    template: `
        <button (click)="showDeleteModal.set(true)">Delete</button>
        <app-modal [isOpen]="showDeleteModal()" (closed)="showDeleteModal.set(false)">
            <p>Delete this user?</p>
        </app-modal>
    `,
})
export class UserList {
    readonly showDeleteModal = signal(false);
}
```

```typescript file src/dashboard.ts
import { Component } from '@angular/core';

@Component({ selector: 'app-dashboard', template: `<h1>Dashboard</h1>` })
export class Dashboard {}
```

```json expect
{
  "claims": [
    { "kind": "intent.opens", "element": "user-list-opens-modal", "outcome": "verified", "at": "src/user-list.ts:9" },
    { "kind": "intent.opens", "element": "dashboard-opens-modal", "outcome": "contradicted" },
    { "kind": "component.kind", "element": "modal", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/intent.opens", "severity": "error", "subject": "dashboard-opens-modal" }
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
