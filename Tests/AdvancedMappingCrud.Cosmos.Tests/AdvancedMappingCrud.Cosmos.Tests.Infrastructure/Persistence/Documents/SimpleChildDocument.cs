using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    internal class SimpleChildDocument : ISimpleChildDocument
    {
        public string Id { get; set; } = default!;
        public string Thing { get; set; } = default!;

        public SimpleChild ToEntity(SimpleChild? entity = default)
        {
            entity ??= new SimpleChild();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Thing = Thing ?? throw new Exception($"{nameof(entity.Thing)} is null");

            return entity;
        }

        public SimpleChildDocument PopulateFromEntity(SimpleChild entity)
        {
            Id = entity.Id;
            Thing = entity.Thing;

            return this;
        }

        public static SimpleChildDocument? FromEntity(SimpleChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SimpleChildDocument().PopulateFromEntity(entity);
        }
    }
}