# Intent.Eventing.Wolverine

This module integrates [WolverineFx](https://wolverine.netlify.app/) as a message broker for publishing and subscribing to integration events and commands in .NET applications.

## What is Wolverine?

Wolverine is a modern, open-source .NET mediator and messaging framework maintained by JasperFx. It combines a MediatR-style in-process mediator with message-broker transports (RabbitMQ, Azure Service Bus, Amazon SQS) behind a single `WolverineOptions` configuration surface, and is a common migration target for teams moving off a licensed message bus such as MassTransit's commercial tiers.

This module is layered on top of `Intent.Wolverine.Common`, which owns the single, shared `builder.Host.UseWolverine(opts => ...)` registration for the application's ASP.NET host — see its [README](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.Wolverine.Common/docs/README.md) for why that arbitration exists. `Intent.Eventing.Wolverine` (this module) and `Intent.Application.Wolverine` (CQRS command/query dispatch) each contribute their own configuration into that one registration, so installing both never causes one module's handlers to silently overwrite another's.

For more information, see the [Wolverine documentation](https://wolverine.netlify.app/).

### Licensing

Every package this module declares or generates code against is MIT-licensed — no licence-gated JasperFx Software product (Critter Stack Pro, the AI Skills packages) is referenced anywhere. That's the whole reason a MassTransit team escaping a commercial tier would look at Wolverine at all; see [Migrating from MassTransit](#migrating-from-masstransit) below.

## Modeling Integration Events and Commands

Integration Events and Integration Commands are modeled in the Services designer via `Intent.Modelers.Eventing`, the same designer module every broker in this repository builds on — see its [documentation](https://docs.intentarchitect.com/articles/modules-common/intent-modelers-eventing/intent-modelers-eventing.html) for how to model the message contracts themselves.

Two stereotypes on top of that designer control how Wolverine specifically routes a message:

- **`Wolverine Message`** (on the Message/Integration Command) overrides the publish/send name — the convention is a kebab-cased version of the type name, and this only needs setting when the convention doesn't fit (see [Subscriber Queue Naming](#subscriber-queue-naming) for the more common case).
- **`Command Distribution` → `Destination Queue Name`**, attached to the **Integration Command element itself** — not the send association. A command's destination queue is a property of the command, not of any one application's decision to send it: putting it on the element means every sender resolves the same queue, instead of two applications potentially routing the same command to different destinations.

## What This Module Generates

- `WolverineEventingConfiguration` — a generated static class with a single `Configure{Transport}(WolverineOptions, IConfiguration)` method matching the selected Transport setting, contributed into `Intent.Wolverine.Common`'s shared `UseWolverine(opts => ...)` lambda.
- `WolverineMessageBus` — this module's `IMessageBus` implementation, buffering `Publish`/`Send` calls until the surrounding unit of work flushes them (see [Which IMessageBus To Inject](#which-imessagebus-to-inject)).
- Per-message publish and send rules, listener wiring, and one `opts.Discovery.IncludeType<T>()` per subscribed message's `IIntegrationEventHandler<T>` implementation.
- The Transactional Outbox and Error Handling Policy configuration for the selected settings.
- `appsettings.json` default entries for the selected transport's connection settings.

## Which `IMessageBus` To Inject

Two different interfaces are both called `IMessageBus` in an application with this module installed: `Intent.Modules.Eventing.Contracts`'s own `IMessageBus`, and Wolverine's `Wolverine.IMessageBus`. Application code should always inject the **Contracts** `IMessageBus` — the one this module registers `WolverineMessageBus` against — never Wolverine's own.

Injecting `Wolverine.IMessageBus` directly bypasses two things this module relies on: the Composite Message Bus routing that lets an application publish through more than one broker technology without its handlers knowing which one is in play, and the buffered `Publish`/`Send` + explicit `FlushAllAsync` pattern `WolverineMessageBus` implements, which defers the actual Wolverine call until the surrounding unit of work is ready to commit. Code that injects `Wolverine.IMessageBus` instead sends immediately, outside of that buffering, and stops participating in Composite Message Bus dispatch entirely.

> [!WARNING]
> This module never generates the call that invokes `FlushAllAsync` — that call belongs to the application's dispatch mechanism (`Intent.Application.Wolverine` contributes it as middleware; a MediatR application contributes it as a pipeline behaviour). An application that installs this module and publishes through `WolverineMessageBus`, but has no dispatch module installed to call `FlushAllAsync`, buffers every message and dispatches none. Generation succeeds and the code compiles either way.

## Module Settings

### Transport

Selects the underlying message transport, and drives which NuGet package and `Configure{Transport}` method are generated:

| Setting             | NuGet                         |
| ------------------- | ----------------------------- |
| `Local` (default)   | None — `WolverineFx` only     |
| `RabbitMQ`          | `WolverineFx.RabbitMQ`        |
| `Azure Service Bus` | `WolverineFx.AzureServiceBus` |
| `Amazon SQS`        | `WolverineFx.AmazonSqs`       |

`Local` is in-process only, and needs no `Configure` logic of its own — Wolverine already defaults every message to a local, in-process queue when nothing else is configured. Change the setting in Intent Architect's application settings and rerun the Software Factory to switch; only the package matching the selected transport is ever added.

Azure Service Bus and Amazon SQS have no built-in default connection, unlike RabbitMQ (which defaults to `localhost`/`guest`/`guest`) — omitting `Wolverine:AzureServiceBus:ConnectionString` or `Wolverine:AmazonSqs:Region` is a startup-time configuration error the generated code throws `InvalidOperationException` for, rather than silently defaulting around.

### Transactional Outbox

Controls whether Wolverine's durable outbox/inbox guarantees a message is only sent once the related database transaction commits, and that a re-delivered message is safe to handle again.

| Setting          | Behaviour                                                                                                                                                                                                                                            |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `None` (default) | No outbox/inbox. Wolverine has no non-durable outbox option — this is the only choice for a MassTransit migrator previously relying on its in-memory outbox.                                                                                         |
| `Durable`        | Persists outgoing/incoming messages in the same database as the application (`PersistMessagesWithSqlServer`/`PersistMessagesWithPostgresql`, `UseEntityFrameworkCoreTransactions`, `AutoApplyTransactions`, durable outbox/inbox on every endpoint). |

The durable storage technology isn't its own setting — it's derived from the modelled **Database Provider** (the same one `Intent.EntityFrameworkCore` exposes), and only **SQL Server** and **PostgreSQL** are supported. `Intent.EntityFrameworkCore` must be installed for `Durable` at all; either constraint being unmet is a stopping condition, not a silent fallback.

> [!NOTE]
> The durable inbox is what makes a re-delivered message safe to handle again, via Wolverine's own envelope tracking — this module does no de-duplication of its own on top of it. With `Transactional Outbox = None`, a re-delivered message is not deduplicated at all.
> 
> Durable outbox replaces the application layer's explicit message-bus flush with a splice directly into `ApplicationDbContext.SaveChanges`/`SaveChangesAsync`, and fires on **every** save regardless of which dispatch stack is installed. A path that publishes without saving doesn't flush, and an EF bulk operation (`ExecuteUpdate`/`ExecuteDelete`) bypasses `SaveChangesAsync` entirely — an acknowledged upstream limitation ([JasperFx/wolverine#1735](https://github.com/JasperFx/wolverine/issues/1735)), not something this module works around.

### Error Handling Policy

Controls the `opts.OnException<Exception>()...` policy applied once per host. Every branch ends in `.MoveToErrorQueue()`, so a message that exhausts its retries is never silently dropped.

| Setting                       | Behaviour                                       | `appsettings.json` key                                                                      |
| ----------------------------- | ----------------------------------------------- | ------------------------------------------------------------------------------------------- |
| `None`                        | No retry — straight to the error queue.         | _(none)_                                                                                    |
| `Retry`                       | Fixed number of attempts, then the error queue. | `Wolverine:ErrorHandling:Retry:Attempts` (default `3`)                                      |
| `RetryWithCooldown` (default) | Retries at fixed delays, then the error queue.  | `Wolverine:ErrorHandling:RetryWithCooldown:Delays` (default `00:00:01, 00:00:05, 00:00:15`) |
| `ScheduleRetry`               | Schedules retries at the given delays.          | `Wolverine:ErrorHandling:ScheduleRetry:Delays` (default `00:01:00, 00:05:00, 00:15:00`)     |

Delay lists are comma/semicolon-separated `TimeSpan` values; an empty list skips straight to `.MoveToErrorQueue()`. `appsettings.json` registration is additive-only, so changing this setting leaves the previous policy's key behind for you to remove by hand.

## Subscriber Queue Naming

A subscribed Integration Event listens on a queue (RabbitMQ, Amazon SQS) or subscription (Azure Service Bus) named `{application-name-kebab}-{message-name-kebab}` by default — e.g. `warehouse-order-created-event`. This scales with the application's own display name, which has no length ceiling, while Azure Service Bus caps subscription names at 50 characters and AWS SQS/SNS names allow only alphanumerics, hyphens and underscores.

Apply the **`Wolverine Subscription`** stereotype to the subscription (on the Integration Event Handler's association to the Message) and set **Subscriber Queue Name** to override it when the convention name doesn't fit, or when a platform team has pre-provisioned a specific queue/subscription name for you to listen on. An override is used verbatim — it's your responsibility to keep it valid for the transport you chose.

An Integration Command listens on its **Destination Queue Name** unchanged, with no application-name prefix — that point-to-point queue is shared by design.

## `appsettings.json` Configuration

Only the section matching the selected Transport is generated, and registration is additive-only — there's no API to remove a previously-registered key, so switching Transport or uninstalling the module leaves earlier keys behind.

```json
{
  "Wolverine": {
    "RabbitMq": {
      "Host": "localhost",
      "Port": "5672",
      "VirtualHost": "/",
      "Username": "guest",
      "Password": "guest"
    },
    "AzureServiceBus": {
      "ConnectionString": "Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<key>"
    },
    "AmazonSqs": {
      "Region": "",
      "AccessKey": "",
      "SecretKey": ""
    }
  }
}
```

`ConnectionString` (Azure Service Bus) and `Region` (Amazon SQS) have no default and are registered empty. `AccessKey`/`SecretKey` are genuinely optional — leave them empty to fall back to the [AWS credential chain](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/creds-assign.html).

## Broker Infrastructure

The module always declares the exchanges, topics and queues the application uses — it emits `.AutoProvision()` unconditionally, and there's no setting to turn that off. An application whose broker credentials don't allow declaring a destination isn't supported in this release: joining a pre-provisioned estate owned by a platform team is out of scope, though you can still match that team's exact destination names using the `Topic Name` and `Destination Queue Name` overrides.

## Local Development

### Local Transport (no infrastructure required)

The default, and the fastest way to verify the module is wired up before choosing a real transport — runs entirely in-process, no broker connection required.

### RabbitMQ (Docker)

```bash
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.11-management
```

Admin console: `http://localhost:15672/` (guest/guest) — the module's defaults work against this out of the box.

### Azure Service Bus

Use a real Azure namespace, or the [Azure Service Bus emulator](https://learn.microsoft.com/en-us/azure/service-bus-messaging/overview-emulator) for local development. Store the connection string in [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```bash
dotnet user-secrets set "Wolverine:AzureServiceBus:ConnectionString" "<your-connection-string>"
```

### Amazon SQS

```bash
dotnet user-secrets set "Wolverine:AmazonSqs:Region" "us-east-1"
```

Leave `AccessKey`/`SecretKey` unset to use the [AWS credential chain](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/creds-assign.html), or set both for explicit credentials.

## Migrating from MassTransit

This section is for a developer replacing `Intent.Eventing.MassTransit` with this module in an existing application. Read it end to end before starting — the review and coexistence steps below matter more than they look.

1. **Uninstall** `Intent.Eventing.MassTransit` (and its `.EntityFrameworkCore` / `.Scheduling` / `.RequestResponse` companions, if installed).
2. **Install** `Intent.Eventing.Wolverine` — this brings in `Intent.Wolverine.Common` automatically; you don't install it separately.
3. **Re-choose settings** using the table below. None of MassTransit's setting values carry across automatically.
4. **Rerun the Software Factory and review every staged change before applying.** This isn't a drop-in swap — the generated `MassTransitConfiguration`/`MassTransitEventBus`/per-message consumer classes disappear, replaced by `WolverineEventingConfiguration`/`WolverineMessageBus`.
5. **Build**, and work through the [leftover artefacts](#artefacts-a-swap-leaves-behind) below before considering the migration complete.

### Setting equivalence

| MassTransit setting                                                               | Wolverine equivalent                                                                               | Notes                                                                                                                                                                                                                                                                                |
| --------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Messaging Service Provider: In Memory / RabbitMQ / Azure Service Bus / Amazon SQS | Transport: Local / RabbitMQ / Azure Service Bus / Amazon SQS                                       | Direct equivalents.                                                                                                                                                                                                                                                                  |
| Outbox Pattern: None                                                              | Transactional Outbox: None                                                                         |                                                                                                                                                                                                                                                                                      |
| Outbox Pattern: In Memory                                                         | **No equivalent**                                                                                  | Wolverine has no non-durable outbox. The closest option is `Durable` (SQL Server/PostgreSQL only) — a stronger guarantee, not a like-for-like swap — or `None`, which drops the guarantee entirely.                                                                                  |
| Outbox Pattern: Entity Framework                                                  | Transactional Outbox: Durable                                                                      | The storage technology is derived from **Database Provider** rather than chosen directly.                                                                                                                                                                                            |
| Retry Policy: None                                                                | Error Handling Policy: None                                                                        |                                                                                                                                                                                                                                                                                      |
| Retry Policy: Immediate                                                           | **No equivalent**                                                                                  | Use `Retry` or `RetryWithCooldown` as the nearest available choice.                                                                                                                                                                                                                  |
| Retry Policy: Interval                                                            | Error Handling Policy: RetryWithCooldown                                                           | Both retry on a list of delays before an error/dead-letter queue.                                                                                                                                                                                                                    |
| Retry Policy: Incremental / Exponential                                           | **No equivalent**                                                                                  | No stepped or exponential-backoff option in this module. Approximate with a hand-authored delay list on `RetryWithCooldown` or `ScheduleRetry`.                                                                                                                                      |
| Use Pre-Commercial Version                                                        | **Not applicable**                                                                                 | Existed only to pin MassTransit below its commercial-licensing threshold — Wolverine has no licensing tiers.                                                                                                                                                                         |
| `Message Topology Settings` → `Entity Name`                                       | `Message Topology Settings` → `Topic Name`                                                         | Same stereotype and attachment point.                                                                                                                                                                                                                                                |
| `Command Distribution` → `Destination Queue Name` (on the **send association**)   | Same name, on the **Integration Command element**                                                  | The attachment point moves — re-set it on the command itself; a value on the old association isn't read.                                                                                                                                                                             |
| Consumer Settings → `Endpoint Name` (on the **subscribe association**)            | `Wolverine Subscription` → `Subscriber Queue Name` (on the **subscribe association's target end**) | Same attachment point. The unset default differs: MassTransit's is the consumer class name; Wolverine's is `{application-name}-{message-name}` kebab-cased — a name MassTransit never needed to override may need overriding here (e.g. for Azure Service Bus's 50-character limit). |

### Artefacts a swap leaves behind

Search for and remove any hand-written code still referencing what MassTransit stops generating: `MassTransitConfiguration`, `MassTransitEventBus`, per-message `WrapperConsumer<THandler, TMessage>`/`WrapperConsumerDefinition<...>` classes, and (if multi-tenancy was installed) the Finbuckle filters (`FinbuckleConsumingFilter`, `FinbucklePublishingFilter`, `FinbuckleSendingFilter`, `FinbuckleMessageHeaderStrategy`).

Also search your own application code for MassTransit types this module can't detect automatically: `IPublishEndpoint`, `ISendEndpointProvider`, `IBus`, `IRequestClient<T>`, `ConsumeContext<T>`, and any `MassTransit.*` using directive. Code written against Intent's own `IMessageBus`/`IEventBus` and `IIntegrationEventHandler<T>` needs no change — both modules implement the same Intent-owned contracts.

### What doesn't migrate

Nothing in either module drains a broker for you — messages already in flight, or sitting on a MassTransit queue or its error queue, aren't migrated by this process. A Wolverine publisher and a MassTransit subscriber also aren't wire-compatible: different envelopes and serialization conventions mean a message published by one can't be consumed by the other, even against the same broker and queue name.

### Staged, big-bang-free migration

For an application where an all-at-once cutover is unacceptable, both providers can run side by side:

1. Install **both** `Intent.Eventing.MassTransit` and `Intent.Eventing.Wolverine`.
2. Apply the `Wolverine Message` stereotype to each Message/Integration Command you want carried by Wolverine — an undesignated message keeps flowing through MassTransit.
3. Move messages in batches: designate a batch, rerun the Software Factory, verify, then move to the next.
4. Once every message is designated to Wolverine, uninstall `Intent.Eventing.MassTransit` and work through the artefact list above.

## Uninstalling

Uninstalling and rerunning the Software Factory removes everything this module generates — transport/broker-topology/outbox/error-handling setup, publish rules and listeners, and its contribution to `Intent.Wolverine.Common`'s shared registration. Your hand-written `IIntegrationEventHandler<T>` classes are untouched; this module never owns that file, `Intent.Eventing.Contracts` does. There's no per-message handler registration to remove either — a subscribed message's handler is reached through the host's conventional assembly discovery, not a registration this module writes per message.

`appsettings.json` isn't cleaned up — the additive-only registration mechanism means every key this module ever added stays behind. Once you've confirmed they're no longer needed, remove by hand whichever of these you actually had configured: `Wolverine:RabbitMq:*`, `Wolverine:AzureServiceBus:ConnectionString`, `Wolverine:AmazonSqs:*`, `Wolverine:ErrorHandling:*`, and (if multi-tenancy was installed) `Wolverine:TenantHeader`.

## Related Modules

### [Intent.Wolverine.Common](https://docs.intentarchitect.com/articles/modules-dotnet/intent-wolverine-common/intent-wolverine-common.html)

Owns the single shared `builder.Host.UseWolverine(opts => ...)` registration this module contributes into, and arbitrates contribution order between it and every other Wolverine-based module.

### [Intent.Application.Wolverine](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-wolverine/intent-application-wolverine.html)

Wires Wolverine as the application's CQRS command/query dispatcher. Not required to use this module standalone, but the two share the same host registration and, when both are installed, `Intent.Application.Wolverine`'s middleware is what actually calls `FlushAllAsync` on the bus this module registers.

### [Intent.Eventing.Contracts](https://docs.intentarchitect.com/articles/modules-dotnet/intent-eventing-contracts/intent-eventing-contracts.html)

Owns the transport-agnostic `IMessageBus` interface this module implements against, plus the Composite Message Bus that routes between providers when more than one broker module is installed.

### [Intent.EntityFrameworkCore](https://docs.intentarchitect.com/articles/modules-dotnet/intent-entityframeworkcore/intent-entityframeworkcore.html)

Required when **Transactional Outbox** is set to `Durable` — the outbox persists outgoing/incoming messages through this module's `DbContext` and database provider.
