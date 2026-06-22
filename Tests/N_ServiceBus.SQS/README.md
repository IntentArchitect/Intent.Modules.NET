# NServiceBus.SQS — Test Application

Verifies the `Intent.Eventing.NServiceBus` module with the **Amazon SQS** transport.

## Purpose

Confirms that Integration Event publish/subscribe and Integration Command routing work correctly over AWS SQS (queues) and SNS (topics for events).

## What is tested

- Integration Event publish → handler fires via SNS topic → SQS subscription.
- Integration Command send → handler fires via SQS queue routing.
- `[HANDLER HIT]` log lines confirm handler execution.

## Infrastructure required

AWS account with SQS and SNS access. `EnableInstallers()` creates queues and SNS topics automatically on startup — no manual AWS console setup needed.

## Credentials

SQS uses the [AWS credential chain](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/creds-assign.html). Set up credentials before running:

```powershell
# Option 1 — IAM access keys via AWS CLI
aws configure

# Option 2 — environment variables
$env:AWS_ACCESS_KEY_ID     = "..."
$env:AWS_SECRET_ACCESS_KEY = "..."
$env:AWS_REGION            = "eu-west-1"
```

> [!NOTE]
>
> `aws login` (SSO) does not write classic `~/.aws/credentials`. If using SSO, set `$env:AWS_PROFILE` to the profile name.

## How to run

1. Configure AWS credentials (see above).
2. Start the application. NServiceBus creates the SQS queue and SNS topic automatically.
3. Publish a `TestMessageEvent`:
   ```
   PUT /api/external-message-publish/publish-external-message
   Body: { "message": "hello" }
   ```
4. Confirm in console output:
   ```
   [HANDLER HIT] SQS.TestMessageHandler received: hello
   ```
5. Send an `OrderAnimal` command:
   ```
   POST /api/animals
   Body: { "name": "Rex", "type": "Dog" }
   ```
6. Confirm in console output:
   ```
   [HANDLER HIT] SQS.CatchAllHandler received OrderAnimal: Name=Rex, Type=Dog
   ```

## Module settings

| Setting | Value |
|---|---|
| Transport | Amazon SQS |
| Outbox Pattern | None |
| Recoverability | Immediate and Delayed |
