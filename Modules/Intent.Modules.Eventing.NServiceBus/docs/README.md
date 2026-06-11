# Intent.Eventing.NServiceBus

This module integrates [NServiceBus](https://particular.net/nservicebus) as a message broker for publishing and subscribing to integration events and commands in .NET applications.

## What is NServiceBus?

NServiceBus is a battle-tested service bus for .NET that provides reliable messaging, automatic retries, error queues, and durable outbox support. It is built around the concept of a single named endpoint per application, with support for multiple transport technologies.

For more information, see the [NServiceBus documentation](https://docs.particular.net/nservicebus/).

## What's in this module?

- Modeling Integration Events and Integration Commands in the Services designer.
- NServiceBus endpoint configuration generation (transport, serialization, recoverability, installers).
- Open generic handler wiring (`NServiceBusMessageHandler<TMessage>`) with explicit registry registration — no assembly scanning required.
- Command routing (`RouteToEndpoint`) for commands sent to other endpoints.
- SQL Persistence transactional outbox (with EF Core shared connection).
- `appsettings.json` configuration entries.
- Dependency Injection wiring.
- Multi-broker coexistence support via the `NServiceBus` stereotype.
- .NET 8/9 (`UseNServiceBusHost`) and .NET 10+ (`AddNServiceBusEndpoint`) host registration.

## Module Settings

### Transport

Selects the underlying message transport. Options:

| Setting | Transport | NuGet |
|---|---|---|
| `Learning Transport` | File-based, local only — ideal for development | Included in `NServiceBus` |
| `RabbitMQ` | RabbitMQ (Quorum queues, Conventional routing) | `NServiceBus.RabbitMQ` |
| `Azure Service Bus` | Azure Service Bus Topics | `NServiceBus.Transport.AzureServiceBus` |
| `Amazon SQS` | AWS SQS + SNS | `NServiceBus.AmazonSQS` |

The default is `Learning Transport`. Change the setting in Intent Architect's application settings and rerun the Software Factory to switch transports.

### Outbox Pattern

Enables the NServiceBus transactional outbox, which ensures messages are only dispatched after the database transaction commits.

| Setting | Behaviour |
|---|---|
| `None` | No outbox. Messages are dispatched inline. |
| `Sql Persistence` | Uses `NServiceBus.Persistence.Sql` with a shared EF Core `DbConnection`/`DbTransaction`. Requires `Intent.EntityFrameworkCore`. |

> [!IMPORTANT]
>
> Setting OutboxPattern to `Sql Persistence` without the `Intent.EntityFrameworkCore` module installed will cause the Software Factory to fail with a descriptive error.

### Recoverability Policy

Configures automatic retry behaviour when message processing fails.

| Setting | Behaviour |
|---|---|
| `None` | No retries. Failed messages are immediately sent to the error queue. |
| `Immediate Only` | Immediate consecutive retries before moving to the error queue. |
| `Delayed Only` | Delayed retries with configurable back-off before moving to the error queue. |
| `Immediate and Delayed` | Immediate retries followed by delayed retries. |

Retry counts and delay values are driven by `appsettings.json` and can be overridden per environment.

## Modeling Integration Events and Commands

Integration Events and Integration Commands are modeled in the Services designer. This module installs the `Intent.Modelers.Eventing` module which provides the designer elements.

### Integration Events

Events represent something that happened. Any application can publish an event; any application can subscribe to it. Events do not require an endpoint name — they are broadcast via the transport's pub/sub mechanism (SNS for SQS, Topics for Azure Service Bus, Exchanges for RabbitMQ).

### Integration Commands

Commands target a specific endpoint. The **NServiceBus stereotype** is mandatory on every Integration Command:

1. Open the Integration Command in the Services designer.
2. Right-click → **Add Stereotype** → **NServiceBus**.
3. Set **Endpoint Name** to the `NServiceBus:EndpointName` config value of the application that handles this command.

> [!IMPORTANT]
>
> `EndpointName` must be set on every NServiceBus Integration Command — both in the application that sends it and in the application that subscribes to it. The value identifies which endpoint owns the command. Omitting it causes the Software Factory to fail with a descriptive error pointing to the offending command.

## Generated Code

### NServiceBusConfiguration

The primary generated file. Contains:

- `AddNServiceBusConfiguration(IServiceCollection, IConfiguration)` — registers the message bus in DI and (on .NET 10+) starts the NServiceBus endpoint.
- `UseNServiceBusHost(IHostBuilder)` — (.NET 8/9 only) extension method called from `Program.cs` to start the endpoint on the host builder.
- `ConfigureMainEndpoint(IConfiguration)` — private method that builds the full `EndpointConfiguration`: transport, persistence, installers, serialization, recoverability, message conventions, handler registration, and command routing.

### Handler Pattern

NServiceBus handler discovery via assembly scanning does not work for open generic types. This module uses explicit registry registration instead:

```csharp
RegisterHandler<NServiceBusMessageHandler<TMessage>, TMessage>(endpointConfiguration);
```

`NServiceBusMessageHandler<TMessage>` implements `IHandleMessages<TMessage>` and delegates to the application-layer `IIntegrationEventHandler<TMessage>`, keeping infrastructure concerns out of the application layer.

### Command Routing

Commands sent to other endpoints get a `RouteToEndpoint` entry using the endpoint name from the NServiceBus stereotype:

```csharp
routing.RouteToEndpoint(typeof(OrderAnimal), configuration["NServiceBus:Routing:Commands:OrderAnimal"] ?? "TargetApp");
```

The configuration key allows per-environment overrides without regenerating.

Commands handled by the same endpoint are self-routed (routed to `endpointName`).

## Working with Multiple Message Bus Providers

This module can coexist with other message bus providers (e.g. MassTransit) in the same application. When multiple providers are installed, Intent Architect generates a **Composite Message Bus** that routes messages to the correct broker.

### Designating Messages for NServiceBus

When only this module is installed, all messages automatically use NServiceBus — no additional configuration is needed.

When multiple providers are installed, mark messages for NServiceBus using the **NServiceBus** stereotype:

1. Right-click on a **Package** or **Folder** in the Services designer.
2. Select **Add Stereotype** → **NServiceBus**.

The stereotype can be applied at package or folder level. Child elements inherit the designation automatically.

## `appsettings.json` Configuration

The module generates default entries. Key sections:

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
    "AzureServiceBus": "Endpoint=sb://<namespace>.servicebus.windows.net/;..."
  }
}
```

Amazon SQS uses the AWS credential chain — no connection string is needed.

## Dependency Injection Wiring

The module registers `AddNServiceBusConfiguration` into the Infrastructure DI extension:

```csharp
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    // ...
    services.AddNServiceBusConfiguration(configuration);
    // ...
}
```

On .NET 8/9, the host builder call is also injected into `Program.cs`:

```csharp
builder.Host.UseNServiceBusHost();
```

## Local Development

### Learning Transport (no infrastructure required)

The Learning Transport stores messages as files on disk. It requires no external service and is the fastest way to verify handler wiring locally. Set Transport to `Learning Transport` in module settings.

### RabbitMQ (Docker)

```bash
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.11-management
```

Admin console: `http://localhost:15672/` (guest/guest).

### Azure Service Bus

Use a real Azure namespace with a connection string, or use the [Azure Service Bus emulator](https://learn.microsoft.com/en-us/azure/service-bus-messaging/overview-emulator) for local development.

Store connection strings in [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```bash
dotnet user-secrets set "ConnectionStrings:AzureServiceBus" "<your-connection-string>"
```

### Amazon SQS

SQS uses the [AWS credential chain](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/creds-assign.html). Set up credentials via IAM access keys or AWS SSO:

```bash
# IAM access keys
aws configure

# Or set environment variables
$env:AWS_ACCESS_KEY_ID = "..."
$env:AWS_SECRET_ACCESS_KEY = "..."
$env:AWS_REGION = "eu-west-1"
```

`EnableInstallers()` automatically creates SQS queues and SNS topics on first run.
