# NServiceBus.AzureServiceBus — Test Application

Verifies the `Intent.Eventing.NServiceBus` module with the **Azure Service Bus** transport.

## Purpose

Confirms that Integration Event publish/subscribe and Integration Command routing work correctly over Azure Service Bus using the default Topic topology.

## What is tested

- Integration Event publish → handler fires via Azure Service Bus topic subscription.
- Integration Command send → handler fires via Azure Service Bus queue routing.
- `[HANDLER HIT]` log lines confirm handler execution.

## Infrastructure required

An Azure Service Bus namespace (Standard or Premium tier). `EnableInstallers()` creates queues and topic subscriptions automatically on startup.

## How to run

1. Store the connection string in user secrets (do not commit it):
   ```bash
   dotnet user-secrets set "ConnectionStrings:AzureServiceBus" "<your-connection-string>"
   ```
2. Start the application.
3. Publish a `TestMessageEvent`:
   ```
   PUT /api/external-message-publish/publish-external-message
   Body: { "message": "hello" }
   ```
4. Confirm in console output:
   ```
   [HANDLER HIT] AzureServiceBus.TestMessageHandler received TestMessageEvent
   ```

## Module settings

| Setting | Value |
|---|---|
| Transport | Azure Service Bus |
| Outbox Pattern | None |
| Recoverability | Immediate and Delayed |
