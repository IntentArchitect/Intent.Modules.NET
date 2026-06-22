# NServiceBus.OutboxPattern.Publish — Test Application

Verifies the `Intent.Eventing.NServiceBus` module with the **RabbitMQ transport** and **SQL Persistence transactional outbox**, publisher side.

## Purpose

Confirms that Integration Events are buffered in the SQL outbox during the database transaction and dispatched to RabbitMQ only after the transaction commits. Run alongside `NServiceBus.OutboxPattern.Subscribe` to observe end-to-end delivery.

## What is tested

- Event published via HTTP endpoint is written to the SQL outbox, not dispatched inline.
- After commit, NServiceBus dispatches the event to RabbitMQ.
- The Subscribe app receives the event and fires its handler.

## Infrastructure required

- RabbitMQ broker (see RabbitMQ test app README).
- SQL Server database for the outbox tables. Connection string: `ConnectionStrings:DefaultConnection`.

NServiceBus creates the outbox schema tables automatically on startup (`EnableInstallers`).

## How to run

1. Start RabbitMQ.
2. Start the SQL Server instance and ensure `ConnectionStrings:DefaultConnection` is configured.
3. Start this application (Publish).
4. Start `NServiceBus.OutboxPattern.Subscribe`.
5. Trigger an event on the Publish app:
   ```
   PUT /api/test-event-send
   ```
6. Confirm in the Subscribe app console:
   ```
   [HANDLER HIT] Subscribe.AnotherTestMessageHandler received: ...
   ```

## Module settings

| Setting | Value |
|---|---|
| Transport | RabbitMQ |
| Outbox Pattern | Sql Persistence |
| Recoverability | Immediate and Delayed |
