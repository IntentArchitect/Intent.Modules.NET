using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents
{
    public class ParentDto : IMapFrom<Parent>
    {
        public ParentDto()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public static ParentDto Create(string id, string name)
        {
            return new ParentDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Parent, ParentDto>();

            profile.CreateMap<IParentDocument, ParentDto>();
        }
    }
}