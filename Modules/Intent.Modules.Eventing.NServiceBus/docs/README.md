# Intent.Eventing.NServiceBus

This module integrates [NServiceBus](https://particular.net/nservicebus) as a message broker for publishing and subscribing to integration events and commands in .NET applications.

## What is NServiceBus?

NServiceBus is a mature, commercially-supported service bus for .NET, built by Particular Software. It wraps a message transport (RabbitMQ, Azure Service Bus, Amazon SQS, SQL Server, or its file-based Learning Transport) behind a single named endpoint per application, and adds durable retries, an error queue, audit trails and a transactional outbox on top.

For more information, see the [NServiceBus documentation](https://docs.particular.net/nservicebus/).

## Modeling Integration Events and Commands

Integration Events and Integration Commands are modeled in the Services designer, using the same `Intent.Modelers.Eventing` module every other broker in this repository builds on — see its [README](https://github.com/IntentArchitect/Intent.Modules/blob/development/Modules/Intent.Modules.Modelers.Eventing/README.md) for how to model the message contracts themselves. `Intent.Eventing.NServiceBus` installs it automatically.

- **Integration Events** are broadcast: any application can publish one, any application can subscribe, and NServiceBus's pub/sub mechanism (SNS for SQS, Topics for Azure Service Bus, exchanges for RabbitMQ) takes care of fan-out. They carry no endpoint name.
- **Integration Commands** are point-to-point: exactly one endpoint owns and handles a given command type, and every sender needs to know where to send it.

### The `NServiceBus` Stereotype on Integration Commands

Because a command is only ever handled by one endpoint, that endpoint has to be named somewhere both the sender and the handler agree on. Apply the **NServiceBus** stereotype to the Integration Command and set **Endpoint Name** to the `NServiceBus:EndpointName` value of the application that handles it:

1. Open the Integration Command in the Services designer.
2. Right-click → **Add Stereotype** → **NServiceBus**.
3. Set **Endpoint Name**.

> [!IMPORTANT]
> `EndpointName` is a property of the command itself, not of any one sender — both the sending application and the handling application must carry the same value. Omit it and the Software Factory fails with an `ElementException` naming the offending command, rather than generating something that silently can't be routed.

## What This Module Generates

- `NServiceBusConfiguration` — builds the `EndpointConfiguration` (transport, persistence, installers, serialization, recoverability, message conventions) and registers the message bus in DI.
- `NServiceBusMessageHandler<TMessage>` — a single open-generic handler, registered once per subscribed message, that delegates to the application layer's `IIntegrationEventHandler<TMessage>`.
- Command routing entries (`RouteToEndpoint`) for every Integration Command this application sends.
- `appsettings.json` default entries for the selected transport and recoverability policy.

## Open Generic Handlers, Not Assembly Scanning

NServiceBus's own handler discovery scans assemblies for types implementing `IHandleMessages<T>` — but it explicitly skips open generic type definitions, which is exactly what `NServiceBusMessageHandler<TMessage>` is. Registering it via DI doesn't help either: DI is only consulted once NServiceBus's internal handler registry already knows a type should be resolved, so a handler DI can see but the registry doesn't know about is never invoked.

This module works around that by registering each subscribed message's handler directly against NServiceBus's internal registries:

```csharp
RegisterHandler<NServiceBusMessageHandler<TMessage>, TMessage>(endpointConfiguration);
```

There's no generated per-message consumer class to look at or customize — `NServiceBusMessageHandler<TMessage>` is the only handler type, for every subscribed message, and your business logic lives entirely in the `IIntegrationEventHandler<TMessage>` implementation `Intent.Eventing.Contracts` scaffolds.

## Module Settings

### Transport

| Setting | Transport | NuGet |
|---|---|---|
| `Learning Transport` (default) | File-based, local only | Included in `NServiceBus` |
| `RabbitMQ` | RabbitMQ, Quorum queues, conventional routing | `NServiceBus.RabbitMQ` |
| `Azure Service Bus` | Azure Service Bus Topics | `NServiceBus.Transport.AzureServiceBus` |
| `Amazon SQS` | AWS SQS + SNS | `NServiceBus.AmazonSQS` |
| `SQL Server` | SQL Server tables used as queues — no separate broker to run | `NServiceBus.Transport.SqlServer` |

Change the setting in Intent Architect's application settings and rerun the Software Factory to switch. Amazon SQS is the one transport with nothing to configure in `appsettings.json` — it relies entirely on the [AWS credential chain](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/creds-assign.html).

### Persistence and Enable Outbox

**Persistence** selects the storage behind sagas, subscriptions and (when the outbox is on) exactly-once dispatch: `None`, `SQL Persistence` (shares the EF Core `DbConnection`/`DbTransaction`, requires `Intent.EntityFrameworkCore`), or `NHibernate` (manages its own session, no EF Core dependency).

**Enable Outbox** turns on NServiceBus's transactional outbox, so a published message only leaves the endpoint once the database transaction that produced it has committed. It requires `Persistence` to be `SQL Persistence` or `NHibernate` — the Software Factory fails with a descriptive error if that dependency isn't satisfied.

### Recoverability Policy

Controls what happens when handling a message throws: `None` (straight to the error queue), `Immediate Only`, `Delayed Only`, or `Immediate and Delayed`. Retry counts and delay values live in `appsettings.json` so they can be tuned per environment without regenerating.

### Enable Audit Queue / Enable Instance Identification

Both are opt-in. Audit Queue forwards every processed message to an audit queue (`NServiceBus:AuditQueue`, plus an optional `NServiceBus:AuditTimeToBeReceived`); Instance Identification tags the running process for the Particular Service Platform's monitoring tools (`NServiceBus:InstanceId`).

### License Path

Not a module setting — if `NServiceBus:LicensePath` is present in `appsettings.json`, the generated configuration calls `endpointConfiguration.LicensePath(path)`. Leave the key out entirely to fall back to NServiceBus's own license discovery.

## `appsettings.json` Configuration

```json
{
  "NServiceBus": {
    "EndpointName": "MyApplication",
    "ErrorQueue": "error",
    "Recoverability": {
      "ImmediateRetries": 5,
      "DelayedRetries": 3,
      "DelayIncreaseSeconds": 10
    },
    "LearningTransport": {
      "StorageDirectory": "%TEMP%\\nservicebus-learning"
    },
    "Routing": {
      "Commands": {
        "OrderAnimal": "TargetApp"
      }
    }
  },
  "ConnectionStrings": {
    "RabbitMQ": "amqp://guest:guest@localhost:5672",
    "AzureServiceBus": "Endpoint=sb://<namespace>.servicebus.windows.net/;...",
    "NServiceBus": "Server=.;Database=NServiceBus;Integrated Security=true;TrustServerCertificate=true"
  }
}
```

`LicensePath`, `AuditQueue`, `AuditTimeToBeReceived` and `InstanceId` are only written when their corresponding setting is enabled. The `ConnectionStrings:NServiceBus` entry is only relevant to the SQL Server transport, whose `EnableInstallers()` call creates the queue tables on first run — no manual schema setup required.

## Working with Multiple Message Bus Providers

This module coexists with other message bus providers — MassTransit, Kafka, and the rest — in the same application. With only `Intent.Eventing.NServiceBus` installed, every message automatically routes through it, no configuration required.

Once a second provider is installed, Intent Architect generates a **Composite Message Bus** that routes each message to the right broker, and every message needs a broker designation: apply the **NServiceBus** stereotype to a Package or Folder in the Services designer, and its child elements inherit it automatically. An Integration Command or Event installed in a multi-broker application with no designation at all fails the Software Factory run outright, rather than silently going nowhere.

For the full architecture behind this — the generated `CompositeMessageBus`, the `MessageBrokerRegistry`, and how `IMessageBus` routing actually works — see the [Intent.Eventing.Contracts documentation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-eventing-contracts/intent-eventing-contracts.html).

## Local Development

### Learning Transport (no infrastructure required)

The default. Stores messages as files on disk — no external service needed, and the fastest way to confirm handler wiring locally.

### SQL Server Transport (LocalDB or Docker)

LocalDB, already installed alongside Visual Studio, works out of the box:

```json
{
  "ConnectionStrings": {
    "NServiceBus": "Server=(localdb)\\MSSQLLocalDB;Database=NServiceBus;Integrated Security=true;TrustServerCertificate=true"
  }
}
```

Or run SQL Server in Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

Either way, `EnableInstallers()` creates the queue tables automatically on first run.

### RabbitMQ (Docker)

```bash
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.11-management
```

Admin console: `http://localhost:15672/` (guest/guest).

### Azure Service Bus

Use a real Azure namespace, or the [Azure Service Bus emulator](https://learn.microsoft.com/en-us/azure/service-bus-messaging/overview-emulator) for local development. Store the connection string in [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) rather than `appsettings.json`:

```bash
dotnet user-secrets set "ConnectionStrings:AzureServiceBus" "<your-connection-string>"
```

### Amazon SQS

Uses the [AWS credential chain](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/creds-assign.html) — no connection string. Either run `aws configure`, or set the environment variables directly:

```bash
$env:AWS_ACCESS_KEY_ID = "..."
$env:AWS_SECRET_ACCESS_KEY = "..."
$env:AWS_REGION = "eu-west-1"
```

`EnableInstallers()` creates the SQS queues and SNS topics automatically on first run.

## Related Modules

### [Intent.Eventing.Contracts](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.Eventing.Contracts/docs/README.md)

Owns the transport-agnostic `IMessageBus` interface and `IIntegrationEventHandler<T>` this module implements against, plus the Composite Message Bus that routes between providers when more than one is installed.

### [Intent.EntityFrameworkCore](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.EntityFrameworkCore/docs/README.md)

Required when **Persistence** is set to `SQL Persistence` and **Enable Outbox** is on — the outbox shares this module's `DbContext` connection and transaction to get exactly-once dispatch.
