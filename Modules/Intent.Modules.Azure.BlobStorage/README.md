# Azure Blob Storage

The Azure Blob Storage module simplifies working with Azure Blob Storage in .NET applications by providing a robust C# client wrapper. This module includes an integration of the `Azure.Storage.Blobs` NuGet package, facilitating direct interactions with Azure Blob Storage services.

## Module Overview

Azure Blob Storage is designed to store massive amounts of unstructured data, such as files and media. The Azure Blob Storage Module abstracts these interactions through the `IBlobStorage` interface, which defines methods for essential operations like upload, download, list, and delete. This interface ensures that the implementation details are encapsulated, allowing developers to focus on business logic rather than infrastructure management.

## Practical Example

Imagine a scenario in a business application where financial reports need to be stored and retrieved from Azure Blob Storage. Below is an example of how one might use the `IBlobStorage` interface within a service class to handle these operations:

```csharp
public class FinancialReportService
{
    private readonly IBlobStorage _blobStorage;
    private readonly string _containerName = "financial-reports";

    public FinancialReportService(IBlobStorage blobStorage)
    {
        _blobStorage = blobStorage;
    }

    public async Task<Uri> SaveReportAsync(string reportName, string content)
    {
        var reportUri = await _blobStorage.UploadStringAsync(_containerName, reportName, content);
        return reportUri;
    }

    public async Task<string> GetReportAsync(string reportName)
    {
        var reportContent = await _blobStorage.DownloadAsStringAsync(_containerName, reportName);
        return reportContent;
    }

    public async Task<IEnumerable<Uri>> ListReportsAsync()
    {
        var reports = new List<Uri>();
        await foreach (var uri in _blobStorage.ListAsync(_containerName))
        {
            reports.Add(uri);
        }
        return reports;
    }
}
```

This service class abstracts all operations related to storing and retrieving financial reports, making the overall codebase cleaner and easier to manage.

## Local Development

For local development, Azurite is recommended to simulate Azure services without the need for an Azure subscription. This setup facilitates the testing and development of applications that use Azure Blob Storage.

To use Azurite locally, you have a few installation options:

1. **CLI Installation**:
   ```bash
   npm install -g azurite
   ```
   
   **Simply run**

   ```bash
   azurite
   ```

2. **IDE Options**:
    - **Visual Studio**: Use the Azure development workload, which supports storage emulators.

After installation, configure your application to use the default Azurite settings as follows in your `appsettings.json`:
```json
"AzureBlobStorage": "UseDevelopmentStorage=true"
```

## Production Connection String

For production environments, you typically obtain an Azure Blob Storage connection string directly from the Azure portal. Here's how:

1. Navigate to your Azure Blob Storage account on the Azure Portal.
2. Click on the "Access keys" section under "Security + networking".
3. Copy the connection string provided in the "key1" or "key2" section.

A typical Azure connection string looks like this:
```json
"ConnectionStrings": {
    "AzureBlobStorage": "DefaultEndpointsProtocol=https;AccountName=myaccountname;AccountKey=myaccountkey;EndpointSuffix=core.windows.net;"
}
```

This connection string should replace `"UseDevelopmentStorage=true"` in your `appsettings.json` for production deployments.

## Azure Storage Explorer

Connect to either your production Azure Blob Storage or Azurite using [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer/) for previewing and managing your storage containers and blobs.
