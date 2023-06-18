using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class IdTestingDocument : IItem
    {
        private string? _type;

        public IdTestingDocument()
        {
            Identifier = null!;
            Id = null!;
        }

        [JsonProperty("id")]
        string IItem.Id
        {
            get => Identifier;
            set => Identifier = value;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().Name;
            set => _type = value;
        }
        string IItem.PartitionKey => Identifier;
        public string Identifier { get; set; }
        [JsonProperty("@id")]
        public string Id { get; set; }
    }
}