### Version 3.4.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 3.4.2

- Improvement: Updated NuGet packages to latest stables.

### Version 3.4.1

- Improvement: Updated `BlobStorageExtensions` with more `ConfigureAwait` configurations.
- Improvement: Updated module project to .NET 8.

### Version 3.4.0

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 3.3.2

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.
- Improvement: Normalized methods to have `Async` suffix.

### Version 3.3.1

- Improvement: Updated supported client version to [3.2.0, 5.0.0).

### Version 3.3.0

- New Feature: Client used to perform Upload, Download and Delete request against Azure Storage Accounts for Blob containers.
- Note: To use, inject `IBlobStorage` interface.
