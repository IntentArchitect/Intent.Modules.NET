using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    public class EmbeddedChildDocument : IEmbeddedChildDocument
    {
        public string Name { get; set; } = default!;
        public int Age { get; set; }

        public EmbeddedChild ToEntity(EmbeddedChild? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<EmbeddedChild>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Age), Age);

            return entity;
        }

        public EmbeddedChildDocument PopulateFromEntity(EmbeddedChild entity)
        {
            Name = entity.Name;
            Age = entity.Age;

            return this;
        }

        public static EmbeddedChildDocument? FromEntity(EmbeddedChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new EmbeddedChildDocument().PopulateFromEntity(entity);
        }
    }
}