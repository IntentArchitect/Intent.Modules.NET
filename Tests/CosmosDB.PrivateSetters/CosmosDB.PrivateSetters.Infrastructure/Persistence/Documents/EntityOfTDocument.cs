using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class EntityOfTDocument<T> : IEntityOfTDocument<T>, ICosmosDBDocument<EntityOfT<T>, EntityOfTDocument<T>>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;
        public T GenericAttribute { get; set; } = default!;

        public EntityOfT<T> ToEntity(EntityOfT<T>? entity = default)
        {
            entity ??= new EntityOfT<T>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id);
            ReflectionHelper.ForceSetProperty(entity, nameof(GenericAttribute), GenericAttribute);

            return entity;
        }

        public EntityOfTDocument<T> PopulateFromEntity(EntityOfT<T> entity)
        {
            Id = entity.Id;
            GenericAttribute = entity.GenericAttribute;

            return this;
        }

        public static EntityOfTDocument<T>? FromEntity(EntityOfT<T>? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new EntityOfTDocument<T>().PopulateFromEntity(entity);
        }
    }
}