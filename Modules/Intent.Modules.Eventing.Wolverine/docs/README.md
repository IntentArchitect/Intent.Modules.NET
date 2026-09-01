# Intent.Eventing.Wolverine

This module integrates [WolverineFx](https://wolverine.netlify.app/) as a message broker for publishing and subscribing to integration events and commands in .NET applications.

## What is Wolverine?

Wolverine is a modern, open-source .NET mediator and messaging framework maintained by JasperFx. It combines a MediatR-style in-process mediator with message-broker transports (RabbitMQ, Azure Service Bus, Amazon SQS) behind a single `WolverineOptions` configuration surface, and is a common migration target for teams moving off a licensed message bus such as MassTransit's commercial tiers.

This module is layered on top of `Intent.Wolverine.Common`, which owns the single, shared `builder.Host.UseWolverine(opts => ...)` registration for the application's ASP.NET host. `Intent.Eventing.Wolverine` (this module) and `Intent.Application.Wolverine` (CQRS command/query dispatch) each contribute their own configuration into that one registration, so installing both never causes one module's handlers to silently overwrite another's.

For more information, see the [Wolverine documentation](https://wolverine.netlify.app/).

## Licensing

This module declares and generates code against only MIT-licensed WolverineFx packages — no licence-gated JasperFx Software product (CritterWatch's licensed features, Critter Stack Pro, the AI Skills packages) is referenced anywhere. The table below is the complete declared set at the pinned version; it is re-checked whenever the module's declared package versions are raised.

| Package                           | Version | Licence |
| --------------------------------- | ------- | ------- |
| `WolverineFx`                     | 5.39.5  | MIT     |
| `WolverineFx.RabbitMQ`            | 5.39.5  | MIT     |
| `WolverineFx.EntityFrameworkCore` | 5.39.5  | MIT     |
| `WolverineFx.SqlServer`           | 5.39.5  | MIT     |
| `WolverineFx.Postgresql`          | 5.39.5  | MIT     |
| `WolverineFx.AzureServiceBus`     | 5.39.5  | MIT     |
| `WolverineFx.AmazonSqs`           | 5.39.5  | MIT     |
| `WolverineFx.AmazonSns`           | 5.39.5  | MIT     |

## What's in this module?

- `WolverineEventingConfiguration` — a generated static class with a single `Configure{Transport}(WolverineOptions, IConfiguration)` method matching the selected Transport setting.
- A host-configuration contribution into `Intent.Wolverine.Common`'s shared `UseWolverine(opts => ...)` lambda, so the generated `Configure{Transport}` method is actually invoked at application startup.
- `appsettings.json` default entries for the selected transport's connection settings.

- Per-message publish and send rules, listener wiring, and one Handler Type Registration per subscribed message's `IIntegrationEventHandler<T>` implementation.
- The Transactional Outbox and Error Handling Policy registrations for the selected settings.

## Module Settings

### Transport

Selects the underlying message transport. Exactly one `Configure{Transport}` method is generated, matching this setting:

| Setting             | Generated method           | NuGet                         |
| ------------------- | -------------------------- | ----------------------------- |
| `Local` (default)   | `ConfigureLocal`           | None — `WolverineFx` only     |
| `RabbitMQ`          | `ConfigureRabbitMq`        | `WolverineFx.RabbitMQ`        |
| `Azure Service Bus` | `ConfigureAzureServiceBus` | `WolverineFx.AzureServiceBus` |
| `Amazon SQS`        | `ConfigureAmazonSqs`       | `WolverineFx.AmazonSqs`       |

`Local` is in-process only — no external broker connection is required. Wolverine already defaults every message to a local, in-process queue when nothing else is configured, so `ConfigureLocal` does not need to do anything until per-message routing is generated (a later wave).

Change the setting in Intent Architect's application settings and rerun the Software Factory to switch transports. Only the NuGet package matching the selected Transport is added — the module never speculatively references a transport package the application does not use.

## Fail-Fast Configuration

Azure Service Bus and Amazon SQS have no built-in default for their connection settings — omitting them is a startup-time configuration error, not something the module can silently default around (unlike RabbitMQ, whose settings default to `localhost`/`guest`/`guest` for local development). The generated code throws `InvalidOperationException` at application startup if the required key is missing:

- Azure Service Bus: `Wolverine:AzureServiceBus:ConnectionString`
- Amazon SQS: `Wolverine:AmazonSqs:Region`

## Which `IMessageBus` To Inject

Two different interfaces are both called `IMessageBus` in an application with this module installed: `Intent.Modules.Eventing.Contracts`'s own `IMessageBus`, and Wolverine's `Wolverine.IMessageBus`. Application code should always inject the **Contracts** `IMessageBus` — the one this module registers `WolverineMessageBus` against — never Wolverine's own.

Injecting `Wolverine.IMessageBus` directly bypasses two things this module relies on: the Composite Message Bus routing that lets an application publish through more than one broker technology without its handlers knowing which one is in play, and the buffered `Publish`/`Send` + explicit `FlushAllAsync` pattern `WolverineMessageBus` implements (see `WolverineMessageBusTemplatePartial.cs`), which defers the actual Wolverine call until the surrounding unit of work is ready to commit. Code that injects `Wolverine.IMessageBus` instead sends immediately, outside of that buffering, and stops participating in Composite Message Bus dispatch entirely.

> [!WARNING]
> This module never generates the call that invokes `FlushAllAsync` — that call belongs to the application's dispatch mechanism (`Intent.Application.Wolverine` contributes it as middleware; a MediatR application contributes it as a pipeline behaviour). An application that installs this module and publishes through `WolverineMessageBus`, but has no dispatch module installed to call `FlushAllAsync`, buffers every message and dispatches none. Generation succeeds and the code compiles either way.

## Generated Code

### WolverineEventingConfiguration

The primary generated file. Contains exactly one `Configure{Transport}` method, matching the selected Transport:

```csharp
public static class WolverineEventingConfiguration
{
    public static void ConfigureRabbitMq(WolverineOptions opts, IConfiguration configuration)
    {
        var section = configuration.GetSection("Wolverine:RabbitMq");
        var host = section["Host"] ?? "localhost";
        // ...
        var transport = opts.UseRabbitMq(rabbit => { /* ... */ });
        transport.AutoProvision();
    }
}
```

### Host Registration

The module contributes a call to the generated `Configure{Transport}` method into `Intent.Wolverine.Common`'s shared `UseWolverine(opts => ...)` lambda in `Program.cs`:

```csharp
builder.Host.UseWolverine(opts =>
{
    WolverineConfiguration.Configure(opts);                              // Intent.Application.Wolverine
    WolverineEventingConfiguration.ConfigureRabbitMq(opts, builder.Configuration); // Intent.Eventing.Wolverine
});
```

## `appsettings.json` Configuration

The module publishes default entries for the selected transport only. Registration is **additive only** — there is no API to remove a previously-registered key, so switching Transport or uninstalling the module leaves any keys it once registered behind for the developer to clean up by hand.

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

`ConnectionString` (Azure Service Bus) and `Region` (Amazon SQS) have no default and are registered empty — see [Fail-Fast Configuration](#fail-fast-configuration). `AccessKey`/`SecretKey` are genuinely optional: leave them empty to fall back to the [AWS credential chain](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/creds-assign.html).

Only the section matching the selected Transport is generated.

## Transport Support Matrix

| Transport         | NuGet package                              |
| ----------------- | ------------------------------------------ |
| Local             | _(none — `WolverineFx` base package only)_ |
| RabbitMQ          | `WolverineFx.RabbitMQ`                     |
| Azure Service Bus | `WolverineFx.AzureServiceBus`              |
| Amazon SQS        | `WolverineFx.AmazonSqs`                    |

`WolverineFx` (the base package) is always added, regardless of the selected Transport.

## Local Development

### Local Transport (no infrastructure required)

The default. Runs entirely in-process — no broker connection is required. This is the fastest way to verify the module is wired up before choosing a real transport.

### RabbitMQ (Docker)

```bash
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.11-management
```

Admin console: `http://localhost:15672/` (guest/guest). The module's defaults (`localhost`/`guest`/`guest`) work against this out of the box.

### Azure Service Bus

Use a real Azure namespace with a connection string, or the [Azure Service Bus emulator](https://learn.microsoft.com/en-us/azure/service-bus-messaging/overview-emulator) for local development. Store the connection string in [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```bash
dotnet user-secrets set "Wolverine:AzureServiceBus:ConnectionString" "<your-connection-string>"
```

### Amazon SQS

`Region` is required. Leave `AccessKey`/`SecretKey` unset to use the [AWS credential chain](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/creds-assign.html), or set both to use explicit credentials:

```bash
dotnet user-secrets set "Wolverine:AmazonSqs:Region" "us-east-1"
```

## Transactional Outbox and Error Handling

### Transactional Outbox

Controls whether Wolverine's durable outbox/inbox is used to guarantee messages are only sent once the related database transaction commits, and that a re-delivered message is safe to handle again.

| Setting          | Behaviour                                                                                                                                                                                                                                                                                                                                             |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `None` (default) | No outbox/inbox. Wolverine has no non-durable (e.g. in-memory) outbox option — this is the only choice for a MassTransit migrator previously using MassTransit's in-memory outbox.                                                                                                                                                                    |
| `Durable`        | Wolverine persists outgoing/incoming messages in the same database as the application: `PersistMessagesWithSqlServer(...)`/`PersistMessagesWithPostgresql(...)` (matching the Database Provider), `UseEntityFrameworkCoreTransactions(...)`, `AutoApplyTransactions()`, `UseDurableOutboxOnAllSendingEndpoints()` and `UseDurableInboxOnAllListeners()`. |

The durable storage technology is **not** a separate setting — it is derived from the modelled **Database Provider** (the same setting `Intent.EntityFrameworkCore` exposes). Only **SQL Server** and **PostgreSQL** are supported; any other provider (in-memory, Cosmos, MySQL, Oracle, SQLite) is a stopping condition when Transactional Outbox is `Durable` — either switch the Database Provider to a supported one, or set Transactional Outbox back to `None`.

Setting Transactional Outbox to `Durable` requires the `Intent.EntityFrameworkCore` module to be installed — this is also a stopping condition if it is not.

> [!NOTE]
> Durable outbox replaces the application layer's explicit message-bus flush with a splice directly into `ApplicationDbContext.SaveChanges`/`SaveChangesAsync` — this is what actually dispatches a published message once buffered, and it fires on **every** `SaveChangesAsync` regardless of which dispatch stack (or none) is installed. A path that publishes without saving does not flush, and an EF bulk operation (`ExecuteUpdate`/`ExecuteDelete`) bypasses `SaveChangesAsync` entirely — this is an acknowledged upstream limitation ([JasperFx/wolverine#1735](https://github.com/JasperFx/wolverine/issues/1735)), not something this module works around.
>
> `UseEntityFrameworkCoreTransactions(...)` opts into `TransactionMiddlewareMode.Lightweight` whenever this application also has a dispatch-layer unit of work that opens its own `TransactionScope` (Wolverine's `UnitOfWorkMiddleware` or MediatR's `UnitOfWorkBehaviour`) — Wolverine's default (Eager) opens its own EF transaction before the handler runs, which collides with that ambient scope (`InvalidOperationException: An ambient transaction has been detected`). Lightweight makes `SaveChangesAsync()` the transactional boundary instead, which enlists cleanly; outbox atomicity is unaffected, since outgoing envelopes are persisted on the same connection as part of that same `SaveChangesAsync`.

> [!IMPORTANT]
> The durable inbox is what makes a re-delivered message safe to handle again (deduplication via Wolverine's own envelope tracking). This module does **no de-duplication of its own** on top of it. `Transactional Outbox = None` means neither an outbox nor an inbox — re-delivered messages are not deduplicated at all.

### Error Handling Policy

Controls the `opts.OnException<Exception>()...` policy applied once per host in the generated `ApplyErrorHandlingPolicy` method. Every branch always ends in `.MoveToErrorQueue()`, so a message that exhausts its retries is never silently dropped.

| Setting                       | Behaviour                                                              | `appsettings.json` key(s) registered                                                        |
| ----------------------------- | ---------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- |
| `None`                        | Moves straight to the error queue on any exception, no retry.          | _(none)_                                                                                    |
| `Retry`                       | Retries a fixed number of times, then moves to the error queue.        | `Wolverine:ErrorHandling:Retry:Attempts` (default `3`)                                      |
| `RetryWithCooldown` (default) | Retries with the given cooldown delays, then moves to the error queue. | `Wolverine:ErrorHandling:RetryWithCooldown:Delays` (default `00:00:01, 00:00:05, 00:00:15`) |
| `ScheduleRetry`               | Schedules retries at the given delays, then moves to the error queue.  | `Wolverine:ErrorHandling:ScheduleRetry:Delays` (default `00:01:00, 00:05:00, 00:15:00`)     |

Delay lists are comma/semicolon-separated `TimeSpan` values. An empty list for `RetryWithCooldown`/`ScheduleRetry` skips straight to `.MoveToErrorQueue()`.

> [!NOTE]
> `appsettings.json` registration is additive only — there is no API to remove a previously registered key. Changing the Error Handling Policy and rerunning the Software Factory leaves the **previous** policy's key(s) behind; delete them by hand if they are no longer relevant: `Wolverine:ErrorHandling:Retry:Attempts`, `Wolverine:ErrorHandling:RetryWithCooldown:Delays`, `Wolverine:ErrorHandling:ScheduleRetry:Delays`.

## Subscriber Queue Naming

A subscribed Integration Event listens on a queue (RabbitMQ, Amazon SQS) or subscription (Azure Service Bus, bound to the publisher's topic) named `{application-name-kebab}-{message-name-kebab}` by default — the subscribing application's name and the message's name, each kebab-cased and joined by a hyphen (e.g. `warehouse-order-created-event`).

Apply the `Wolverine Subscription` stereotype to the subscription (on the Integration Event Handler's association to the Message) and set **Subscriber Queue Name** to override it. An override is used verbatim — it is not kebab-cased or otherwise transformed, so it's your responsibility to keep it valid for the transport you chose. Two cases call for one:

- **The convention name doesn't fit the transport.** It scales with the application's own display name, which has no length ceiling, while Azure Service Bus caps subscription names at 50 characters and AWS SQS/SNS names allow only alphanumerics, hyphens and underscores (a convention name is sanitized for that character set automatically — see the module's `CONTEXT.md` — but sanitizing doesn't shorten it).
- **A platform team pre-provisions broker infrastructure** and needs this application to listen on a specific, already-existing queue/subscription name rather than the convention-generated one.

A subscribed Integration Command listens on its Destination Queue Name unchanged, with no application-name prefix — that point-to-point queue is shared by design.

## Uninstalling

Uninstalling the module and rerunning the Software Factory removes everything the module generates: the `WolverineEventingConfiguration` transport/broker-topology/outbox/error-handling setup, the publish rules and listeners for every message, and the module's contribution to `Intent.Wolverine.Common`'s shared `UseWolverine(opts => ...)` registration. Your hand-written `IIntegrationEventHandler<T>` classes are left in place untouched — this module never owns that file, `Intent.Eventing.Contracts` does, and removal only ever un-generates infrastructure this module itself produced.

There is no per-message handler registration to remove, because none is generated in the first place: a subscribed message's handler is reached through the host's conventional assembly discovery (owned by `Intent.Wolverine.Common`), not through a registration this module writes per message. Uninstalling therefore has nothing message-specific to clean up on the discovery side — only the transport/broker/outbox/error-handling configuration above.

### `appsettings.json` is not cleaned up

The app-settings registration mechanism this module uses is **additive only** — there is no API to request the removal of a previously-registered key, so uninstalling the module and rerunning the Software Factory leaves every key it ever added sitting in `appsettings.json`. Delete these by hand once you have confirmed they are no longer needed:

| Setting area                         | Keys                                                                         |
| ------------------------------------ | ---------------------------------------------------------------------------- |
| RabbitMQ                             | `Wolverine:RabbitMq:Host`, `:Port`, `:VirtualHost`, `:Username`, `:Password` |
| Azure Service Bus                    | `Wolverine:AzureServiceBus:ConnectionString`                                 |
| Amazon SQS                           | `Wolverine:AmazonSqs:Region`, `:AccessKey`, `:SecretKey`                     |
| Error Handling — Retry               | `Wolverine:ErrorHandling:Retry:Attempts`                                     |
| Error Handling — Retry with cooldown | `Wolverine:ErrorHandling:RetryWithCooldown:Delays`                           |
| Error Handling — Schedule retry      | `Wolverine:ErrorHandling:ScheduleRetry:Delays`                               |
| Multi-tenancy (Finbuckle installed)  | `Wolverine:TenantHeader`                                                     |

Only the rows matching settings you actually had configured will be present — the module only ever wrote the section matching the selected Transport and Error Handling Policy at the time.

## Migrating from MassTransit

This section is for a developer replacing `Intent.Eventing.MassTransit` with this module in an existing application. Read it end to end before starting — steps 4 and 5 below matter more than they look.

### Migration steps

1. **Uninstall** `Intent.Eventing.MassTransit` (and its `Intent.Eventing.MassTransit.EntityFrameworkCore` / `.Scheduling` / `.RequestResponse` companions, if installed).
2. **Install** `Intent.Eventing.Wolverine` — this brings in `Intent.Wolverine.Common` automatically as a module dependency; you do not install it separately.
3. **Re-choose settings** using the equivalence table below — Transport, Transactional Outbox and Error Handling Policy all need re-selecting, because none of MassTransit's setting values carry across automatically.
4. **Rerun the Software Factory** and **review every staged change** before applying — this is not a drop-in swap. The generated `MassTransitConfiguration`/`MassTransitEventBus`/per-message consumer classes disappear and are replaced by `WolverineEventingConfiguration`/`WolverineMessageBus`; nothing generated by MassTransit survives untouched.
5. **Build**, and work through the leftover-artefact list below before considering the migration complete.

### Setting equivalence

| MassTransit setting                                                             | Wolverine equivalent                                                                         | Notes                                                                                                                                                                                                                                                                                                                    |
| ------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Messaging Service Provider: In Memory                                           | Transport: Local                                                                             | Both are in-process only.                                                                                                                                                                                                                                                                                                |
| Messaging Service Provider: Rabbit MQ                                           | Transport: RabbitMQ                                                                          |                                                                                                                                                                                                                                                                                                                          |
| Messaging Service Provider: Azure Service Bus                                   | Transport: Azure Service Bus                                                                 |                                                                                                                                                                                                                                                                                                                          |
| Messaging Service Provider: Amazon SQS                                          | Transport: Amazon SQS                                                                        |                                                                                                                                                                                                                                                                                                                          |
| Outbox Pattern: None                                                            | Transactional Outbox: None                                                                   |                                                                                                                                                                                                                                                                                                                          |
| Outbox Pattern: In Memory                                                       | **No equivalent**                                                                            | Wolverine has no non-durable outbox. The closest available option is Transactional Outbox: Durable (SQL Server/PostgreSQL only), which is a stronger guarantee, not a like-for-like swap — or None, which drops the guarantee entirely.                                                                                  |
| Outbox Pattern: Entity Framework                                                | Transactional Outbox: Durable                                                                | Durable storage technology is derived from the modelled Database Provider (SQL Server or PostgreSQL only) rather than chosen directly.                                                                                                                                                                                   |
| Retry Policy: None                                                              | Error Handling Policy: None                                                                  |                                                                                                                                                                                                                                                                                                                          |
| Retry Policy: Immediate                                                         | **No equivalent**                                                                            | Wolverine has no zero-delay repeated-retry policy in this module's option set. Use Retry (fixed attempt count) or Retry with cooldown as the nearest available choice.                                                                                                                                                   |
| Retry Policy: Interval                                                          | Error Handling Policy: Retry with cooldown                                                   | Both retry on a list of delays before moving to an error/dead-letter queue.                                                                                                                                                                                                                                              |
| Retry Policy: Incremental                                                       | **No equivalent**                                                                            | Wolverine's options in this module are fixed-count Retry, fixed-delay-list Retry with cooldown, or scheduled Schedule retry — none step the delay by an increasing increment the way MassTransit's Incremental policy does. Retry with cooldown with a hand-authored increasing delay list is the closest approximation. |
| Retry Policy: Exponential                                                       | **No equivalent**                                                                            | Same reasoning as Incremental — no exponential-backoff option exists in this module. Approximate with a hand-authored delay list on Retry with cooldown or Schedule retry.                                                                                                                                               |
| Use Pre-Commercial Version                                                      | **No equivalent — not applicable**                                                           | This setting exists solely to pin MassTransit below its commercial-licensing threshold. Wolverine has no licensing tiers, so nothing corresponds to it; it is simply dropped.                                                                                                                                            |
| `Message Topology Settings` → `Entity Name` (on the `Message`)                  | `Message Topology Settings` → `Topic Name` (on the `Message`)                                | Same stereotype name and same attachment point — carries across directly.                                                                                                                                                                                                                                                |
| `Command Distribution` → `Destination Queue Name` (on the **send association**) | `Command Distribution` → `Destination Queue Name` (on the **`Integration Command` element**) | The name and property carry across, but **the attachment point moves** — re-set it on the Integration Command itself; a value on the old association is not read.                                                                                                                                                        |
| `Azure Service Bus Consumer Settings`/`RabbitMQ Consumer Settings` → `Endpoint Name` (on the **subscribe association**) | `Wolverine Subscription` → `Subscriber Queue Name` (on the **subscribe association's target end**) | Same attachment point and purpose. The convention-generated default differs: MassTransit's unset default is the consumer class's own name; Wolverine's is `{application-name}-{message-name}`, kebab-cased — so a name MassTransit never needed to override may need overriding here, e.g. to fit Azure Service Bus's 50-character subscription name limit. |

### Artefacts a swap leaves behind

Generated files/classes that stop being produced once `Intent.Eventing.MassTransit` is uninstalled — search for and remove any hand-written code that still references them:

- `MassTransitConfiguration` (DI/bus configuration class)
- `MassTransitEventBus` (the `IEventBus` implementation)
- Per-message `WrapperConsumer<THandler, TMessage>` / `WrapperConsumerDefinition<...>` classes, and the `IAzureServiceBusConsumerSettings` / `IRabbitMQConsumerSettings` types they reference
- The Finbuckle multi-tenancy filters (`FinbuckleConsumingFilter`, `FinbucklePublishingFilter`, `FinbuckleSendingFilter`, `FinbuckleMessageHeaderStrategy`), if the multi-tenancy module was installed

MassTransit types a hand-written application may reference directly and that this module cannot detect or remove automatically — search your own code for these: `IPublishEndpoint`, `ISendEndpointProvider`, `IBus`, `IRequestClient<T>`, `ConsumeContext<T>`, and any `MassTransit.*` using directive. Code that goes through Intent's own `IEventBus` / `IMessageBus` abstraction and `IIntegrationEventHandler<T>` needs no change — both modules implement the same Intent-owned contracts.

### What does not migrate

Messages already in flight, and messages sitting on a MassTransit queue or its error queue, are **not** migrated by this process — nothing in either module drains a broker for you. A Wolverine publisher and a MassTransit subscriber are **not wire-compatible**: they use different message envelopes and serialization conventions, so a message published by one is not a message the other can consume, even against the same broker and queue name.

### Staged, big-bang-free migration

For an application where an all-at-once cutover is unacceptable, both providers can run side by side rather than swapping in one step:

1. Install **both** `Intent.Eventing.MassTransit` and `Intent.Eventing.Wolverine` in the same application.
2. Apply the `Wolverine Message` stereotype to each Message/Integration Command you want carried by Wolverine — designation, not uninstalling MassTransit, is what routes it. An undesignated message keeps flowing through MassTransit.
3. Move messages in batches: designate a batch, rerun the Software Factory, verify, then move to the next batch — rather than every message at once.
4. Once every message is designated to Wolverine, uninstall `Intent.Eventing.MassTransit` and follow the artefact-removal step above.

## Broker Infrastructure

The module always declares the exchanges, topics and queues the application uses — it emits the transport's provisioning call (`.AutoProvision()`) unconditionally. There is no setting for this.

An application whose broker credentials lack permission to declare a destination is therefore **not supported** in this release: it will attempt to declare on startup and the broker will reject it. Joining a pre-provisioned estate owned by a platform team is out of scope. You can still match that team's exact destination names using the `Topic Name` and `Destination Queue Name` overrides on the `Wolverine Message` stereotype, but the declaration attempt itself cannot currently be suppressed.
