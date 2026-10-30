---
contentHash: 087B746F72AFC5F23CF744DA0F32E3400888AFCD1D682BF4B3F1A60A471EF167
---
# Handler intent: the Services recipes for what each handler does

Add these recipes to `guardrails/services.drift.json` when the Services model has intent: entity actions, invocations, published integration events or sent integration commands, modelled on Commands, Queries, Service operations or event handlers' subscriptions. They are tested against handlers in each style below. Copy the `facts` into the recipe file you started from the HTTP guide, make the decisions below, then adapt them to this codebase's own conventions. Don't write them again from scratch.

## Recognise it

The model's interaction associations, each one fact about its handler: `intent.creates`, `intent.updates`, `intent.deletes` and `intent.queries` (`Create`, `Update`, `Delete` and `Query Entity Action` to a Domain class), `intent.invokes` (`Perform Invocation`), `intent.publishes` (`Publish Integration Event`) and `intent.sends` (`Send Integration Command`). The recipes find the handler's method and read its body for each target. They find it in three styles:

- **A Service operation:** the Service's method, `class:{Service} > method:{Operation}`; parameter names choose between overloads.
- **A Command or Query:** its MediatR handler, the class implementing `IRequestHandler<CreateOrderCommand, …>` (or `ICommandHandler<…>`, `IQueryHandler<…>`), and its `Handle` method.
- **An event handler's subscription:** the handler class (`{Service}`) and its method taking the message or domain event (`{Operation}`): Intent's `IIntegrationEventHandler<T>.HandleAsync(T …)`, MassTransit's `IConsumer<T>.Consume(ConsumeContext<T> …)`, NServiceBus's `IHandleMessages<T>.Handle(T …)` and MediatR's `INotificationHandler<T>.Handle(T …)`.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| Do the handlers live where the recipe file's `files` looks? | The HTTP recipes often look only at the API project | Give each recipe below its own `files` naming the application projects |
| How does a handler reach an entity? | A repository field named after the entity (`IOrderRepository _orderRepository`), or a `DbContext` with a `DbSet<Order> Orders` | Repository fields named otherwise, or irregular `DbSet` plurals: see Adapting to house style |
| Are request handlers found another way? | A base handler class (`IdentifiedCommandHandler<T, R>`), or Wolverine-style `Handle(CreateOrderCommand command)` methods in plain classes | Add a find path for that style before the last: `class with :IdentifiedCommandHandler<{Operation}[,>] > method with \\bHandle\\({Operation} ` |

## The recipe file

```json
{
  "files": "src/**/*.cs",
  "facts": {
    "intent.creates": {
      "find": [
        "class:{Service} > method:{Operation}",
        "class:{Service} > method with [(<]{Operation}[ >]",
        "class with [:,]I(?:Request|Command|Query)Handler<{Operation}[,>] > method with \\bHandle(?:Async)?\\({Operation} "
      ],
      "read": [
        { "part": "body", "match": "\\bnew ({Entity})\\b" },
        { "part": "body", "match": "\\b({Entity})\\.{Member}\\(" },
        { "part": "body", "match": "\\.Add({Entity})\\(" }
      ],
      "default": "",
      "unstated": "no `new {Entity}`, factory call or `Add{Entity}(…)` in the handler"
    },
    "intent.updates": {
      "find": [
        "class:{Service} > method:{Operation}",
        "class:{Service} > method with [(<]{Operation}[ >]",
        "class with [:,]I(?:Request|Command|Query)Handler<{Operation}[,>] > method with \\bHandle(?:Async)?\\({Operation} "
      ],
      "read": [
        { "part": "body", "match": "\\.{Member}\\(", "value": "{Entity}" },
        { "part": "body", "match": "\\b_?{Entity|camel}Repository{Member}\\.(?:Update|Get|Find)\\w*\\(", "value": "{Entity}" },
        { "part": "body", "match": "\\.{Entity}(?:s|es)?{Member}\\.(?:Update|Find|First|Single)\\w*\\(", "value": "{Entity}" }
      ],
      "default": "",
      "unstated": "the handler neither calls the operation the action targets nor loads a `{Entity}` to change"
    },
    "intent.deletes": {
      "find": [
        "class:{Service} > method:{Operation}",
        "class:{Service} > method with [(<]{Operation}[ >]",
        "class with [:,]I(?:Request|Command|Query)Handler<{Operation}[,>] > method with \\bHandle(?:Async)?\\({Operation} "
      ],
      "read": [
        { "part": "body", "match": "\\b_?{Entity|camel}Repository\\.Remove\\w*\\(", "value": "{Entity}" },
        { "part": "body", "match": "\\.{Entity}(?:s|es)?\\.Remove\\w*\\(", "value": "{Entity}" },
        { "part": "body", "match": "\\.Remove({Entity})\\(" }
      ],
      "default": "",
      "unstated": "no `Remove(…)` of a `{Entity}` in the handler"
    },
    "intent.queries": {
      "find": [
        "class:{Service} > method:{Operation}",
        "class:{Service} > method with [(<]{Operation}[ >]",
        "class with [:,]I(?:Request|Command|Query)Handler<{Operation}[,>] > method with \\bHandle(?:Async)?\\({Operation} "
      ],
      "read": [
        { "part": "body", "match": "\\b_?{Entity|camel}Repository\\.(?:Find|Get|List|Any|Count)\\w*\\(", "value": "{Entity}" },
        { "part": "body", "match": "\\.{Entity}(?:s|es)?\\.\\w+\\(", "value": "{Entity}" },
        { "part": "body", "match": "\\bSet<{Entity}>\\(\\)", "value": "{Entity}" }
      ],
      "default": "",
      "unstated": "the handler doesn't read a `{Entity}` through its repository or `DbSet`"
    },
    "intent.invokes": {
      "find": [
        "class:{Service} > method:{Operation}",
        "class:{Service} > method with [(<]{Operation}[ >]",
        "class with [:,]I(?:Request|Command|Query)Handler<{Operation}[,>] > method with \\bHandle(?:Async)?\\({Operation} "
      ],
      "read": [
        { "part": "body", "match": "(?<!\\w)_?{InvokedOwner|camel}\\.({Invoked})(?:Async)?\\(" },
        { "part": "body", "match": "\\bnew ({Invoked})\\b" },
        { "part": "body", "match": "\\.{InvokedOwner}({Invoked})(?:Async)?\\(" }
      ],
      "default": "",
      "unstated": "no call of `{Invoked}` and no `new {Invoked}` in the handler"
    },
    "intent.publishes": {
      "find": [
        "class:{Service} > method:{Operation}",
        "class:{Service} > method with [(<]{Operation}[ >]",
        "class with [:,]I(?:Request|Command|Query)Handler<{Operation}[,>] > method with \\bHandle(?:Async)?\\({Operation} "
      ],
      "read": [
        { "part": "body", "match": "\\bnew ({Message})\\b" },
        { "part": "body", "match": "\\bPublish(?:Async)?<({Message})>" }
      ],
      "default": "",
      "unstated": "no `new {Message}` or `Publish<{Message}>` in the handler"
    },
    "intent.sends": {
      "find": [
        "class:{Service} > method:{Operation}",
        "class:{Service} > method with [(<]{Operation}[ >]",
        "class with [:,]I(?:Request|Command|Query)Handler<{Operation}[,>] > method with \\bHandle(?:Async)?\\({Operation} "
      ],
      "read": [
        { "part": "body", "match": "\\bnew ({Message})\\b" },
        { "part": "body", "match": "\\bSend(?:Async)?<({Message})>" }
      ],
      "default": "",
      "unstated": "no `new {Message}` or `Send<{Message}>` in the handler"
    }
  }
}
```

The find paths are tried in order: a Service operation's method, then an event handler's method taking the message, then a Command's or Query's handler. `{Service}` is empty for a Command or Query, so the first two find nothing for one.

`{Member}` and `{InvokedOwner}` are empty when the model doesn't name one. A case that puts one next to other text (`Repository{Member}\\.`, `\\.{InvokedOwner}({Invoked})`) therefore reads only when it is empty, and a case that reads it on its own (`\\.{Member}\\(`) only when it isn't. Keep that when adapting a case, so that an action that targets an operation is read from that operation's call, and an invocation of a Service's operation from that Service's field.

## How each form is modelled

The recipes read the handlers in the Services designer's terms, so the model must use the same terms (reverse-engineer-services §5). Every intent fact is a modelled interaction the handler should perform, so its value comes from the code: one from `default` is a handler that doesn't do what the model says, or a form the recipe doesn't read yet.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `intent.creates` | `new Order(…)`: `Create Entity Action` → `Order`, or → its constructor. `Buyer.Create(…)`: → that static factory operation. A child added through its aggregate, `order.AddOrderItem(…)`: → the child, `OrderItem` | Code |
| `intent.updates` | A call of the entity's operation (`order.SetCancelledStatus()`): `Update Entity Action` → that operation. Changing what it loaded through its repository or `DbSet` (`_orderRepository.FindByIdAsync(…)`, then setting a property): → the class | Code |
| `intent.deletes` | `_orderRepository.Remove(order)`, `_db.Orders.Remove(order)`, or a child through its aggregate (`order.RemoveOrderItem(…)`): `Delete Entity Action` → that entity | Code |
| `intent.queries` | Reading it through its repository (`FindByIdAsync`, `FindAllAsync`, `GetAsync`, …) or its `DbSet` (`_db.Orders.Where(…)`, `Set<Order>()`): `Query Entity Action` → the class. An update reads first, so it gets no `Query Entity Action` too | Code |
| `intent.invokes` | `_orderService.Approve(…)`, `_paymentsClient.ChargeAsync(…)`, `_pricingService.Quote(…)`: `Perform Invocation` → that Service, Service Proxy, Domain Service or Repository `Operation`, its owner's field named after it. `_sender.Send(new ApproveOrderCommand(…))`: → the `Command` or `Query`. A stored procedure's call, on any object: → the `Stored Procedure` | Code |
| `intent.publishes` | `new OrderStartedIntegrationEvent(…)` handed to `IEventBus.Publish`, `IMessageBus.Publish`, MassTransit's `Publish`, NServiceBus's `context.Publish` or an outbox (`AddAndSaveEventAsync(…)`), or MassTransit's `Publish<OrderStarted>(new { … })`: `Publish Integration Event` → the integration event | Code |
| `intent.sends` | `new CancelPaymentCommand(…)` handed to `Send` (an endpoint from `ISendEndpointProvider`, `IMessageBus`, NServiceBus's `context.Send`), or `Send<CancelPayment>(new { … })`: `Send Integration Command` → the integration command | Code |

## Adapting to house style

| You see | Change |
| --- | --- |
| Repository fields named otherwise (`_orders`, `_repository`, `this.orders`) | In each `Repository` case, change `_?{Entity|camel}Repository` to the codebase's pattern, such as `_{Entity|camel}s`, keeping `{Member}` where it is |
| A `DbSet` whose name isn't the entity's plural with `s` or `es` (`Categories`, `People`) | A case naming it, before the others: `\\.Categories\\.\\w+\\(`, value `Category` |
| Service fields named otherwise (`_orders` for `OrderService`) | Change `_?{InvokedOwner|camel}` in `intent.invokes`' first case to the pattern, such as `_{InvokedOwner|camel|trimEnd:Service}s` |
| Messages built by a factory (`OrderEvents.Started(order)`) or published through a domain event | A case for that call, value `{Message}`, or model the invocation the handler performs instead |
| A handler whose work is in a private method of its class | Change the last step of each find path to the class (`class:{Service}`, `class with [:,]I(?:Request|Command|Query)Handler<{Operation}[,>]`), so the cases read the whole class, and keep `part: body` |

## What it can't read

- Whether an interaction happens, only that the handler's code states it: a `new Order(…)` that is never saved, or an event that is built but not published, reads as done.
- An interaction performed by another class the handler calls (a domain operation that publishes, a helper service): it reads as missing. Model the invocation of that class's operation instead, or widen the find to it as above.
- An interaction the code performs and the model doesn't show: no fact states it, so nothing reports it. Model it when you see it.
- The same interaction twice in one handler: the probe changes the first and warns that the second still reads the same. The reading is right; tell the developer rather than weakening the recipe.
- A `Query Entity Action`'s multiplicity, an action's mappings, and the order of a handler's interactions aren't checked.
