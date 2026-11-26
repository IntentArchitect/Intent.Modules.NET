# appEvent.publishes

An App Event is an app-wide message any UI element or client module can publish or subscribe to (an event bus, an RxJS Subject, a Redux action, an IPC channel), its Properties the optional payload (Intent.Modelers.UI 2.0.0). An element that raises one has a Publishes association to it. `intent.ui` claims the App Event's `type.exists` (a class, record, struct, interface or type alias stating its Properties and fields) and each Publishes as `appEvent.publishes`, an existence kind, `stated: always`; subscribing is [appEvent.subscribes](appEvent.subscribes.cases.md).

The kind names no framework. The MVVM Toolkit (`intent.mvvmtoolkit`, CommunityToolkit.Mvvm, as MAUI, WPF, WinUI, Avalonia and Blazor code uses it) reads it from the element's messenger: a `Send` on a messenger (`WeakReferenceMessenger.Default`, `StrongReferenceMessenger.Default`, an `IMessenger`, an `ObservableRecipient`'s `Messenger`) of the event, created in place or wherever it comes from (a local, the code's own helper that sends what it is handed), or named as its type argument (`Send<T>()`). The toolkit's `ValueChangedMessage<T>` is declared as a library type with its `Value`, so an App Event deriving from it states that Property.

## verified-send

eShop's basket view model sends `ProductCountChangedMessage` when the basket is checked out. The message derives from the toolkit's `ValueChangedMessage<int>`, so its modelled `Value` is an `int`.

```json case
{ "phase": 6, "kinds": ["appEvent.publishes"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "basket", "type": "View Model", "name": "BasketViewModel" },
          { "id": "count-changed", "type": "App Event", "name": "ProductCountChangedMessage", "children": [
            { "id": "count-value", "type": "Property", "name": "Value", "typeRef": { "type": "int" } }
          ] }
        ]
      }],
      "associations": [
        { "id": "basket-publishes", "type": "Publishes", "source": { "element": "basket" }, "target": { "element": "count-changed" } }
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
```

```csharp file src/Shop/ViewModels/BasketViewModel.cs
using CommunityToolkit.Mvvm.Messaging;
using Shop.Messages;

namespace Shop.ViewModels;

public class BasketViewModel
{
    public void Checkout()
    {
        WeakReferenceMessenger.Default.Send(new ProductCountChangedMessage(0));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "appEvent.publishes", "element": "basket-publishes", "outcome": "verified", "at": "src/Shop/ViewModels/BasketViewModel.cs:10" },
    { "kind": "type.exists", "element": "count-changed", "outcome": "verified" },
    { "kind": "member.type", "element": "count-value", "outcome": "verified", "expected": "int", "actual": "int" }
  ],
  "others": "verified",
  "noOtherFindings": true
}
```

## contradicted-not-sent

A planted drift: the checkout no longer sends the message. One finding, the Publishes'.

```json case
{ "phase": 6, "base": "verified-send", "kinds": ["appEvent.publishes"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/Shop/ViewModels/BasketViewModel.cs
using CommunityToolkit.Mvvm.Messaging;
using Shop.Messages;

namespace Shop.ViewModels;

public class BasketViewModel
{
    public void Checkout()
    {
    }
}
```

```json expect
{
  "claims": [
    { "kind": "appEvent.publishes", "element": "basket-publishes", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/appEvent.publishes", "severity": "error", "subject": "basket-publishes", "messageContains": "`BasketViewModel` publishes `ProductCountChangedMessage`: in the model, not in the code. `BasketViewModel` sends no message on a messenger." }
  ],
  "noOtherFindings": true
}
```

## verified-through-a-helper

The message is made into a local and handed to the view model's own helper, which sends it on the `IMessenger` the view model is given. Another message it sends, which the model doesn't link, states nothing.

```json case
{ "phase": 6, "base": "verified-send", "kinds": ["appEvent.publishes"], "requirements": ["R2"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/Shop/ViewModels/BasketViewModel.cs
using CommunityToolkit.Mvvm.Messaging;
using Shop.Messages;

namespace Shop.ViewModels;

public class BasketViewModel(IMessenger messenger)
{
    public void Checkout()
    {
        var changed = new ProductCountChangedMessage(0);
        Notify(changed);
        messenger.Send(new BasketClearedMessage());
    }

    private void Notify<T>(T message) where T : class => messenger.Send(message);
}

public class BasketClearedMessage;
```

```json expect
{
  "claims": [
    { "kind": "appEvent.publishes", "element": "basket-publishes", "outcome": "verified", "at": "src/Shop/ViewModels/BasketViewModel.cs:11" }
  ],
  "noOtherFindings": true
}
```

## event-not-in-code

The message's class is gone. Its `type.exists` is the one finding: the Publishes waits on the event the code doesn't declare, and so does its Property.

```json case
{ "phase": 6, "base": "verified-send", "without": ["src/Shop/Messages/ProductCountChangedMessage.cs"], "kinds": ["appEvent.publishes"], "cases": ["engine/blocked"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/Shop/ViewModels/BasketViewModel.cs
using CommunityToolkit.Mvvm.Messaging;

namespace Shop.ViewModels;

public class BasketViewModel
{
    public void Checkout()
    {
    }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "count-changed", "outcome": "contradicted" },
    { "kind": "appEvent.publishes", "element": "basket-publishes", "outcome": "cantVerify", "reason": "blocked" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "count-changed" }
  ],
  "noOtherFindings": true
}
```
