### Version 1.1.9

- Improvement: Updated NuGet package versions.

### Version 1.1.8

- Improvement: Updated NuGet package versions.

### Version 1.1.7

- Improvement: Updated NuGet package versions.

### Version 1.1.6

- Improvement: Updated NuGet package versions.

### Version 1.1.5

- Improvement: Updated NuGet package versions.

### Version 1.1.4

- Improvement: Updated NuGet package versions.

### Version 1.1.3

- Improvement: Updated NuGet package versions.

### Version 1.1.2

- Improvement: Updated NuGet package versions.

### Version 1.1.1

- Improvement: Updated NuGet package versions.

### Version 1.1.0

> ⚠️ **NOTE**
>
> This module update may cause a compilation breaks if you have written any custom code which uses AWSSDK features which are not supported on v4.
> Any generated code will be compliant.
> For details on what the breaking changes are check out the [change logs](https://github.com/aws/aws-sdk-net/blob/main/changelogs/SDK.CHANGELOG.2025.md) and [migration guide](https://docs.aws.amazon.com/sdk-for-net/v4/developer-guide/net-dg-v4.html).

- Improvement: Updated NuGet package versions.
- Fixed: `EnumeratorCancellation` attribute was missing on `CancellationToken` parameters on methods returning `IAsyncEnumerable<T>`.
- Fixed: `Microsoft.Extensions.Configuration` using was not added to the `AmazonS3ObjectStorageImplementation` template.

### Version 1.0.6

- Improvement: Updated NuGet package versions.

### Version 1.0.5

- Improvement: Updated NuGet package versions.

### Version 1.0.4

- Improvement: Included module help topic.

### Version 1.0.3

- Improvement: Updated NuGet package versions.

### Version 1.0.2

- Improvement: Updated module NuGet packages infrastructure.
- Improvement: Updated templates to use new NuGet package system.

### Version 1.0.1

- Improvement: Updated NuGet packages to latest stables.

### Version 1.0.0

- New Feature: Client used to perform Upload, Download and Delete request against Amazon S3 Buckets.
