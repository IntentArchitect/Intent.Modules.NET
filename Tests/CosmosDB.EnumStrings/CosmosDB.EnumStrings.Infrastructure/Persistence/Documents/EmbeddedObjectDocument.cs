using System;
using CosmosDB.EnumStrings.Domain;
using CosmosDB.EnumStrings.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocument", Version = "1.0")]

namespace CosmosDB.EnumStrings.Infrastructure.Persistence.Documents
{
    public class EmbeddedObjectDocument : IEmbeddedObjectDocument
    {
        public string Name { get; set; } = default!;
        [JsonConverter(typeof(EnumJsonConverter))]
        public EnumExample EnumExample { get; set; }
        [JsonConverter(typeof(EnumJsonConverter))]
        public EnumExample? NullableEnumExample { get; set; }

        public EmbeddedObject ToEntity(EmbeddedObject? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<EmbeddedObject>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(EnumExample), EnumExample);
            ReflectionHelper.ForceSetProperty(entity, nameof(NullableEnumExample), NullableEnumExample);

            return entity;
        }

        public EmbeddedObjectDocument PopulateFromEntity(EmbeddedObject entity)
        {
            Name = entity.Name;
            EnumExample = entity.EnumExample;
            NullableEnumExample = entity.NullableEnumExample;

            return this;
        }

        public static EmbeddedObjectDocument? FromEntity(EmbeddedObject? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new EmbeddedObjectDocument().PopulateFromEntity(entity);
        }
    }
}