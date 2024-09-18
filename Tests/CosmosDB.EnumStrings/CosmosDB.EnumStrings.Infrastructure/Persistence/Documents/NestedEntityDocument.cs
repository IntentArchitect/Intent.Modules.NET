using System;
using CosmosDB.EnumStrings.Domain;
using CosmosDB.EnumStrings.Domain.Entities;
using CosmosDB.EnumStrings.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EnumStrings.Infrastructure.Persistence.Documents
{
    internal class NestedEntityDocument : INestedEntityDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        [JsonConverter(typeof(EnumJsonConverter))]
        public EnumExample EnumExample { get; set; }
        [JsonConverter(typeof(EnumJsonConverter))]
        public EnumExample? NullableEnumExample { get; set; }
        public EmbeddedObjectDocument EmbeddedObject { get; set; } = default!;
        IEmbeddedObjectDocument INestedEntityDocument.EmbeddedObject => EmbeddedObject;

        public NestedEntity ToEntity(NestedEntity? entity = default)
        {
            entity ??= new NestedEntity();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.EnumExample = EnumExample;
            entity.NullableEnumExample = NullableEnumExample;
            entity.EmbeddedObject = EmbeddedObject.ToEntity() ?? throw new Exception($"{nameof(entity.EmbeddedObject)} is null");

            return entity;
        }

        public NestedEntityDocument PopulateFromEntity(NestedEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            EnumExample = entity.EnumExample;
            NullableEnumExample = entity.NullableEnumExample;
            EmbeddedObject = EmbeddedObjectDocument.FromEntity(entity.EmbeddedObject)!;

            return this;
        }

        public static NestedEntityDocument? FromEntity(NestedEntity? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new NestedEntityDocument().PopulateFromEntity(entity);
        }
    }
}