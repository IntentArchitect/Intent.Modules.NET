using System;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal abstract class BaseOfTDocument<T> : IBaseOfTDocument<T>, ICosmosDBDocument<BaseOfT<T>, BaseOfTDocument<T>>
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

        public BaseOfT<T> ToEntity(BaseOfT<T>? entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.Id = Id;
            entity.GenericAttribute = GenericAttribute;

            return entity;
        }

        public BaseOfTDocument<T> PopulateFromEntity(BaseOfT<T> entity)
        {
            Id = entity.Id;
            GenericAttribute = entity.GenericAttribute;

            return this;
        }
    }
}