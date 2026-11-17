# Intent.AzureFunctions.AzureQueueStorage

This module uses Azure Functions to receive Azure Queue Storage messages and dispatch them to the integration event handlers modelled in the Services designer.

It builds on [Intent.Eventing.AzureQueueStorage](https://docs.intentarchitect.com/articles/modules-dotnet/intent-eventing-azurequeuestorage/intent-eventing-azurequeuestorage.html), which models the messages and queues and provides the dispatcher and event bus, and on [Intent.AzureFunctions](https://docs.intentarchitect.com/articles/modules-dotnet/intent-azurefunctions/intent-azurefunctions.html), which hosts the functions on the isolated worker model.

## What it generates

For each Integration Event Handler that subscribes to an integration event or integration command, the module generates a `<HandlerName>Consumer` Azure Function (the `Handler` suffix is dropped). The function:

- is triggered by a `[QueueTrigger]` on the subscribed message's queue, using the `QueueStorage:DefaultEndpoint` connection;
- hands the received message envelope to the Azure Queue Storage event dispatcher, which deserializes it (camel-case JSON) and invokes the handler;
- flushes the event bus once the handler completes, so any messages the handler published are sent;
- logs and rethrows any exception, so the Functions runtime retries the message and moves it to the poison queue after the retry limit.

The queue name comes from the subscribed message, or from the `Azure Queue Storage` stereotype's `Queue Name` when it is set (see Intent.Eventing.AzureQueueStorage).

When the Intent.AzureFunctions `Simple Function Names` setting is off, the function name is prefixed with the handler's folder path.

## Configuration

- **Connection:** set `QueueStorage:DefaultEndpoint` in the Function App's settings (`local.settings.json` when running locally) to the storage account connection string.
- **Message encoding:** when Intent.Eventing.AzureQueueStorage's `Message Encoding` setting is `None`, the module adds `extensions.queues.messageEncoding = "none"` to `host.json`, so the function reads messages that were not Base64-encoded. With `Base64` the Functions default applies.

## NuGet packages

The module adds `Microsoft.Azure.Functions.Worker.Extensions.Storage.Queues` to the project that hosts the functions.
