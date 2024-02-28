### Version 1.1.2

- Improvement: Updated Interoperable dependency versions.

### Version 1.1.1

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 1.0.6

- Improvement: Updated to be compatible with .NET 8.

### Version 1.0.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 1.0.0

* New: This introduces [Google Cloud Pub/Sub](https://cloud.google.com/pubsub/) as a framework for modeling Publisher/Subscriber communication between applications.
* Notes for use:
  * To use the Emulator refer to this [link](https://cloud.google.com/pubsub/docs/emulator) and also set the `UsePubSubEmulator` setting to `true` in your `appsettings.josn` file.
  * To verify incoming push notifications you need to set the `ShouldAuthorizePushNotification` to `true` and specify your own unique token value `VerificationToken` in your `appsettings.json` file. That same token needs to be supplied as part of your push endpoint URL when creating a Push Subscription in Google Cloud Pub/Sub.
  * Topics and Subscriptions can be automatically constructed when `ShouldSetupCloudResources` is `true` inside your `appsettings.json` file.