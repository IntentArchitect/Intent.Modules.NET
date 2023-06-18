using CosmosDB.Domain;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class ClientDocument : IItem
    {
        private string? _type;

        public ClientDocument()
        {
            Identifier = null!;
            Name = null!;
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
        [JsonProperty("@type")]
        public ClientType Type { get; set; }
        public string Name { get; set; }
    }
}