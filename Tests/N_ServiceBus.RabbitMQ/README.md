# NServiceBus.RabbitMQ — Test Application

Verifies the `Intent.Eventing.NServiceBus` module with the **RabbitMQ** transport.

## Purpose

Confirms that Integration Event publish/subscribe and Integration Command routing work correctly over a real RabbitMQ broker using Conventional routing topology with Quorum queues.

## What is tested

- Integration Event publish → handler fires via RabbitMQ exchange fan-out.
- Integration Command send → handler fires via RabbitMQ queue routing.
- `[HANDLER HIT]` log lines confirm handler execution.

## Infrastructure required

RabbitMQ broker. Start locally with Docker:

```bash
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.11-management
```

Admin console: `http://localhost:15672/` (guest/guest).

## How to run

1. Start RabbitMQ.
2. Start the application. NServiceBus will create queues and exchanges automatically on startup (`EnableInstallers`).
3. Publish a `TestMessageEvent`:
   ```
   PUT /api/external-message-publish/publish-external-message
   Body: { "message": "hello" }
   ```
4. Confirm in console output:
   ```
   [HANDLER HIT] RabbitMQ.TestMessageHandler received TestMessageEvent
   ```

## Module settings

| Setting | Value |
|---|---|
| Transport | RabbitMQ |
| Outbox Pattern | None |
| Recoverability | Immediate and Delayed |

## Connection string

Configured via `appsettings.json` or user secrets:

```json
"ConnectionStrings": {
  "RabbitMQ": "amqp://guest:guest@localhost:5672"
}
```
