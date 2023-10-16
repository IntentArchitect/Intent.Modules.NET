using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class WithoutPartitionKeyDocument : IWithoutPartitionKeyDocument, ICosmosDBDocument<IWithoutPartitionKey, WithoutPartitionKey, WithoutPartitionKeyDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;

        public WithoutPartitionKey ToEntity(WithoutPartitionKey? entity = default)
        {
            entity ??= new WithoutPartitionKey();

            entity.Id = Id;

            return entity;
        }

        public WithoutPartitionKeyDocument PopulateFromEntity(IWithoutPartitionKey entity)
        {
            Id = entity.Id;

            return this;
        }

        public static WithoutPartitionKeyDocument? FromEntity(IWithoutPartitionKey? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new WithoutPartitionKeyDocument().PopulateFromEntity(entity);
        }
    }
}