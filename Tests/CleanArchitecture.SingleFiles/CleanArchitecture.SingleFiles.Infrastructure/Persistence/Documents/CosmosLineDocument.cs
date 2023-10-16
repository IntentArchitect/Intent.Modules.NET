using System;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure.Persistence.Documents
{
    internal class CosmosLineDocument : ICosmosLineDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        public CosmosLine ToEntity(CosmosLine? entity = default)
        {
            entity ??= new CosmosLine();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

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