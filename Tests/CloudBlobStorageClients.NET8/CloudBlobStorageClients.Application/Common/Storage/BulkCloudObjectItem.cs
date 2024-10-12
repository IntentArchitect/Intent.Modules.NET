using System.IO;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Google.CloudStorage.BulkCloudObjectItem", Version = "1.0")]

namespace CloudBlobStorageClients.Application.Common.Storage
{
    /// <summary>
    /// Represents a single item used for bulk uploads to object storage.
    /// </summary>
    public record BulkCloudObjectItem
    {
        /// <summary>
        /// Constructor for the object which represents a single item used for bulk uploads to object storage
        /// </summary>
        /// <param name="Name">The name of the object.</param>
        /// <param name="DataStream">The stream of data to upload.</param>
        /// <param name="ContentType">The content type of the object. This should be a MIME type. Can be null.</param>
        public BulkCloudObjectItem(string Name, Stream DataStream, string? ContentType = null)
        {
            this.Name = Name;
            this.DataStream = DataStream;
            this.ContentType = ContentType;
        }

        public string Name { get; init; }
        public Stream DataStream { get; init; }
        public string? ContentType { get; init; }
    }
}