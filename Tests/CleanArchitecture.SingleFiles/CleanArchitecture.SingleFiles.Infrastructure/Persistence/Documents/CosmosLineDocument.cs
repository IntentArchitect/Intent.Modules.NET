using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure.Persistence.Documents
{
    internal class CosmosLineDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        public CosmosLine ToEntity(CosmosLine? entity = default)
        {
            entity ??= new CosmosLine();

            entity.Id = Id;
            entity.Name = Name;

            return entity;
        }

        public CosmosLineDocument PopulateFromEntity(CosmosLine entity)
        {
            Id = entity.Id;
            Name = entity.Name;

            return this;
        }

        public static CosmosLineDocument? FromEntity(CosmosLine? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CosmosLineDocument().PopulateFromEntity(entity);
        }
    }
}