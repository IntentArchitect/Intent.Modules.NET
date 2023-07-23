using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class DerivedOfTDocument : DerivedOfT, ICosmosDBDocument<DerivedOfTDocument, DerivedOfT>
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

        public DerivedOfTDocument PopulateFromEntity(DerivedOfT entity)
        {
            DerivedAttribute = entity.DerivedAttribute;
            Id = entity.Id;
            GenericAttribute = entity.GenericAttribute;

            return this;
        }
    }
}