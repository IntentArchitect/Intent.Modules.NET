using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    internal class ParentDetailsTagsDocument : IParentDetailsTagsDocument
    {
        public string Id { get; set; } = default!;
        public string TagName { get; set; } = default!;
        public string TagValue { get; set; } = default!;

        public ParentDetailsTags ToEntity(ParentDetailsTags? entity = default)
        {
            entity ??= new ParentDetailsTags();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.TagName = TagName ?? throw new Exception($"{nameof(entity.TagName)} is null");
            entity.TagValue = TagValue ?? throw new Exception($"{nameof(entity.TagValue)} is null");

            return entity;
        }

        public ParentDetailsTagsDocument PopulateFromEntity(ParentDetailsTags entity)
        {
            Id = entity.Id;
            TagName = entity.TagName;
            TagValue = entity.TagValue;

            return this;
        }

        public static ParentDetailsTagsDocument? FromEntity(ParentDetailsTags? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ParentDetailsTagsDocument().PopulateFromEntity(entity);
        }
    }
}