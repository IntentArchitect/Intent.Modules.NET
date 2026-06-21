# NServiceBus.OutboxPattern.Subscribe — Test Application

Verifies the `Intent.Eventing.NServiceBus` module with the **RabbitMQ transport** and **SQL Persistence transactional outbox**, subscriber side.

## Purpose

Receives events dispatched by `NServiceBus.OutboxPattern.Publish` after their outbox transaction commits. Confirms end-to-end durable delivery through the outbox pattern.

## What is tested

- Event arrives from RabbitMQ after the Publish app's outbox commits.
- Handler fires and logs `[HANDLER HIT]`.

## Infrastructure required

- RabbitMQ broker (see RabbitMQ test app README).
- SQL Server database for the outbox tables. Connection string: `ConnectionStrings:DefaultConnection`.

## How to run

Run alongside `NServiceBus.OutboxPattern.Publish`. See that app's README for the full end-to-end steps.

Expected output on the Subscribe console after triggering the Publish app:

```
[HANDLER HIT] Subscribe.AnotherTestMessageHandler received: ...
```

## Module settings

| Setting | Value |
|---|---|
| Transport | RabbitMQ |
| Outbox Pattern | Sql Persistence |
| Recoverability | Immediate and Delayed |
