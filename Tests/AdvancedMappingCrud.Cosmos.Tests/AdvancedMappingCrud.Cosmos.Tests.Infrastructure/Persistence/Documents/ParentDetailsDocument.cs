using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    internal class ParentDetailsDocument : IParentDetailsDocument
    {
        public string DetailsLine1 { get; set; } = default!;
        public string DetailsLine2 { get; set; } = default!;
        public ParentSubDetailsDocument? ParentSubDetails { get; set; }
        IParentSubDetailsDocument IParentDetailsDocument.ParentSubDetails => ParentSubDetails;
        public List<ParentDetailsTagsDocument>? ParentDetailsTags { get; set; }
        IReadOnlyList<IParentDetailsTagsDocument> IParentDetailsDocument.ParentDetailsTags => ParentDetailsTags;

        public ParentDetails ToEntity(ParentDetails? entity = default)
        {
            entity ??= new ParentDetails();

            entity.DetailsLine1 = DetailsLine1 ?? throw new Exception($"{nameof(entity.DetailsLine1)} is null");
            entity.DetailsLine2 = DetailsLine2 ?? throw new Exception($"{nameof(entity.DetailsLine2)} is null");
            entity.ParentSubDetails = ParentSubDetails?.ToEntity();
            entity.ParentDetailsTags = ParentDetailsTags?.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public ParentDetailsDocument PopulateFromEntity(ParentDetails entity)
        {
            DetailsLine1 = entity.DetailsLine1;
            DetailsLine2 = entity.DetailsLine2;
            ParentSubDetails = ParentSubDetailsDocument.FromEntity(entity.ParentSubDetails);
            ParentDetailsTags = entity.ParentDetailsTags?.Select(x => ParentDetailsTagsDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static ParentDetailsDocument? FromEntity(ParentDetails? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ParentDetailsDocument().PopulateFromEntity(entity);
        }
    }
}