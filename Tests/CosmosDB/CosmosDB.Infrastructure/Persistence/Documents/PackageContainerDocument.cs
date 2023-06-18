using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class PackageContainerDocument : IItem
    {
        private string? _type;

        public PackageContainerDocument()
        {
            Id = null!;
            PackagePartitionKey = null!;
        }

        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().Name;
            set => _type = value;
        }
        string IItem.PartitionKey => PackagePartitionKey;
        [JsonProperty("id")]
        public string Id { get; set; }
        public string PackagePartitionKey { get; set; }
    }
}