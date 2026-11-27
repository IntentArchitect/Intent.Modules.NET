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

## signalr-client

ASP.NET Core SignalR's JavaScript client (`intent.signalr`). The hub sends its clients `ReceiveComment`, which the User Interface designer models as an App Event of that name: the store's `hubConnection.on('ReceiveComment', (comment) => ...)` is the event's code, its handler's parameter the event's Property, and the store subscribes to it. The store's `invoke('SendComment', body)` calls the hub method of that name, the Operation of the Service whose class derives from `Hub`, matched without case as SignalR matches it.

```json case
{ "phase": "later", "kinds": ["appEvent.subscribes", "type.exists", "member.exists", "intent.invokes", "intent.target"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.services", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Api",
        "elements": [
          { "id": "chat-hub", "type": "Service", "name": "ChatHub", "children": [
            { "id": "send-comment", "type": "Operation", "name": "SendComment", "children": [
              { "id": "send-comment-body", "type": "Parameter", "name": "body", "typeRef": { "type": "string" } }
            ] }
          ] }
        ]
      }]
    },
    "User Interface": {
      "packages": [{
        "id": "client", "name": "client-app", "type": "User Interface Package",
        "elements": [
          { "id": "activity-store", "type": "Store", "name": "ActivityStore" },
          { "id": "receive-comment", "type": "App Event", "name": "ReceiveComment", "children": [
            { "id": "receive-comment-comment", "type": "Property", "name": "comment", "typeRef": { "type": "string" } }
          ] }
        ]
      }],
      "associations": [
        { "id": "store-subscribes", "type": "Subscribes", "source": { "element": "activity-store" }, "target": { "element": "receive-comment" } },
        { "id": "store-sends-comment", "type": "Call Service Operation Action", "source": { "element": "activity-store" }, "target": { "element": "send-comment" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Api/ChatHub.cs
using Microsoft.AspNetCore.SignalR;

namespace Shop.Api;

public class ChatHub : Hub
{
    public async Task SendComment(string body)
    {
        await Clients.All.SendAsync("ReceiveComment", body);
    }
}
```

```ts file src/client/stores/activityStore.ts
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { observable } from 'mobx';

export default class ActivityStore {
    @observable.ref hubConnection: HubConnection | null = null;
    @observable comments: string[] = [];

    createHubConnection = () => {
        this.hubConnection = new HubConnectionBuilder().withUrl('/chat').build();
        this.hubConnection.on('ReceiveComment', (comment) => {
            this.comments.push(comment);
        });
        this.hubConnection.start();
    };

    addComment = async (body: string) => {
        await this.hubConnection!.invoke('sendComment', body);
    };
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "receive-comment", "outcome": "verified", "at": "src/client/stores/activityStore.ts:10" },
    { "kind": "member.exists", "element": "receive-comment-comment", "outcome": "verified" },
    { "kind": "appEvent.subscribes", "element": "store-subscribes", "outcome": "verified", "at": "src/client/stores/activityStore.ts:10" },
    { "kind": "intent.invokes", "element": "store-sends-comment", "outcome": "verified", "at": "src/client/stores/activityStore.ts:17" },
    { "kind": "intent.target", "element": "store-sends-comment", "outcome": "verified", "actual": "ChatHub" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "count": 0 },
    { "rule": "claim/appEvent.subscribes", "count": 0 },
    { "rule": "claim/intent.invokes", "count": 0 }
  ]
}
```

## signalr-client-renamed

The client now handles `CommentReceived` and invokes `PostComment`: the App Event has no code, the store subscribes to nothing of its name, and it invokes no `SendComment`.

```json case
{ "phase": "later", "base": "signalr-client", "kinds": ["appEvent.subscribes", "type.exists", "intent.invokes"], "requirements": ["R14"], "packs": ["intent.core", "intent.services", "intent.ui"], "language": "typescript" }
```

```ts file src/client/stores/activityStore.ts
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { observable } from 'mobx';

export default class ActivityStore {
    @observable.ref hubConnection: HubConnection | null = null;
    @observable comments: string[] = [];

    createHubConnection = () => {
        this.hubConnection = new HubConnectionBuilder().withUrl('/chat').build();
        this.hubConnection.on('CommentReceived', (comment) => {
            this.comments.push(comment);
        });
        this.hubConnection.start();
    };

    addComment = async (body: string) => {
        await this.hubConnection!.invoke('PostComment', body);
    };
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "receive-comment", "outcome": "contradicted" },
    { "kind": "appEvent.subscribes", "element": "store-subscribes", "outcome": "contradicted" },
    { "kind": "intent.invokes", "element": "store-sends-comment", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "receive-comment" },
    { "rule": "claim/appEvent.subscribes", "severity": "error", "subject": "store-subscribes" },
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "store-sends-comment", "messageContains": "makes no `invoke`, `send` or `stream` of `SendComment`" }
  ]
}
```
