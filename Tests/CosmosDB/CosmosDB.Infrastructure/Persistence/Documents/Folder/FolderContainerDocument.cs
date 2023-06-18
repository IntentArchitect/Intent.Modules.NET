using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents.Folder
{
    internal class FolderContainerDocument : IItem
    {
        private string? _type;

        public FolderContainerDocument()
        {
            Id = null!;
            FolderPartitionKey = null!;
        }

        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().Name;
            set => _type = value;
        }
        string IItem.PartitionKey => FolderPartitionKey;
        [JsonProperty("id")]
        public string Id { get; set; }
        public string FolderPartitionKey { get; set; }
    }
}