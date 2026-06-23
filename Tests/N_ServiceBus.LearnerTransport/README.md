# NServiceBus.LearnerTransport — Test Application

Verifies the `Intent.Eventing.NServiceBus` module with the **Learning Transport**.

## Purpose

The Learning Transport stores messages as files on disk. It requires no external service and is the simplest end-to-end verification scenario.

## What is tested

- Integration Event publish → handler fires in the same process.
- Integration Command send → handler fires in the same process (self-routed endpoint).
- `[HANDLER HIT]` log lines confirm handler execution.

## Infrastructure required

None. The Learning Transport is file-based and runs locally.

## How to run

1. Start the application.
2. Publish a `TestMessageEvent`:
   ```
   PUT /api/external-message-publish/publish-external-message
   Body: { "message": "hello" }
   ```
3. Confirm in console output:
   ```
   [HANDLER HIT] TestMessageHandler received: hello
   ```
4. Send an `OrderAnimal` command:
   ```
   POST /api/animals
   Body: { "name": "Rex", "type": "Dog" }
   ```
5. Confirm in console output:
   ```
   [HANDLER HIT] ... received OrderAnimal: Name=Rex, Type=Dog
   ```

## Module settings

| Setting | Value |
|---|---|
| Transport | Learning Transport |
| Outbox Pattern | None |
| Recoverability | Immediate and Delayed |
