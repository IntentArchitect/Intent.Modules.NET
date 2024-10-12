# Google Cloud Storage

The Google Cloud Storage module simplifies working with Google Cloud Storage in .NET applications by providing a robust C# client wrapper. This module includes an integration of the `Google.Cloud.Storage.V1` NuGet package, facilitating direct interactions with Google Cloud Storage services.

## Module Overview

Google Cloud Storage is a managed service designed for storing any amount of unstructured data and retrieving it as often as one likes. The Google Cloud Storage Module abstracts these interactions through the `ICloudStorage` interface, which defines methods for essential operations like upload, download, list, and delete. This interface ensures that the implementation details are encapsulated, allowing developers to focus on business logic rather than infrastructure management.

## Practical Example

Consider a scenario within a corporate software system where it is necessary to store and fetch marketing materials from Google Cloud Storage. Below is an illustration of how the `ICloudStorage` interface can be implemented in a service class to manage these operations:

```csharp
public class MarketingMaterialService
{
    private readonly ICloudStorage _cloudStorage;
    private readonly string _bucketName = "marketing-materials";

    public MarketingMaterialService(ICloudStorage cloudStorage)
    {
        _cloudStorage = cloudStorage;
    }

    public async Task<Uri> SaveMaterialAsync(string materialName, Stream content, string? contentType = null)
    {
        var materialUri = await _cloudStorage.UploadAsync(_bucketName, materialName, content, contentType);
        return materialUri;
    }

    public async Task<Stream> GetMaterialAsync(string materialName)
    {
        return  await _cloudStorage.DownloadAsync(_bucketName, materialName);
    }

    public async IAsyncEnumerable<Uri> ListMaterialsAsync()
    {
        List<Uri> materials = [];
        await foreach (var uri in _cloudStorage.ListAsync(_bucketName))
        {
            yield return uri;
        }
    }
}
```

This service class simplifies all functions related to storing and retrieving marketing materials, enhancing the manageability and cleanliness of the overall codebase.

## Pre-Signed Expiry Urls

Performing a `GetAsync` will give you back a link that you can use to gain access to an object for a limited amount of time ([link](https://cloud.google.com/storage/docs/access-control/signed-urls)).

To configure this expiry time you can add this entry in your `appsettings.json` file and specify the expiry duration as a TimeSpan.

```json
"GCP": {
    "PreSignedUrlExpiry": "00:05:00",
    "CloudStorageAuthFileLocation": ""
  },
```

## Authentication

THe module currently supports Google Service Account JSON key authentication. If additional authentication methods are required, the module can be updated to support these.

### JSON key Creation

An example of how to create a Service Account JSON key is detailed below. The steps and process my differ based on your organisation's security requirements:

- Log into your [GCP console](https://console.cloud.google.com) account

The first step is to create a _Role_ which has permission to the _Cloud Storage_ objects:

- Search for `Roles`. This option is available under the `IAM & Admin` section
- Once on the `Roles` screen, click `+ CREATE ROLE`
- Give the role a title (e.g. StorageFullAccess) and click `+ ADD PERMISSIONS`
- On the `Add Permissions` dialog which appears, under the _Filter property name or value_ filter, enter `storage.objects`. This will list all permissions related to objects in storage.
- The required permissions could be different based on your specific use case, but for the full functionality of this module to be leveraged, the following permisisons are required:
  - `storage.objects.create`
  - `storage.objects.delete`
  - `storage.objects.get`
  - `storage.objects.list`
  - `storage.objects.update`
- Click `ADD` on the dialog, and then click `CREATE`

A _role_ has now been created. Next step is to create a `Service Account` to assign the role to.

Still under the `IAM & Admin` section:

- Click `Service Accounts` (or search while in the GCP console)
- Click `+ CREATE SERVICE ACCOUNT`
- Enter a _Service Account Name_ (e.g. {_ApplicationName_}StorageAccount)
- Click `CREATE AND CONTINUE`
- From the _Role_ dropdown, filter for and find the _role_ create in the previous step
- Click `DONE`
- Once back to the _Service Accounts List_ page, click on the newly created service account
- Click `KEYS` and then `ADD KEY` => `Create new key`
- Make sure `JSON` is selected and click `CREATE`
- The JSON key file will be downloaded

Finally copy the JSON file from yours downloads folder to another folder of your choice, and make sure the `CloudStorageAuthFileLocation` setting in your applications _appsettings.json_ is updated to the location of the key file:

```json
"GCP": {
    "PreSignedUrlExpiry": "00:05:00",
    "CloudStorageAuthFileLocation": "C:\\gcpkeys\\my-project-183005-10ed20d64009.json"
  },
```

Now when using `ICloudStorage` the key file will be used to authenticate with Google Cloud Storage.

> Ensure to create the necessary bucket(s) in Google Cloud Storage prior to performing any operations with them.
