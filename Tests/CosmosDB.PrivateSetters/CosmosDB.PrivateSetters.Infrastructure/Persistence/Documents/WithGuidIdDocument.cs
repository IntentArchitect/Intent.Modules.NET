using System;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class WithGuidIdDocument : IWithGuidIdDocument, ICosmosDBDocument<WithGuidId, WithGuidIdDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; }
        public string Field { get; set; } = default!;

        public WithGuidId ToEntity(WithGuidId? entity = default)
        {
            entity ??= new WithGuidId();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Guid.Parse(Id));
            ReflectionHelper.ForceSetProperty(entity, nameof(Field), Field ?? throw new Exception($"{nameof(entity.Field)} is null"));

            return entity;
        }

        public WithGuidIdDocument PopulateFromEntity(WithGuidId entity)
        {
            Id = entity.Id.ToString();
            Field = entity.Field;

            return this;
        }

        public static WithGuidIdDocument? FromEntity(WithGuidId? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new WithGuidIdDocument().PopulateFromEntity(entity);
        }
    }
}