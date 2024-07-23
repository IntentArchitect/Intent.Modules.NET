using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ExplicitETags
{
    public class ExplicitETagDto : IMapFrom<ExplicitETag>
    {
        public ExplicitETagDto()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string? ETag { get; set; }

        public static ExplicitETagDto Create(string id, string name, string? eTag)
        {
            return new ExplicitETagDto
            {
                Id = id,
                Name = name,
                ETag = eTag
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ExplicitETag, ExplicitETagDto>();

            profile.CreateMap<IExplicitETagDocument, ExplicitETagDto>();
        }
    }
}