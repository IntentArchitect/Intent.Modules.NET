using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class DerivedTypeAggregateDocument : IDerivedTypeAggregateDocument, ICosmosDBDocument<DerivedTypeAggregate, DerivedTypeAggregateDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;

        public DerivedTypeAggregate ToEntity(DerivedTypeAggregate? entity = default)
        {
            entity ??= new DerivedTypeAggregate();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id);

            return entity;
        }

        public DerivedTypeAggregateDocument PopulateFromEntity(DerivedTypeAggregate entity)
        {
            Id = entity.Id;

            return this;
        }

        public static DerivedTypeAggregateDocument? FromEntity(DerivedTypeAggregate? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedTypeAggregateDocument().PopulateFromEntity(entity);
        }
    }
}