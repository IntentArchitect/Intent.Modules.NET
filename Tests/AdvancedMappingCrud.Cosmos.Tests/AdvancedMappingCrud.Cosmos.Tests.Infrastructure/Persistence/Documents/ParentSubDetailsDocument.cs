using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    internal class ParentSubDetailsDocument : IParentSubDetailsDocument
    {
        public string SubDetailsLine1 { get; set; } = default!;
        public string SubDetailsLine2 { get; set; } = default!;

        public ParentSubDetails ToEntity(ParentSubDetails? entity = default)
        {
            entity ??= new ParentSubDetails();

            entity.SubDetailsLine1 = SubDetailsLine1 ?? throw new Exception($"{nameof(entity.SubDetailsLine1)} is null");
            entity.SubDetailsLine2 = SubDetailsLine2 ?? throw new Exception($"{nameof(entity.SubDetailsLine2)} is null");

            return entity;
        }

        public ParentSubDetailsDocument PopulateFromEntity(ParentSubDetails entity)
        {
            SubDetailsLine1 = entity.SubDetailsLine1;
            SubDetailsLine2 = entity.SubDetailsLine2;

            return this;
        }

        public static ParentSubDetailsDocument? FromEntity(ParentSubDetails? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ParentSubDetailsDocument().PopulateFromEntity(entity);
        }
    }
}