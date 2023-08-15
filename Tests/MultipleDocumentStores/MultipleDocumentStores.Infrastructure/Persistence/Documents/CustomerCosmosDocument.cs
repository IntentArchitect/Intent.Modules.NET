using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using MultipleDocumentStores.Domain.Entities;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace MultipleDocumentStores.Infrastructure.Persistence.Documents
{
    internal class CustomerCosmosDocument : CustomerCosmos, ICosmosDBDocument<CustomerCosmosDocument, CustomerCosmos>
    {
        private string? _type;
        [JsonProperty("id")]
        string IItem.Id
        {
            get => Id;
            set => Id = value;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }

        public CustomerCosmosDocument PopulateFromEntity(CustomerCosmos entity)
        {
            Id = entity.Id;
            Name = entity.Name;

            return this;
        }
    }
}