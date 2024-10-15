using System.IO;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Google.CloudStorage.BulkCloudObjectItem", Version = "1.0")]

namespace CloudBlobStorageClients.Application.Common.Storage
{
    /// <summary>
    /// Represents a single item used for bulk uploads to object storage.
    /// </summary>
    /// <param name="Name">The name of the object.</param>
    /// <param name="DataStream">The stream of data to upload.</param>
    /// <param name="ContentType">The content type of the object. This should be a MIME type. Can be null.</param>
    public record BulkCloudObjectItem(string Name, Stream DataStream, string? ContentType = null);

}