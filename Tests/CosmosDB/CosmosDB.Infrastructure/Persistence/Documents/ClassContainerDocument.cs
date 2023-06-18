using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class ClassContainerDocument : IItem
    {
        private string? _type;

        public ClassContainerDocument()
        {
            Id = null!;
            ClassPartitionKey = null!;
        }

        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().Name;
            set => _type = value;
        }
        string IItem.PartitionKey => ClassPartitionKey;
        [JsonProperty("id")]
        public string Id { get; set; }
        public string ClassPartitionKey { get; set; }
    }
}