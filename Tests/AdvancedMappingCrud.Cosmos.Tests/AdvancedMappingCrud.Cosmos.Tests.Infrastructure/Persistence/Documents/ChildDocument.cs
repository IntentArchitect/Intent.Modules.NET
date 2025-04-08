using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    internal class ChildDocument : IChildDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Age { get; set; }

        public Child ToEntity(Child? entity = default)
        {
            entity ??= new Child();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Age = Age;

            return entity;
        }

        public ChildDocument PopulateFromEntity(Child entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Age = entity.Age;

            return this;
        }

        public static ChildDocument? FromEntity(Child? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ChildDocument().PopulateFromEntity(entity);
        }
    }
}