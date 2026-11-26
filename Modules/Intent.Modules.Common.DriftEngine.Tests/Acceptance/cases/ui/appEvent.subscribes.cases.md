# appEvent.subscribes

An element that listens for an App Event and reacts to it has a Subscribes association to it (Intent.Modelers.UI 2.0.0). `intent.ui` claims each as `appEvent.subscribes`, an existence kind, `stated: always`; publishing is [appEvent.publishes](appEvent.publishes.cases.md).

The MVVM Toolkit (`intent.mvvmtoolkit`) reads it from the element's code: a `Register` on a messenger naming the event as a type argument (`Register<CatalogViewModel, ProductCountChangedMessage>`), or the class being an `IRecipient<ProductCountChangedMessage>`, which `RegisterAll` or an active `ObservableRecipient` registers.

## verified-register

eShop's catalog view model registers for the basket's count changes, to update its badge.

```json case
{ "phase": 6, "kinds": ["appEvent.subscribes"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "catalog", "type": "View Model", "name": "CatalogViewModel" },
          { "id": "count-changed", "type": "App Event", "name": "ProductCountChangedMessage" }
        ]
      }],
      "associations": [
        { "id": "catalog-subscribes", "type": "Subscribes", "source": { "element": "catalog" }, "target": { "element": "count-changed" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Shop/Messages/ProductCountChangedMessage.cs
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Shop.Messages;

public class ProductCountChangedMessage(int count) : ValueChangedMessage<int>(count);

public class BasketClearedMessage;
```

```csharp file src/Shop/ViewModels/CatalogViewModel.cs
using CommunityToolkit.Mvvm.Messaging;
using Shop.Messages;

namespace Shop.ViewModels;

public class CatalogViewModel
{
    public CatalogViewModel()
    {
        WeakReferenceMessenger.Default.Register<CatalogViewModel, ProductCountChangedMessage>(this, (recipient, message) => recipient.BadgeCount = message.Value);
        WeakReferenceMessenger.Default.Register<CatalogViewModel, BasketClearedMessage>(this, (recipient, message) => recipient.BadgeCount = 0);
    }

    public int BadgeCount { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "appEvent.subscribes", "element": "catalog-subscribes", "outcome": "verified", "at": "src/Shop/ViewModels/CatalogViewModel.cs:10" }
  ],
  "others": "verified",
  "noOtherFindings": true
}
```

## verified-recipient

The view model is an `IRecipient<ProductCountChangedMessage>` of an `ObservableRecipient`, which registers it when it is active.

```json case
{ "phase": 6, "base": "verified-register", "kinds": ["appEvent.subscribes"], "requirements": ["R2"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/Shop/ViewModels/CatalogViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Shop.Messages;

namespace Shop.ViewModels;

public class CatalogViewModel : ObservableRecipient, IRecipient<ProductCountChangedMessage>
{
    public int BadgeCount { get; set; }

    public void Receive(ProductCountChangedMessage message) => BadgeCount = message.Value;
}
```

```json expect
{
  "claims": [
    { "kind": "appEvent.subscribes", "element": "catalog-subscribes", "outcome": "verified", "at": "src/Shop/ViewModels/CatalogViewModel.cs:7" }
  ],
  "noOtherFindings": true
}
```

## contradicted-registers-another

A planted drift: the view model registers for the toolkit's own `ValueChangedMessage<int>` instead. One finding, the Subscribes', naming what it registers.

```json case
{ "phase": 6, "base": "verified-register", "kinds": ["appEvent.subscribes"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/Shop/ViewModels/CatalogViewModel.cs
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Shop.Messages;

namespace Shop.ViewModels;

public class CatalogViewModel
{
    public CatalogViewModel()
    {
        WeakReferenceMessenger.Default.Register<CatalogViewModel, ValueChangedMessage<int>>(this, (recipient, message) => recipient.BadgeCount = message.Value);
    }

    public int BadgeCount { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "appEvent.subscribes", "element": "catalog-subscribes", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/appEvent.subscribes", "severity": "error", "subject": "catalog-subscribes", "messageContains": "`CatalogViewModel` subscribes to `ProductCountChangedMessage`: in the model, not in the code. `CatalogViewModel` registers `WeakReferenceMessenger.Default.Register<CatalogViewModel, ValueChangedMessage<int>>(" }
  ],
  "noOtherFindings": true
}
```

## sending-isnt-subscribing

The view model sends the event it should listen for: a send isn't a subscription, so it is one finding.

```json case
{ "phase": 6, "base": "verified-register", "kinds": ["appEvent.subscribes"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/Shop/ViewModels/CatalogViewModel.cs
using CommunityToolkit.Mvvm.Messaging;
using Shop.Messages;

namespace Shop.ViewModels;

public class CatalogViewModel
{
    public void Clear() => WeakReferenceMessenger.Default.Send(new ProductCountChangedMessage(0));
}
```

```json expect
{
  "claims": [
    { "kind": "appEvent.subscribes", "element": "catalog-subscribes", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/appEvent.subscribes", "severity": "error", "subject": "catalog-subscribes", "messageContains": "registers for no message on a messenger" }
  ],
  "noOtherFindings": true
}
```
